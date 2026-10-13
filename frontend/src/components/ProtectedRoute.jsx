import React from 'react';
import { useAuth } from '../context/AuthContext';
import { AccessDeniedView } from '../views/AccessDeniedView';

/**
 * Componente Guard / ProtectedRoute para control de acceso por roles.
 * 
 * @param {React.ReactNode} children - Vista o componente protegido.
 * @param {string[]} allowedRoles - Lista de roles permitidos (ej. ['Administrador', 'Veterinario']).
 * @param {Function} onNavigateBack - Callback para volver en caso de 403.
 */
export const ProtectedRoute = ({ children, allowedRoles = [], onNavigateBack }) => {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '50vh' }}>
        <p style={{ color: '#0284c7', fontWeight: 600 }}>Verificando credenciales de acceso...</p>
      </div>
    );
  }

  // Si no hay usuario autenticado
  if (!user) {
    return (
      <AccessDeniedView 
        requiredRoles={allowedRoles} 
        onGoBack={onNavigateBack} 
      />
    );
  }

  // Si se especificaron roles permitidos, verificar si el rol del usuario coincide
  if (allowedRoles.length > 0) {
    const userRoleNormalizado = (user.role || '').trim().toLowerCase();
    const tienePermiso = allowedRoles.some(
      (rol) => rol.trim().toLowerCase() === userRoleNormalizado
    );

    if (!tienePermiso) {
      return (
        <AccessDeniedView 
          requiredRoles={allowedRoles} 
          onGoBack={onNavigateBack} 
        />
      );
    }
  }

  // Autorizado: Renderizar vista protegida
  return <>{children}</>;
};

export default ProtectedRoute;
