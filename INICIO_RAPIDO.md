# 🚀 Inicio Rápido - Control de Gastos con Autenticación

## ⚡ 3 Pasos para Empezar

### **1️⃣ Base de Datos (2 minutos)**

Abre **SQL Server Management Studio** y ejecuta:

```sql
-- Ejecutar estos archivos EN ORDEN:
-- 1. Database/01_CreateDatabase.sql
-- 2. Database/02_StoredProcedures.sql
-- 3. Database/03_AddAuthenticationTables.sql
-- 4. Database/04_CreateAdminUser.sql (OPCIONAL - Crear usuario admin)
```

✅ **Listo!** La base de datos está configurada.

**🔑 Usuario Administrador (OPCIONAL):**
- Si ejecutaste el script `04_CreateAdminUser.sql`:
  - **Usuario:** admin
  - **Contraseña:** Admin123!
- Si no, simplemente regístrate desde la aplicación

---

### **2️⃣ Backend (1 minuto)**

```bash
cd Backend/ControlGastos.API
dotnet run
```

✅ **Listo!** API corriendo en `http://localhost:5000`

---

### **3️⃣ Frontend (1 minuto)**

```bash
cd Frontend/control-gastos-app
ng serve
```

✅ **Listo!** App corriendo en `http://localhost:4200`

---

## 🎯 Usar la Aplicación

1. Abre tu navegador: `http://localhost:4200`
2. **Regístrate** como nuevo usuario
3. **¡Empieza a usar el sistema!**

---

## 📱 Pantallas Principales

### **Login** (`http://localhost:4200/login`)
- Pantalla inicial
- Permite iniciar sesión o ir a registro

### **Registro** (`http://localhost:4200/registro`)
- Crear nueva cuenta de usuario
- Automáticamente inicia sesión después del registro

### **Home** (`http://localhost:4200/home`)
- Dashboard principal (protegido)
- Requiere autenticación

### **Mantenimientos**
- **Tipos de Gasto:** Categorías de gastos
- **Fondos Monetarios:** Cuentas y cajas

### **Movimientos**
- **Presupuestos:** Definir límites mensuales
- **Registro de Gastos:** Registrar gastos
- **Depósitos:** Registrar ingresos

### **Consultas y Reportes**
- **Consulta de Movimientos:** Ver historial
- **Gráfico Comparativo:** Análisis visual

---

## 🔐 Características de Seguridad

- ✅ Cada usuario solo ve **SUS PROPIOS datos**
- ✅ Las rutas están **protegidas con autenticación**
- ✅ Los tokens JWT expiran en **7 días**
- ✅ Si el token expira, **redirección automática al login**

---

## 🧪 Probar Multi-Usuario

1. Registra usuario 1: "Juan" (juan@example.com)
2. Crea algunos tipos de gasto
3. **Cierra sesión**
4. Registra usuario 2: "María" (maria@example.com)
5. ✅ María NO verá los datos de Juan
6. Crea datos para María
7. **Cierra sesión**
8. Inicia sesión con Juan
9. ✅ Juan NO verá los datos de María

---

## ⚙️ Endpoints de la API

### **Autenticación (Públicos)**
- `POST /api/auth/registro` - Registrar usuario
- `POST /api/auth/login` - Iniciar sesión

### **Datos (Protegidos - Requieren Token)**
- `GET /api/TipoGasto` - Obtener tipos de gasto del usuario
- `GET /api/FondoMonetario` - Obtener fondos del usuario
- `GET /api/Presupuesto` - Obtener presupuestos del usuario
- `GET /api/RegistroGasto` - Obtener gastos del usuario
- `GET /api/Deposito` - Obtener depósitos del usuario
- `GET /api/Reporte` - Obtener reportes del usuario

**Todos los endpoints filtran automáticamente por el usuario autenticado.**

---

## 🛠️ Swagger (Pruebas de API)

1. Backend corriendo: `dotnet run`
2. Abre: `http://localhost:5000/swagger`
3. Prueba endpoints de autenticación
4. Usa el botón **"Authorize"** con el token recibido
5. Prueba endpoints protegidos

