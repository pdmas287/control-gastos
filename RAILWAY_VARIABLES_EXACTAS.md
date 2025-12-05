# Variables de Entorno para Railway - COPIA Y PEGA

## 🎯 Instrucciones

1. Ve a Railway: https://railway.app
2. Abre tu proyecto "control-gastos"
3. Click en tu servicio
4. Click en la pestaña **"Variables"**
5. Para cada variable a continuación, haz click en **"+ New Variable"**
6. Copia el **Nombre** exactamente como está (respeta mayúsculas y dobles guiones bajos)
7. Copia el **Valor** exactamente como está
8. Click en "Add" para cada una

---

## 📋 Variables a Agregar (5 en total)

### Variable 1: Conexión a Base de Datos Supabase

**Variable Name:**
```
ConnectionStrings__DefaultConnection
```

**Variable Value:**
```
Host=db.vpcfyvzxytddrcdeyrrx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=GraciasDios28.;SSL Mode=Require;Trust Server Certificate=true
```

**⚠️ IMPORTANTE:**
- El nombre debe tener DOBLE guión bajo: `ConnectionStrings__DefaultConnection`
- NO uses un solo guión bajo `_`

---

### Variable 2: Clave JWT

**Variable Name:**
```
Jwt__Key
```

**Variable Value:**
```
jqrO5IH8BLQwZaitcSD7oVxCKnp2hJ0umUlAM3ERdGPgWbTvYeXFz4916fysNk
```

---

### Variable 3: JWT Issuer

**Variable Name:**
```
Jwt__Issuer
```

**Variable Value:**
```
ControlGastosAPI
```

---

### Variable 4: JWT Audience

**Variable Name:**
```
Jwt__Audience
```

**Variable Value:**
```
ControlGastosApp
```

---

### Variable 5: Ambiente de Ejecución

**Variable Name:**
```
ASPNETCORE_ENVIRONMENT
```

**Variable Value:**
```
Production
```

---

## ✅ Verificación

Después de agregar todas las variables, deberías tener **5 variables** en total:

- [x] `ConnectionStrings__DefaultConnection`
- [x] `Jwt__Key`
- [x] `Jwt__Issuer`
- [x] `Jwt__Audience`
- [x] `ASPNETCORE_ENVIRONMENT`

---

## 🔄 Re-deployment Automático

Railway detectará automáticamente las nuevas variables y **re-desplegará** tu aplicación.

**Tiempo estimado:** 2-3 minutos

Verás en la pestaña "Deployments":
- Un nuevo deployment iniciándose
- Build process ejecutándose
- Container reiniciándose con las nuevas variables

---

## 🧪 Cómo Verificar que Funcionó

### Paso 1: Espera a que Railway termine de re-desplegar

Ve a la pestaña "Deployments" y espera a ver:
```
✅ Deployment successful
```

### Paso 2: Prueba con Swagger

1. Abre en tu navegador:
   ```
   https://control-gastos-production-3edf.up.railway.app/swagger
   ```

2. Busca el endpoint **POST /api/Auth/login**

3. Click en **"Try it out"**

4. Ingresa:
   ```json
   {
     "nombreUsuario": "admin",
     "contrasena": "Admin123!"
   }
   ```

5. Click en **"Execute"**

6. Deberías ver una respuesta **200 OK** con un token JWT:
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "nombreUsuario": "admin",
     "rol": "Administrador"
   }
   ```

**Si ves esto ↑ = ¡TODO FUNCIONA PERFECTAMENTE!** 🎉

---

## 🌐 Prueba desde el Frontend

Una vez que Swagger funcione:

1. Abre tu frontend:
   ```
   https://control-gastos-flax.vercel.app
   ```

2. Deberías ver la página de login

3. Ingresa:
   - **Usuario:** `admin`
   - **Contraseña:** `Admin123!`

4. Click en "Iniciar Sesión"

5. Si funciona, serás redirigido al dashboard

---

## ❌ Si algo no funciona

### Error: "Connection refused" o "Cannot connect to database"

**Solución:** Revisa que la variable `ConnectionStrings__DefaultConnection` esté exactamente como se muestra arriba, sin espacios extras.

### Error: "Unauthorized" o "Invalid token"

**Solución:** Revisa que las variables JWT (`Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`) estén correctas.

### Error: Login no responde desde el frontend

**Solución:**
1. Abre las herramientas de desarrollador del navegador (F12)
2. Ve a la pestaña "Console"
3. Ve a la pestaña "Network"
4. Intenta hacer login de nuevo
5. Busca errores de CORS o de red
6. Comparte el error conmigo

---

## 📊 Resumen de URLs

| Servicio | URL |
|----------|-----|
| Backend API | `https://control-gastos-production-3edf.up.railway.app` |
| Swagger Docs | `https://control-gastos-production-3edf.up.railway.app/swagger` |
| Frontend | `https://control-gastos-flax.vercel.app` |
| Base de Datos | Supabase (configurada ✅) |

---

## 🎯 Estado Esperado Después de Configurar

```
✅ Backend desplegado en Railway
✅ Variables de entorno configuradas
✅ Conectado a Supabase exitosamente
✅ Swagger funcionando
✅ Login funcionando desde API
✅ Frontend desplegado en Vercel
✅ Login funcionando desde frontend
```

---

**¡Sigue estos pasos y tendrás tu aplicación completamente funcional!**
