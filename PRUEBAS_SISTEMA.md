# 🧪 Guía de Pruebas - Sistema de Autenticación

## ✅ Estado de la Implementación

**COMPLETADO AL 100%** - Todos los componentes están implementados y listos para probar.

---

## 📋 Checklist de Verificación

### **Backend**
- [x] Script de base de datos creado
- [x] Modelo Usuario creado
- [x] AuthService implementado
- [x] AuthController implementado
- [x] JWT configurado
- [x] Todos los servicios filtrados por usuario
- [x] Todos los controladores protegidos

### **Frontend**
- [x] auth.model.ts creado
- [x] auth.service.ts implementado
- [x] auth.guard.ts implementado
- [x] auth.interceptor.ts implementado
- [x] LoginComponent creado
- [x] RegistroComponent creado
- [x] Rutas protegidas configuradas
- [x] Navbar con logout
- [x] Interceptor registrado en main.ts

---

## 🚀 Pasos para Ejecutar y Probar

### **PASO 1: Preparar la Base de Datos**

```sql
-- 1. Abrir SQL Server Management Studio
-- 2. Conectarse a: localhost\SQLEXPRESS
-- 3. Ejecutar en orden:

-- Si es primera vez (base de datos nueva):
USE master;
GO

-- Ejecutar: Database/01_CreateDatabase.sql
-- Ejecutar: Database/02_StoredProcedures.sql

-- Ejecutar el nuevo script de autenticación:
-- Ejecutar: Database/03_AddAuthenticationTables.sql

-- Verificar que las tablas se crearon correctamente:
USE ControlGastosDB;
GO

SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Usuario';
-- Debería mostrar la tabla Usuario

SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TipoGasto' AND COLUMN_NAME = 'UsuarioId';
-- Debería mostrar la columna UsuarioId
```

**⚠️ Si ya tienes datos en la base de datos:**
```sql
-- Primero, crea un usuario de prueba manualmente
-- (o regístralo desde el frontend después)

-- Luego asigna todos los datos existentes a ese usuario:
UPDATE TipoGasto SET UsuarioId = 1 WHERE UsuarioId IS NULL;
UPDATE FondoMonetario SET UsuarioId = 1 WHERE UsuarioId IS NULL;
UPDATE Presupuesto SET UsuarioId = 1 WHERE UsuarioId IS NULL;
UPDATE RegistroGastoEncabezado SET UsuarioId = 1 WHERE UsuarioId IS NULL;
UPDATE Deposito SET UsuarioId = 1 WHERE UsuarioId IS NULL;
```

---

### **PASO 2: Ejecutar el Backend**

```bash
# En una terminal, navega a:
cd Backend/ControlGastos.API

# Compila el proyecto:
dotnet build

# Si hay errores de compilación, revísalos
# Si compila exitosamente, ejecuta:
dotnet run

# Deberías ver algo como:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: http://localhost:5000
#       Application started.
```

**Verificar que el backend funciona:**
1. Abre tu navegador
2. Ve a: `http://localhost:5000/swagger`
3. Deberías ver la documentación de Swagger con todos los endpoints

---

### **PASO 3: Ejecutar el Frontend**

```bash
# En OTRA terminal (deja el backend corriendo), navega a:
cd Frontend/control-gastos-app

# Si es la primera vez, instala dependencias:
npm install

# Ejecuta el servidor de desarrollo:
ng serve

# Deberías ver algo como:
# ** Angular Live Development Server is listening on localhost:4200 **
# ✔ Compiled successfully.
```

**Verificar que el frontend funciona:**
1. Abre tu navegador
2. Ve a: `http://localhost:4200`
3. **Deberías ser redirigido automáticamente a `/login`**

---

## 🧪 PRUEBAS A REALIZAR

### **Prueba 1: Registro de Usuario**

