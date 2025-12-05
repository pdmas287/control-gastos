# Guía de Uso de Guards - Sistema de Roles

## Guards Disponibles

### 1. `authGuard`
**Propósito:** Protege rutas que requieren autenticación (usuario logueado)
**Archivo:** `src/app/guards/auth.guard.ts`
**Comportamiento:**
- ✅ Permite acceso si el usuario está autenticado
- ❌ Redirige a `/login` si no está autenticado
- 💾 Guarda la URL de retorno para redirigir después del login

### 2. `adminGuard`
**Propósito:** Protege rutas exclusivas de administrador
**Archivo:** `src/app/guards/admin.guard.ts`
**Comportamiento:**
- ✅ Permite acceso si el usuario está autenticado Y es Admin
- ❌ Redirige a `/login` si no está autenticado
- ❌ Redirige a `/dashboard` si está autenticado pero NO es Admin

---

## Cómo Usar los Guards en las Rutas

### Ejemplo 1: Proteger rutas que requieren autenticación

```typescript
// app.routes.ts o app-routing.module.ts
import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'registro', component: RegistroComponent },

  // Rutas protegidas con authGuard - requieren estar logueado
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard]  // Solo usuarios autenticados
  },
  {
    path: 'presupuestos',
    component: PresupuestosComponent,
    canActivate: [authGuard]  // Solo usuarios autenticados
  },
  {
    path: 'gastos',
    component: GastosComponent,
    canActivate: [authGuard]  // Solo usuarios autenticados
  },
];
```

### Ejemplo 2: Proteger rutas exclusivas de admin

```typescript
// app.routes.ts o app-routing.module.ts
import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },

  // Rutas normales - requieren autenticación
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard]
  },

  // Rutas de administrador - requieren rol Admin
  {
    path: 'admin/usuarios',
    component: UsuariosAdminComponent,
    canActivate: [adminGuard]  // Solo administradores
  },
  {
    path: 'admin/reportes-globales',
    component: ReportesGlobalesComponent,
    canActivate: [adminGuard]  // Solo administradores
  },
];
```

### Ejemplo 3: Proteger un módulo completo de admin

```typescript
// app.routes.ts
import { Routes } from '@angular/router';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  // Módulo admin completo protegido
  {
    path: 'admin',
    canActivate: [adminGuard],  // Guard aplicado a todas las rutas hijas
    children: [
      { path: 'usuarios', component: UsuariosComponent },
      { path: 'reportes', component: ReportesGlobalesComponent },
      { path: 'configuracion', component: ConfiguracionComponent },
      { path: 'auditoria', component: AuditoriaComponent },
    ]
  },

  // Rutas normales
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard]
  },
];
```

### Ejemplo 4: Usar múltiples guards (NOTA: No necesario en este caso)

```typescript
// Si necesitas aplicar múltiples guards en orden
{
  path: 'admin/reportes',
  component: ReportesComponent,
  canActivate: [authGuard, adminGuard]  // Se ejecutan en orden
}

// NOTA: adminGuard ya verifica autenticación internamente,
// por lo que NO es necesario usar ambos guards.
// Solo usa adminGuard para rutas de admin.
```

---

## Estructura de Rutas Recomendada

```typescript
import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  // Rutas públicas (sin guard)
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'registro', component: RegistroComponent },

  // Rutas de usuario autenticado (authGuard)
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard]
  },
  {
    path: 'presupuestos',
    component: PresupuestosComponent,
    canActivate: [authGuard]
  },
  {
    path: 'gastos',
    component: GastosComponent,
    canActivate: [authGuard]
  },
  {
    path: 'depositos',
    component: DepositosComponent,
    canActivate: [authGuard]
  },
  {
    path: 'reportes',
    component: ReportesComponent,
    canActivate: [authGuard]
  },
  {
    path: 'perfil',
    component: PerfilComponent,
    canActivate: [authGuard]
  },

  // Rutas de administrador (adminGuard)
  {
    path: 'admin',
    canActivate: [adminGuard],
    children: [
      { path: '', redirectTo: 'usuarios', pathMatch: 'full' },
      { path: 'usuarios', component: UsuariosAdminComponent },
      { path: 'reportes-globales', component: ReportesGlobalesComponent },
      { path: 'auditoria', component: AuditoriaComponent },
      { path: 'configuracion', component: ConfiguracionSistemaComponent },
    ]
  },

  // Ruta 404
  { path: '**', redirectTo: '/dashboard' }
];
```

