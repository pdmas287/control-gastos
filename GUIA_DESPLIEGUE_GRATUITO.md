# 🆓 Guía de Despliegue GRATUITO - Control de Gastos

## 🎯 Opciones 100% Gratuitas

Esta guía te mostrará cómo desplegar tu aplicación **completamente gratis** usando diferentes alternativas.

---

## 📊 Comparativa de Opciones Gratuitas

| Componente | Opción 1 (Recomendada) | Opción 2 | Opción 3 |
|------------|------------------------|----------|----------|
| **Base de Datos** | Supabase (500MB) | Azure SQL (prueba 12 meses) | Railway (500MB) |
| **Backend .NET** | Railway / Render | Azure App Service (prueba) | Fly.io |
| **Frontend Angular** | Vercel / Netlify | Azure Static Web Apps | GitHub Pages |
| **Límites** | Permanente | 12 meses | Permanente |
| **Dificultad** | ⭐⭐ Media | ⭐⭐⭐ Media-Alta | ⭐⭐⭐ Alta |

---

## 🏆 OPCIÓN 1: Supabase + Railway + Vercel (RECOMENDADA)

### ✅ Ventajas

- **100% Gratuito** permanentemente
- Fácil configuración
- Base de datos PostgreSQL (en vez de SQL Server)
- No requiere tarjeta de crédito (excepto Railway)

### ⚠️ Consideraciones

- Necesitarás adaptar el backend para usar PostgreSQL en lugar de SQL Server
- Railway requiere tarjeta pero no cobra si no superas límites

---

## 🚀 IMPLEMENTACIÓN - Opción 1

### Paso 1: Base de Datos con Supabase

#### 1.1 Crear cuenta en Supabase

1. Ir a: <https://supabase.com>
2. Click "Start your project"
3. Sign up con GitHub (gratis, sin tarjeta)

#### 1.2 Crear proyecto

```yaml
Organization: [Tu nombre]
Project name: control-gastos
Database Password: [Contraseña segura - ¡guárdala!]
Region: East US (más cercano a ti)
Pricing Plan: Free (500MB, 2GB transferencia)
```

#### 1.3 Crear tablas

En Supabase Dashboard:

1. Ir a "SQL Editor"
2. Click "New query"
3. Ejecutar este script adaptado:

```sql
-- Tabla Usuarios
CREATE TABLE Usuario (
    UsuarioId SERIAL PRIMARY KEY,
    NombreCompleto VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(256) NOT NULL,
    FechaCreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Activo BOOLEAN DEFAULT TRUE
);

-- Tabla Roles (si usas sistema de roles)
CREATE TABLE Rol (
    RolId SERIAL PRIMARY KEY,
    Nombre VARCHAR(50) UNIQUE NOT NULL,
    Descripcion VARCHAR(200)
);

-- Tabla UsuarioRoles
CREATE TABLE UsuarioRoles (
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId),
    RolId INTEGER REFERENCES Rol(RolId),
    PRIMARY KEY (UsuarioId, RolId)
);

-- Tabla TipoGasto
CREATE TABLE TipoGasto (
    TipoGastoId SERIAL PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(200),
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId),
    Activo BOOLEAN DEFAULT TRUE
);

-- Tabla FondoMonetario
CREATE TABLE FondoMonetario (
    FondoId SERIAL PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(200),
    SaldoInicial DECIMAL(18,2) DEFAULT 0,
    SaldoActual DECIMAL(18,2) DEFAULT 0,
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId),
    Activo BOOLEAN DEFAULT TRUE
);

-- Tabla Presupuesto
CREATE TABLE Presupuesto (
    PresupuestoId SERIAL PRIMARY KEY,
    TipoGastoId INTEGER REFERENCES TipoGasto(TipoGastoId),
    Monto DECIMAL(18,2) NOT NULL,
    Periodo VARCHAR(20),
    FechaInicio DATE,
    FechaFin DATE,
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId)
);

-- Tabla RegistroGasto
CREATE TABLE RegistroGasto (
    RegistroGastoId SERIAL PRIMARY KEY,
    TipoGastoId INTEGER REFERENCES TipoGasto(TipoGastoId),
    FondoId INTEGER REFERENCES FondoMonetario(FondoId),
    Monto DECIMAL(18,2) NOT NULL,
    Descripcion VARCHAR(200),
    Fecha DATE NOT NULL,
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId)
);

-- Tabla Deposito
CREATE TABLE Deposito (
    DepositoId SERIAL PRIMARY KEY,
    FondoId INTEGER REFERENCES FondoMonetario(FondoId),
    Monto DECIMAL(18,2) NOT NULL,
    Descripcion VARCHAR(200),
    Fecha DATE NOT NULL,
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId)
);

-- Crear usuario admin por defecto (opcional)
INSERT INTO Rol (Nombre, Descripcion) VALUES
('Administrador', 'Acceso completo al sistema'),
('Usuario', 'Usuario estándar');

-- Insertar usuario admin (password: Admin123! en SHA256)
INSERT INTO Usuario (NombreCompleto, Email, PasswordHash, Activo)
VALUES ('Administrador', 'admin@example.com',
        '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',
        TRUE);

-- Asignar rol administrador
INSERT INTO UsuarioRoles (UsuarioId, RolId)
VALUES (1, 1);
```

