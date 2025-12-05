# Componentes de Administración - Control de Gastos

## Resumen

Se han creado componentes de administración para que los usuarios con rol **Admin** puedan gestionar el sistema completo.

## Componentes Creados

### 1. AdminLayoutComponent
**Archivo:** `src/app/components/admin/admin-layout.component.ts`

**Propósito:** Layout principal del panel de administración con sidebar y navegación.

**Características:**
- Sidebar colapsable con navegación
- Muestra información del usuario actual
- Enlaces a las secciones de administración
- Responsive (móvil y desktop)

**Rutas internas:**
- `/admin/usuarios` - Gestión de usuarios
- `/admin/reportes-globales` - Reportes de todos los usuarios

---

### 2. UsuariosAdminComponent
**Archivo:** `src/app/components/admin/usuarios-admin.component.ts`

**Propósito:** Gestión completa de usuarios del sistema.

**Características:**
- ✅ **Vista de todos los usuarios** - Tabla con información completa
- ✅ **Estadísticas** - Resumen de usuarios activos, inactivos, por rol
- ✅ **Filtros avanzados** - Por texto, rol y estado
- ✅ **Activar/Desactivar usuarios** - Control de acceso
- ✅ **Cambiar roles** - Promover usuarios a admin o viceversa

**Modelo de datos:**
```typescript
interface UsuarioAdmin {
  usuarioId: number;
  nombreUsuario: string;
  email: string;
  nombreCompleto: string;
  rol: string;
  activo: boolean;
  fechaCreacion: Date;
  fechaModificacion?: Date;
  ultimoAcceso?: Date;
}
```

**Servicio:**
- `UsuarioAdminService` - Comunicación con el backend
- **Endpoints esperados** (por implementar en backend):
  - `GET /api/usuario` - Obtener todos los usuarios
  - `GET /api/usuario/{id}` - Obtener usuario por ID
  - `PUT /api/usuario/{id}` - Actualizar usuario
  - `PUT /api/usuario/{id}/activar` - Activar usuario
  - `PUT /api/usuario/{id}/desactivar` - Desactivar usuario
  - `PUT /api/usuario/{id}/cambiar-rol` - Cambiar rol de usuario
  - `GET /api/usuario/estadisticas` - Estadísticas de usuarios

**Acciones disponibles:**
- Activar usuario inactivo
- Desactivar usuario activo
- Cambiar rol (Admin ↔ Usuario)

---

### 3. ReportesGlobalesComponent
**Archivo:** `src/app/components/admin/reportes-globales.component.ts`

**Propósito:** Vista de reportes agregados de todos los usuarios.

**Características:**
- ✅ **Filtros por fecha** - Selección de período
- ✅ **Resumen financiero** - Total gastos, depósitos, balance
- ✅ **Visualización clara** - Cards con iconos y colores
- 📝 **Nota**: Actualmente muestra datos de ejemplo, requiere integración con servicio de reportes

**Integración sugerida:**
```typescript
// Conectar con los servicios de reportes existentes
this.reporteService.getMovimientosAsync(fechaInicio, fechaFin).subscribe({
  next: (data) => {
    // Procesar datos globales
    this.calcularResumen(data);
  }
});
```

---

## Estructura de Archivos

```
src/app/
├── components/
│   └── admin/
│       ├── admin-layout.component.ts        # Layout principal
│       ├── admin-layout.component.html
│       ├── admin-layout.component.css
│       ├── usuarios-admin.component.ts      # Gestión de usuarios
│       ├── usuarios-admin.component.html
│       ├── usuarios-admin.component.css
│       ├── reportes-globales.component.ts   # Reportes globales
│       ├── reportes-globales.component.html
│       └── reportes-globales.component.css
├── models/
│   └── usuario-admin.model.ts               # Modelos de datos
├── services/
│   └── usuario-admin.service.ts             # Servicio HTTP
└── guards/
    ├── auth.guard.ts                         # Guard de autenticación
    └── admin.guard.ts                        # Guard de admin
```

---

## Rutas Configuradas

```typescript
// app.routes.ts
{
  path: 'admin',
  loadComponent: () => import('./components/admin/admin-layout.component'),
  canActivate: [adminGuard],  // Solo admins
  children: [
    {
      path: '',
      redirectTo: 'usuarios',
      pathMatch: 'full'
    },
    {
      path: 'usuarios',
      loadComponent: () => import('./components/admin/usuarios-admin.component')
    },
    {
      path: 'reportes-globales',
      loadComponent: () => import('./components/admin/reportes-globales.component')
    }
  ]
}
```

---

## Protección de Rutas

Las rutas de admin están protegidas por el `adminGuard`:

```typescript
// admin.guard.ts
export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Verificar autenticación
  if (!authService.isAuthenticated()) {
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  // Verificar rol Admin
  if (authService.isAdmin()) {
    return true;
  }

  // Redirigir si no es admin
  router.navigate(['/dashboard']);
  return false;
};
```

