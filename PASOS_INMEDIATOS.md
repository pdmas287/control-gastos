# Pasos Inmediatos para Completar el Despliegue

## Estado Actual
- ✅ Codigo pusheado a GitHub (commit ebe3455)
- ✅ Dockerfile configurado correctamente
- ✅ .railway.toml configurado
- ✅ Proyecto compila localmente sin errores
- ⏳ Esperando verificacion de Railway

## PASO 1: Verificar Estado de Railway

### Opcion A: Si Railway muestra "cd executable not found"
Esto significa que Railway tiene cacheada una configuracion antigua. Solucion:

1. Ve al Dashboard de Railway
2. Click en tu servicio
3. Click en "Settings"
4. Busca la seccion "Deploy"
5. **Verifica que NO haya un "Custom Start Command"**
   - Si existe, ELIMINALO
6. Click en "Deployments" en el menu lateral
7. Click en los tres puntos (...) del ultimo deployment
8. Selecciona "Redeploy"

### Opcion B: Si Railway muestra otro error
Copia el mensaje de error completo y consultame.

### Opcion C: Si Railway despliega exitosamente pero falla al iniciar
Probablemente faltan las variables de entorno. Continua con PASO 2.

## PASO 2: Configurar Supabase (Base de Datos)

1. Ve a https://supabase.com
2. Click en "Start your project"
3. Crea una cuenta o inicia sesion
4. Click en "New project"
5. Completa:
   - Name: `control-gastos`
   - Database Password: **GUARDA ESTA CONTRASEÑA** (la necesitaras)
   - Region: South America (o la mas cercana)
6. Click "Create new project" (tarda 2-3 minutos)

### Una vez creado el proyecto:

7. En el panel izquierdo, click en "SQL Editor"
8. Click en "New query"
9. Abre el archivo `Database/supabase-schema.sql` de este proyecto
10. Copia TODO el contenido
11. Pegalo en el editor SQL de Supabase
12. Click en "Run" (esquina inferior derecha)
13. Espera confirmacion: "Success. No rows returned"

### Obtener la cadena de conexion:

14. Click en "Project Settings" (icono engranaje, parte inferior izquierda)
15. Click en "Database"
16. Busca la seccion "Connection string"
17. Selecciona "URI"
18. Click en "Copy"
19. La cadena se ve asi:
```
postgresql://postgres.xxxxx:[PASSWORD]@aws-0-us-west-1.pooler.supabase.com:6543/postgres
```

20. **Reemplaza [PASSWORD] con tu contraseña real**

## PASO 3: Configurar Variables de Entorno en Railway

1. Ve al Dashboard de Railway
2. Click en tu servicio
3. Click en "Variables"
4. Agrega las siguientes variables (una por una):

### Variable 1: ConnectionStrings__DefaultConnection
```
Nombre: ConnectionStrings__DefaultConnection
Valor: [TU CADENA DE CONEXION DE SUPABASE]
```
**IMPORTANTE:** Asegurate de usar DOBLE guion bajo: `ConnectionStrings__DefaultConnection`

Ejemplo del valor:
```
Host=aws-0-us-west-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.xxxxx;Password=TU_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

### Variable 2: Jwt__Key
```
Nombre: Jwt__Key
Valor: [GENERA UNA CLAVE SECRETA]
```

Para generar una clave segura, ejecuta en PowerShell:
```powershell
-join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

### Variable 3: Jwt__Issuer
```
Nombre: Jwt__Issuer
Valor: ControlGastosAPI
```

### Variable 4: Jwt__Audience
```
Nombre: Jwt__Audience
Valor: ControlGastosApp
```

### Variable 5: ASPNETCORE_ENVIRONMENT
```
Nombre: ASPNETCORE_ENVIRONMENT
Valor: Production
```

### Variable 6: Jwt__ExpirationDays (opcional)
```
Nombre: Jwt__ExpirationDays
Valor: 7
```