---

## Ejemplos de Navegación en Componentes

### Verificar rol antes de mostrar opciones de menú

```typescript
// navbar.component.ts
import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-navbar',
  template: `
    <nav>
      <a routerLink="/dashboard">Dashboard</a>
      <a routerLink="/presupuestos">Presupuestos</a>
      <a routerLink="/gastos">Gastos</a>

      <!-- Solo mostrar para admin -->
      <a *ngIf="authService.isAdmin()" routerLink="/admin/usuarios">
        Admin - Usuarios
      </a>
      <a *ngIf="authService.isAdmin()" routerLink="/admin/reportes-globales">
        Admin - Reportes Globales
      </a>

      <button (click)="authService.logout()">Cerrar Sesión</button>
    </nav>
  `
})
export class NavbarComponent {
  constructor(public authService: AuthService) {}
}
```

### Navegar programáticamente según el rol

```typescript
// login.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onLogin(credentials: any) {
    this.authService.login(credentials).subscribe({
      next: (response) => {
        // Redirigir según el rol
        if (this.authService.isAdmin()) {
          this.router.navigate(['/admin/usuarios']);
        } else {
          this.router.navigate(['/dashboard']);
        }
      },
      error: (err) => {
        console.error('Error en login:', err);
      }
    });
  }
}
```

---

## Mensajes y Notificaciones

### Mostrar mensaje cuando se deniega acceso (Opcional)

Puedes mejorar el `admin.guard.ts` para mostrar un mensaje al usuario:

```typescript
// admin.guard.ts - versión mejorada con notificación
import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
// import { NotificationService } from '../services/notification.service'; // Si tienes un servicio de notificaciones

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  // const notificationService = inject(NotificationService);

  if (!authService.isAuthenticated()) {
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  if (authService.isAdmin()) {
    return true;
  }

  // Mostrar notificación (si tienes un servicio de notificaciones)
  // notificationService.error('Acceso denegado: Se requiere rol de Administrador');

  console.warn('Acceso denegado: Se requiere rol de Administrador');
  router.navigate(['/dashboard']);
  return false;
};
```

---

## Testing de Guards

### Ejemplo de prueba unitaria para adminGuard

```typescript
// admin.guard.spec.ts
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { adminGuard } from './admin.guard';

describe('AdminGuard', () => {
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated', 'isAdmin']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    });

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('debe permitir acceso si el usuario es admin', () => {
    authService.isAuthenticated.and.returnValue(true);
    authService.isAdmin.and.returnValue(true);

    const result = TestBed.runInInjectionContext(() =>
      adminGuard({} as any, { url: '/admin/usuarios' } as any)
    );

    expect(result).toBe(true);
  });

  it('debe redirigir a login si no está autenticado', () => {
    authService.isAuthenticated.and.returnValue(false);

    const result = TestBed.runInInjectionContext(() =>
      adminGuard({} as any, { url: '/admin/usuarios' } as any)
    );

    expect(result).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/login'], {
      queryParams: { returnUrl: '/admin/usuarios' }
    });
  });

  it('debe redirigir a dashboard si está autenticado pero no es admin', () => {
    authService.isAuthenticated.and.returnValue(true);
    authService.isAdmin.and.returnValue(false);

    const result = TestBed.runInInjectionContext(() =>
      adminGuard({} as any, { url: '/admin/usuarios' } as any)
    );

    expect(result).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });
});
```

---

## Resumen

| Guard | Uso | Verifica |
|-------|-----|----------|
| `authGuard` | Rutas que requieren login | Usuario autenticado |
| `adminGuard` | Rutas exclusivas de admin | Usuario autenticado + Rol Admin |

### Cuándo usar cada guard:

- **`authGuard`**: Dashboard, Presupuestos, Gastos, Depósitos, Reportes personales, Perfil
- **`adminGuard`**: Gestión de usuarios, Reportes globales, Auditoría, Configuración del sistema

---

**Fecha de Creación:** 2025-12-01
**Versión:** 1.0
