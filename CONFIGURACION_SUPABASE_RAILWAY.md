# Configuración de Supabase y Railway - Guía Paso a Paso

## Estado Actual
✅ **Railway está corriendo exitosamente**
- Container iniciado en puerto 8080
- Aplicación .NET funcionando
- ⚠️ Falta configurar base de datos y variables de entorno

---

## PARTE 1: Configurar Supabase (Base de Datos PostgreSQL)

### Paso 1.1: Crear Cuenta y Proyecto en Supabase

1. Abre tu navegador y ve a: **https://supabase.com**
2. Click en **"Start your project"** (esquina superior derecha)
3. Puedes iniciar sesión con:
   - GitHub (recomendado - usa la misma cuenta donde está tu repositorio)
   - Google
   - Email

4. Una vez dentro, click en **"New project"**

5. Completa el formulario:
   ```
   Organization: [Selecciona o crea una organización]
   Name: control-gastos
   Database Password: [CREA UNA CONTRASEÑA SEGURA]
   Region: South America (sao-paulo) - o la más cercana a ti
   Pricing Plan: Free
   ```

   **⚠️ IMPORTANTE:** Guarda la contraseña en un lugar seguro. La necesitarás después.

6. Click en **"Create new project"**
   - Supabase tardará 2-3 minutos en crear la base de datos
   - Verás una barra de progreso

### Paso 1.2: Ejecutar el Schema SQL

1. Mientras esperas, abre el archivo de este proyecto:
   ```
   Database/supabase-schema.sql
   ```

2. **Copia TODO el contenido del archivo** (Ctrl+A, Ctrl+C)

3. Una vez que Supabase termine de crear el proyecto, en el panel izquierdo busca el ícono de SQL:
   - 📊 **SQL Editor**

4. Click en **"+ New query"**

5. **Pega** todo el contenido que copiaste del archivo `supabase-schema.sql`

6. Click en **"Run"** (o presiona Ctrl+Enter)

7. Espera la confirmación:
   ```
   Success. No rows returned
   ```

8. ¡Perfecto! La base de datos está lista con:
   - ✅ Todas las tablas creadas
   - ✅ Usuario admin creado (admin / Admin123!)
   - ✅ Roles configurados
   - ✅ Datos iniciales insertados

### Paso 1.3: Obtener la Cadena de Conexión

1. En el panel izquierdo, click en el ícono de engranaje:
   - ⚙️ **Project Settings** (parte inferior)

2. En el menú de Settings, click en:
   - **Database**

3. Desplázate hasta la sección **"Connection string"**

4. Asegúrate de que esté seleccionada la pestaña:
   - **"URI"** (no "Session mode")

5. Verás algo como:
   ```
   postgresql://postgres:[YOUR-PASSWORD]@db.xxxxxxxxxxxxx.supabase.co:5432/postgres
   ```

6. **IMPORTANTE:** Tienes que modificarla para .NET. Copia esta plantilla y reemplaza los valores:

   ```
   Host=db.xxxxxxxxxxxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=[TU-PASSWORD];SSL Mode=Require;Trust Server Certificate=true
   ```

   Donde:
   - `db.xxxxxxxxxxxxx.supabase.co` = Tu host de Supabase
   - `[TU-PASSWORD]` = La contraseña que creaste en el Paso 1.1

7. **Copia la cadena completa y guárdala**. La necesitarás en el siguiente paso.

**Ejemplo completo:**
```
Host=db.abcdefghijklmn.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=MiPasswordSeguro123!;SSL Mode=Require;Trust Server Certificate=true
```

---

## PARTE 2: Configurar Variables de Entorno en Railway

### Paso 2.1: Generar Clave JWT

Ya hemos generado una clave JWT para ti. La encontrarás al final de esta guía.

### Paso 2.2: Agregar Variables en Railway

1. Ve al **Dashboard de Railway**: https://railway.app

2. Click en tu proyecto **control-gastos**

3. Click en tu servicio (debería mostrarse como "ControlGastos.API" o similar)

4. En el menú superior, click en la pestaña **"Variables"**

5. Ahora vas a agregar **5 variables de entorno**. Para cada una:
   - Click en **"+ New Variable"**
   - Ingresa el nombre EXACTAMENTE como se muestra (con mayúsculas y dobles guiones bajos)
   - Ingresa el valor
   - Click en **"Add"**

#### Variable 1: Conexión a Base de Datos
```
Variable Name: ConnectionStrings__DefaultConnection
Variable Value: [Pega aquí la cadena de conexión que obtuviste de Supabase en Paso 1.3]
```

**⚠️ SUPER IMPORTANTE:**
- El nombre debe ser: `ConnectionStrings__DefaultConnection`
- Con DOBLE guión bajo: `__` (dos underscores)
- No uses un solo guión bajo `_`

**Ejemplo del valor:**
```
Host=db.abcdefghijklmn.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=MiPasswordSeguro123!;SSL Mode=Require;Trust Server Certificate=true
```

#### Variable 2: Clave JWT
```
Variable Name: Jwt__Key
Variable Value: jqrO5IH8BLQwZaitcSD7oVxCKnp2hJ0umUlAM3ERdGPgWbTvYeXFz4916fysNk
```