1. En `http://localhost:4200`, deberías ver la pantalla de **Login**
2. Click en **"Regístrate aquí"**
3. Completa el formulario de registro:
   - **Nombre Completo:** Juan Pérez
   - **Nombre de Usuario:** juanperez
   - **Email:** juan@example.com
   - **Contraseña:** password123
   - **Confirmar Contraseña:** password123
4. Click en **"Registrarse"**

**Resultado esperado:**
- ✅ Deberías ser redirigido automáticamente a `/home`
- ✅ Deberías ver la navbar con tu nombre "Juan Pérez" arriba a la derecha
- ✅ Deberías ver el botón "Cerrar Sesión"

**Si hay error:**
- ❌ Revisa la consola del navegador (F12 → Console)
- ❌ Revisa la consola del backend
- ❌ Verifica que el backend esté corriendo

---

### **Prueba 2: Crear Datos**

1. Estando logueado, ve a **"Mantenimientos" → "Tipos de Gasto"**
2. Crea un nuevo tipo de gasto:
   - **Descripción:** Alimentación
3. Ve a **"Mantenimientos" → "Fondos Monetarios"**
4. Crea un fondo:
   - **Nombre:** Caja Chica
   - **Tipo Fondo:** Caja Menuda
   - **Saldo Actual:** 1000

**Resultado esperado:**
- ✅ Los datos se crean correctamente
- ✅ Puedes verlos en la lista

---

### **Prueba 3: Cerrar Sesión**

1. Click en el botón **"Cerrar Sesión"** (arriba a la derecha)

**Resultado esperado:**
- ✅ Deberías ser redirigido a `/login`
- ✅ La navbar debería desaparecer
- ✅ No deberías poder acceder a rutas protegidas

**Prueba manual:**
- Intenta ir directamente a `http://localhost:4200/home`
- ✅ Deberías ser redirigido automáticamente a `/login`

---

### **Prueba 4: Iniciar Sesión**

1. En la pantalla de login, ingresa:
   - **Usuario o Email:** juanperez (o juan@example.com)
   - **Contraseña:** password123
2. Click en **"Iniciar Sesión"**

**Resultado esperado:**
- ✅ Deberías ser redirigido a `/home`
- ✅ Deberías ver tus datos (tipos de gasto, fondos, etc.)

---

### **Prueba 5: Multi-Usuario (Aislamiento de Datos)**

1. **Estando logueado**, nota cuántos tipos de gasto tienes
2. Click en **"Cerrar Sesión"**
3. Click en **"Regístrate aquí"**
4. Registra un **SEGUNDO usuario:**
   - **Nombre Completo:** María García
   - **Nombre de Usuario:** mariagarcia
   - **Email:** maria@example.com
   - **Contraseña:** password456
   - **Confirmar Contraseña:** password456
5. Una vez logueado con María, ve a **"Mantenimientos" → "Tipos de Gasto"**

**Resultado esperado:**
- ✅ La lista debería estar **VACÍA** (María no tiene tipos de gasto)
- ✅ María NO debería ver los datos de Juan
- ✅ Puedes crear datos para María
6. Cierra sesión con María
7. Inicia sesión con Juan (juanperez / password123)
8. Ve a tipos de gasto

**Resultado esperado:**
- ✅ Deberías ver **SOLO los datos de Juan**
- ✅ NO deberías ver los datos de María

**✅ ESTO CONFIRMA QUE EL AISLAMIENTO POR USUARIO FUNCIONA CORRECTAMENTE**

---

### **Prueba 6: Token Expirado**

1. Inicia sesión
2. Abre las **DevTools** del navegador (F12)
3. Ve a la pestaña **"Application"** (o "Almacenamiento")
4. En el menú izquierdo, selecciona **"Local Storage" → "http://localhost:4200"**
5. Deberías ver un item `currentUser` con el token
6. **Borra** ese item (click derecho → Delete)
7. Intenta navegar a cualquier página del sistema

**Resultado esperado:**
- ✅ Deberías ser redirigido automáticamente a `/login`
- ✅ Deberías ver el mensaje de error o simplemente la pantalla de login

