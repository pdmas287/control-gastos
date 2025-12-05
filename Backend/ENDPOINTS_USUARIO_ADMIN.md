# Endpoints de Gestión de Usuarios (Admin)

## Resumen

Endpoints para que los administradores gestionen usuarios del sistema. Todos los endpoints requieren autenticación JWT y rol de **Admin**.

**Base URL:** `http://localhost:5000/api/usuario`

---

## Autenticación

Todos los endpoints requieren:
- **Header:** `Authorization: Bearer {token}`
- **Rol:** Admin

Si el usuario no es admin, recibirá un `403 Forbidden`.

---

## Endpoints Disponibles

### 1. Obtener Todos los Usuarios

**GET** `/api/usuario`

Obtiene la lista completa de usuarios del sistema.

**Respuesta exitosa (200):**
```json
[
  {
    "usuarioId": 1,
    "nombreUsuario": "josbelmillan",
    "email": "pdmas287@gmail.com",
    "nombreCompleto": "Josbel Millan",
    "rol": "Admin",
    "activo": true,
    "fechaCreacion": "2025-12-01T12:00:00",
    "fechaModificacion": null,
    "ultimoAcceso": "2025-12-01T14:30:00"
  },
  {
    "usuarioId": 2,
    "nombreUsuario": "usuario1",
    "email": "usuario1@test.com",
    "nombreCompleto": "Usuario Prueba",
    "rol": "Usuario",
    "activo": true,
    "fechaCreacion": "2025-12-01T13:00:00",
    "fechaModificacion": null,
    "ultimoAcceso": null
  }
]
```

---

### 2. Obtener Usuario por ID

**GET** `/api/usuario/{id}`

Obtiene la información de un usuario específico.

**Parámetros de ruta:**
- `id` (int): ID del usuario

**Respuesta exitosa (200):**
```json
{
  "usuarioId": 1,
  "nombreUsuario": "josbelmillan",
  "email": "pdmas287@gmail.com",
  "nombreCompleto": "Josbel Millan",
  "rol": "Admin",
  "activo": true,
  "fechaCreacion": "2025-12-01T12:00:00",
  "fechaModificacion": null,
  "ultimoAcceso": "2025-12-01T14:30:00"
}
```

**Respuesta de error (404):**
```json
{
  "message": "Usuario no encontrado"
}
```

---

### 3. Actualizar Usuario

**PUT** `/api/usuario/{id}`

Actualiza la información de un usuario.

**Parámetros de ruta:**
- `id` (int): ID del usuario

**Body (JSON):**
```json
{
  "nombreUsuario": "nuevo_usuario",
  "email": "nuevo@email.com",
  "nombreCompleto": "Nuevo Nombre Completo",
  "rol": "Admin",
  "activo": true
}
```

Todos los campos son opcionales. Solo se actualizarán los campos proporcionados.

**Respuesta exitosa (200):**
```json
{
  "usuarioId": 2,
  "nombreUsuario": "nuevo_usuario",
  "email": "nuevo@email.com",
  "nombreCompleto": "Nuevo Nombre Completo",
  "rol": "Admin",
  "activo": true,
  "fechaCreacion": "2025-12-01T13:00:00",
  "fechaModificacion": "2025-12-01T15:00:00",
  "ultimoAcceso": null
}
```

**Respuesta de error (400):**
```json
{
  "message": "El nombre de usuario ya está en uso"
}
```

**Respuesta de error (404):**
```json
{
  "message": "Usuario no encontrado"
}
```

---

### 4. Activar Usuario

**PUT** `/api/usuario/{id}/activar`

Activa un usuario inactivo.

**Parámetros de ruta:**
- `id` (int): ID del usuario

**Body:** Ninguno

**Respuesta exitosa (200):**
```json
{
  "message": "Usuario activado exitosamente"
}
```

**Respuesta de error (404):**
```json
{
  "message": "Usuario no encontrado"
}
```

---

### 5. Desactivar Usuario

**PUT** `/api/usuario/{id}/desactivar`

Desactiva un usuario (impide que inicie sesión).

**Parámetros de ruta:**
- `id` (int): ID del usuario

**Body:** Ninguno

**Respuesta exitosa (200):**
```json
{
  "message": "Usuario desactivado exitosamente"
}
```

**Respuesta de error (404):**
```json
{
  "message": "Usuario no encontrado"
}
```

---

### 6. Cambiar Rol de Usuario

**PUT** `/api/usuario/{id}/cambiar-rol`

Cambia el rol de un usuario (Admin ↔ Usuario).

**Parámetros de ruta:**
- `id` (int): ID del usuario

**Body (JSON):**
```json
{
  "nuevoRol": "Admin"
}
```

Valores válidos para `nuevoRol`: `"Admin"` o `"Usuario"`