**Nota:** Esta es la clave que generamos con el script. Si quieres generar una nueva, ejecuta:
```powershell
.\scripts\generar-jwt-key.ps1
```

#### Variable 3: JWT Issuer
```
Variable Name: Jwt__Issuer
Variable Value: ControlGastosAPI
```

#### Variable 4: JWT Audience
```
Variable Name: Jwt__Audience
Variable Value: ControlGastosApp
```

#### Variable 5: Ambiente de Ejecución
```
Variable Name: ASPNETCORE_ENVIRONMENT
Variable Value: Production
```

### Paso 2.3: Redeploy (Opcional)

Railway debería re-desplegar automáticamente cuando agregues las variables.

Si no lo hace:
1. Ve a la pestaña **"Deployments"**
2. Click en los **tres puntos (...)** del último deployment
3. Click en **"Redeploy"**

---

## PARTE 3: Verificar que Todo Funcione

### Paso 3.1: Obtener URL de Railway

1. En Railway, ve a la pestaña **"Settings"** de tu servicio
2. Busca la sección **"Domains"**
3. Deberías ver una URL como:
   ```
   https://control-gastos-production.up.railway.app
   ```
4. Copia esta URL

### Paso 3.2: Probar la API con Swagger

1. Agrega `/swagger` al final de tu URL:
   ```
   https://control-gastos-production.up.railway.app/swagger
   ```

2. Abre esa URL en tu navegador

3. Deberías ver la **documentación interactiva de la API** (Swagger UI)

### Paso 3.3: Probar el Login

1. En Swagger, busca la sección **"Auth"**

2. Click en **"POST /api/Auth/login"**

3. Click en **"Try it out"**

4. Ingresa las credenciales del usuario admin:
   ```json
   {
     "nombreUsuario": "admin",
     "contrasena": "Admin123!"
   }
   ```

5. Click en **"Execute"**

6. Si todo está bien, deberías recibir una respuesta **200** con un token JWT:
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "nombreUsuario": "admin",
     "rol": "Administrador"
   }
   ```

7. **¡ÉXITO!** 🎉 Tu backend está completamente funcional.

---

## PARTE 4: Actualizar Frontend para Producción

### Paso 4.1: Configurar URL del Backend

1. Abre el archivo:
   ```
   Frontend/control-gastos-app/src/environments/environment.prod.ts
   ```

2. Reemplaza `[YOUR-RAILWAY-APP-URL]` con tu URL real de Railway:
   ```typescript
   export const environment = {
     production: true,
     apiUrl: 'https://control-gastos-production.up.railway.app/api'
   };
   ```

3. Guarda el archivo

4. Haz commit y push:
   ```bash
   git add Frontend/control-gastos-app/src/environments/environment.prod.ts
   git commit -m "feat: configurar URL de producción del backend"
   git push origin main
   ```

---

## Resumen de Variables de Entorno en Railway

| Variable | Valor |
|----------|-------|
| `ConnectionStrings__DefaultConnection` | Tu cadena de conexión de Supabase |
| `Jwt__Key` | `jqrO5IH8BLQwZaitcSD7oVxCKnp2hJ0umUlAM3ERdGPgWbTvYeXFz4916fysNk` |
| `Jwt__Issuer` | `ControlGastosAPI` |
| `Jwt__Audience` | `ControlGastosApp` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

---

## Credenciales de Usuario Admin

```
Usuario: admin
Contraseña: Admin123!
Rol: Administrador
```

**⚠️ IMPORTANTE:** Después de hacer login por primera vez, deberías cambiar esta contraseña desde el frontend.

---

## Problemas Comunes y Soluciones

### Error: "No connection string named 'DefaultConnection' found"
**Solución:** Verifica que la variable en Railway sea exactamente:
```
ConnectionStrings__DefaultConnection
```
Con DOBLE guión bajo `__`

### Error: "Could not connect to server"
**Solución:**
- Verifica que la cadena de conexión de Supabase sea correcta
- Verifica que hayas reemplazado `[YOUR-PASSWORD]` con tu contraseña real
- Verifica que no haya espacios adicionales en la cadena

### Error: "Invalid credentials" al hacer login
**Solución:**
- Verifica que hayas ejecutado el script SQL completo en Supabase
- Las credenciales son case-sensitive:
  - Usuario: `admin` (minúsculas)
  - Contraseña: `Admin123!` (exacta)

### Error: "Unauthorized" o problemas con JWT
**Solución:**
- Verifica que la variable `Jwt__Key` tenga al menos 32 caracteres
- Verifica que `Jwt__Issuer` y `Jwt__Audience` estén correctos

---

## Siguiente Paso: Desplegar Frontend en Vercel

Una vez que la API esté funcionando, el siguiente paso es desplegar el frontend en Vercel.

Consulta el archivo `PASOS_INMEDIATOS.md` para instrucciones detalladas sobre el deployment en Vercel.

---

## ¿Necesitas Ayuda?

Si encuentras algún error:
1. Copia el mensaje de error completo
2. Revisa los logs en Railway (Deployments → Click en el deployment → View Logs)
3. Verifica que todas las variables estén correctamente configuradas
4. Consulta la sección "Problemas Comunes" arriba
