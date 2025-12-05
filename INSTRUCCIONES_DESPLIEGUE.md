# Instrucciones de Despliegue - Opción 1: Supabase + Railway + Vercel

## Resumen de Cambios Realizados

Tu proyecto ha sido configurado automáticamente para desplegarse usando:

- **Supabase**: Base de datos PostgreSQL gratuita
- **Railway**: Backend .NET API
- **Vercel**: Frontend Angular

---

## Paso 1: Configurar Base de Datos en Supabase

### 1.1 Crear cuenta en Supabase

1. Ve a: <https://supabase.com>
2. Click en "Start your project"
3. Regístrate con GitHub (gratis, sin tarjeta de crédito)

### 1.2 Crear nuevo proyecto

1. Click en "New Project"
2. Completa los datos:

   ```text
   Organization: [Tu nombre]
   Project name: control-gastos
   Database Password: [Crea una contraseña segura - ¡GUÁRDALA!]
   Region: East US
   Pricing Plan: Free (500MB, 2GB transferencia)
   ```

3. Click "Create new project" y espera 2-3 minutos

### 1.3 Crear las tablas en la base de datos

1. En Supabase Dashboard, ve a "SQL Editor"
2. Click "New query"
3. Ejecuta el siguiente script SQL:

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

-- Tabla Roles
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

-- Insertar roles por defecto
INSERT INTO Rol (Nombre, Descripcion) VALUES
('Administrador', 'Acceso completo al sistema'),
('Usuario', 'Usuario estándar');

-- Insertar usuario admin por defecto
-- Email: admin@example.com
-- Password: Admin123!
INSERT INTO Usuario (NombreCompleto, Email, PasswordHash, Activo)
VALUES ('Administrador', 'admin@example.com',
        '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',
        TRUE);

-- Asignar rol administrador
INSERT INTO UsuarioRoles (UsuarioId, RolId)
VALUES (1, 1);
```

### 1.4 Obtener Connection String

1. En Supabase Dashboard → Settings → Database
2. En "Connection string" → "URI", copia la cadena de conexión
3. Debería verse así:

   ```text
   postgresql://postgres.[PROJECT-REF]:[YOUR-PASSWORD]@aws-0-us-east-1.pooler.supabase.com:6543/postgres
   ```

---

## Paso 2: Desplegar Backend en Railway

### 2.1 Crear cuenta en Railway

1. Ve a: <https://railway.app>
2. Sign up con GitHub
3. Verifica tu cuenta (puede requerir tarjeta, pero no cobra en plan gratuito)

### 2.2 Crear nuevo proyecto desde GitHub

1. Click "New Project"
2. Selecciona "Deploy from GitHub repo"
3. Autoriza Railway para acceder a tus repositorios
4. Selecciona el repositorio de este proyecto
5. Selecciona la rama `main`

### 2.3 Configurar variables de entorno

1. En Railway, ve a tu proyecto → Variables
2. Agrega las siguientes variables:

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
ConnectionStrings__DefaultConnection=[TU CONNECTION STRING DE SUPABASE AQUÍ]
Jwt__Key=TU_CLAVE_SECRETA_SUPER_SEGURA_DE_AL_MENOS_32_CARACTERES
Jwt__Issuer=ControlGastosAPI
Jwt__Audience=ControlGastosApp
Jwt__ExpirationDays=7
```

**IMPORTANTE**: Reemplaza `[TU CONNECTION STRING DE SUPABASE AQUÍ]` con la cadena que copiaste en el paso 1.4

Ejemplo de ConnectionStrings__DefaultConnection:

```text
Host=db.abcdefghij.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TuPasswordDeSupabase;SSL Mode=Require;Trust Server Certificate=true
```

### 2.4 Generar dominio público

1. En Railway → Settings → Networking
2. Click "Generate Domain"
3. Se generará una URL como: `https://control-gastos-api-production.up.railway.app`
4. **COPIA ESTA URL** - la necesitarás para el frontend

### 2.5 Verificar despliegue

1. Espera a que Railway construya y despliegue (2-5 minutos)
2. Verifica que funcione visitando: `https://[TU-URL-RAILWAY]/swagger`
3. Deberías ver la documentación de Swagger UI

---

## Paso 3: Desplegar Frontend en Vercel

### 3.1 Actualizar URL del API

1. Abre el archivo: `Frontend/control-gastos-app/src/environments/environment.prod.ts`
2. Reemplaza `[YOUR-RAILWAY-APP-URL]` con tu URL de Railway:

   ```typescript
   export const environment = {
     production: true,
     apiUrl: 'https://control-gastos-api-production.up.railway.app/api'
   };
   ```

3. **GUARDA** el archivo y haz commit de los cambios:

```bash
git add .
git commit -m "Configurar URL de producción"
git push
```

### 3.2 Crear cuenta en Vercel

1. Ve a: <https://vercel.com>
2. Sign up con GitHub (gratis, sin tarjeta de crédito)

### 3.3 Importar proyecto