#### 1.4 Obtener Connection String

1. En Supabase Dashboard → Settings → Database
2. Copiar "Connection string" → "URI"

```bash
postgresql://postgres:[YOUR-PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres
```

---

### Paso 2: Adaptar Backend para PostgreSQL

#### 2.1 Instalar paquete Npgsql

```bash
cd Backend/ControlGastos.API
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

#### 2.2 Actualizar appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.[PROJECT-REF].supabase.co;Port=5432;Database=postgres;Username=postgres;Password=[YOUR-PASSWORD];SSL Mode=Require;Trust Server Certificate=true"
  },
  "Jwt": {
    "Key": "TU_CLAVE_SECRETA_SUPER_SEGURA_DE_AL_MENOS_32_CARACTERES",
    "Issuer": "ControlGastosAPI",
    "Audience": "ControlGastosApp",
    "ExpirationDays": 7
  }
}
```

#### 2.3 Modificar Program.cs (si usas Entity Framework)

Buscar donde configuras SQL Server:

```csharp
// ANTES (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// DESPUÉS (PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
```

---

### Paso 3: Desplegar Backend en Railway

#### 3.1 Crear cuenta en Railway

1. Ir a: <https://railway.app>
2. Sign up con GitHub
3. ⚠️ Verificar cuenta (puede requerir tarjeta, pero no cobra en plan gratuito)

#### 3.2 Plan Gratuito de Railway

```text
Límites gratuitos:
- 500 horas/mes (suficiente para app personal)
- 512MB RAM
- 1GB disco
- 100GB transferencia
```

#### 3.3 Crear nuevo proyecto

1. Click "New Project"
2. Seleccionar "Deploy from GitHub repo"
3. Conectar tu repositorio de GitHub
4. Seleccionar rama `main`

#### 3.4 Configurar el servicio

1. Railway detectará automáticamente .NET
2. Si no, crear `railway.toml` en la raíz del proyecto:

```toml
[build]
builder = "nixpacks"
buildCommand = "cd Backend/ControlGastos.API && dotnet publish -c Release -o out"

[deploy]
startCommand = "cd Backend/ControlGastos.API/out && dotnet ControlGastos.API.dll"
restartPolicyType = "on_failure"
restartPolicyMaxRetries = 10
```

#### 3.5 Configurar variables de entorno

En Railway → Variables:

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
ConnectionStrings__DefaultConnection=[TU CONNECTION STRING DE SUPABASE]
Jwt__Key=TU_CLAVE_SECRETA_SUPER_SEGURA_DE_AL_MENOS_32_CARACTERES
Jwt__Issuer=ControlGastosAPI
Jwt__Audience=ControlGastosApp
Jwt__ExpirationDays=7
```

#### 3.6 Obtener URL pública

1. Railway → Settings → Networking
2. Click "Generate Domain"
3. Obtendrás algo como: `https://control-gastos-api-production.up.railway.app`

---

### Paso 4: Desplegar Frontend en Vercel

#### 4.1 Crear cuenta en Vercel

1. Ir a: <https://vercel.com>
2. Sign up con GitHub (gratis, sin tarjeta)

#### 4.2 Plan Gratuito de Vercel

```text
Límites gratuitos:
- 100GB bandwidth/mes
- Deployments ilimitados
- Dominios ilimitados
- SSL automático
```

