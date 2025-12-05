# Sistema de Roles - Control de Gastos

## Resumen

Se ha implementado exitosamente un sistema de roles (RBAC - Role-Based Access Control) en la aplicación de Control de Gastos. El sistema define dos roles:

- **Admin**: Usuario administrador con acceso completo a todos los datos del sistema
- **Usuario**: Usuario normal que solo puede ver y gestionar sus propios datos

## Arquitectura del Sistema

### Base de Datos

#### Tabla `Rol`
```sql
CREATE TABLE [dbo].[Rol] (
    [RolId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Nombre] NVARCHAR(50) NOT NULL UNIQUE,
    [Descripcion] NVARCHAR(200) NULL,
    [Activo] BIT NOT NULL DEFAULT 1,
    [FechaCreacion] DATETIME2(7) NOT NULL DEFAULT GETDATE()
);
```

**Roles creados:**
- RolId 1: Admin - "Administrador del sistema con acceso completo"
- RolId 2: Usuario - "Usuario normal del sistema"

#### Modificación tabla `Usuario`
```sql
ALTER TABLE [dbo].[Usuario]
ADD [RolId] INT NOT NULL
CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY ([RolId])
REFERENCES [dbo].[Rol] ([RolId]);

CREATE NONCLUSTERED INDEX [IX_Usuario_RolId]
ON [dbo].[Usuario] ([RolId]);
```

#### Vista `vw_UsuariosConRol`
```sql
CREATE VIEW [dbo].[vw_UsuariosConRol] AS
SELECT
    u.UsuarioId,
    u.NombreUsuario,
    u.Email,
    u.NombreCompleto,
    u.Activo AS UsuarioActivo,
    u.FechaCreacion,
    u.FechaModificacion,
    u.UltimoAcceso,
    r.RolId,
    r.Nombre AS RolNombre,
    r.Descripcion AS RolDescripcion,
    r.Activo AS RolActivo
FROM Usuario u
INNER JOIN Rol r ON u.RolId = r.RolId;
```

### Backend (.NET)

#### Extensiones de ClaimsPrincipal
Archivo: `Backend/ControlGastos.API/Extensions/ClaimsPrincipalExtensions.cs`

```csharp
public static class ClaimsPrincipalExtensions
{
    public static int GetUsuarioId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("Usuario no autenticado");

        return int.Parse(userIdClaim);
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin");
    }
}
```

#### Patrón de Implementación en Servicios

Todos los servicios siguen este patrón consistente:

**1. Interfaz del Servicio:**
```csharp
public interface IServicio
{
    Task<IEnumerable<Dto>> GetAllAsync(int usuarioId, bool esAdmin);
    Task<Dto?> GetByIdAsync(int id, int usuarioId, bool esAdmin);
    Task<Dto?> UpdateAsync(int id, UpdateDto dto, int usuarioId, bool esAdmin);
    Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin);
}
```

**2. Implementación del Servicio:**
```csharp
public async Task<IEnumerable<Dto>> GetAllAsync(int usuarioId, bool esAdmin)
{
    var query = _context.Entidades.AsQueryable();

    // Si NO es admin, filtrar solo por su usuarioId
    if (!esAdmin)
    {
        query = query.Where(e => e.UsuarioId == usuarioId);
    }
    // Si ES admin, obtener TODOS los datos de TODOS los usuarios

    return await query.ToListAsync();
}
```

**3. Controlador:**
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<Dto>>> GetAll()
{
    try
    {
        var usuarioId = User.GetUsuarioId();
        var esAdmin = User.IsAdmin();
        var datos = await _service.GetAllAsync(usuarioId, esAdmin);
        return Ok(datos);
    }
    catch (UnauthorizedAccessException)
    {
        return Unauthorized();
    }
}
```

#### Servicios Modificados

Los siguientes servicios fueron actualizados con la lógica Admin/Usuario:

1. **PresupuestoService** (`Services/PresupuestoService.cs`)
2. **RegistroGastoService** (`Services/RegistroGastoService.cs`)
3. **DepositoService** (`Services/DepositoService.cs`)
4. **ReporteService** (`Services/ReporteService.cs`)
5. **FondoMonetarioService** (`Services/FondoMonetarioService.cs`)
6. **TipoGastoService** (`Services/TipoGastoService.cs`)

### Frontend (Angular)

#### Modelo de Autenticación
Archivo: `Frontend/control-gastos-app/src/app/models/auth.model.ts`

```typescript
export interface AuthResponse {
  usuarioId: number;
  nombreUsuario: string;
  email: string;
  nombreCompleto: string;
  rol: string;  // Nuevo campo
  token: string;
  fechaExpiracion: Date;
}