5. Despues de agregar todas las variables, Railway re-desplegara automaticamente

## PASO 4: Verificar el Despliegue

1. Espera a que Railway complete el deployment
2. Verifica el estado en "Deployments"
3. Si es exitoso, Railway te dara una URL publica
4. Copia esa URL (ejemplo: `https://control-gastos-production.up.railway.app`)
5. Agrega `/swagger` al final: `https://control-gastos-production.up.railway.app/swagger`
6. Abre esa URL en tu navegador
7. Deberias ver la documentacion de la API

## PASO 5: Probar la API

En Swagger:

1. Busca el endpoint `POST /api/Auth/login`
2. Click en "Try it out"
3. Ingresa las credenciales por defecto:
```json
{
  "nombreUsuario": "admin",
  "contrasena": "Admin123!"
}
```
4. Click "Execute"
5. Si recibes un token JWT, ¡TODO FUNCIONA!

## PASO 6: Configurar Frontend para Vercel

1. Abre `Frontend/control-gastos-app/src/environments/environment.prod.ts`
2. Reemplaza `[YOUR-RAILWAY-APP-URL]` con tu URL real de Railway
3. Ejemplo:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://control-gastos-production.up.railway.app/api'
};
```
4. Guarda el archivo
5. Commit y push:
```bash
git add Frontend/control-gastos-app/src/environments/environment.prod.ts
git commit -m "feat: configurar URL de produccion para frontend"
git push origin main
```

## PASO 7: Desplegar Frontend en Vercel

1. Ve a https://vercel.com
2. Click "Sign up" o "Log in"
3. Usa tu cuenta de GitHub
4. Click "Add New Project"
5. Busca tu repositorio `control-gastos`
6. Click "Import"
7. Configuracion:
   - Framework Preset: Angular
   - Root Directory: `Frontend/control-gastos-app`
   - Build Command: `npm run vercel-build`
   - Output Directory: `dist/control-gastos-app/browser`
8. Click "Deploy"
9. Espera 2-3 minutos
10. Vercel te dara una URL (ejemplo: `https://control-gastos-xyz.vercel.app`)

## PASO 8: Actualizar CORS en Backend (si es necesario)

Si tienes problemas de CORS:

1. Abre `Backend/ControlGastos.API/Program.cs`
2. Busca la configuracion de CORS
3. Agrega la URL especifica de Vercel:
```csharp
policy.WithOrigins(
    "http://localhost:4200",
    "https://*.vercel.app",
    "https://control-gastos-xyz.vercel.app"  // Tu URL real
)
```
4. Commit y push
5. Railway re-desplegara automaticamente

## Checklist Final

- [ ] Railway desplegado sin errores
- [ ] Supabase configurado con schema
- [ ] Variables de entorno configuradas en Railway
- [ ] API responde en `/swagger`
- [ ] Login funciona y retorna token
- [ ] Frontend desplegado en Vercel
- [ ] Frontend se conecta al backend
- [ ] Login funciona desde el frontend

## Problemas Comunes

### Error: "Connection refused" en Railway
- Verifica que la variable `ConnectionStrings__DefaultConnection` este correcta
- Verifica que la contraseña de Supabase sea correcta

### Error: "Unauthorized" al hacer login
- Verifica que hayas ejecutado el script SQL en Supabase
- Verifica que las credenciales sean exactamente: `admin` / `Admin123!`

### Frontend no se conecta al backend
- Verifica que la URL en `environment.prod.ts` sea correcta
- Verifica que termine con `/api`
- Verifica CORS en Program.cs

### Railway sigue mostrando "cd executable not found"
- Elimina el "Custom Start Command" en Settings → Deploy
- Redeploy manualmente
- Si persiste, borra el servicio y crealo de nuevo

## ¿Necesitas Ayuda?

Si encuentras algun error:
1. Copia el mensaje de error COMPLETO
2. Indicame en que paso estas
3. Enviame capturas de pantalla si es posible