---

## Navegación

### Acceso al panel de admin

1. **Desde cualquier componente:**
```typescript
this.router.navigate(['/admin']);
```

2. **Desde HTML:**
```html
<a routerLink="/admin">Panel Admin</a>
```

3. **Condicional según rol:**
```html
<a *ngIf="authService.isAdmin()" routerLink="/admin">
  Panel Admin
</a>
```

### Navegación dentro del panel

El `AdminLayoutComponent` proporciona un sidebar con enlaces:
- Gestión de Usuarios
- Reportes Globales
- Volver al Dashboard
- Cerrar Sesión

---

## Integración con Backend

### Endpoints Requeridos (Por implementar)

#### 1. Gestión de Usuarios

```csharp
// UsuarioController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]  // Solo admin
public class UsuarioController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAllUsuarios()
    {
        // Devolver todos los usuarios
    }

    [HttpPut("{id}/activar")]
    public async Task<ActionResult> ActivarUsuario(int id)
    {
        // Activar usuario
    }

    [HttpPut("{id}/desactivar")]
    public async Task<ActionResult> DesactivarUsuario(int id)
    {
        // Desactivar usuario
    }

    [HttpPut("{id}/cambiar-rol")]
    public async Task<ActionResult> CambiarRol(int id, [FromBody] CambiarRolDto dto)
    {
        // Cambiar rol del usuario
    }

    [HttpGet("estadisticas")]
    public async Task<ActionResult<EstadisticasUsuarios>> GetEstadisticas()
    {
        // Devolver estadísticas
    }
}
```

#### 2. Reportes ya implementados

Los reportes ya funcionan con el sistema de roles:
- `GET /api/Reporte/movimientos` - Admin ve todos, Usuario ve solo suyos
- `GET /api/Reporte/comparativo-presupuesto` - Admin ve todos, Usuario ve solo suyos

---

## Personalización

### Agregar nuevas secciones de admin

1. **Crear el componente:**
```bash
ng generate component components/admin/nueva-seccion --standalone
```

2. **Agregar ruta en `app.routes.ts`:**
```typescript
{
  path: 'admin',
  children: [
    // ... otras rutas
    {
      path: 'nueva-seccion',
      loadComponent: () => import('./components/admin/nueva-seccion.component')
    }
  ]
}
```

3. **Agregar enlace en el sidebar** (`admin-layout.component.html`):
```html
<a routerLink="/admin/nueva-seccion" routerLinkActive="active" class="nav-item">
  <span class="icon">🔧</span>
  <span class="text">Nueva Sección</span>
</a>
```

---

## Estilos y Diseño

### Paleta de colores

- **Sidebar:** `#2c3e50` (azul oscuro)
- **Hover:** `#34495e` (gris azulado)
- **Activo:** `#3498db` (azul)
- **Admin badge:** `#dc3545` (rojo)
- **Usuario badge:** `#6c757d` (gris)
- **Activo:** `#28a745` (verde)
- **Inactivo:** `#ffc107` (amarillo)

### Responsive

Los componentes son completamente responsive:
- **Desktop:** Sidebar fijo a la izquierda
- **Mobile:** Sidebar colapsable con botón hamburguesa
- **Tablets:** Adaptación automática

---

## Testing

### Verificar acceso como Admin

1. Iniciar sesión como admin (josbelmillan / AdminJosbel2024!)
2. Navegar a `http://localhost:4200/admin`
3. Verificar que se muestra el panel de administración
4. Probar filtros y acciones en gestión de usuarios

### Verificar restricción para usuarios normales

1. Iniciar sesión como usuario normal
2. Intentar acceder a `http://localhost:4200/admin`
3. Verificar que redirige a `/dashboard`
4. Verificar que no se muestran enlaces de admin en el menú

---

## Próximos Pasos Sugeridos

1. **Implementar endpoints de gestión de usuarios en el backend**
2. **Conectar ReportesGlobalesComponent con el servicio real de reportes**
3. **Agregar componente de auditoría** - Registrar acciones de admin
4. **Agregar componente de configuración del sistema**
5. **Implementar paginación** en la tabla de usuarios
6. **Agregar gráficos** en reportes globales con Chart.js o similar

---

## Soporte

Para más información sobre el sistema de roles, consultar:
- [SISTEMA_ROLES.md](../../SISTEMA_ROLES.md) - Documentación completa del sistema de roles
- [GUARDS_USAGE_EXAMPLE.md](GUARDS_USAGE_EXAMPLE.md) - Ejemplos de uso de guards

---

**Fecha de Creación:** 2025-12-01
**Versión:** 1.0
**Estado:** ✅ Completado (Pendiente: Implementar endpoints de backend)
