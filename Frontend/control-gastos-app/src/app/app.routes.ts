import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  // Landing Page como página principal
  {
    path: '',
    loadComponent: () => import('./components/landing/landing.component').then(m => m.LandingComponent)
  },

  // Rutas públicas (sin autenticación)
  {
    path: 'login',
    loadComponent: () => import('./components/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'registro',
    loadComponent: () => import('./components/auth/registro/registro.component').then(m => m.RegistroComponent)
  },

  // Rutas de administrador (requieren rol Admin)
  {
    path: 'admin',
    loadComponent: () => import('./components/admin/admin-layout.component').then(m => m.AdminLayoutComponent),
    canActivate: [adminGuard],
    children: [
      {
        path: '',
        redirectTo: 'usuarios',
        pathMatch: 'full'
      },
      {
        path: 'usuarios',
        loadComponent: () => import('./components/admin/usuarios-admin.component').then(m => m.UsuariosAdminComponent)
      },
      {
        path: 'reportes-globales',
        loadComponent: () => import('./components/admin/reportes-globales.component').then(m => m.ReportesGlobalesComponent)
      }
    ]
  },

  // Rutas protegidas (requieren autenticación)
  {
    path: 'home',
    loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent),
    canActivate: [authGuard]
  },
  {
    path: 'tipos-gasto',
    loadComponent: () => import('./components/tipo-gasto/tipo-gasto-list/tipo-gasto-list.component').then(m => m.TipoGastoListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'fondos-monetarios',
    loadComponent: () => import('./components/fondo-monetario/fondo-monetario-list/fondo-monetario-list.component').then(m => m.FondoMonetarioListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'presupuestos',
    loadComponent: () => import('./components/presupuesto/presupuesto-form/presupuesto-form.component').then(m => m.PresupuestoFormComponent),
    canActivate: [authGuard]
  },
  {
    path: 'registro-gastos',
    loadComponent: () => import('./components/registro-gasto/registro-gasto-form/registro-gasto-form.component').then(m => m.RegistroGastoFormComponent),
    canActivate: [authGuard]
  },
  {
    path: 'depositos',
    loadComponent: () => import('./components/deposito/deposito-form/deposito-form.component').then(m => m.DepositoFormComponent),
    canActivate: [authGuard]
  },
  {
    path: 'consulta-movimientos',
    loadComponent: () => import('./components/reportes/consulta-movimientos/consulta-movimientos.component').then(m => m.ConsultaMovimientosComponent),
    canActivate: [authGuard]
  },
  {
    path: 'grafico-comparativo',
    loadComponent: () => import('./components/reportes/grafico-comparativo/grafico-comparativo.component').then(m => m.GraficoComparativoComponent),
    canActivate: [authGuard]
  },

  // Ruta 404 - Redirigir a la landing page
  { path: '**', redirectTo: '' }
];