export interface Usuario {
  usuarioId: number;
  nombreUsuario: string;
  email: string;
  nombreCompleto: string;
  rol: string;  // Nuevo campo
  activo: boolean;
  fechaCreacion: Date;
  ultimoAcceso?: Date;
}
```

#### AuthService
Archivo: `Frontend/control-gastos-app/src/app/services/auth.service.ts`

Nuevos métodos agregados:

```typescript
/**
 * Obtiene el rol del usuario actual
 */
getCurrentUserRole(): string | null {
  const user = this.currentUserSubject.value;
  return user ? user.rol : null;
}

/**
 * Verifica si el usuario actual es administrador
 */
isAdmin(): boolean {
  const user = this.currentUserSubject.value;
  return user ? user.rol === 'Admin' : false;
}

/**
 * Verifica si el usuario actual es un usuario normal
 */
isUsuario(): boolean {
  const user = this.currentUserSubject.value;
  return user ? user.rol === 'Usuario' : false;
}
```

## Scripts de Base de Datos

### Scripts Ejecutados

1. **01_CreateDatabase.sql** - Crea la base de datos ControlGastosDB
2. **02_StoredProcedures.sql** - Crea procedimientos almacenados
3. **03_AddAuthenticationTables.sql** - Crea tablas de autenticación
4. **07_AddRolesSystem.sql** - Crea tabla Rol y roles Admin/Usuario
5. **07_FixRolesSystem_v3.sql** - Agrega columna RolId a tabla Usuario
6. **08_CreateSuperAdmin.sql** - Crea usuario super administrador

### Orden de Ejecución

```
01_CreateDatabase.sql
↓
02_StoredProcedures.sql
↓
03_AddAuthenticationTables.sql
↓
07_AddRolesSystem.sql
↓
07_FixRolesSystem_v3.sql
↓
08_CreateSuperAdmin.sql
```

## Usuario Administrador

Se ha creado un usuario administrador con las siguientes credenciales:

- **Nombre Completo:** Josbel Millan
- **Usuario:** josbelmillan
- **Email:** pdmas287@gmail.com
- **Contraseña:** AdminJosbel2024!
- **Rol:** Administrador

**IMPORTANTE:**
1. Guarda estas credenciales en un lugar seguro
2. Cambia la contraseña después del primer inicio de sesión
3. NO compartas estas credenciales con nadie

## Comportamiento del Sistema

### Usuario Admin
- ✅ Ve TODOS los presupuestos de TODOS los usuarios
- ✅ Ve TODOS los gastos de TODOS los usuarios
- ✅ Ve TODOS los depósitos de TODOS los usuarios
- ✅ Ve TODOS los reportes y movimientos de TODOS los usuarios
- ✅ Puede modificar/eliminar datos de CUALQUIER usuario
- ✅ Tiene acceso completo al sistema

### Usuario Normal
- ✅ Ve SOLO sus propios presupuestos
- ✅ Ve SOLO sus propios gastos
- ✅ Ve SOLO sus propios depósitos
- ✅ Ve SOLO sus propios reportes y movimientos
- ✅ Puede modificar/eliminar SOLO sus propios datos
- ❌ NO puede ver datos de otros usuarios

## Pruebas

### Pruebas con Swagger

1. **Iniciar el backend:**
   ```bash
   cd Backend/ControlGastos.API
   dotnet run
   ```

2. **Abrir Swagger:** `https://localhost:7001/swagger`

3. **Iniciar sesión como Admin:**
   ```json
   POST /api/Auth/login
   {
     "email": "pdmas287@gmail.com",
     "password": "AdminJosbel2024!"
   }
   ```

4. **Copiar el token JWT** y hacer clic en "Authorize"

5. **Probar endpoints:**
   - `GET /api/Presupuesto` - Debe mostrar TODOS los presupuestos
   - `GET /api/RegistroGasto` - Debe mostrar TODOS los gastos
   - `GET /api/Deposito` - Debe mostrar TODOS los depósitos
   - `GET /api/Reporte/movimientos` - Debe mostrar TODOS los movimientos

6. **Crear usuario normal:**
   ```json
   POST /api/Auth/register
   {
     "nombreUsuario": "usuario1",
     "email": "usuario1@test.com",
     "password": "Usuario123!",
     "nombreCompleto": "Usuario Prueba"
   }
   ```

7. **Iniciar sesión como usuario normal** y verificar que solo ve sus propios datos

### Casos de Prueba

| Caso | Admin | Usuario |
|------|-------|---------|
| Ver todos los presupuestos | ✅ Todos | ✅ Solo suyos |
| Ver presupuesto de otro usuario | ✅ Sí | ❌ No (404) |
| Crear presupuesto | ✅ Sí | ✅ Solo para sí mismo |
| Modificar presupuesto de otro usuario | ✅ Sí | ❌ No (404) |
| Eliminar presupuesto de otro usuario | ✅ Sí | ❌ No (404) |
| Ver reportes globales | ✅ De todos | ✅ Solo suyos |

