# Guía de Implementación del Sistema de Autenticación

## 🎯 Resumen

Se ha implementado un sistema completo de autenticación con JWT para el proyecto Control de Gastos. Ahora cada usuario puede registrarse, iniciar sesión y ver solo su propia información.

---

## ✅ COMPLETADO - Backend

### 1. Base de Datos
- ✅ **Tabla Usuario** creada con campos: UsuarioId, NombreUsuario, Email, PasswordHash, NombreCompleto, Activo, FechaCreacion, UltimoAcceso
- ✅ **Columna UsuarioId** agregada a todas las tablas: TipoGasto, FondoMonetario, Presupuesto, RegistroGastoEncabezado, Deposito
- ✅ **Índices** creados para mejorar rendimiento
- ✅ **Constraints únicos** actualizados para incluir UsuarioId

**Archivo:** [Database/03_AddAuthenticationTables.sql](Database/03_AddAuthenticationTables.sql)

### 2. Modelos
- ✅ **Usuario.cs** - Modelo principal de usuario
- ✅ Todos los modelos existentes actualizados con relación a Usuario
- ✅ **AuthDto.cs** - DTOs para registro, login, y respuestas de autenticación

### 3. Servicios
- ✅ **IAuthService.cs** / **AuthService.cs** - Lógica de autenticación
  - Registro de usuarios
  - Login con generación de JWT
  - Hash de contraseñas con SHA256
  - Cambio de contraseña
  - Validación de tokens

### 4. Controllers
- ✅ **AuthController.cs** - Endpoints de autenticación
  - `POST /api/auth/registro` - Registrar nuevo usuario
  - `POST /api/auth/login` - Iniciar sesión
  - `GET /api/auth/perfil` - Obtener perfil del usuario (requiere autenticación)
  - `PUT /api/auth/cambiar-password` - Cambiar contraseña (requiere autenticación)
  - `GET /api/auth/verificar-token` - Verificar validez del token

### 5. Configuración
- ✅ **Program.cs** actualizado con:
  - Middleware de autenticación JWT
  - Configuración de tokens
  - Registro del servicio AuthService
- ✅ **appsettings.json** con configuración JWT

---

## 📋 PENDIENTE - Completar Implementación

### Backend - Modificar Servicios Existentes

**IMPORTANTE:** Todos los servicios existentes deben ser modificados para:
1. Recibir el `usuarioId` del usuario autenticado
2. Filtrar todos los datos por `UsuarioId`
3. Asignar automáticamente el `UsuarioId` al crear nuevos registros

#### Ejemplo de modificación necesaria:

**Antes (TipoGastoService.cs):**
```csharp
public async Task<List<TipoGastoDto>> ObtenerTodosAsync()
{
    return await _context.TiposGasto
        .Where(t => t.Activo)
        .Select(t => new TipoGastoDto { ... })
        .ToListAsync();
}
```

**Después:**
```csharp
public async Task<List<TipoGastoDto>> ObtenerTodosAsync(int usuarioId)
{
    return await _context.TiposGasto
        .Where(t => t.Activo && t.UsuarioId == usuarioId)
        .Select(t => new TipoGastoDto { ... })
        .ToListAsync();
}
```

#### Controladores que necesitan modificación:
1. **TipoGastoController.cs** - Obtener usuarioId de Claims y pasar a servicios
2. **FondoMonetarioController.cs** - Filtrar fondos por usuario
3. **PresupuestoController.cs** - Filtrar presupuestos por usuario
4. **RegistroGastoController.cs** - Filtrar gastos por usuario
5. **DepositoController.cs** - Filtrar depósitos por usuario
6. **ReporteController.cs** - Filtrar reportes por usuario

#### Ejemplo de cómo obtener el usuarioId en un Controller:

```csharp
[HttpGet]
[Authorize] // ← Agregar este atributo
public async Task<ActionResult<List<TipoGastoDto>>> ObtenerTodos()
{
    // Obtener el ID del usuario autenticado
    var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
    {
        return Unauthorized();
    }

    var tipos = await _tipoGastoService.ObtenerTodosAsync(usuarioId);
    return Ok(tipos);
}
```

---

## 🚀 Frontend - Angular (Implementación Completa Necesaria)

### 1. Modelos e Interfaces

