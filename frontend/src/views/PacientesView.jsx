import React, { useState, useEffect } from 'react';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';
import { Dog, Plus, Search, Edit2, Trash2, X, AlertCircle, Filter, RotateCcw } from 'lucide-react';

export const PacientesView = () => {
  const { hasRole, isAdmin } = useAuth();
  const canCreateOrEdit = hasRole(['Administrador', 'Veterinario']);
  const canDelete = isAdmin;

  const [pacientes, setPacientes] = useState([]);
  const [clientes, setClientes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');

  // Filtros de Endpoint
  const [filters, setFilters] = useState({
    nombre: '',
    especie: '',
    raza: '',
    dueno: ''
  });

  const [formData, setFormData] = useState({
    clienteId: '',
    nombre: '',
    especie: 'Canino',
    raza: '',
    peso: '',
    alergias: ''
  });

  useEffect(() => {
    fetchClientes();
  }, []);

  useEffect(() => {
    const timer = setTimeout(() => {
      fetchPacientes();
    }, 300);
    return () => clearTimeout(timer);
  }, [filters]);

  const fetchClientes = async () => {
    try {
      const res = await api.get('/api/v1/clientes');
      setClientes(res.data || []);
    } catch (err) {
      console.error(err);
    }
  };

  const fetchPacientes = async () => {
    setLoading(true);
    try {
      const params = {};
      if (filters.nombre) params.nombre = filters.nombre;
      if (filters.especie) params.especie = filters.especie;
      if (filters.raza) params.raza = filters.raza;
      if (filters.dueno) params.dueno = filters.dueno;

      const res = await api.get('/api/v1/pacientes', { params });
      setPacientes(res.data || []);
    } catch (err) {
      console.error(err);
      setError('Error al cargar la lista de pacientes.');
    } finally {
      setLoading(false);
    }
  };

  const handleResetFilters = () => {
    setFilters({
      nombre: '',
      especie: '',
      raza: '',
      dueno: ''
    });
  };

  const handleOpenModal = (paciente = null) => {
    setError('');
    if (paciente) {
      setEditingId(paciente.id);
      setFormData({
        clienteId: paciente.clienteId,
        nombre: paciente.nombre,
        especie: paciente.especie,
        raza: paciente.raza || '',
        peso: paciente.peso || '',
        alergias: paciente.alergias || ''
      });
    } else {
      setEditingId(null);
      setFormData({
        clienteId: clientes[0]?.id || '',
        nombre: '',
        especie: 'Canino',
        raza: '',
        peso: '',
        alergias: ''
      });
    }
    setIsModalOpen(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    const payload = {
      clienteId: Number(formData.clienteId),
      nombre: formData.nombre,
      especie: formData.especie,
      raza: formData.raza || null,
      peso: formData.peso ? parseFloat(formData.peso) : 0.1,
      alergias: formData.alergias || null
    };

    try {
      if (editingId) {
        await api.put(`/api/v1/pacientes/${editingId}`, payload);
      } else {
        await api.post('/api/v1/pacientes', payload);
      }
      setIsModalOpen(false);
      fetchPacientes();
    } catch (err) {
      if (err.response?.data?.errors) {
        setError(Object.values(err.response.data.errors).flat().join(' '));
      } else {
        setError(err.response?.data?.message || 'Error al guardar la información del paciente.');
      }
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('¿Confirma que desea eliminar este paciente?')) return;
    try {
      await api.delete(`/api/v1/pacientes/${id}`);
      fetchPacientes();
    } catch (err) {
      alert(err.response?.data?.message || 'No se pudo eliminar el paciente. Puede tener citas o consultas vinculadas.');
    }
  };

  return (
    <div>
      <div className="card">
        <div className="card-header">
          <div>
            <h2 className="card-title">
              <Dog size={24} color="#0284c7" />
              Gestión de Pacientes (Mascotas)
            </h2>
            <p style={{ color: '#64748b', fontSize: '0.88rem', marginTop: '0.2rem' }}>
              Búsqueda y filtros avanzados consumiendo `/api/v1/pacientes`
            </p>
          </div>
          {canCreateOrEdit && (
            <button className="btn btn-primary" onClick={() => handleOpenModal()}>
              <Plus size={18} />
              Nuevo Paciente
            </button>
          )}
        </div>

        {/* Barra de Filtros para Endpoint */}
        <div className="filter-panel">
          <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '0.75rem', fontWeight: 600, fontSize: '0.88rem', color: '#334155' }}>
            <Filter size={16} /> Filtros de Búsqueda (API)
          </div>
          <div className="filter-grid">
            <div>
              <label style={{ fontSize: '0.75rem', color: '#64748b', fontWeight: 600 }}>Nombre</label>
              <input
                type="text"
                className="form-input"
                placeholder="Filtrar por nombre..."
                value={filters.nombre}
                onChange={(e) => setFilters({ ...filters, nombre: e.target.value })}
              />
            </div>
            <div>
              <label style={{ fontSize: '0.75rem', color: '#64748b', fontWeight: 600 }}>Especie</label>
              <select
                className="form-select"
                value={filters.especie}
                onChange={(e) => setFilters({ ...filters, especie: e.target.value })}
              >
                <option value="">Todas las especies</option>
                <option value="Canino">Canino</option>
                <option value="Felino">Felino</option>
                <option value="Ave">Ave</option>
                <option value="Conejo">Conejo</option>
                <option value="Otro">Otro</option>
              </select>
            </div>
            <div>
              <label style={{ fontSize: '0.75rem', color: '#64748b', fontWeight: 600 }}>Raza</label>
              <input
                type="text"
                className="form-input"
                placeholder="Filtrar por raza..."
                value={filters.raza}
                onChange={(e) => setFilters({ ...filters, raza: e.target.value })}
              />
            </div>
            <div>
              <label style={{ fontSize: '0.75rem', color: '#64748b', fontWeight: 600 }}>Dueño / Cliente</label>
              <input
                type="text"
                className="form-input"
                placeholder="Buscar por nombre o DNI..."
                value={filters.dueno}
                onChange={(e) => setFilters({ ...filters, dueno: e.target.value })}
              />
            </div>
            <div style={{ display: 'flex', alignItems: 'flex-end' }}>
              <button className="btn btn-secondary" style={{ width: '100%' }} onClick={handleResetFilters}>
                <RotateCcw size={15} /> Limpiar
              </button>
            </div>
          </div>
        </div>

        {loading ? (
          <p style={{ textAlign: 'center', padding: '2rem', color: '#64748b' }}>Cargando pacientes...</p>
        ) : pacientes.length === 0 ? (
          <p style={{ textAlign: 'center', padding: '2rem', color: '#94a3b8' }}>
            No se encontraron pacientes con los filtros seleccionados.
          </p>
        ) : (
          <div className="table-responsive">
            <table className="custom-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Nombre</th>
                  <th>Especie</th>
                  <th>Raza</th>
                  <th>Peso</th>
                  <th>Dueño</th>
                  <th>Alergias</th>
                  <th style={{ textAlign: 'right' }}>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {pacientes.map((p) => {
                  const cliente = clientes.find(c => c.id === p.clienteId);
                  return (
                    <tr key={p.id}>
                      <td data-label="ID" style={{ fontWeight: 600 }}>#{p.id}</td>
                      <td data-label="Nombre">
                        <strong style={{ color: '#0f172a' }}>{p.nombre}</strong>
                      </td>
                      <td data-label="Especie">
                        <span className="badge badge-primary">{p.especie}</span>
                      </td>
                      <td data-label="Raza">{p.raza || '—'}</td>
                      <td data-label="Peso">{p.peso ? `${p.peso} kg` : '—'}</td>
                      <td data-label="Dueño">{cliente ? cliente.nombreCompleto : `Cliente #${p.clienteId}`}</td>
                      <td data-label="Alergias">
                        {p.alergias ? (
                          <span className="badge badge-danger">{p.alergias}</span>
                        ) : (
                          <span style={{ color: '#94a3b8' }}>Ninguna</span>
                        )}
                      </td>
                      <td data-label="Acciones" style={{ textAlign: 'right' }}>
                        {canCreateOrEdit && (
                          <button className="btn-icon" onClick={() => handleOpenModal(p)} title="Editar" aria-label="Editar paciente">
                            <Edit2 size={16} />
                          </button>
                        )}
                        {canDelete && (
                          <button className="btn-icon danger" onClick={() => handleDelete(p.id)} title="Eliminar" aria-label="Eliminar paciente">
                            <Trash2 size={16} />
                          </button>
                        )}
                        {!canCreateOrEdit && !canDelete && (
                          <span style={{ fontSize: '0.8rem', color: '#94a3b8' }}>Solo Lectura</span>
                        )}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Modal Crear / Editar */}
      {isModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content">
            <div className="modal-header">
              <h3 style={{ fontSize: '1.2rem', fontWeight: 700, color: '#0f172a' }}>
                {editingId ? 'Editar Paciente' : 'Registrar Nuevo Paciente'}
              </h3>
              <button className="btn-icon" onClick={() => setIsModalOpen(false)}>
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleSubmit}>
              <div className="modal-body">
                {error && (
                  <div className="alert alert-danger">
                    <AlertCircle size={18} />
                    <span>{error}</span>
                  </div>
                )}

                <div className="form-group">
                  <label className="form-label">Dueño / Cliente Responsable</label>
                  <select
                    className="form-select"
                    required
                    value={formData.clienteId}
                    onChange={(e) => setFormData({ ...formData, clienteId: e.target.value })}
                  >
                    <option value="">Seleccione un cliente...</option>
                    {clientes.map(c => (
                      <option key={c.id} value={c.id}>
                        {c.nombreCompleto} (DNI: {c.documentoIdentidad})
                      </option>
                    ))}
                  </select>
                </div>

                <div className="form-grid">
                  <div className="form-group">
                    <label className="form-label">Nombre del Paciente</label>
                    <input
                      type="text"
                      className="form-input"
                      placeholder="Ej. Max, Luna, Toby"
                      required
                      value={formData.nombre}
                      onChange={(e) => setFormData({ ...formData, nombre: e.target.value })}
                    />
                  </div>

                  <div className="form-group">
                    <label className="form-label">Especie</label>
                    <select
                      className="form-select"
                      required
                      value={formData.especie}
                      onChange={(e) => setFormData({ ...formData, especie: e.target.value })}
                    >
                      <option value="Canino">Canino (Perro)</option>
                      <option value="Felino">Felino (Gato)</option>
                      <option value="Ave">Ave</option>
                      <option value="Conejo">Conejo</option>
                      <option value="Otro">Otro</option>
                    </select>
                  </div>
                </div>

                <div className="form-grid">
                  <div className="form-group">
                    <label className="form-label">Raza</label>
                    <input
                      type="text"
                      className="form-input"
                      placeholder="Ej. Golden Retriever"
                      value={formData.raza}
                      onChange={(e) => setFormData({ ...formData, raza: e.target.value })}
                    />
                  </div>

                  <div className="form-group">
                    <label className="form-label">Peso (kg)</label>
                    <input
                      type="number"
                      step="0.1"
                      className="form-input"
                      placeholder="Ej. 12.5"
                      value={formData.peso}
                      onChange={(e) => setFormData({ ...formData, peso: e.target.value })}
                    />
                  </div>
                </div>

                <div className="form-group">
                  <label className="form-label">Alergias o Condiciones Especiales</label>
                  <textarea
                    className="form-textarea"
                    rows="3"
                    placeholder="Detalle alergias a medicamentos, alimentos, vacunas..."
                    value={formData.alergias}
                    onChange={(e) => setFormData({ ...formData, alergias: e.target.value })}
                  />
                </div>
              </div>

              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={() => setIsModalOpen(false)}>
                  Cancelar
                </button>
                <button type="submit" className="btn btn-primary">
                  {editingId ? 'Guardar Cambios' : 'Registrar Paciente'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
