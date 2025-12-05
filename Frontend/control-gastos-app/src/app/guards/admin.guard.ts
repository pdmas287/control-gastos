import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Guard para proteger rutas exclusivas de administrador
 * Verifica que el usuario esté autenticado Y tenga rol de Admin
 * Redirige al dashboard si el usuario no es admin o al login si no está autenticado
 */
export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Verificar si el usuario está autenticado
  if (!authService.isAuthenticated()) {
    // Redirigir al login y guardar la URL a la que intentaba acceder
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  // Verificar si el usuario tiene rol de Admin
  if (authService.isAdmin()) {
    return true;
  }

  // Si no es admin, redirigir al dashboard con mensaje
  console.warn('Acceso denegado: Se requiere rol de Administrador');
  router.navigate(['/dashboard']);
  return false;
};