**Crear:** `Frontend/control-gastos-app/src/app/models/auth.model.ts`

```typescript
export interface RegistroUsuario {
  nombreUsuario: string;
  email: string;
  password: string;
  nombreCompleto: string;
}

export interface Login {
  nombreUsuarioOEmail: string;
  password: string;
}

export interface AuthResponse {
  usuarioId: number;
  nombreUsuario: string;
  email: string;
  nombreCompleto: string;
  token: string;
  fechaExpiracion: Date;
}

export interface Usuario {
  usuarioId: number;
  nombreUsuario: string;
  email: string;
  nombreCompleto: string;
  activo: boolean;
  fechaCreacion: Date;
  ultimoAcceso?: Date;
}

export interface CambiarPassword {
  passwordActual: string;
  nuevaPassword: string;
}
```

### 2. Servicio de Autenticación

**Crear:** `Frontend/control-gastos-app/src/app/services/auth.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResponse, Login, RegistroUsuario, Usuario } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5000/api/auth';
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(this.getUserFromStorage());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  private getUserFromStorage(): AuthResponse | null {
    const userJson = localStorage.getItem('currentUser');
    return userJson ? JSON.parse(userJson) : null;
  }

  registro(datos: RegistroUsuario): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/registro`, datos).pipe(
      tap(response => this.setCurrentUser(response))
    );
  }

  login(datos: Login): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, datos).pipe(
      tap(response => this.setCurrentUser(response))
    );
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  private setCurrentUser(user: AuthResponse): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  getToken(): string | null {
    const user = this.currentUserSubject.value;
    return user ? user.token : null;
  }

  isAuthenticated(): boolean {
    const user = this.currentUserSubject.value;
    if (!user) return false;

    const expiration = new Date(user.fechaExpiracion);
    return expiration > new Date();
  }

  getPerfil(): Observable<Usuario> {
    return this.http.get<Usuario>(`${this.apiUrl}/perfil`);
  }

  cambiarPassword(datos: CambiarPassword): Observable<any> {
    return this.http.put(`${this.apiUrl}/cambiar-password`, datos);
  }
}
```

### 3. HTTP Interceptor para JWT

**Crear:** `Frontend/control-gastos-app/src/app/interceptors/auth.interceptor.ts`

```typescript
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};
```

**Registrar en:** `Frontend/control-gastos-app/src/app/app.config.ts` o `main.ts`

```typescript
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([authInterceptor])),
    // ... otros providers
  ]
};
```

### 4. Auth Guard

**Crear:** `Frontend/control-gastos-app/src/app/guards/auth.guard.ts`

```typescript
import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
```

### 5. Componente de Login

**Crear:** `Frontend/control-gastos-app/src/app/components/auth/login/login.component.ts`

```typescript
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { Login } from '../../../models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData: Login = {
    nombreUsuarioOEmail: '',
    password: ''
  };
  error = '';
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    this.error = '';
    this.loading = true;

    this.authService.login(this.loginData).subscribe({
      next: () => {
        this.router.navigate(['/home']);
      },
      error: (err) => {
        this.error = err.error?.message || 'Error al iniciar sesión';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}
```

**Crear:** `Frontend/control-gastos-app/src/app/components/auth/login/login.component.html`

```html
<div class="login-container">
  <div class="login-card">
    <h2>Iniciar Sesión</h2>

    <form (ngSubmit)="onSubmit()" #loginForm="ngForm">
      <div class="form-group">
        <label for="nombreUsuarioOEmail">Usuario o Email</label>
        <input
          type="text"
          id="nombreUsuarioOEmail"
          [(ngModel)]="loginData.nombreUsuarioOEmail"
          name="nombreUsuarioOEmail"
          required
          [disabled]="loading"
        />
      </div>

      <div class="form-group">
        <label for="password">Contraseña</label>
        <input
          type="password"
          id="password"
          [(ngModel)]="loginData.password"
          name="password"
          required
          [disabled]="loading"
        />
      </div>

      <div *ngIf="error" class="error-message">
        {{ error }}
      </div>

      <button type="submit" [disabled]="!loginForm.valid || loading">
        {{ loading ? 'Iniciando sesión...' : 'Iniciar Sesión' }}
      </button>
    </form>

    <p class="register-link">
      ¿No tienes cuenta? <a routerLink="/registro">Regístrate aquí</a>
    </p>
  </div>
</div>
```

### 6. Componente de Registro

**Crear:** `Frontend/control-gastos-app/src/app/components/auth/registro/registro.component.ts`

```typescript
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { RegistroUsuario } from '../../../models/auth.model';

@Component({
  selector: 'app-registro',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './registro.component.html',
  styleUrls: ['./registro.component.css']
})
export class RegistroComponent {
  registroData: RegistroUsuario = {
    nombreUsuario: '',
    email: '',
    password: '',
    nombreCompleto: ''
  };
  confirmPassword = '';
  error = '';
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    if (this.registroData.password !== this.confirmPassword) {
      this.error = 'Las contraseñas no coinciden';
      return;
    }

    this.error = '';
    this.loading = true;

    this.authService.registro(this.registroData).subscribe({
      next: () => {
        this.router.navigate(['/home']);
      },
      error: (err) => {
        this.error = err.error?.message || 'Error al registrarse';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}
```

### 7. Actualizar Rutas

**Modificar:** `Frontend/control-gastos-app/src/app/app.routes.ts`

```typescript
import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { LoginComponent } from './components/auth/login/login.component';
import { RegistroComponent } from './components/auth/registro/registro.component';
// ... otros imports

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'registro', component: RegistroComponent },

  // Rutas protegidas
  {
    path: 'home',
    loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent),
    canActivate: [authGuard]
  },
  {
    path: 'tipo-gasto',
    loadComponent: () => import('./components/tipo-gasto/tipo-gasto-list/tipo-gasto-list.component').then(m => m.TipoGastoListComponent),
    canActivate: [authGuard]
  },
  // ... todas las demás rutas con canActivate: [authGuard]
];
```

### 8. Actualizar App Component con Logout

**Modificar:** `Frontend/control-gastos-app/src/app/app.component.ts`

Agregar botón de logout y mostrar nombre de usuario en la navegación.

---

## 📝 Pasos para Ejecutar la Implementación

### 1. Base de Datos
```sql
-- Ejecutar en SQL Server Management Studio
USE ControlGastosDB;
GO

-- Ejecutar el script de autenticación
-- Ruta: Database/03_AddAuthenticationTables.sql
```

### 2. Backend
```bash
cd Backend/ControlGastos.API

# Restaurar paquetes NuGet si es necesario
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run
```

### 3. Frontend (después de implementar los componentes)
```bash
cd Frontend/control-gastos-app

# Instalar dependencias
npm install

# Ejecutar en modo desarrollo
ng serve
```

### 4. Probar el Sistema

1. Abrir navegador en `http://localhost:4200`
2. Debería redirigir a `/login`
3. Registrar un nuevo usuario
4. Iniciar sesión
5. Verificar que todas las funcionalidades funcionan con datos filtrados por usuario

---

## 🔐 Seguridad Implementada

- ✅ Contraseñas hasheadas con SHA256
- ✅ Tokens JWT con expiración de 7 días
- ✅ Protección de endpoints con `[Authorize]`
- ✅ Validación de modelos en Backend
- ✅ Guard de autenticación en Frontend
- ✅ Interceptor automático para agregar tokens

---

## 📌 Notas Importantes

1. **Migración de Datos Existentes:** Si ya tienes datos en la base de datos, necesitarás asignar manualmente un UsuarioId a todos los registros existentes después de ejecutar el script.

2. **Clave JWT:** La clave en `appsettings.json` es para desarrollo. En producción, usa una clave más segura y guárdala en variables de entorno.

3. **Hash de Contraseñas:** Actualmente usa SHA256. Para mayor seguridad en producción, considera usar `BCrypt` o `PBKDF2`.

4. **CORS:** Asegúrate de que el puerto del frontend (4200) está permitido en el backend.

---

## 🆘 Soporte

Si tienes problemas durante la implementación:
1. Verifica que SQL Server esté corriendo
2. Revisa los logs del backend en la consola
3. Usa las herramientas de desarrollador del navegador para ver errores
4. Verifica que Swagger funcione: `http://localhost:5000/swagger`

---

**¡La implementación del Backend está completa!**
**Ahora necesitas completar el Frontend siguiendo las instrucciones anteriores.**
