# ✅ Checklist para Desplegar en Railway

## 📋 Preparación del Repositorio

### ✅ Archivos Configurados

- [x] **railway.toml** - Configuración de Railway creada
- [x] **Program.cs** - Actualizado para usar PostgreSQL (Npgsql)
- [x] **appsettings.json** - Template con connection string de PostgreSQL
- [x] **.gitignore** - Configurado para no subir archivos sensibles
- [x] **ControlGastos.API.csproj** - Paquete Npgsql.EntityFrameworkCore.PostgreSQL instalado

### 🔍 Verificaciones Antes de Subir a GitHub

#### 1. Verificar que NO subes archivos sensibles

```bash
# Revisa que estos archivos NO estén en Git
git status

# Asegúrate de que NO aparezcan:
# - appsettings.Development.json
# - appsettings.Production.json
# - Archivos .env
# - Certificados (.pfx, .key, .pem)
```

#### 2. Verificar estructura del proyecto

```bash
# Tu estructura debe verse así:
.
├── Backend/
│   └── ControlGastos.API/
│       ├── Controllers/
│       ├── Data/
│       ├── Models/
│       ├── Services/
│       ├── Program.cs
│       ├── appsettings.json (solo template)
│       └── ControlGastos.API.csproj
├── Frontend/
│   └── control-gastos-app/
├── Database/
│   └── supabase-schema.sql
├── railway.toml
├── README.md
└── .gitignore
```

#### 3. Limpiar archivos compilados antes del commit

```bash
# Navega al directorio backend
cd Backend/ControlGastos.API

# Limpia los archivos de build
dotnet clean

# Elimina directorios bin y obj si existen
Remove-Item -Recurse -Force bin,obj -ErrorAction SilentlyContinue
```

#### 4. Verificar que el proyecto compila

```bash
# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Si compila exitosamente, estás listo!
```

## 🚀 Subir a GitHub

### Paso 1: Inicializar Git (si no está inicializado)

```bash
# Verifica si ya tienes Git inicializado
git status

# Si no está inicializado, ejecuta:
git init
```

### Paso 2: Agregar archivos

```bash
# Agregar todos los archivos
git add .

# Verificar qué se va a commitear
git status
```

### Paso 3: Commit inicial

```bash
git commit -m "feat: configuración inicial del proyecto para Railway deployment

- Backend configurado con PostgreSQL (Npgsql)
- Frontend Angular con environments configurados
- Documentación completa (README, CONTRIBUTING)
- Templates de GitHub (Issues, PRs)
- railway.toml configurado para deployment
- Scripts SQL para Supabase
"
```

### Paso 4: Crear repositorio en GitHub

