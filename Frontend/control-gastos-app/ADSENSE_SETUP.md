# Configuración de Google AdSense

## Pasos completados

✅ **1. Componente AdSense creado**
   - Ubicación: `src/app/components/shared/adsense.component.ts`
   - Es un componente standalone reutilizable

✅ **2. Script de AdSense agregado**
   - Ubicación: `src/index.html`
   - Script cargado de forma asíncrona

## Pasos pendientes (debes completar)

### 1. Obtener tu código de AdSense

1. Ve a [Google AdSense](https://www.google.com/adsense)
2. Crea una cuenta o inicia sesión
3. Obtén tu **Código de publicador** (formato: `ca-pub-XXXXXXXXXXXXXXXX`)
4. Crea **unidades de anuncios** y obtén los **slots** (IDs únicos para cada anuncio)

### 2. Reemplazar los placeholders

#### En `src/index.html` (línea 33):
```html
<!-- ANTES -->
<script async src="https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js?client=ca-pub-XXXXXXXXXXXXXXXX"

<!-- DESPUÉS (reemplaza con tu código real) -->
<script async src="https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js?client=ca-pub-1234567890123456"
```

#### En `src/app/components/shared/adsense.component.ts` (línea 35):
```typescript
// ANTES
@Input() adClient: string = 'ca-pub-XXXXXXXXXXXXXXXX';

// DESPUÉS (reemplaza con tu código real)
@Input() adClient: string = 'ca-pub-1234567890123456';
```

### 3. Usar el componente en tus vistas

#### Ejemplo 1: Anuncio en el componente Home

Edita `src/app/components/home/home.component.ts`:

```typescript
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdsenseComponent } from '../shared/adsense.component'; // Importar

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, AdsenseComponent], // Agregar a imports
  template: `
    <div class="home-container">
      <div class="card">
        <h1>Bienvenido al Sistema de Control de Gastos</h1>
        <p class="subtitle">Gestiona tus finanzas personales de manera eficiente</p>

        <!-- Anuncio aquí -->
        <app-adsense
          adSlot="1234567890"
          adFormat="horizontal">
        </app-adsense>

        <div class="features">
          <!-- ... resto del contenido ... -->
        </div>
      </div>
    </div>
  `,
  // ... resto del componente
})
```

#### Ejemplo 2: Anuncio en reportes

```typescript
<div class="card">
  <h2>Gráfico Comparativo</h2>

  <!-- Anuncio antes del gráfico -->
  <app-adsense
    adSlot="9876543210"
    adFormat="rectangle">
  </app-adsense>

  <!-- Contenido del gráfico -->
</div>
```

#### Ejemplo 3: Anuncio responsive

```typescript
<app-adsense
  adSlot="1111111111"
  adFormat="auto"
  fullWidthResponsive="true">
</app-adsense>
```

### 4. Ubicaciones estratégicas recomendadas

**Mejores lugares para anuncios:**

1. **Landing Page** (`landing.component.ts`)
   - Entre la sección hero y las características
   - Al final de la página

2. **Home/Dashboard** (`home.component.ts`)
   - Después del mensaje de bienvenida
   - Entre las tarjetas de características

3. **Reportes** (`grafico-comparativo.component.ts`, `consulta-movimientos.component.ts`)
   - Antes del gráfico
   - Después de la tabla de datos

4. **Sidebar** (si implementas uno)
   - Anuncio vertical en la barra lateral

**IMPORTANTE: No abuses de los anuncios**
- Máximo 3 anuncios por página
- Espacialos bien para no saturar al usuario
- Mantén buena experiencia de usuario

### 5. Tipos de formato de anuncios

```typescript
adFormat="auto"        // Se adapta automáticamente
adFormat="horizontal"  // Anuncio horizontal (banner)
adFormat="vertical"    // Anuncio vertical (skyscraper)
adFormat="rectangle"   // Rectángulo (cuadrado)
```

### 6. Verificación y aprobación

1. **Implementa los anuncios** en tu aplicación
2. **Despliega a producción** (Vercel)
3. **Solicita revisión** en tu cuenta de AdSense
4. **Espera aprobación** (puede tomar días o semanas)
5. **Una vez aprobado**, los anuncios reales comenzarán a mostrarse

### 7. Consideraciones importantes

⚠️ **Políticas de AdSense:**
- No hagas clic en tus propios anuncios
- No pidas a otros que hagan clic
- Cumple con las políticas de contenido de Google
- No modifiques el código de AdSense

⚠️ **Rendimiento:**
- Los anuncios pueden afectar la velocidad de carga
- Usa `loading="lazy"` si es posible
- Monitorea el rendimiento de tu sitio

⚠️ **Experiencia de usuario:**
- No coloques anuncios que interfieran con la navegación
- Deja espacio suficiente entre anuncios y contenido
- Los anuncios deben ser claramente distinguibles del contenido

### 8. Testing

Durante el desarrollo, verás un placeholder gris que dice "Publicidad". Los anuncios reales solo se mostrarán:
- En producción (dominio real)
- Después de la aprobación de AdSense
- Con códigos de publicador reales

### 9. Monitoreo

Accede a tu cuenta de AdSense para ver:
- Impresiones de anuncios
- Clics
- Ingresos estimados
- CTR (Click-Through Rate)
- RPM (Revenue Per Mille)

## Ejemplo completo de implementación

Archivo: `src/app/components/home/home.component.ts`

```typescript
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdsenseComponent } from '../shared/adsense.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, AdsenseComponent],
  template: `
    <div class="home-container">
      <div class="card">
        <h1>Bienvenido al Sistema de Control de Gastos</h1>
        <p class="subtitle">Gestiona tus finanzas personales de manera eficiente</p>

        <!-- Anuncio horizontal después del título -->
        <app-adsense
          adClient="ca-pub-1234567890123456"
          adSlot="1234567890"
          adFormat="horizontal"
          fullWidthResponsive="true">
        </app-adsense>

        <div class="features">
          <div class="feature-card">
            <h3>Mantenimientos</h3>
            <p>Administra tipos de gastos y fondos monetarios</p>
          </div>

          <div class="feature-card">
            <h3>Movimientos</h3>
            <p>Registra y controla tus transacciones financieras</p>
          </div>

          <div class="feature-card">
            <h3>Consultas y Reportes</h3>
            <p>Analiza y visualiza tu información financiera</p>
          </div>
        </div>

        <!-- Anuncio al final de la página -->
        <app-adsense
          adClient="ca-pub-1234567890123456"
          adSlot="9876543210"
          adFormat="rectangle"
          fullWidthResponsive="true">
        </app-adsense>
      </div>
    </div>
  `,
  styles: [/* ... tus estilos ... */]
})
export class HomeComponent {}
```

## Soporte

Si tienes problemas:
1. Revisa la [documentación oficial de AdSense](https://support.google.com/adsense)
2. Verifica la consola del navegador para errores
3. Asegúrate de que tu sitio cumpla con las políticas de AdSense
4. Contacta al soporte de Google AdSense

## Checklist final

- [ ] Obtener código de publicador de AdSense
- [ ] Reemplazar `ca-pub-XXXXXXXXXXXXXXXX` en `index.html`
- [ ] Reemplazar `ca-pub-XXXXXXXXXXXXXXXX` en `adsense.component.ts`
- [ ] Crear unidades de anuncios en AdSense
- [ ] Obtener slot IDs para cada anuncio
- [ ] Importar `AdsenseComponent` en los componentes donde lo uses
- [ ] Agregar `<app-adsense>` en las ubicaciones deseadas
- [ ] Desplegar a producción
- [ ] Solicitar revisión en AdSense
- [ ] Esperar aprobación
- [ ] Monitorear rendimiento