**Respuesta exitosa (200):**
```json
{
  "message": "Rol cambiado a Admin exitosamente"
}
```

**Respuesta de error (400):**
```json
{
  "message": "El rol debe ser 'Admin' o 'Usuario'"
}
```

**Respuesta de error (404):**
```json
{
  "message": "Usuario o rol no encontrado"
}
```

---

### 7. Obtener Estadísticas

**GET** `/api/usuario/estadisticas`

Obtiene estadísticas generales de usuarios.

**Respuesta exitosa (200):**
```json
{
  "totalUsuarios": 10,
  "usuariosActivos": 8,
  "usuariosInactivos": 2,
  "administradores": 1,
  "usuariosNormales": 9
}
```

---

### 8. Eliminar Usuario

**DELETE** `/api/usuario/{id}`

Elimina un usuario del sistema. Usar con precaución.

**Parámetros de ruta:**
- `id` (int): ID del usuario

**Restricciones:**
- No se puede eliminar el último administrador activo del sistema

**Respuesta exitosa (200):**
```json
{
  "message": "Usuario eliminado exitosamente"
}
```

**Respuesta de error (400):**
```json
{
  "message": "No se puede eliminar el último administrador activo del sistema"
}
```

**Respuesta de error (404):**
```json
{
  "message": "Usuario no encontrado"
}
```

---

## Ejemplos de Uso con cURL

### Obtener todos los usuarios
```bash
curl -X GET "http://localhost:5000/api/usuario" \
  -H "Authorization: Bearer {token}"
```

### Activar usuario
```bash
curl -X PUT "http://localhost:5000/api/usuario/2/activar" \
  -H "Authorization: Bearer {token}"
```

### Cambiar rol
```bash
curl -X PUT "http://localhost:5000/api/usuario/2/cambiar-rol" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"nuevoRol": "Admin"}'
```

### Obtener estadísticas
```bash
curl -X GET "http://localhost:5000/api/usuario/estadisticas" \
  -H "Authorization: Bearer {token}"
```

---

## Códigos de Estado HTTP

| Código | Descripción |
|--------|-------------|
| 200 | Operación exitosa |
| 400 | Bad Request - Datos inválidos |
| 401 | Unauthorized - Token inválido o expirado |
| 403 | Forbidden - Usuario no tiene rol de Admin |
| 404 | Not Found - Usuario no encontrado |
| 500 | Internal Server Error - Error del servidor |

---

## Integración con Frontend

El frontend ya tiene el servicio `UsuarioAdminService` configurado para consumir estos endpoints:

```typescript
// usuario-admin.service.ts
getAllUsuarios(): Observable<UsuarioAdmin[]>
getUsuarioById(id: number): Observable<UsuarioAdmin>
updateUsuario(id: number, usuario: UsuarioUpdateAdmin): Observable<UsuarioAdmin>
activarUsuario(id: number): Observable<any>
desactivarUsuario(id: number): Observable<any>
cambiarRol(dto: CambiarRolDto): Observable<any>
getEstadisticas(): Observable<EstadisticasUsuarios>
deleteUsuario(id: number): Observable<any>
```

---

## Pruebas con Swagger

1. Iniciar el backend:
```bash
cd Backend/ControlGastos.API
dotnet run
```

2. Abrir Swagger: `https://localhost:7001/swagger`

3. Iniciar sesión como admin:
   - Endpoint: `POST /api/Auth/login`
   - Credenciales:
     ```json
     {
       "nombreUsuarioOEmail": "pdmas287@gmail.com",
       "password": "AdminJosbel2024!"
     }
     ```

4. Copiar el token JWT

5. Hacer clic en "Authorize" en Swagger

6. Pegar: `Bearer {token}`

7. Probar los endpoints de `/api/usuario`

---

## Notas de Seguridad

1. **Solo Admin:** Todos los endpoints están protegidos con `[Authorize(Roles = "Admin")]`
2. **Último Admin:** No se puede eliminar ni desactivar el último administrador activo
3. **Validaciones:** Se validan emails y nombres de usuario duplicados
4. **Auditoría:** Se actualiza `FechaModificacion` en cada cambio

---

## Troubleshooting

### Error 403 Forbidden
- Verificar que el usuario tenga rol "Admin"
- Verificar que el token JWT contenga el claim de rol correcto

### Error: "El nombre de usuario ya está en uso"
- Elegir un nombre de usuario diferente
- El sistema verifica unicidad de nombres de usuario y emails

### Error: "No se puede eliminar el último administrador"
- Debe existir al menos un administrador activo en el sistema
- Crear otro administrador antes de eliminar el actual

---

**Fecha de Creación:** 2025-12-01
**Versión:** 1.0
**Estado:** ✅ Implementado y Listo para Usar