#### 4.3 Actualizar environment.prod.ts

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://control-gastos-api-production.up.railway.app/api'
};
```

#### 4.4 Crear vercel.json en Frontend/control-gastos-app

```json
{
  "version": 2,
  "builds": [
    {
      "src": "package.json",
      "use": "@vercel/static-build",
      "config": {
        "distDir": "dist/control-gastos-app/browser"
      }
    }
  ],
  "routes": [
    {
      "src": "/(.*)",
      "dest": "/index.html"
    }
  ]
}
```

#### 4.5 Actualizar package.json

Agregar script de build para Vercel:

```json
{
  "scripts": {
    "build": "ng build --configuration production",
    "vercel-build": "ng build --configuration production"
  }
}
```

#### 4.6 Deploy en Vercel

#### Opción A: Desde Web UI

1. Ir a Vercel Dashboard
2. Click "New Project"
3. Import Git Repository (conectar GitHub)
4. Configurar:

   ```yaml
   Framework Preset: Angular
   Root Directory: Frontend/control-gastos-app
   Build Command: npm run build
   Output Directory: dist/control-gastos-app/browser
   ```yaml
5. Click "Deploy"

#### Opción B: Desde CLI

```bash
# Instalar Vercel CLI
npm i -g vercel

# Desde Frontend/control-gastos-app
cd Frontend/control-gastos-app

# Login
vercel login

# Deploy
vercel --prod
```

#### 4.7 Obtener URL

Vercel te dará una URL como:

```bash
https://control-gastos-app.vercel.app
```

---

### Paso 5: Configurar CORS

Actualizar backend para permitir el dominio de Vercel:

En `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "https://control-gastos-app.vercel.app",  // Tu dominio de Vercel
            "https://*.vercel.app"  // Todos los previews de Vercel
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

---

## 🏆 OPCIÓN 2: Azure Free Tier (12 meses)

### ✅ Ventajas (Opción 2)

- Mantener SQL Server (no adaptar código)
- 12 meses completamente gratis
- $200 crédito el primer mes

### ⚠️ Limitaciones (Opción 2)

- Requiere tarjeta de crédito
- Solo gratis por 12 meses
- Después pagas o migras

### Servicios Incluidos (12 meses gratis)

```text
✅ Azure SQL Database (250GB)
✅ Azure App Service (10 web apps)
✅ Azure Static Web Apps (siempre gratis)
```

### Implementación

Seguir la guía principal: `GUIA_DESPLIEGUE_AZURE.md`

Pero usar estos tiers:

- SQL Database: Free (solo primeros 12 meses)
- App Service: F1 Free tier
- Static Web App: Free tier (permanente)

---

## 🏆 OPCIÓN 3: Render.com (Todo en uno)

### ✅ Ventajas (Opción 3)

- Todo en una plataforma
- PostgreSQL gratis permanentemente
- Sin tarjeta de crédito

### ⚠️ Limitaciones (Opción 3)

- Servicios "dormidos" después de inactividad (tardan 30s en despertar)
- 750 horas/mes de compute

### Paso a Paso

#### 1. Crear cuenta en Render

<https://render.com> → Sign up con GitHub

#### 2. Crear PostgreSQL Database

1. Dashboard → New → PostgreSQL
2. Configurar:

   ```yaml
   Name: control-gastos-db
   Database: control_gastos
   User: [autogenerado]
   Region: Oregon (más cercano gratis)
   Plan: Free
   ```

#### 3. Ejecutar SQL

1. Conectar con psql o herramienta externa
2. Ejecutar los scripts adaptados para PostgreSQL

#### 4. Crear Web Service (Backend)

1. Dashboard → New → Web Service
2. Connect repository
3. Configurar:

   ```yaml
   Name: control-gastos-api
   Environment: .NET
   Region: Oregon
   Branch: main
   Root Directory: Backend/ControlGastos.API
   Build Command: dotnet publish -c Release -o out
   Start Command: dotnet out/ControlGastos.API.dll
   Plan: Free
   ```

4. Variables de entorno:

   ```yaml
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=[Copiar de Render PostgreSQL]
   Jwt__Key=[tu clave]
   ```

#### 5. Deploy Frontend

Usar Vercel (paso anterior) o Render Static Site:

1. Dashboard → New → Static Site
2. Configurar:

   ```yaml
   Name: control-gastos-app
   Root Directory: Frontend/control-gastos-app
   Build Command: npm run build
   Publish Directory: dist/control-gastos-app/browser
   ```

---

## 💡 OPCIÓN 4: Fly.io (Alternativa moderna)

### Servicios Gratuitos

```text
- 3 VMs compartidas (256MB RAM cada una)
- 3GB de almacenamiento persistente
- 160GB de transferencia
```

### Quick Start