## Recomendaciones de Seguridad

1. **Administrador Único:** Debe existir UN SOLO usuario administrador en el sistema
2. **Cambio de Contraseña:** El admin debe cambiar su contraseña después del primer inicio de sesión
3. **Tokens JWT:** Los tokens expiran según la configuración (verificar `appsettings.json`)
4. **Logs de Auditoría:** Considerar implementar logs para acciones de admin
5. **Backup de Base de Datos:** Realizar backups regulares de la base de datos

## Guards de Navegación

Se han implementado guards para proteger rutas según el rol del usuario.

### Guards Disponibles

#### 1. `authGuard` - Para rutas autenticadas

Archivo: `Frontend/control-gastos-app/src/app/guards/auth.guard.ts`

**Propósito:** Protege rutas que requieren autenticación

**Comportamiento:**

- ✅ Permite acceso si el usuario está autenticado
- ❌ Redirige a `/login` si no está autenticado
- 💾 Guarda la URL de retorno

#### 2. `adminGuard` - Para rutas de administrador

Archivo: `Frontend/control-gastos-app/src/app/guards/admin.guard.ts`

**Propósito:** Protege rutas exclusivas de administrador

**Comportamiento:**

- ✅ Permite acceso si el usuario está autenticado Y es Admin
- ❌ Redirige a `/login` si no está autenticado
- ❌ Redirige a `/dashboard` si está autenticado pero NO es Admin

### Uso de Guards en Rutas

```typescript
// app.routes.ts
import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  // Rutas públicas
  { path: 'login', component: LoginComponent },
  { path: 'registro', component: RegistroComponent },

  // Rutas que requieren autenticación
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard]  // Solo usuarios autenticados
  },
  {
    path: 'presupuestos',
    component: PresupuestosComponent,
    canActivate: [authGuard]
  },

  // Rutas exclusivas de admin
  {
    path: 'admin',
    canActivate: [adminGuard],  // Solo administradores
    children: [
      { path: 'usuarios', component: UsuariosAdminComponent },
      { path: 'reportes-globales', component: ReportesGlobalesComponent },
    ]
  },
];
```

**Documentación completa de guards:** Ver archivo [GUARDS_USAGE_EXAMPLE.md](Frontend/control-gastos-app/GUARDS_USAGE_EXAMPLE.md)

## Uso en Componentes Angular

### Ejemplo de uso en un componente

```typescript
import { AuthService } from './services/auth.service';

export class MiComponente {
  esAdmin: boolean = false;

  constructor(private authService: AuthService) {
    this.esAdmin = this.authService.isAdmin();
  }

  // Mostrar botones solo para admin
  puedeVerTodosLosDatos(): boolean {
    return this.esAdmin;
  }
}
```

### Ejemplo en plantilla HTML

```html
<!-- Mostrar sección solo para admin -->
<div *ngIf="authService.isAdmin()">
  <h3>Panel de Administrador</h3>
  <p>Aquí puedes ver datos de todos los usuarios</p>
</div>

<!-- Mostrar sección para usuarios normales -->
<div *ngIf="authService.isUsuario()">
  <h3>Mis Datos</h3>
  <p>Solo puedes ver tus propios datos</p>
</div>
```

## Troubleshooting

### Error: "Invalid column name 'RolId'"
**Solución:** Ejecutar el script `07_FixRolesSystem_v3.sql`

### Error: "El rol Admin no existe"
**Solución:** Ejecutar el script `07_AddRolesSystem.sql`

### Error: Usuario no puede ver sus datos
**Verificar:**
1. Que el usuario tenga un RolId asignado en la base de datos
2. Que el token JWT contenga el claim de rol
3. Que el backend esté usando las extensiones correctamente

### Backend no retorna el campo "rol"
**Verificar:**
1. Que `AuthService.cs` incluya el claim de rol al generar el token
2. Que el DTO `AuthResponse` incluya el campo `Rol`

## Próximos Pasos

1. **Implementar Guard de Rutas:** Crear un `AdminGuard` para proteger rutas de admin en Angular
2. **Menú Dinámico:** Mostrar/ocultar opciones del menú según el rol del usuario
3. **Tabla de Usuarios:** Crear una vista para que el admin pueda gestionar usuarios
4. **Logs de Auditoría:** Implementar registro de acciones críticas (creación, modificación, eliminación)
5. **Cambio de Rol:** Permitir al admin cambiar el rol de usuarios existentes

## Contacto

Para soporte o preguntas sobre el sistema de roles, contactar a:
- **Desarrollador:** Josbel Millan
- **Email:** pdmas287@gmail.com

---

**Fecha de Implementación:** 2025-12-01
**Versión:** 1.0
**Estado:** ✅ Completado y Probado