1. En Vercel Dashboard, click "New Project"
2. Click "Import Git Repository"
3. Busca y selecciona tu repositorio
4. Configura el proyecto:

   ```text
   Framework Preset: Angular
   Root Directory: Frontend/control-gastos-app
   Build Command: npm run build
   Output Directory: dist/control-gastos-app/browser
   Install Command: npm install
   ```

5. Click "Deploy"

### 3.4 Obtener URL de Vercel

1. Espera a que Vercel construya y despliegue (2-4 minutos)
2. Obtendrás una URL como: `https://control-gastos-app.vercel.app`
3. **COPIA ESTA URL**

---

## Paso 4: Actualizar CORS en Railway

### 4.1 Agregar dominio de Vercel a CORS

Tu archivo `Program.cs` ya está configurado para aceptar dominios de Vercel (`*.vercel.app`), pero si quieres ser más específico:

1. Ve al archivo `Backend/ControlGastos.API/Program.cs`
2. Busca la sección de CORS (líneas 52-66)
3. Puedes agregar tu dominio específico de Vercel:

   ```csharp
   policy.WithOrigins(
       "http://localhost:4200",
       "https://control-gastos-app.vercel.app",  // Tu dominio específico
       "https://*.vercel.app"  // Todos los previews
   )
   ```

4. Haz commit y push de los cambios - Railway se redespliegará automáticamente

---

## Paso 5: Probar la aplicación

### 5.1 Acceder a la aplicación

1. Visita tu URL de Vercel: `https://control-gastos-app.vercel.app`
2. Intenta hacer login con:
   - Email: `admin@example.com`
   - Password: `Admin123!`

### 5.2 Verificar funcionalidad

- Prueba crear un tipo de gasto
- Prueba crear un fondo monetario
- Prueba registrar un gasto
- Verifica que los reportes funcionan

---

## Checklist de Despliegue

- [ ] Cuenta Supabase creada
- [ ] Base de datos PostgreSQL configurada
- [ ] Tablas creadas con el script SQL
- [ ] Connection string de Supabase copiada
- [ ] Cuenta Railway creada
- [ ] Backend desplegado en Railway
- [ ] Variables de entorno configuradas en Railway
- [ ] URL de Railway copiada
- [ ] environment.prod.ts actualizado con URL de Railway
- [ ] Cambios commiteados y pusheados
- [ ] Cuenta Vercel creada
- [ ] Frontend desplegado en Vercel
- [ ] CORS configurado correctamente
- [ ] Login funciona correctamente
- [ ] CRUD de gastos funciona

---

## Problemas Comunes

### Error: "Connection refused" en Railway

**Solución:**

- Verifica que el ConnectionString en las variables de entorno esté correcto
- Asegúrate de que SSL Mode esté configurado: `SSL Mode=Require;Trust Server Certificate=true`

### Error: "CORS policy" en el navegador

**Solución:**

- Verifica que la URL de Vercel esté permitida en Program.cs
- Asegúrate de que Railway se haya redespliegado después de cambiar CORS

### Error: "Command not found: ng" en Vercel

**Solución:**

- Verifica que `@angular/cli` esté en devDependencies en package.json
- En la configuración de Vercel, asegúrate de que Build Command sea: `npm run build`

### El frontend no se conecta al backend

**Solución:**

- Verifica que environment.prod.ts tenga la URL correcta de Railway
- Asegúrate de haber hecho commit y push de los cambios
- Verifica que Vercel haya redespliegado después del cambio

---

## Límites del Plan Gratuito

### Supabase Free Tier

- 500 MB de base de datos
- 2 GB de transferencia/mes
- Sin tarjeta de crédito requerida

### Railway Free Tier

- 500 horas/mes de ejecución
- $5 de crédito/mes (requiere tarjeta pero no cobra si no superas el límite)
- 512 MB RAM
- 1 GB disco

### Vercel Free Tier

- 100 GB bandwidth/mes
- Deployments ilimitados
- Sin tarjeta de crédito requerida

---

## Monitoreo y Mantenimiento

### Logs en Railway

1. Ve a tu proyecto en Railway
2. Click en el servicio
3. Ve a la pestaña "Deployments"
4. Click en el último deployment → "View Logs"

### Logs en Vercel

1. Ve a tu proyecto en Vercel
2. Click en la pestaña "Deployments"
3. Click en el último deployment → "View Function Logs"

### Backups de Base de Datos

1. En Supabase Dashboard → Database → Backups
2. El plan gratuito hace backups automáticos por 7 días
3. Puedes hacer backups manuales exportando el SQL

---

## Próximos Pasos Recomendados

1. **Custom Domain**: Agrega tu propio dominio en Vercel (gratis)
2. **Monitoreo**: Configura alertas en Railway y Vercel
3. **Analytics**: Agrega Google Analytics al frontend
4. **Performance**: Optimiza el bundle size de Angular
5. **Seguridad**: Cambia el password del usuario admin

---

## Recursos de Ayuda

- [Documentación Supabase](https://supabase.com/docs)
- [Documentación Railway](https://docs.railway.app)
- [Documentación Vercel](https://vercel.com/docs)

---

**¡Tu aplicación está lista para producción GRATIS!**