---

## 📂 Estructura de Archivos Importantes

```
control_gasto/
├── INICIO_RAPIDO.md ← ¡Estás aquí!
├── IMPLEMENTACION_COMPLETA.md ← Documentación detallada
├── PRUEBAS_SISTEMA.md ← Guía de pruebas completas
├── AUTENTICACION_GUIA.md ← Guía técnica
│
├── Database/
│   ├── 01_CreateDatabase.sql
│   ├── 02_StoredProcedures.sql
│   └── 03_AddAuthenticationTables.sql ✨ NUEVO
│
├── Backend/ControlGastos.API/
│   ├── Controllers/AuthController.cs ✨ NUEVO
│   ├── Services/AuthService.cs ✨ NUEVO
│   ├── Models/Usuario.cs ✨ NUEVO
│   └── ... (todos modificados)
│
└── Frontend/control-gastos-app/
    └── src/app/
        ├── components/auth/ ✨ NUEVO
        ├── services/auth.service.ts ✨ NUEVO
        ├── guards/auth.guard.ts ✨ NUEVO
        └── interceptors/auth.interceptor.ts ✨ NUEVO
```

---

## ❓ Preguntas Frecuentes

### **¿Puedo cambiar el puerto del backend?**
Sí, en `Backend/ControlGastos.API/Properties/launchSettings.json`

También actualiza la URL en todos los servicios de Angular:
```typescript
// En cada servicio de Angular
private apiUrl = 'http://localhost:TU_PUERTO/api/...';
```

### **¿Cómo cambio el tiempo de expiración del token?**
En `Backend/ControlGastos.API/Services/AuthService.cs` línea 195:
```csharp
expires: DateTime.Now.AddDays(7), // Cambiar aquí
```

### **¿Cómo agrego más validaciones al registro?**
Modifica `Backend/ControlGastos.API/DTOs/AuthDto.cs`:
```csharp
[StringLength(100, MinimumLength = 8, ErrorMessage = "...")]
public string Password { get; set; }
```

### **¿Los datos existentes se perderán?**
No, si ejecutas el script correctamente. Solo asegúrate de asignar un `UsuarioId` a los datos existentes.

### **¿Puedo usar otro método de hash para contraseñas?**
Sí, reemplaza SHA256 por BCrypt en `AuthService.cs` para mayor seguridad.

---

## 🆘 Problemas Comunes

| Problema | Solución |
|----------|----------|
| Backend no compila | `dotnet clean` y luego `dotnet build` |
| Frontend no compila | `rm -rf node_modules` y luego `npm install` |
| Error CORS | Verifica `Program.cs` tenga CORS configurado |
| Error 401 en todos los endpoints | Verifica que el token se esté enviando |
| No redirige al login | Limpia localStorage (F12 → Application) |
| Tabla Usuario no existe | Ejecuta `03_AddAuthenticationTables.sql` |

---

## 📞 Ayuda Adicional

- **Guía Completa:** [IMPLEMENTACION_COMPLETA.md](IMPLEMENTACION_COMPLETA.md)
- **Guía de Pruebas:** [PRUEBAS_SISTEMA.md](PRUEBAS_SISTEMA.md)
- **Guía Técnica:** [AUTENTICACION_GUIA.md](AUTENTICACION_GUIA.md)

---

## ✅ Checklist Rápido

Antes de empezar a usar, verifica:

- [ ] SQL Server está corriendo
- [ ] Base de datos `ControlGastosDB` existe
- [ ] Script `03_AddAuthenticationTables.sql` ejecutado
- [ ] Backend compila sin errores
- [ ] Backend corre en puerto 5000
- [ ] Frontend compila sin errores
- [ ] Frontend corre en puerto 4200
- [ ] Puedes acceder a `http://localhost:4200`
- [ ] Eres redirigido automáticamente a `/login`

Si todos tienen ✅: **¡Estás listo para usar el sistema!** 🎉

---

**Creado con:** .NET 10 + Angular 17 + SQL Server + JWT
