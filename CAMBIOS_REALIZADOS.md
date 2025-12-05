# Resumen de Cambios para Despliegue Gratuito

## Objetivo
Adaptar el proyecto de Control de Gastos para despliegue gratuito usando:
- **Supabase** (Base de datos PostgreSQL)
- **Railway** (Backend .NET)
- **Vercel** (Frontend Angular)

---

## Cambios Realizados en el Backend

### 1. Paquetes NuGet Agregados
- ✅ `Npgsql.EntityFrameworkCore.PostgreSQL` v10.0.0

### 2. Archivos Modificados

#### `Backend/ControlGastos.API/Program.cs`
- **Línea 11-12**: Cambiado de `UseSqlServer` a `UseNpgsql` para PostgreSQL
- **Líneas 52-66**: Actualizada configuración de CORS para permitir dominios de Vercel
  - Agregado `https://*.vercel.app` con soporte de wildcards
  - Agregado `.SetIsOriginAllowedToAllowWildcardSubdomains()`
  - Agregado `.AllowCredentials()`

#### `Backend/ControlGastos.API/appsettings.json`
- **ConnectionStrings**: Actualizado con template de Supabase PostgreSQL
  - Formato: `Host=db.[PROJECT-REF].supabase.co;Port=5432;Database=postgres;...`
- **Jwt.ExpirationDays**: Agregado campo para configurar expiración del token

### 3. Archivos Nuevos Creados

#### `railway.toml` (raíz del proyecto)
Configuración para Railway:
- Comando de build: Compila el proyecto .NET
- Comando de start: Ejecuta el DLL compilado
- Política de reinicio en caso de fallo

---

## Cambios Realizados en el Frontend

### 1. Estructura de Archivos Creados

#### `Frontend/control-gastos-app/src/environments/`
- **`environment.ts`**: Configuración para desarrollo local
  - `apiUrl: 'http://localhost:5000/api'`
- **`environment.prod.ts`**: Configuración para producción
  - `apiUrl: 'https://[YOUR-RAILWAY-APP-URL].up.railway.app/api'`

#### `Frontend/control-gastos-app/vercel.json`
Configuración de Vercel:
- Build config para Angular 17
- Rutas SPA (todas redirigen a index.html)
- Output directory: `dist/control-gastos-app/browser`

### 2. Archivos Modificados

#### `Frontend/control-gastos-app/package.json`
- **Scripts actualizados**:
  - `build`: Agregado `--configuration production`
  - `vercel-build`: Nuevo script para builds de Vercel

#### Servicios Angular - Todos actualizados para usar `environment`

Los siguientes servicios fueron modificados para usar `environment.apiUrl` en lugar de URLs hardcodeadas:

1. **`auth.service.ts`**
   - Importado `environment`
   - Cambiado: `private apiUrl = '${environment.apiUrl}/auth'`

2. **`tipo-gasto.service.ts`**
   - Importado `environment`
   - Cambiado: `private apiUrl = '${environment.apiUrl}/TipoGasto'`

3. **`registro-gasto.service.ts`**
   - Importado `environment`
   - Cambiado: `private apiUrl = '${environment.apiUrl}/RegistroGasto'`

4. **`fondo-monetario.service.ts`**
   - Importado `environment`
   - Cambiado: `private apiUrl = '${environment.apiUrl}/FondoMonetario'`

5. **`presupuesto.service.ts`**
   - Importado `environment`
   - Cambiado: `private apiUrl = '${environment.apiUrl}/Presupuesto'`

6. **`deposito.service.ts`**
   - Importado `environment`
   - Cambiado: `private apiUrl = '${environment.apiUrl}/Deposito'`

7. **`reporte.service.ts`**
   - Importado `environment`
   - Cambiado: `private apiUrl = '${environment.apiUrl}/Reporte'`

8. **`usuario-admin.service.ts`**
   - Importado `environment`
   - Cambiado: `private apiUrl = '${environment.apiUrl}/usuario'`

---

## Base de Datos

### Nuevo Archivo: `Database/supabase-schema.sql`

Script SQL completo para PostgreSQL con:
- ✅ Todas las tablas adaptadas a sintaxis PostgreSQL
- ✅ Índices para optimización de consultas
- ✅ Vistas útiles para reportes
- ✅ Triggers para actualización automática de saldos
- ✅ Funciones PL/pgSQL
- ✅ Datos iniciales (roles y usuario admin)
- ✅ Comentarios de documentación

### Diferencias con SQL Server:

| SQL Server | PostgreSQL |
|------------|------------|
| `IDENTITY(1,1)` | `SERIAL` |
| `NVARCHAR` | `VARCHAR` |
| `BIT` | `BOOLEAN` |
| `GETDATE()` | `CURRENT_TIMESTAMP` |
| `T-SQL` | `PL/pgSQL` |

---

## Documentación Creada

### 1. `INSTRUCCIONES_DESPLIEGUE.md`
Guía paso a paso para:
- ✅ Configurar Supabase
- ✅ Desplegar en Railway
- ✅ Desplegar en Vercel
- ✅ Configurar variables de entorno
- ✅ Actualizar CORS
- ✅ Solución de problemas comunes
- ✅ Checklist de despliegue

### 2. `CAMBIOS_REALIZADOS.md` (este archivo)
Documentación técnica de todos los cambios realizados