---

### **Prueba 7: Swagger (Endpoints del Backend)**

1. Abre: `http://localhost:5000/swagger`
2. Busca el endpoint **`POST /api/auth/registro`**
3. Click en **"Try it out"**
4. Ingresa un JSON:
```json
{
  "nombreUsuario": "testuser",
  "email": "test@example.com",
  "password": "password123",
  "nombreCompleto": "Test User"
}
```
5. Click en **"Execute"**

**Resultado esperado:**
- ✅ Código de respuesta: **200 OK**
- ✅ Response body con:
  ```json
  {
    "usuarioId": 3,
    "nombreUsuario": "testuser",
    "email": "test@example.com",
    "nombreCompleto": "Test User",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "fechaExpiracion": "2024-12-07T..."
  }
  ```

6. **Copia el token** (el string largo que empieza con `eyJ...`)
7. Click en el botón **"Authorize"** (candado verde arriba a la derecha)
8. En el campo "Value", escribe: `Bearer TU_TOKEN_AQUI` (reemplaza TU_TOKEN_AQUI con el token copiado)
9. Click en **"Authorize"**
10. Click en **"Close"**

Ahora prueba un endpoint protegido:
11. Busca **`GET /api/TipoGasto`**
12. Click en **"Try it out"**
13. Click en **"Execute"**

**Resultado esperado:**
- ✅ Código de respuesta: **200 OK**
- ✅ Lista de tipos de gasto (vacía o con datos, dependiendo de lo que haya creado ese usuario)

**Prueba sin token:**
14. Click en **"Authorize"** de nuevo
15. Click en **"Logout"**
16. Intenta ejecutar **`GET /api/TipoGasto`** de nuevo

**Resultado esperado:**
- ✅ Código de respuesta: **401 Unauthorized**

---

## ❌ Errores Comunes y Soluciones

### **Error: "Cannot GET /"**
**Causa:** El backend no está corriendo
**Solución:** Ejecuta `dotnet run` en `Backend/ControlGastos.API`

---

### **Error: CORS en consola del navegador**
**Mensaje:** `Access to XMLHttpRequest at 'http://localhost:5000/api/auth/login' from origin 'http://localhost:4200' has been blocked by CORS policy`

**Causa:** CORS no configurado correctamente
**Solución:**
1. Verifica que en `Backend/ControlGastos.API/Program.cs` esté:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```
2. Y que esté aplicado:
```csharp
app.UseCors("AllowAngular");
```

---

### **Error: "Cannot find module '@angular/...' "**
**Causa:** Dependencias no instaladas
**Solución:** Ejecuta `npm install` en `Frontend/control-gastos-app`

---

### **Error: "Cannot read property 'UsuarioId' of null"**
**Causa:** No estás autenticado pero intentas acceder a datos
**Solución:**
1. Cierra sesión
2. Vuelve a iniciar sesión
3. Si persiste, limpia localStorage (F12 → Application → Local Storage → Clear All)

---

### **Error: "Invalid column name 'UsuarioId'"**
**Causa:** No ejecutaste el script de migración de base de datos
**Solución:** Ejecuta `Database/03_AddAuthenticationTables.sql`

---

### **Error: "The constraint does not exist"**
**Causa:** Intentas ejecutar el script de migración dos veces
**Solución:**
- Es seguro, puedes ignorar este error
- El script tiene validaciones IF NOT EXISTS

---

## 📊 Verificación de la Base de Datos

Ejecuta estas queries para verificar que todo esté correcto:

```sql
USE ControlGastosDB;
GO

-- Verificar usuarios registrados
SELECT UsuarioId, NombreUsuario, Email, NombreCompleto, Activo, FechaCreacion
FROM Usuario;

-- Verificar que TipoGasto tiene UsuarioId
SELECT TOP 5 TipoGastoId, Codigo, Descripcion, UsuarioId
FROM TipoGasto;

