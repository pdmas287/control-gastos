# 🚀 Guía de Despliegue en Azure - Control de Gastos

## 📋 Tabla de Contenidos

1. [Requisitos Previos](#requisitos-previos)
2. [Resumen de Servicios Azure](#resumen-de-servicios-azure)
3. [Paso 1: Configurar Base de Datos (Azure SQL)](#paso-1-configurar-base-de-datos-azure-sql)
4. [Paso 2: Desplegar Backend (.NET API)](#paso-2-desplegar-backend-net-api)
5. [Paso 3: Desplegar Frontend (Angular)](#paso-3-desplegar-frontend-angular)
6. [Paso 4: Configuración Final](#paso-4-configuración-final)
7. [Costos Estimados](#costos-estimados)
8. [Troubleshooting](#troubleshooting)

---

## Requisitos Previos

### ✅ Software Necesario

- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) instalado
- [Node.js](https://nodejs.org/) (v18 o superior)
- [.NET SDK](https://dotnet.microsoft.com/download) (v8 o superior)
- [Git](https://git-scm.com/) instalado

### ✅ Cuenta Azure

- Cuenta de Azure activa (prueba gratuita disponible)
- Suscripción con permisos de administrador

### ✅ Verificar instalaciones

```bash
# Verificar instalaciones
az --version
node --version
dotnet --version
git --version
```

---

## Resumen de Servicios Azure

Para este proyecto necesitarás:

| Servicio | Propósito | Costo Aproximado |
|----------|-----------|------------------|
| **Azure SQL Database** | Base de datos SQL Server | $5-15/mes (Basic tier) |
| **Azure App Service (Backend)** | Hosting de API .NET | $13-55/mes (B1-B2) |
| **Azure Static Web Apps** | Hosting de Angular | GRATIS (tier gratuito) |
| **Total estimado** | | **$18-70/mes** |

---

## Paso 1: Configurar Base de Datos (Azure SQL)

### 1.1 Crear Azure SQL Database desde el Portal

1. **Iniciar sesión en Azure Portal**
   - Ir a: <https://portal.azure.com>

2. **Crear Recurso**
   - Click en "Create a resource"
   - Buscar "SQL Database"
   - Click en "Create"

3. **Configuración Básica**

   ```yaml
   Subscription: [Tu suscripción]
   Resource Group: rg-control-gastos (crear nuevo)
   Database name: db-control-gastos
   Server: [Crear nuevo servidor]
   ```

4. **Configurar SQL Server** (Click en "Create new" en Server)

   ```yaml
   Server name: sql-control-gastos-[tu-nombre] (debe ser único)
   Location: East US (o tu región preferida)
   Authentication method: Use SQL authentication
   Server admin login: sqladmin
   Password: [Contraseña segura - guárdala!]
   ```

5. **Configurar Compute + Storage**

   ```yaml
   Service tier: Basic (5 DTUs, 2GB) - Para empezar
   ```

6. **Networking**

   ```yaml
   Connectivity method: Public endpoint

   Firewall rules:
   ✅ Allow Azure services and resources to access this server
   ✅ Add current client IP address

   ```yaml

7. **Review + Create**

- Verificar configuración
  - Click "Create"
  - ⏱️ Esperar 3-5 minutos

### 1.2 Configurar Base de Datos con Scripts SQL

1. **Conectar a Azure SQL Database**

   Opción A - Usando Azure Portal:
   - En el portal, ir a tu base de datos
   - Click en "Query editor"
   - Iniciar sesión con las credenciales creadas

   Opción B - Usando SQL Server Management Studio (SSMS):

      ```yaml
      Server name: sql-control-gastos-[tu-nombre].database.windows.net
   Authentication: SQL Server Authentication
   Login: sqladmin
   Password: [tu contraseña]
   ```

2. **Ejecutar Scripts EN ORDEN**

   Abrir y ejecutar cada archivo SQL:

   ```sql
   -- ⚠️ IMPORTANTE: NO ejecutar 01_CreateDatabase.sql
   -- La base de datos ya existe en Azure

   -- 1. Ejecutar: Database/02_StoredProcedures.sql
   -- 2. Ejecutar: Database/03_AddAuthenticationTables.sql
   -- 3. Ejecutar: Database/04_CreateAdminUser.sql (OPCIONAL)
   -- 4. Ejecutar: Database/07_AddRolesSystem.sql (Si usas roles)
   ```

3. **Verificar Tablas Creadas**

   ```sql
   SELECT TABLE_NAME
   FROM INFORMATION_SCHEMA.TABLES
   WHERE TABLE_TYPE = 'BASE TABLE'
   ORDER BY TABLE_NAME;
   ```

   Deberías ver:
   - Usuario
   - TipoGasto
   - FondoMonetario
   - Presupuesto
   - RegistroGasto
   - Deposito
   - Roles (si aplicaste el sistema de roles)

### 1.3 Obtener Connection String

1. En Azure Portal, ir a tu base de datos
2. Click en "Connection strings" (en el menú izquierdo)
3. Copiar el "ADO.NET" connection string:

```bash
Server=tcp:sql-control-gastos-[tu-nombre].database.windows.net,1433;Initial Catalog=db-control-gastos;Persist Security Info=False;User ID=sqladmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

1. **⚠️ IMPORTANTE:** Reemplazar `{your_password}` con tu contraseña real
2. Guardar esta cadena - la necesitarás para el backend

---

## Paso 2: Desplegar Backend (.NET API)

### 2.1 Preparar el Proyecto Backend

1. **Abrir terminal en tu proyecto**

   ```bash
   cd Backend/ControlGastos.API
   ```

2. **Actualizar appsettings.json** (Para producción)

   Crear archivo `appsettings.Production.json`:

      ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "SERA_CONFIGURADO_EN_AZURE"
     },
     "Jwt": {
       "Key": "TU_CLAVE_SECRETA_SUPER_SEGURA_DE_AL_MENOS_32_CARACTERES",
       "Issuer": "ControlGastosAPI",
       "Audience": "ControlGastosApp",
       "ExpirationDays": 7
     },
     "AllowedHosts": "*",
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     }
   }
   ```

3. **Verificar que el proyecto compile**

   ```bash
   dotnet build --configuration Release
   ```

### 2.2 Crear Azure App Service para Backend

#### Opción A: Usando Azure Portal (Más Visual)

1. **Ir a Azure Portal** → "Create a resource" → "Web App"

2. **Configuración Básica**

   ```yaml
   Resource Group: rg-control-gastos (el mismo de antes)
   Name: app-control-gastos-api-[tu-nombre]
   Publish: Code
   Runtime stack: .NET 8 (LTS)
   Operating System: Windows
   Region: East US (misma región que la BD)
   ```

3. **App Service Plan**

   ```yaml
   Windows Plan: [Crear nuevo]
   Sku and size: B1 (Basic - $13/mes)
   ```

4. **Deployment** (Tab)

   ```yaml
   Continuous deployment: Disable (por ahora)
   ```

5. **Review + Create** → "Create"

#### Opción B: Usando Azure CLI (Más Rápido)

```bash
# Login a Azure
az login

# Crear App Service Plan
az appservice plan create \
  --name plan-control-gastos \
  --resource-group rg-control-gastos \
  --sku B1 \
  --is-linux false

# Crear Web App
az webapp create \
  --name app-control-gastos-api-[tu-nombre] \
  --resource-group rg-control-gastos \
  --plan plan-control-gastos \
  --runtime "DOTNET:8"
```

### 2.3 Configurar Variables de Entorno

1. **En Azure Portal**, ir a tu App Service
2. Click en "Configuration" (menú izquierdo)
3. Click en "New connection string"

   ```yaml
   Name: DefaultConnection
   Value: [Tu connection string de Azure SQL]
   Type: SQLAzure
   ```

4. **Agregar Application Settings**

   Click "New application setting" para cada uno:

   ```yaml
   Jwt__Key = TU_CLAVE_SECRETA_SUPER_SEGURA_DE_AL_MENOS_32_CARACTERES
   Jwt__Issuer = ControlGastosAPI
   Jwt__Audience = ControlGastosApp
   Jwt__ExpirationDays = 7
   ```

5. **Configurar CORS**
   - Ir a "CORS" (menú izquierdo)
   - En "Allowed Origins" agregar:

     ```yaml
     https://[tu-app-frontend].azurestaticapps.net
     http://localhost:4200
     ```

   - ✅ Enable Access-Control-Allow-Credentials

6. **Guardar** cambios

### 2.4 Desplegar el Backend

#### Método 1: Deploy desde Visual Studio

1. Click derecho en el proyecto → "Publish"
2. Target: Azure
3. Specific Target: Azure App Service (Windows)
4. Seleccionar tu App Service
5. Click "Publish"

#### Método 2: Deploy con Azure CLI

```bash
# Desde Backend/ControlGastos.API/

# Publicar proyecto localmente
dotnet publish -c Release -o ./publish

# Comprimir archivos
cd publish
zip -r ../deploy.zip .
cd ..

# Desplegar a Azure
az webapp deployment source config-zip \
  --resource-group rg-control-gastos \
  --name app-control-gastos-api-[tu-nombre] \
  --src deploy.zip
```

**Método 3: Deploy con GitHub Actions** (Recomendado)

1. **Subir código a GitHub**

   ```bash
   # Desde la raíz del proyecto
   git init
   git add .
   git commit -m "Initial commit"
   git branch -M main
   git remote add origin https://github.com/tu-usuario/control-gastos.git
   git push -u origin main
   ```

2. **Obtener Publish Profile**
   - En Azure Portal, en tu App Service
   - Click "Get publish profile"
   - Guardar el contenido XML

3. **Configurar GitHub Secret**
   - En GitHub, ir a Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Value: [Pegar el contenido del publish profile]

4. **Crear GitHub Workflow**

   Crear archivo: `.github/workflows/deploy-backend.yml`

   ```yaml
   name: Deploy Backend to Azure

   on:
     push:
       branches:
         - main
       paths:
         - 'Backend/**'
     workflow_dispatch:

   jobs:
     build-and-deploy:
       runs-on: windows-latest

       steps:
       - uses: actions/checkout@v3

       - name: Setup .NET
         uses: actions/setup-dotnet@v3
         with:
           dotnet-version: '8.0.x'

       - name: Build and publish
         run: |
           cd Backend/ControlGastos.API
           dotnet restore
           dotnet build --configuration Release
           dotnet publish -c Release -o ./publish

       - name: Deploy to Azure Web App
         uses: azure/webapps-deploy@v2
         with:
           app-name: 'app-control-gastos-api-[tu-nombre]'
           publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
           package: Backend/ControlGastos.API/publish
   ```

5. **Commit y Push**

   ```bash
   git add .
   git commit -m "Add GitHub Actions deployment"
   git push
   ```

### 2.5 Verificar Backend

1. **Abrir URL del backend**

   ```bash
   https://app-control-gastos-api-[tu-nombre].azurewebsites.net
   ```bash

2. **Probar Swagger** (si está habilitado)

   ```bash
   https://app-control-gastos-api-[tu-nombre].azurewebsites.net/swagger
   ```bash

3. **Probar endpoint de salud**

   ```bash
   curl https://app-control-gastos-api-[tu-nombre].azurewebsites.net/api/auth/login
   ```

---

## Paso 3: Desplegar Frontend (Angular)

### 3.1 Preparar el Proyecto Frontend

1. **Navegar al proyecto frontend**

   ```bash
   cd Frontend/control-gastos-app
   ```

2. **Actualizar URLs de API**

   Crear archivo: `src/environments/environment.prod.ts`

   ```typescript
   export const environment = {
     production: true,
     apiUrl: 'https://app-control-gastos-api-[tu-nombre].azurewebsites.net/api'
   };
   ```

   Asegúrate de que `src/environments/environment.ts` sea:

   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'http://localhost:5000/api'
   };
   ```

3. **Actualizar servicios para usar environment**

   En cada servicio (auth.service.ts, tipo-gasto.service.ts, etc.):

   ```typescript
   import { environment } from '../../environments/environment';

   export class AuthService {
     private apiUrl = `${environment.apiUrl}/auth`;
     // ...
   ```

4. **Instalar dependencias**

   ```bash
   npm install
   ```

5. **Build de producción (prueba local)**

   ```bash
   npm run build -- --configuration production
   ```

### 3.2 Crear Azure Static Web App

#### Opción A: Usando Azure Portal

1. **Ir a Azure Portal** → "Create a resource" → "Static Web App"

2. **Configuración Básica**

   ```yaml
   Resource Group: rg-control-gastos
   Name: swa-control-gastos-[tu-nombre]
   Plan type: Free
   Region: East US 2 (para free tier)
   ```

3. **Deployment Details**

   ```yaml
   Source: GitHub

   [Autorizar GitHub]

   Organization: [Tu usuario]
   Repository: control-gastos
   Branch: main
   ```

4. **Build Details**

   ```yaml
   Build Presets: Angular
   App location: /Frontend/control-gastos-app
   Api location: [dejar vacío]
   Output location: dist/control-gastos-app/browser
   ```

5. **Review + Create** → "Create"

#### Opción B: Usando Azure CLI con GitHub

```bash
# Login a Azure
az login

# Crear Static Web App
az staticwebapp create \
  --name swa-control-gastos-[tu-nombre] \
  --resource-group rg-control-gastos \
  --source https://github.com/tu-usuario/control-gastos \
  --location "eastus2" \
  --branch main \
  --app-location "/Frontend/control-gastos-app" \
  --output-location "dist/control-gastos-app/browser" \
  --login-with-github
```

### 3.3 Configurar Build Workflow

Azure creará automáticamente un workflow. Verificar/modificar:

**Archivo:** `.github/workflows/azure-static-web-apps-[nombre].yml`

```yaml
name: Azure Static Web Apps CI/CD

on:
  push:
    branches:
      - main
    paths:
      - 'Frontend/**'
  pull_request:
    types: [opened, synchronize, reopened, closed]
    branches:
      - main

jobs:
  build_and_deploy_job:
    if: github.event_name == 'push' || (github.event_name == 'pull_request' && github.event.action != 'closed')
    runs-on: ubuntu-latest
    name: Build and Deploy Job
    steps:
      - uses: actions/checkout@v3
        with:
          submodules: true

      - name: Build And Deploy
        id: builddeploy
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN_[ID] }}
          repo_token: ${{ secrets.GITHUB_TOKEN }}
          action: "upload"
          app_location: "/Frontend/control-gastos-app"
          api_location: ""
          output_location: "dist/control-gastos-app/browser"

  close_pull_request_job:
    if: github.event_name == 'pull_request' && github.event.action == 'closed'
    runs-on: ubuntu-latest
    name: Close Pull Request Job
    steps:
      - name: Close Pull Request
        id: closepullrequest
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN_[ID] }}
          action: "close"
```

### 3.4 Configurar Variables de Entorno en Static Web App

1. **En Azure Portal**, ir a tu Static Web App
2. Click en "Configuration" (menú izquierdo)
3. Click "Add" en Application settings:

   ```yaml
   Name: API_URL
   Value: https://app-control-gastos-api-[tu-nombre].azurewebsites.net/api

   ```yaml

   (Opcional - si quieres leerlo desde la configuración)

4. Guardar cambios

### 3.5 Deploy del Frontend

**Si usaste GitHub:**

- El push automáticamente dispara el deploy
- Monitorear en GitHub Actions

**Si NO usas GitHub:**

```bash
# Instalar SWA CLI
npm install -g @azure/static-web-apps-cli

# Build de producción
npm run build -- --configuration production

# Deploy
swa deploy ./dist/control-gastos-app/browser \
  --deployment-token [TU_DEPLOYMENT_TOKEN] \
  --app-name swa-control-gastos-[tu-nombre]
```

### 3.6 Verificar Frontend

1. **Obtener URL**
   - En Azure Portal, en tu Static Web App
   - Ver "URL" en el Overview

   ```bash
   https://[nombre-generado].azurestaticapps.net
   ```

2. **Probar la aplicación**
   - Abrir URL en navegador
   - Debería redirigir a /login
   - Probar registro y login

---

## Paso 4: Configuración Final

### 4.1 Actualizar CORS en Backend

Ahora que tienes la URL del frontend:

1. Ir al App Service del backend
2. Configuration → CORS
3. Actualizar "Allowed Origins":

   ```yaml
   https://[tu-frontend].azurestaticapps.net
   ```

### 4.2 Configurar Custom Domain (Opcional)

**Para el Backend:**

1. App Service → Custom domains
2. Add custom domain
3. Configurar DNS

**Para el Frontend:**

1. Static Web App → Custom domains
2. Add custom domain
3. Configurar DNS

### 4.3 Habilitar HTTPS/SSL

- **Backend:** Automático con *.azurewebsites.net
- **Frontend:** Automático con *.azurestaticapps.net
- **Custom Domain:** Azure proporciona certificado SSL gratuito

### 4.4 Configurar Monitoring (Opcional pero Recomendado)

1. **Application Insights**

   ```bash
   az monitor app-insights component create \
     --app ai-control-gastos \
     --location eastus \
     --resource-group rg-control-gastos
   ```

2. **Conectar al Backend**
   - App Service → Application Insights → Enable
   - Seleccionar el recurso creado

3. **Ver métricas**
   - Application Insights → Live Metrics
   - Ver requests, errores, performance

### 4.5 Configurar Backups (Recomendado)

**Base de Datos:**

1. Azure SQL Database → Backups
2. Configurar retention policy
3. Los backups automáticos ya están habilitados

**App Service:**

1. App Service → Backups
2. Configurar backup schedule (requiere plan Standard o superior)

---

## Costos Estimados

### Tier Gratuito / Desarrollo

```text
Azure SQL Database (Basic): $5/mes
App Service (B1): $13/mes
Static Web App: GRATIS
------------------------------
Total: ~$18/mes
```

### Tier Producción Pequeña

```text
Azure SQL Database (S0): $15/mes
App Service (B2): $30/mes
Static Web App: GRATIS
------------------------------
Total: ~$45/mes
```

### Tier Producción Media

```text
Azure SQL Database (S1): $30/mes
App Service (S1): $70/mes
Static Web App (Standard): $9/mes
Application Insights: $5/mes
------------------------------
Total: ~$114/mes
```

### Optimización de Costos

1. **Usar tier gratuito de Static Web Apps** - Suficiente para la mayoría de casos
2. **Escalar solo cuando sea necesario** - Empezar con Basic
3. **Usar reservations** - Descuentos de hasta 72% con compromiso de 1-3 años
4. **Auto-shutdown para desarrollo** - Apagar recursos cuando no se usen

---

## Troubleshooting

### Problema: Backend devuelve 500 Error

**Solución:**

1. Ir a App Service → Log stream
2. Ver logs en tiempo real
3. Verificar connection string
4. Verificar que las variables Jwt__ estén configuradas

```bash
# Ver logs
az webapp log tail --name app-control-gastos-api-[tu-nombre] --resource-group rg-control-gastos
```

### Problema: Frontend no puede conectar a Backend

**Solución:**

1. Verificar que environment.prod.ts tenga la URL correcta
2. Verificar CORS en backend
3. Abrir DevTools → Network → Ver errores
4. Verificar que el backend esté respondiendo

### Problema: Error de autenticación JWT

**Solución:**

1. Verificar que Jwt__Key sea la misma en todos los ambientes
2. Verificar que el token no esté expirado
3. Verificar que el header Authorization se envíe correctamente

### Problema: No se pueden conectar a Azure SQL

**Solución:**

1. Verificar firewall rules en SQL Server
2. Agregar IP actual:

   ```bash
   az sql server firewall-rule create \
     --resource-group rg-control-gastos \
     --server sql-control-gastos-[tu-nombre] \
     --name AllowMyIP \
     --start-ip-address [TU_IP] \
     --end-ip-address [TU_IP]
   ```

### Problema: Build falla en GitHub Actions

**Solución:**

1. Ver logs de GitHub Actions
2. Verificar que las rutas en el workflow sean correctas
3. Verificar que node_modules no esté en .gitignore
4. Limpiar cache:

   ```yaml
   - name: Clear npm cache
     run: npm cache clean --force
   ```

### Problema: Static Web App muestra 404

**Solución:**

1. Verificar output_location en workflow
2. Para Angular, debe ser: `dist/control-gastos-app/browser`
3. Verificar build log en GitHub Actions

### Problema: CORS errors

**Solución:**

1. Backend → CORS → Agregar origin del frontend
2. Verificar que incluya https://
3. No usar * en producción
4. Habilitar credentials si usas cookies/auth

---

## Comandos Útiles Azure CLI

```bash
# Ver todos los recursos
az resource list --resource-group rg-control-gastos --output table

# Ver estado de App Service
az webapp show --name app-control-gastos-api-[tu-nombre] --resource-group rg-control-gastos

# Ver logs
az webapp log tail --name app-control-gastos-api-[tu-nombre] --resource-group rg-control-gastos

# Restart App Service
az webapp restart --name app-control-gastos-api-[tu-nombre] --resource-group rg-control-gastos

# Ver connection string
az webapp config connection-string list --name app-control-gastos-api-[tu-nombre] --resource-group rg-control-gastos

# Actualizar app setting
az webapp config appsettings set --name app-control-gastos-api-[tu-nombre] --resource-group rg-control-gastos --settings Jwt__Key="nueva-clave"

# Eliminar todo (CUIDADO!)
az group delete --name rg-control-gastos --yes
```

---

## Checklist de Despliegue

### Base de Datos

- [ ] Azure SQL Database creada
- [ ] Firewall configurado
- [ ] Scripts SQL ejecutados
- [ ] Connection string obtenida
- [ ] Tablas verificadas

### Backend

- [ ] App Service creado
- [ ] Connection string configurada
- [ ] Variables JWT configuradas
- [ ] CORS configurado
- [ ] Aplicación desplegada
- [ ] Swagger funciona
- [ ] Endpoints responden

### Frontend

- [ ] Static Web App creada
- [ ] environment.prod.ts configurado
- [ ] Build exitoso
- [ ] Aplicación desplegada
- [ ] Routing funciona
- [ ] Login funciona
- [ ] Llamadas a API funcionan

### Configuración Final

- [ ] CORS actualizado con URL final
- [ ] SSL/HTTPS habilitado
- [ ] Monitoring configurado (opcional)
- [ ] Backups configurados
- [ ] Documentación actualizada

---

## Próximos Pasos

1. **Configurar CI/CD completo** con GitHub Actions
2. **Agregar Application Insights** para monitoreo
3. **Configurar alerts** para errores y downtime
4. **Optimizar performance** con CDN
5. **Configurar staging environment** para pruebas
6. **Implementar feature flags** para releases graduales
7. **Configurar backup/restore procedures**

---

## Recursos Adicionales

- [Azure SQL Database Docs](https://learn.microsoft.com/en-us/azure/azure-sql/)
- [Azure App Service Docs](https://learn.microsoft.com/en-us/azure/app-service/)
- [Azure Static Web Apps Docs](https://learn.microsoft.com/en-us/azure/static-web-apps/)
- [Azure CLI Reference](https://learn.microsoft.com/en-us/cli/azure/)
- [GitHub Actions for Azure](https://github.com/Azure/actions)

---

**¡Felicidades!** 🎉 Tu aplicación de Control de Gastos está ahora en Azure y lista para producción.

Para soporte, consulta los logs y la documentación de Azure o contacta al equipo de desarrollo.
