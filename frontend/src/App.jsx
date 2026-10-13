import React, { useState, useEffect } from 'react';
import { useAuth } from './context/AuthContext';
import { LoginView } from './views/LoginView';
import { DashboardView } from './views/DashboardView';
import { PacientesView } from './views/PacientesView';
import { ClientesView } from './views/ClientesView';
import { CitasView } from './views/CitasView';
import { ConsultasView } from './views/ConsultasView';
import { InsumosView } from './views/InsumosView';
import { FacturasView } from './views/FacturasView';
import { AdminConfigView } from './views/AdminConfigView';
import { AccessDeniedView } from './views/AccessDeniedView';
import { ProtectedRoute } from './components/ProtectedRoute';

import { 
  Dog, Users, Calendar, Stethoscope, Package, DollarSign, 
  Settings, LogOut, LayoutDashboard, HeartPulse, Menu, X, ShieldAlert, Lock
} from 'lucide-react';

function App() {
  const { user, loading, logout, hasRole } = useAuth();
  const [currentView, setCurrentView] = useState('dashboard');
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [forbiddenToast, setForbiddenToast] = useState(null);
  const [rateLimitToast, setRateLimitToast] = useState(null);

  // Escuchar respuestas HTTP 403 Forbidden y 429 Too Many Requests
  useEffect(() => {
    const handleForbidden = (e) => {
      setForbiddenToast(e.detail || { mensaje: 'Operación denegada por permisos de rol (HTTP 403 Forbidden)' });
    };

    const handleRateLimit = (e) => {
      setRateLimitToast(e.detail || { mensaje: 'Límite de solicitudes alcanzado. Por favor, espera unos segundos.' });
    };

    window.addEventListener('vet:forbidden', handleForbidden);
    window.addEventListener('vet:ratelimit', handleRateLimit);
    return () => {
      window.removeEventListener('vet:forbidden', handleForbidden);
      window.removeEventListener('vet:ratelimit', handleRateLimit);
    };
  }, []);

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <p style={{ color: '#0284c7', fontWeight: 600 }}>Iniciando Clínica Veterinaria...</p>
      </div>
    );
  }

  if (!user) {
    return <LoginView />;
  }

  // Definición de ítems con sus roles permitidos
  const navItems = [
    { 
      id: 'dashboard', 
      label: 'Panel Principal', 
      icon: LayoutDashboard,
      roles: ['Administrador', 'Veterinario', 'Recepcionista'] 
    },
    { 
      id: 'pacientes', 
      label: 'Pacientes (Mascotas)', 
      icon: Dog,
      roles: ['Administrador', 'Veterinario', 'Recepcionista'] 
    },
    { 
      id: 'clientes', 
      label: 'Clientes (Dueños)', 
      icon: Users,
      roles: ['Administrador', 'Veterinario', 'Recepcionista'] 
    },
    { 
      id: 'citas', 
      label: 'Agenda de Citas', 
      icon: Calendar,
      roles: ['Administrador', 'Veterinario', 'Recepcionista'] 
    },
    { 
      id: 'consultas', 
      label: 'Consultas y Recetas', 
      icon: Stethoscope,
      roles: ['Administrador', 'Veterinario', 'Recepcionista'] 
    },
    { 
      id: 'insumos', 
      label: 'Inventario / Insumos', 
      icon: Package,
      roles: ['Administrador', 'Veterinario', 'Recepcionista'] 
    },
    { 
      id: 'facturas', 
      label: 'Facturación', 
      icon: DollarSign,
      roles: ['Administrador', 'Veterinario', 'Recepcionista'] 
    },
    { 
      id: 'admin', 
      label: 'Administración', 
      icon: Settings,
      roles: ['Administrador'] // Exclusivo para Administrador
    },
  ];

  const handleSelectNav = (viewId) => {
    setCurrentView(viewId);
    setIsMobileMenuOpen(false);
  };

  const handleLogout = () => {
    setIsMobileMenuOpen(false);
    logout();
  };

  return (
    <div className="app-container">
      {/* Toast flotante de error 403 Forbidden interceptado desde la API */}
      {forbiddenToast && (
        <div style={{
          position: 'fixed',
          top: '1rem',
          right: '1rem',
          zIndex: 9999,
          maxWidth: '420px',
          width: 'calc(100% - 2rem)',
          backgroundColor: '#991b1b',
          color: '#ffffff',
          borderRadius: '12px',
          padding: '1rem 1.25rem',
          boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.3)',
          display: 'flex',
          alignItems: 'flex-start',
          gap: '0.85rem',
          animation: 'fadeIn 0.25s ease-out'
        }}>
          <ShieldAlert size={24} style={{ flexShrink: 0, marginTop: '2px' }} />
          <div style={{ flex: 1, fontSize: '0.88rem' }}>
            <div style={{ fontWeight: 700, fontSize: '0.95rem', marginBottom: '0.2rem' }}>
              403 Prohibido - Servidor API
            </div>
            <div>{forbiddenToast.mensaje}</div>
          </div>
          <button
            onClick={() => setForbiddenToast(null)}
            style={{
              background: 'transparent',
              border: 'none',
              color: '#ffffff',
              cursor: 'pointer',
              padding: '4px',
              minWidth: '32px',
              minHeight: '32px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center'
            }}
            aria-label="Cerrar notificación"
          >
            <X size={18} />
          </button>
        </div>
      )}

      {/* Toast flotante de 429 Too Many Requests */}
      {rateLimitToast && (
        <div style={{
          position: 'fixed',
          top: forbiddenToast ? '6rem' : '1rem',
          right: '1rem',
          zIndex: 9999,
          maxWidth: '420px',
          width: 'calc(100% - 2rem)',
          backgroundColor: '#d97706',
          color: '#ffffff',
          borderRadius: '12px',
          padding: '1rem 1.25rem',
          boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.3)',
          display: 'flex',
          alignItems: 'flex-start',
          gap: '0.85rem',
          animation: 'fadeIn 0.25s ease-out'
        }}>
          <ShieldAlert size={24} style={{ flexShrink: 0, marginTop: '2px' }} />
          <div style={{ flex: 1, fontSize: '0.88rem' }}>
            <div style={{ fontWeight: 700, fontSize: '0.95rem', marginBottom: '0.2rem' }}>
              429 Solicitudes Excedidas - Rate Limiter
            </div>
            <div>{rateLimitToast.mensaje}</div>
          </div>
          <button
            onClick={() => setRateLimitToast(null)}
            style={{
              background: 'transparent',
              border: 'none',
              color: '#ffffff',
              cursor: 'pointer',
              padding: '4px',
              minWidth: '32px',
              minHeight: '32px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center'
            }}
            aria-label="Cerrar notificación"
          >
            <X size={18} />
          </button>
        </div>
      )}

      {/* Backdrop para cerrar el menú en móvil */}
      <div 
        className={`sidebar-overlay ${isMobileMenuOpen ? 'visible' : ''}`}
        onClick={() => setIsMobileMenuOpen(false)}
        aria-hidden="true"
      />

      {/* Sidebar de Navegación (Off-canvas en Móvil / Sticky en Desktop) */}
      <aside className={`sidebar ${isMobileMenuOpen ? 'open' : ''}`}>
        <div className="sidebar-header">
          <div className="sidebar-brand">
            <div className="sidebar-logo-icon">
              <HeartPulse size={24} />
            </div>
            <div>
              <div className="sidebar-title">VetClinic Pro</div>
              <div style={{ fontSize: '0.72rem', color: '#94a3b8' }}>Sistema Veterinario</div>
            </div>
          </div>
          {/* Botón para cerrar drawer en móvil */}
          <button 
            className="btn-icon mobile-menu-btn" 
            style={{ color: '#cbd5e1', border: 'none' }}
            onClick={() => setIsMobileMenuOpen(false)}
            aria-label="Cerrar menú lateral"
          >
            <X size={22} />
          </button>
        </div>

        <nav className="sidebar-nav">
          {navItems.map((item) => {
            const Icon = item.icon;
            const isActive = currentView === item.id;
            const tienePermiso = hasRole(item.roles);

            return (
              <button
                key={item.id}
                className={`nav-item ${isActive ? 'active' : ''}`}
                onClick={() => handleSelectNav(item.id)}
                style={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  opacity: tienePermiso ? 1 : 0.65
                }}
                title={!tienePermiso ? `Requiere rol: ${item.roles.join(', ')}` : ''}
              >
                <div style={{ display: 'flex', alignItems: 'center', gap: '0.85rem' }}>
                  <Icon size={20} />
                  <span>{item.label}</span>
                </div>
                {!tienePermiso && (
                  <Lock size={15} color="#94a3b8" title="Módulo restringido por rol" />
                )}
              </button>
            );
          })}
        </nav>

        <div className="sidebar-footer">
          <div className="user-profile">
            <div className="user-avatar">
              {user.name ? user.name.charAt(0).toUpperCase() : 'U'}
            </div>
            <div className="user-info">
              <span className="user-name" title={user.name}>{user.name}</span>
              <span className="user-role-badge">{user.role}</span>
            </div>
          </div>
          <button
            className="btn btn-secondary btn-sm"
            style={{ width: '100%', marginTop: '0.4rem', justifyContent: 'center' }}
            onClick={handleLogout}
          >
            <LogOut size={16} /> Cerrar Sesión
          </button>
        </div>
      </aside>

      {/* Área Principal de Contenido */}
      <div className="main-wrapper">
        <header className="topbar">
          <div className="topbar-left">
            <button 
              className="mobile-menu-btn"
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              aria-label="Abrir menú de navegación"
            >
              <Menu size={22} />
            </button>
            <h1 className="topbar-title">
              {navItems.find(i => i.id === currentView)?.label || 'VetClinic Pro'}
            </h1>
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
            <span style={{ 
              fontSize: '0.78rem', 
              padding: '0.25rem 0.65rem', 
              borderRadius: '9999px',
              backgroundColor: '#e0f2fe',
              color: '#0369a1',
              fontWeight: 700
            }}>
              Rol: {user.role}
            </span>
            <span className="topbar-status">
              API Conectada: <strong style={{ color: '#10b981' }}>:5021</strong>
            </span>
          </div>
        </header>

        <main className="content-body">
          {currentView === 'dashboard' && (
            <ProtectedRoute allowedRoles={['Administrador', 'Veterinario', 'Recepcionista']} onNavigateBack={() => setCurrentView('dashboard')}>
              <DashboardView onNavigate={(view) => setCurrentView(view)} />
            </ProtectedRoute>
          )}

          {currentView === 'pacientes' && (
            <ProtectedRoute allowedRoles={['Administrador', 'Veterinario', 'Recepcionista']} onNavigateBack={() => setCurrentView('dashboard')}>
              <PacientesView />
            </ProtectedRoute>
          )}

          {currentView === 'clientes' && (
            <ProtectedRoute allowedRoles={['Administrador', 'Veterinario', 'Recepcionista']} onNavigateBack={() => setCurrentView('dashboard')}>
              <ClientesView />
            </ProtectedRoute>
          )}

          {currentView === 'citas' && (
            <ProtectedRoute allowedRoles={['Administrador', 'Veterinario', 'Recepcionista']} onNavigateBack={() => setCurrentView('dashboard')}>
              <CitasView />
            </ProtectedRoute>
          )}

          {currentView === 'consultas' && (
            <ProtectedRoute allowedRoles={['Administrador', 'Veterinario', 'Recepcionista']} onNavigateBack={() => setCurrentView('dashboard')}>
              <ConsultasView />
            </ProtectedRoute>
          )}

          {currentView === 'insumos' && (
            <ProtectedRoute allowedRoles={['Administrador', 'Veterinario', 'Recepcionista']} onNavigateBack={() => setCurrentView('dashboard')}>
              <InsumosView />
            </ProtectedRoute>
          )}

          {currentView === 'facturas' && (
            <ProtectedRoute allowedRoles={['Administrador', 'Veterinario', 'Recepcionista']} onNavigateBack={() => setCurrentView('dashboard')}>
              <FacturasView />
            </ProtectedRoute>
          )}

          {currentView === 'admin' && (
            <ProtectedRoute allowedRoles={['Administrador']} onNavigateBack={() => setCurrentView('dashboard')}>
              <AdminConfigView />
            </ProtectedRoute>
          )}
        </main>
      </div>
    </div>
  );
}

export default App;