1. Ve a [github.com](https://github.com)
2. Click en el botón **"+"** → **"New repository"**
3. Configura:
   ```
   Repository name: control-gastos
   Description: Sistema de control y gestión de gastos con .NET y Angular
   Public/Private: Elige según tu preferencia
   ✅ NO marques: Add README, .gitignore, license (ya los tienes)
   ```
4. Click **"Create repository"**

### Paso 5: Conectar con GitHub

```bash
# Reemplaza 'tu-usuario' con tu usuario de GitHub
git remote add origin https://github.com/tu-usuario/control-gastos.git

# Verificar que se agregó correctamente
git remote -v
```

### Paso 6: Push al repositorio

```bash
# Renombrar rama a main (si es necesario)
git branch -M main

# Push
git push -u origin main
```

## 🛤️ Conectar con Railway

### Paso 1: Crear cuenta en Railway

1. Ve a [railway.app](https://railway.app)
2. Click **"Login"** → **"Login with GitHub"**
3. Autoriza Railway a acceder a tus repositorios

### Paso 2: Crear nuevo proyecto

1. En el Dashboard de Railway, click **"New Project"**
2. Selecciona **"Deploy from GitHub repo"**
3. Busca y selecciona tu repositorio **"control-gastos"**
4. Railway detectará automáticamente el `railway.toml`

### Paso 3: Configurar Variables de Entorno

Railway abrirá la configuración del proyecto. Ve a **"Variables"** y agrega:

```bash
# IMPORTANTE: Configura estas variables ANTES de desplegar

# 1. Environment
ASPNETCORE_ENVIRONMENT=Production

# 2. Puerto (Railway lo asigna automáticamente)
ASPNETCORE_URLS=http://0.0.0.0:$PORT

# 3. Connection String de Supabase (REEMPLAZA CON TU VALOR REAL)
ConnectionStrings__DefaultConnection=Host=db.xxxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD_DE_SUPABASE;SSL Mode=Require;Trust Server Certificate=true

# 4. JWT Configuration
Jwt__Key=TU_CLAVE_SECRETA_MUY_SEGURA_DE_AL_MENOS_32_CARACTERES_AQUI
Jwt__Issuer=ControlGastosAPI
Jwt__Audience=ControlGastosApp
Jwt__ExpirationDays=7
```

### Paso 4: Generar Connection String de Supabase

Si aún no tienes tu Connection String de Supabase:

1. Ve a [supabase.com](https://supabase.com)
2. Crea un proyecto (sigue las instrucciones en `INSTRUCCIONES_DESPLIEGUE.md`)
3. Ve a **Settings** → **Database**
4. En **Connection string**, copia la URI y conviértela al formato de Npgsql:

```
# Formato Supabase URI:
postgresql://postgres.[ref]:[password]@aws-0-us-east-1.pooler.supabase.com:6543/postgres

# Conviértelo a formato Npgsql:
Host=db.[ref].supabase.co;Port=5432;Database=postgres;Username=postgres;Password=[password];SSL Mode=Require;Trust Server Certificate=true
```

### Paso 5: Desplegar

1. Railway comenzará a construir automáticamente
2. Monitorea el progreso en la pestaña **"Deployments"**
3. Esto tomará 3-5 minutos

### Paso 6: Obtener URL Pública

1. Ve a **Settings** → **Networking**
2. Click **"Generate Domain"**
3. Railway generará una URL como:
   ```
   https://control-gastos-production.up.railway.app
   ```
4. **GUARDA ESTA URL** - la necesitarás para el frontend

## 🔍 Verificar Despliegue

### 1. Verificar que el backend está corriendo

Visita en tu navegador:
```
https://tu-app.up.railway.app/swagger
```

Deberías ver la documentación de Swagger UI.

### 2. Probar endpoint de salud

Si tienes un endpoint de health check:
```
https://tu-app.up.railway.app/health
```

### 3. Revisar Logs

En Railway:
1. Ve a tu servicio
2. Click en **"Deployments"**
3. Click en el deployment activo
4. Revisa los logs para errores

## ⚠️ Problemas Comunes y Soluciones

### Error: "Connection refused" en Railway

**Causa:** Connection string mal configurado

**Solución:**
```bash
# Verifica el formato exacto:
Host=db.xxxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=tu_password;SSL Mode=Require;Trust Server Certificate=true

# NO uses este formato (es para libpq, no Npgsql):
postgresql://postgres:password@host:5432/database
```

### Error: "Application failed to start"

**Causa:** Puerto mal configurado

**Solución:**
```bash
# Asegúrate de tener esta variable en Railway:
ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Railway asigna el puerto automáticamente vía $PORT
```

### Error: "JWT key too short"

**Causa:** Clave JWT muy corta

**Solución:**
```bash
# Tu JWT Key debe tener al menos 32 caracteres:
Jwt__Key=Esta_Es_Una_Clave_Muy_Segura_De_Al_Menos_32_Caracteres_123456

# NO uses claves cortas como:
Jwt__Key=secret123  # ❌ Muy corta
```

### Build falla en Railway

**Causa:** Archivos de proyecto incorrectos

**Solución:**
```bash
# 1. Verifica que railway.toml esté en la raíz
# 2. Verifica que el path sea correcto:
buildCommand = "cd Backend/ControlGastos.API && dotnet publish -c Release -o out"

# 3. Haz un build local para verificar:
cd Backend/ControlGastos.API
dotnet publish -c Release -o out
```

### Variables de entorno no se aplican

**Solución:**
1. Ve a Railway → Settings → Variables
2. Verifica que las variables estén guardadas
3. Reinicia el deployment: **Deployments** → **Restart**

## 📊 Checklist Final

Antes de considerar el despliegue completo:

### Backend (Railway)
- [ ] Repositorio subido a GitHub
- [ ] Railway conectado al repositorio
- [ ] Variables de entorno configuradas
- [ ] Deployment exitoso (sin errores)
- [ ] Swagger UI accesible
- [ ] Logs no muestran errores críticos
- [ ] URL pública generada

### Base de Datos (Supabase)
- [ ] Proyecto de Supabase creado
- [ ] Script SQL ejecutado exitosamente
- [ ] Tablas creadas correctamente
- [ ] Connection string copiado
- [ ] Usuario admin creado (admin@example.com)

### Frontend (Para después)
- [ ] environment.prod.ts actualizado con URL de Railway
- [ ] Cambios commiteados y pusheados
- [ ] Vercel configurado
- [ ] Frontend desplegado
- [ ] Frontend se conecta al backend

## 🎯 Siguiente Paso

Una vez que tu backend esté desplegado en Railway:

1. Copia la URL de Railway
2. Actualiza `Frontend/control-gastos-app/src/environments/environment.prod.ts`:
   ```typescript
   export const environment = {
     production: true,
     apiUrl: 'https://tu-app.up.railway.app/api'  // ← Tu URL aquí
   };
   ```
3. Commit y push
4. Despliega el frontend en Vercel (sigue `INSTRUCCIONES_DESPLIEGUE.md`)

## 📞 Soporte

Si tienes problemas:
1. Revisa los logs en Railway
2. Consulta `INSTRUCCIONES_DESPLIEGUE.md`
3. Revisa la sección de "Problemas Comunes" arriba
4. Abre un issue en GitHub si el problema persiste

---

**¡Tu proyecto está listo para Railway!** 🚀