```bash
# Instalar flyctl
curl -L https://fly.io/install.sh | sh

# Login
flyctl auth login

# Desde Backend/ControlGastos.API
flyctl launch

# Deploy
flyctl deploy
```

---

## 📊 Tabla Comparativa Detallada

| Característica | Supabase+Railway+Vercel | Azure Free | Render | Fly.io |
|----------------|-------------------------|------------|--------|--------|
| **Costo** | $0 permanente | $0 (12 meses) | $0 permanente | $0 permanente |
| **Tarjeta requerida** | Solo Railway | Sí | No | Sí |
| **BD** | PostgreSQL 500MB | SQL Server 250GB | PostgreSQL 1GB | PostgreSQL (manual) |
| **Sleep después de inactividad** | No (Railway paid) | No | Sí (50s wake) | Sí |
| **SSL/HTTPS** | ✅ Automático | ✅ Automático | ✅ Automático | ✅ Automático |
| **Custom Domain** | ✅ Gratis | ✅ Gratis | ✅ Gratis | ✅ Gratis |
| **CI/CD** | ✅ GitHub integrado | ✅ GitHub Actions | ✅ Auto-deploy | ✅ CLI |
| **Dificultad** | ⭐⭐ | ⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐ |

---

## 🎯 Recomendación Final

### Para Desarrollo/Portfolio (Recomendado)

#### Opción 1: Supabase + Railway + Vercel

- Gratis permanentemente
- Fácil de configurar
- Buena para demos y portfolios

### Para Aprendizaje de Azure

#### Opción 2: Azure Free Tier

- Aprende Azure sin costo inicial
- Mantén SQL Server
- 12 meses para decidir

### Para Prototipo Rápido

#### Opción 3: Render (todo en uno)

- Todo en una plataforma
- Setup más rápido
- No requiere tarjeta

---

## 🔄 Migración de PostgreSQL a SQL Server

Si empiezas con PostgreSQL gratis y luego quieres migrar a SQL Server:

### 1. Exportar datos

```bash
pg_dump -h [host] -U [user] -d [database] > backup.sql
```

### 2. Convertir SQL

Usar herramienta: <https://www.sqlines.com/online>

### 3. Importar a SQL Server

```sql
-- En SQL Server Management Studio
-- Ejecutar el SQL convertido
```

---

## 📝 Checklist de Despliegue Gratuito

### Checklist - Opción 1: Supabase + Railway + Vercel

- [ ] Cuenta Supabase creada
- [ ] Base de datos PostgreSQL configurada
- [ ] Tablas creadas con SQL adaptado
- [ ] Backend adaptado para PostgreSQL
- [ ] Cuenta Railway creada
- [ ] Backend desplegado en Railway
- [ ] Variables de entorno configuradas
- [ ] Cuenta Vercel creada
- [ ] Frontend desplegado en Vercel
- [ ] CORS configurado
- [ ] Aplicación funcionando end-to-end

---

## 🆘 Problemas Comunes

### Railway: "Build failed"

**Solución:**

```bash
# Verificar que railway.toml esté en la raíz
# Verificar rutas en buildCommand
```

### Vercel: "Command not found: ng"

**Solución:**

```json
// En package.json, asegurar Angular CLI
{
  "devDependencies": {
    "@angular/cli": "^17.0.0"
  }
}
```

### Supabase: "Connection refused"

**Solución:**

- Verificar que SSL Mode esté configurado
- Usar el connection string correcto (URI format)

---

## 💰 Costos después del Tier Gratuito

Si tu app crece y necesitas más recursos:

### Supabase Pro

- $25/mes
- 8GB base de datos
- 50GB transferencia

### Railway

- $5/mes
- 500 horas adicionales
- $0.000231/GB-s

### Vercel Pro

- $20/mes
- Bandwidth ilimitado
- Analytics avanzado

---

## 🚀 Próximos Pasos

Una vez desplegado:

1. **Monitoreo**: Configurar alertas en cada plataforma
2. **Backups**: Exportar BD regularmente
3. **Custom Domain**: Agregar tu propio dominio (gratis)
4. **Analytics**: Google Analytics o similar
5. **Performance**: Optimizar bundle size de Angular

---

## 📚 Recursos

- [Supabase Docs](https://supabase.com/docs)
- [Railway Docs](https://docs.railway.app)
- [Vercel Docs](https://vercel.com/docs)
- [Render Docs](https://render.com/docs)
- [Fly.io Docs](https://fly.io/docs)

---

**¡Tu app puede estar en producción GRATIS en menos de 2 horas!** 🎉
