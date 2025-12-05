# Guía de Contribución

¡Gracias por tu interés en contribuir al Sistema de Control de Gastos! 🎉

Este documento proporciona las directrices y mejores prácticas para contribuir al proyecto.

## 📋 Tabla de Contenidos

- [Código de Conducta](#código-de-conducta)
- [¿Cómo puedo contribuir?](#cómo-puedo-contribuir)
- [Configuración del Entorno de Desarrollo](#configuración-del-entorno-de-desarrollo)
- [Proceso de Pull Request](#proceso-de-pull-request)
- [Guía de Estilo](#guía-de-estilo)
- [Estructura de Commits](#estructura-de-commits)
- [Reportar Bugs](#reportar-bugs)
- [Solicitar Features](#solicitar-features)

## 📜 Código de Conducta

Este proyecto se adhiere a un Código de Conducta. Al participar, se espera que mantengas este código. Por favor reporta comportamientos inaceptables a [tu-email@example.com].

### Nuestro Compromiso

- Ser respetuoso con diferentes puntos de vista y experiencias
- Aceptar críticas constructivas con gracia
- Enfocarnos en lo que es mejor para la comunidad
- Mostrar empatía hacia otros miembros de la comunidad

## 🤝 ¿Cómo puedo contribuir?

### Reportar Bugs

Antes de crear un reporte de bug, por favor:

1. **Verifica** si el bug ya fue reportado en [Issues](https://github.com/tu-usuario/control-gastos/issues)
2. **Reproduce** el bug para asegurarte de que es consistente
3. **Recopila** información relevante (logs, screenshots, etc.)

#### Template para Reportar Bugs

```markdown
**Descripción del Bug**
Una descripción clara y concisa del bug.

**Pasos para Reproducir**
1. Ve a '...'
2. Haz click en '...'
3. Scroll hasta '...'
4. Observa el error

**Comportamiento Esperado**
Una descripción clara de lo que esperabas que sucediera.

**Comportamiento Actual**
Lo que está sucediendo actualmente.

**Screenshots**
Si es aplicable, agrega screenshots para ayudar a explicar el problema.

**Entorno:**
- OS: [e.g. Windows 10, macOS 12.0, Ubuntu 20.04]
- Navegador: [e.g. Chrome 96, Firefox 94]
- Versión del Backend: [e.g. 1.0.0]
- Versión del Frontend: [e.g. 1.0.0]

**Información Adicional**
Cualquier otra información relevante sobre el problema.
```

### Solicitar Features

Las solicitudes de nuevas características son bienvenidas. Antes de crear una solicitud:

1. **Verifica** que no exista una solicitud similar
2. **Considera** si la feature se alinea con el alcance del proyecto
3. **Proporciona** ejemplos de uso y casos de uso

#### Template para Solicitar Features

```markdown
**¿Tu solicitud de feature está relacionada con un problema?**
Una descripción clara del problema. Ej: Siempre me frustra cuando [...]

**Describe la solución que te gustaría**
Una descripción clara y concisa de lo que quieres que suceda.

**Describe alternativas que hayas considerado**
Una descripción clara de cualquier solución o característica alternativa.

**Contexto adicional**
Agrega cualquier otro contexto o screenshots sobre la solicitud aquí.
```

## 🛠️ Configuración del Entorno de Desarrollo

### Requisitos Previos

- .NET SDK 10.0+
- Node.js 18.x+
- PostgreSQL 15+ (o cuenta en Supabase)
- Angular CLI 17
- Git

### Configuración Inicial

1. **Fork** el repositorio

2. **Clona** tu fork:
   ```bash
   git clone https://github.com/tu-usuario/control-gastos.git
   cd control-gastos
   ```

3. **Agrega** el repositorio original como upstream:
   ```bash
   git remote add upstream https://github.com/usuario-original/control-gastos.git
   ```

4. **Configura** el backend:
   ```bash
   cd Backend/ControlGastos.API
   dotnet restore
   # Configura appsettings.json con tu DB local
   dotnet run
   ```

5. **Configura** el frontend:
   ```bash
   cd Frontend/control-gastos-app
   npm install
   npm start
   ```

### Mantener tu Fork Actualizado

```bash
git fetch upstream
git checkout main
git merge upstream/main
```

## 🔄 Proceso de Pull Request

### 1. Crea una Rama

```bash
git checkout -b feature/nombre-descriptivo
# o
git checkout -b fix/descripcion-del-bug
```

**Convención de nombres de ramas:**

- `feature/` - Para nuevas características
- `fix/` - Para correcciones de bugs
- `docs/` - Para cambios en documentación
- `refactor/` - Para refactorización de código
- `test/` - Para agregar o modificar tests
- `style/` - Para cambios de formato (sin cambios de código)

### 2. Realiza tus Cambios

- Escribe código limpio y legible
- Sigue las guías de estilo del proyecto
- Agrega tests para nuevas funcionalidades
- Actualiza la documentación si es necesario

### 3. Commit tus Cambios

Sigue la [Estructura de Commits](#estructura-de-commits):

```bash
git add .
git commit -m "feat: agregar validación de presupuesto en tiempo real"
```

### 4. Push a tu Fork

```bash
git push origin feature/nombre-descriptivo
```

### 5. Abre un Pull Request

1. Ve a tu fork en GitHub
2. Haz click en "Pull Request"
3. Asegúrate de que la base sea `main` del repositorio original
4. Completa la plantilla de PR

#### Template de Pull Request

```markdown
## Descripción

Breve descripción de los cambios realizados.

## Tipo de Cambio

- [ ] Bug fix (cambio que corrige un problema)
- [ ] Nueva feature (cambio que agrega funcionalidad)
- [ ] Breaking change (fix o feature que causaría que funcionalidad existente no funcione como se espera)
- [ ] Documentación
- [ ] Refactorización
- [ ] Tests

## ¿Cómo se ha probado?

Describe las pruebas que realizaste para verificar tus cambios.

- [ ] Test A
- [ ] Test B

## Checklist

- [ ] Mi código sigue las guías de estilo del proyecto
- [ ] He realizado una auto-revisión de mi código
- [ ] He comentado mi código, particularmente en áreas difíciles de entender
- [ ] He realizado cambios correspondientes en la documentación
- [ ] Mis cambios no generan nuevas advertencias
- [ ] He agregado tests que prueban que mi fix es efectivo o que mi feature funciona
- [ ] Tests unitarios nuevos y existentes pasan localmente con mis cambios
- [ ] Cualquier cambio dependiente ha sido fusionado y publicado en módulos downstream

## Screenshots (si aplica)

Agrega screenshots para ayudar a explicar tus cambios.
```

## 📝 Guía de Estilo

### Backend (.NET / C#)

#### Convenciones de Nomenclatura

```csharp
// Clases: PascalCase
public class RegistroGastoService { }

// Interfaces: I + PascalCase
public interface IRegistroGastoService { }

// Métodos: PascalCase
public void CalcularPresupuesto() { }

// Parámetros y variables locales: camelCase
public void Method(int usuarioId, string nombreCompleto) { }

// Constantes: PascalCase
public const int MaxIntentos = 3;

// Propiedades: PascalCase
public string NombreCompleto { get; set; }
```

#### Mejores Prácticas

- Usa `async/await` para operaciones I/O
- Implementa manejo de errores apropiado
- Usa inyección de dependencias
- Escribe código SOLID
- Documenta métodos públicos con XML comments

```csharp
/// <summary>
/// Calcula el presupuesto restante para un tipo de gasto
/// </summary>
/// <param name="tipoGastoId">ID del tipo de gasto</param>
/// <param name="mes">Mes a calcular</param>
/// <returns>Monto restante del presupuesto</returns>
public async Task<decimal> CalcularPresupuestoRestante(int tipoGastoId, int mes)
{
    // Implementación
}
```

### Frontend (Angular / TypeScript)

#### Convenciones de Nomenclatura

```typescript
// Clases y Interfaces: PascalCase
export class RegistroGasto { }
export interface Usuario { }

// Métodos y variables: camelCase
public calcularTotal(): number { }
private nombreCompleto: string;

// Constantes: UPPER_SNAKE_CASE
const API_BASE_URL = 'http://localhost:5000';

// Archivos: kebab-case
registro-gasto.service.ts
tipo-gasto.component.ts
```

#### Mejores Prácticas

- Usa reactive forms sobre template-driven forms
- Implementa OnDestroy y unsubscribe de observables
- Usa el operador `async` pipe cuando sea posible
- Tipea fuertemente con TypeScript
- Organiza imports (Angular, RxJS, third-party, local)

```typescript
// Orden de imports
import { Component, OnInit, OnDestroy } from '@angular/core'; // Angular
import { Observable, Subject } from 'rxjs'; // RxJS
import { takeUntil } from 'rxjs/operators';
import { ThirdPartyLib } from 'third-party'; // Third-party
import { MyService } from './my.service'; // Local
```

### SQL / Database

- Usa nombres descriptivos para tablas y columnas
- PascalCase para nombres de tablas
- Indexa columnas usadas frecuentemente en WHERE
- Documenta stored procedures y functions complejas

```sql
-- Buenos nombres
CREATE TABLE RegistroGasto (...)
CREATE INDEX idx_registrogasto_fecha ON RegistroGasto(Fecha);

-- Malos nombres
CREATE TABLE rg (...)
CREATE INDEX idx1 ON rg(f);
```

## 📦 Estructura de Commits

Seguimos [Conventional Commits](https://www.conventionalcommits.org/):

### Formato

```
<tipo>[ámbito opcional]: <descripción>

[cuerpo opcional]

[footer opcional]
```

### Tipos

- `feat`: Nueva característica
- `fix`: Corrección de bug
- `docs`: Cambios en documentación
- `style`: Formato, sin cambios de código
- `refactor`: Refactorización de código
- `test`: Agregar o modificar tests
- `chore`: Cambios en build o herramientas
- `perf`: Mejoras de performance

### Ejemplos

```bash
# Feature
git commit -m "feat(auth): agregar autenticación con Google OAuth"

# Bug fix
git commit -m "fix(presupuesto): corregir cálculo de saldo restante"

# Documentación
git commit -m "docs: actualizar README con instrucciones de instalación"

# Refactorización
git commit -m "refactor(services): simplificar lógica de validación"

# Breaking change
git commit -m "feat(api)!: cambiar formato de respuesta de endpoint de usuarios

BREAKING CHANGE: El endpoint /api/usuarios ahora devuelve un objeto paginado"
```

## 🧪 Testing

### Backend Tests

```bash
cd Backend/ControlGastos.API
dotnet test
```

**Cobertura mínima esperada:** 70%

### Frontend Tests

```bash
cd Frontend/control-gastos-app
npm test
```

### Escribir Buenos Tests

- Un test por comportamiento
- Nombres descriptivos
- Arrange-Act-Assert pattern
- Tests independientes
- Mock de dependencias externas

```csharp
[Fact]
public async Task CalcularPresupuesto_ConGastosExcedidos_DebeRetornarAlerta()
{
    // Arrange
    var gastos = new List<RegistroGasto> { /* ... */ };

    // Act
    var resultado = await _service.CalcularPresupuesto(gastos);

    // Assert
    Assert.True(resultado.TieneAlerta);
}
```

## 📚 Recursos Adicionales

- [Documentación de .NET](https://docs.microsoft.com/dotnet/)
- [Guía de Angular](https://angular.io/docs)
- [PostgreSQL Docs](https://www.postgresql.org/docs/)
- [Conventional Commits](https://www.conventionalcommits.org/)

## ❓ Preguntas

Si tienes preguntas, puedes:

1. Abrir un [issue](https://github.com/tu-usuario/control-gastos/issues)
2. Contactar a los mantenedores: [tu-email@example.com]
3. Revisar los [issues existentes](https://github.com/tu-usuario/control-gastos/issues)

## 🙏 Agradecimientos

¡Gracias por contribuir al proyecto! Cada contribución, grande o pequeña, es valiosa.

---

**Happy Coding!** 💻✨