-- Verificar que FondoMonetario tiene UsuarioId
SELECT TOP 5 FondoMonetarioId, Nombre, UsuarioId
FROM FondoMonetario;

-- Contar registros por usuario
SELECT
    u.NombreUsuario,
    COUNT(DISTINCT tg.TipoGastoId) as TiposGasto,
    COUNT(DISTINCT fm.FondoMonetarioId) as Fondos,
    COUNT(DISTINCT p.PresupuestoId) as Presupuestos,
    COUNT(DISTINCT rg.RegistroGastoId) as Gastos,
    COUNT(DISTINCT d.DepositoId) as Depositos
FROM Usuario u
LEFT JOIN TipoGasto tg ON u.UsuarioId = tg.UsuarioId
LEFT JOIN FondoMonetario fm ON u.UsuarioId = fm.UsuarioId
LEFT JOIN Presupuesto p ON u.UsuarioId = p.UsuarioId
LEFT JOIN RegistroGastoEncabezado rg ON u.UsuarioId = rg.UsuarioId
LEFT JOIN Deposito d ON u.UsuarioId = d.UsuarioId
GROUP BY u.UsuarioId, u.NombreUsuario;
```

---

## ✅ Checklist Final de Pruebas

Marca cada prueba cuando la completes:

- [ ] Base de datos creada y script de autenticación ejecutado
- [ ] Backend compila sin errores
- [ ] Backend corre correctamente en puerto 5000
- [ ] Swagger accesible en http://localhost:5000/swagger
- [ ] Frontend compila sin errores
- [ ] Frontend corre correctamente en puerto 4200
- [ ] Redirección automática a /login funciona
- [ ] Registro de usuario funciona
- [ ] Login funciona
- [ ] Navbar muestra nombre de usuario
- [ ] Botón de cerrar sesión funciona
- [ ] Crear tipos de gasto funciona
- [ ] Crear fondos monetarios funciona
- [ ] Multi-usuario: Usuarios no ven datos de otros
- [ ] AuthGuard protege rutas correctamente
- [ ] Token expira y redirige a login
- [ ] Interceptor agrega token automáticamente
- [ ] Swagger: Registro funciona
- [ ] Swagger: Login funciona
- [ ] Swagger: Endpoints protegidos requieren token

---

## 🎯 Prueba de Integración Completa

### **Escenario: Flujo Completo de Usuario**

1. **Registro:**
   - Usuario se registra con nombre "Carlos López"
   - Email: carlos@example.com
   - Usuario: carloslopez

2. **Crear datos:**
   - Crea tipo de gasto: "Transporte"
   - Crea fondo: "Billetera" con saldo 500
   - Crea presupuesto: "Transporte" para Diciembre 2024 con monto 200

3. **Registrar gasto:**
   - Ve a "Movimientos → Registro de Gastos"
   - Registra un gasto de transporte de 50

4. **Consultar reportes:**
   - Ve a "Consultas y Reportes → Consulta de Movimientos"
   - Verifica que aparezca el gasto de 50

5. **Cerrar sesión y verificar:**
   - Cierra sesión
   - Registra otro usuario: "Ana Martínez"
   - Verifica que Ana NO vea los datos de Carlos
   - Cierra sesión con Ana
   - Inicia sesión con Carlos
   - Verifica que Carlos SIGA viendo sus datos

**Si todo esto funciona: ✅ EL SISTEMA ESTÁ COMPLETAMENTE FUNCIONAL**

---

## 🆘 Soporte

Si encuentras algún problema:
1. Revisa la consola del navegador (F12)
2. Revisa la consola del backend
3. Verifica que ambos servidores estén corriendo
4. Revisa que el script de BD se haya ejecutado correctamente
5. Verifica que los puertos 5000 y 4200 estén libres

---

**¡FELICIDADES! Ahora tienes un sistema completo de autenticación multi-usuario funcionando.** 🎉