---

## Configuración de Variables de Entorno

### Railway (Backend)
```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
ConnectionStrings__DefaultConnection=[SUPABASE_CONNECTION_STRING]
Jwt__Key=[CLAVE_SECRETA_32_CARACTERES]
Jwt__Issuer=ControlGastosAPI
Jwt__Audience=ControlGastosApp
Jwt__ExpirationDays=7
```

### Vercel (Frontend)
No requiere variables de entorno adicionales. La configuración se hace en `environment.prod.ts`.

---

## Flujo de Trabajo de Despliegue

### Desarrollo Local
```bash
# Backend
cd Backend/ControlGastos.API
dotnet run

# Frontend
cd Frontend/control-gastos-app
npm start
```

### Despliegue a Producción

1. **Commit cambios**:
   ```bash
   git add .
   git commit -m "Configuración para despliegue"
   git push
   ```

2. **Railway**: Se redespliegue automáticamente al hacer push

3. **Vercel**: Se redespliegue automáticamente al hacer push

---

## Compatibilidad

### Versiones Utilizadas
- .NET: 10.0
- Angular: 17.0
- Entity Framework Core: 10.0
- PostgreSQL: 15+ (Supabase)
- Node.js: 18+ (recomendado para Vercel)

---

## Seguridad

### Consideraciones Importantes

1. **Variables de Entorno**:
   - ❌ NO commitear `appsettings.json` con datos reales
   - ✅ Usar variables de entorno en Railway

2. **JWT Secret**:
   - ❌ NO usar la clave por defecto en producción
   - ✅ Generar una clave segura de al menos 32 caracteres

3. **Usuario Admin**:
   - ❌ NO dejar el password por defecto (`Admin123!`)
   - ✅ Cambiar inmediatamente después del primer login

4. **CORS**:
   - ✅ Configurado para permitir solo dominios autorizados
   - ✅ Soporte para previews de Vercel

---

## Límites de los Planes Gratuitos

### Supabase Free
- 500 MB base de datos
- 2 GB transferencia/mes
- Backups automáticos 7 días
- Sin tarjeta de crédito requerida

### Railway Free
- 500 horas/mes
- $5 crédito/mes
- 512 MB RAM
- Requiere tarjeta (no cobra si no superas límite)

### Vercel Free
- 100 GB bandwidth/mes
- Deployments ilimitados
- Sin tarjeta de crédito requerida

---

## Próximos Pasos Recomendados

1. ✅ Seguir las instrucciones en `INSTRUCCIONES_DESPLIEGUE.md`
2. ✅ Ejecutar el script `Database/supabase-schema.sql` en Supabase
3. ✅ Configurar variables de entorno en Railway
4. ✅ Actualizar `environment.prod.ts` con URL de Railway
5. ✅ Probar la aplicación en producción
6. ⚠️ Cambiar password del usuario admin
7. ⚠️ Generar JWT secret seguro
8. 📊 Configurar monitoreo y alertas
9. 🔒 Implementar rate limiting si es necesario
10. 📈 Optimizar performance del frontend

---

## Soporte y Ayuda

Si tienes problemas durante el despliegue:

1. Revisa la sección "Problemas Comunes" en `INSTRUCCIONES_DESPLIEGUE.md`
2. Verifica los logs en Railway y Vercel
3. Consulta la documentación oficial:
   - [Supabase Docs](https://supabase.com/docs)
   - [Railway Docs](https://docs.railway.app)
   - [Vercel Docs](https://vercel.com/docs)

---

## Resumen de Archivos Creados/Modificados

### Archivos Nuevos (7)
1. ✅ `railway.toml`
2. ✅ `Frontend/control-gastos-app/vercel.json`
3. ✅ `Frontend/control-gastos-app/src/environments/environment.ts`
4. ✅ `Frontend/control-gastos-app/src/environments/environment.prod.ts`
5. ✅ `Database/supabase-schema.sql`
6. ✅ `INSTRUCCIONES_DESPLIEGUE.md`
7. ✅ `CAMBIOS_REALIZADOS.md`

### Archivos Modificados (11)
1. ✅ `Backend/ControlGastos.API/ControlGastos.API.csproj`
2. ✅ `Backend/ControlGastos.API/Program.cs`
3. ✅ `Backend/ControlGastos.API/appsettings.json`
4. ✅ `Frontend/control-gastos-app/package.json`
5. ✅ `Frontend/control-gastos-app/src/app/services/auth.service.ts`
6. ✅ `Frontend/control-gastos-app/src/app/services/tipo-gasto.service.ts`
7. ✅ `Frontend/control-gastos-app/src/app/services/registro-gasto.service.ts`
8. ✅ `Frontend/control-gastos-app/src/app/services/fondo-monetario.service.ts`
9. ✅ `Frontend/control-gastos-app/src/app/services/presupuesto.service.ts`
10. ✅ `Frontend/control-gastos-app/src/app/services/deposito.service.ts`
11. ✅ `Frontend/control-gastos-app/src/app/services/reporte.service.ts`
12. ✅ `Frontend/control-gastos-app/src/app/services/usuario-admin.service.ts`

---

**Total: 18 archivos modificados/creados**

**Estado**: ✅ Proyecto listo para despliegue gratuito
