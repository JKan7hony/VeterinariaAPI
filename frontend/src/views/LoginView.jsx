import React, { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { Stethoscope, Lock, Mail, User, ShieldAlert, ArrowRight } from 'lucide-react';

export const LoginView = () => {
  const { login, register } = useAuth();
  const [isRegister, setIsRegister] = useState(false);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [nombreCompleto, setNombreCompleto] = useState('');
  const [rolId, setRolId] = useState(1); // 1 = Administrador o predeterminado
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      if (isRegister) {
        await register({
          rolId: Number(rolId),
          nombreCompleto,
          email,
          password
        });
        // Auto-login tras registrarse
        await login(email, password);
      } else {
        await login(email, password);
      }
    } catch (err) {
      if (err.response?.status === 401) {
        setError('Credenciales incorrectas. Verifique su correo y contraseña.');
      } else if (err.response?.status === 429) {
        setError('Demasiados intentos. Por favor espere 1 minuto (Rate Limiting).');
      } else if (err.response?.data?.errors) {
        const errorMsg = Object.values(err.response.data.errors).flat().join(' ');
        setError(errorMsg || 'Error de validación en los datos enviados.');
      } else {
        setError(err.response?.data?.message || err.message || 'Error al conectar con el servidor.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{
      minHeight: '100vh',
      minHeight: '100dvh',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      background: 'linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0369a1 100%)',
      padding: 'clamp(0.75rem, 3vw, 1.5rem)'
    }}>
      <div style={{
        backgroundColor: '#ffffff',
        borderRadius: '16px',
        padding: 'clamp(1.25rem, 5vw, 2.5rem)',
        maxWidth: '440px',
        width: '100%',
        boxShadow: '0 25px 50px -12px rgba(0, 0, 0, 0.25)'
      }}>
        {/* Brand Header */}
        <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
          <div style={{
            display: 'inline-flex',
            alignItems: 'center',
            justifyContent: 'center',
            width: '56px',
            height: '56px',
            borderRadius: '14px',
            background: 'linear-gradient(135deg, #0284c7, #06b6d4)',
            color: 'white',
            marginBottom: '1rem',
            boxShadow: '0 10px 15px -3px rgba(2, 132, 199, 0.3)'
          }}>
            <Stethoscope size={30} />
          </div>
          <h2 style={{ fontSize: '1.6rem', fontWeight: 800, color: '#0f172a' }}>
            {isRegister ? 'Crear Cuenta' : 'Clínica Veterinaria'}
          </h2>
          <p style={{ color: '#64748b', fontSize: '0.9rem', marginTop: '0.25rem' }}>
            {isRegister ? 'Regístrese para gestionar pacientes y citas' : 'Ingrese sus credenciales para acceder al sistema'}
          </p>
        </div>

        {error && (
          <div className="alert alert-danger" style={{ marginBottom: '1.5rem', fontSize: '0.85rem' }}>
            <ShieldAlert size={18} style={{ flexShrink: 0 }} />
            <span>{error}</span>
          </div>
        )}

        <form onSubmit={handleSubmit}>
          {isRegister && (
            <>
              <div className="form-group">
                <label className="form-label">Nombre Completo</label>
                <div style={{ position: 'relative' }}>
                  <User size={18} style={{ position: 'absolute', left: '12px', top: '12px', color: '#94a3b8' }} />
                  <input
                    type="text"
                    className="form-input"
                    style={{ paddingLeft: '2.5rem' }}
                    placeholder="Ej. Dr. Carlos Mendoza"
                    required
                    value={nombreCompleto}
                    onChange={(e) => setNombreCompleto(e.target.value)}
                  />
                </div>
              </div>

              <div className="form-group">
                <label className="form-label">Rol del Usuario</label>
                <select
                  className="form-select"
                  value={rolId}
                  onChange={(e) => setRolId(e.target.value)}
                >
                  <option value={1}>1 - Administrador</option>
                  <option value={2}>2 - Veterinario</option>
                  <option value={3}>3 - Recepcionista</option>
                </select>
              </div>
            </>
          )}

          <div className="form-group">
            <label className="form-label">Correo Electrónico</label>
            <div style={{ position: 'relative' }}>
              <Mail size={18} style={{ position: 'absolute', left: '12px', top: '12px', color: '#94a3b8' }} />
              <input
                type="email"
                className="form-input"
                style={{ paddingLeft: '2.5rem' }}
                placeholder="veterinaria@ejemplo.com"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </div>
          </div>

          <div className="form-group" style={{ marginBottom: '1.75rem' }}>
            <label className="form-label">Contraseña</label>
            <div style={{ position: 'relative' }}>
              <Lock size={18} style={{ position: 'absolute', left: '12px', top: '12px', color: '#94a3b8' }} />
              <input
                type="password"
                className="form-input"
                style={{ paddingLeft: '2.5rem' }}
                placeholder="••••••••"
                required
                minLength={6}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>
          </div>

          <button
            type="submit"
            className="btn btn-primary"
            style={{ width: '100%', padding: '0.8rem', fontSize: '0.95rem' }}
            disabled={loading}
          >
            {loading ? 'Procesando...' : (
              <>
                {isRegister ? 'Completar Registro' : 'Iniciar Sesión'}
                <ArrowRight size={18} />
              </>
            )}
          </button>
        </form>

        <div style={{ marginTop: '1.5rem', textAlign: 'center', borderTop: '1px solid #e2e8f0', paddingTop: '1.25rem' }}>
          <button
            type="button"
            onClick={() => {
              setIsRegister(!isRegister);
              setError('');
            }}
            style={{
              background: 'none',
              border: 'none',
              color: '#0284c7',
              fontSize: '0.88rem',
              fontWeight: 600,
              cursor: 'pointer'
            }}
          >
            {isRegister ? '¿Ya tienes una cuenta? Inicia sesión aquí' : '¿No tienes cuenta? Regístrate aquí'}
          </button>
        </div>
      </div>
    </div>
  );
};
