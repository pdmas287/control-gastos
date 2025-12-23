# Unidades de Anuncios - Google AdSense

## Tu código de publicador
**ca-pub-1445179381452360**

## Slots de anuncios configurados en la aplicación

A continuación se listan todos los slots de anuncios que debes crear en tu cuenta de Google AdSense. Los **números de slot son temporales** y debes reemplazarlos con los IDs reales que obtengas de AdSense.

### 1. Landing Page - Hero Banner
- **Ubicación**: `landing.component.html` (línea ~79)
- **Slot temporal**: `1234567890`
- **Formato recomendado**: Horizontal (Banner superior)
- **Nombre sugerido**: "Landing Hero Banner"
- **Descripción**: Anuncio horizontal después de la sección hero en la landing page

### 2. Home - Header Banner
- **Ubicación**: `home.component.ts` (línea ~17)
- **Slot temporal**: `6789012345`
- **Formato recomendado**: Horizontal (Banner)
- **Nombre sugerido**: "Home Header Banner"
- **Descripción**: Anuncio horizontal después del título de bienvenida

### 3. Home - Footer Rectangle
- **Ubicación**: `home.component.ts` (línea ~59)
- **Slot temporal**: `9876543210`
- **Formato recomendado**: Rectangle (Cuadrado/Rectángulo)
- **Nombre sugerido**: "Home Footer Rectangle"
- **Descripción**: Anuncio rectangular al final de la página de inicio

### 4. Gráfico Comparativo - Content Banner
- **Ubicación**: `grafico-comparativo.component.html` (línea ~45)
- **Slot temporal**: `3456789012`
- **Formato recomendado**: Horizontal (Banner)
- **Nombre sugerido**: "Report Content Banner"
- **Descripción**: Anuncio horizontal antes del resumen de datos

---

## Pasos para crear las unidades de anuncios en AdSense

### 1. Accede a tu cuenta de AdSense
- Ve a [https://www.google.com/adsense](https://www.google.com/adsense)
- Inicia sesión con tu cuenta

### 2. Crea cada unidad de anuncio

Para cada slot listado arriba, sigue estos pasos:

1. En el menú lateral, ve a **Anuncios** → **Por unidad de anuncio**
2. Haz clic en **Nueva unidad de anuncio**
3. Selecciona el tipo de anuncio:
   - **Anuncio de display** (recomendado para la mayoría)
   - Configura las opciones según el formato recomendado arriba
4. Dale un nombre descriptivo (usa los "Nombre sugerido" de arriba)
5. Configura el tamaño:
   - **Adaptable**: Se ajusta automáticamente (recomendado)
   - **Fijo**: Elige un tamaño específico si lo prefieres
6. Haz clic en **Crear**
7. **COPIA EL SLOT ID** que te proporciona Google (ejemplo: `1234567890`)

### 3. Reemplaza los slot IDs temporales

Una vez que hayas creado todas las unidades y tengas los slot IDs reales, debes reemplazarlos en los archivos:

#### Archivo: `src/app/components/landing/landing.component.html`
```html
<!-- Línea ~79 -->
<app-adsense
  adSlot="TU_SLOT_ID_REAL_AQUI"  <!-- Reemplazar 1234567890 -->
  adFormat="horizontal"
  fullWidthResponsive="true">
</app-adsense>
```

#### Archivo: `src/app/components/home/home.component.ts`
```typescript
// Línea ~17
<app-adsense
  adSlot="TU_SLOT_ID_REAL_AQUI"  <!-- Reemplazar 6789012345 -->
  adFormat="horizontal"
  fullWidthResponsive="true">
</app-adsense>

// Línea ~59
<app-adsense
  adSlot="TU_SLOT_ID_REAL_AQUI"  <!-- Reemplazar 9876543210 -->
  adFormat="rectangle"
  fullWidthResponsive="true">
</app-adsense>
```

#### Archivo: `src/app/components/reportes/grafico-comparativo/grafico-comparativo.component.html`
```html
<!-- Línea ~45 -->
<app-adsense
  *ngIf="consultado"
  adSlot="TU_SLOT_ID_REAL_AQUI"  <!-- Reemplazar 3456789012 -->
  adFormat="horizontal"
  fullWidthResponsive="true">
</app-adsense>
```

---

## Ejemplo de cómo se ve un slot ID real

Cuando crees la unidad de anuncio en AdSense, te darán un código similar a este:

```html
<ins class="adsbygoogle"
     style="display:block"
     data-ad-client="ca-pub-1445179381452360"
     data-ad-slot="1234567890"  <!-- Este es tu slot ID real -->
     data-ad-format="auto"></ins>
```

Solo necesitas copiar el número del `data-ad-slot` (ejemplo: `1234567890`).

---

## Formatos de anuncio recomendados

### Horizontal (Banner)
- Ideal para: Encabezados de sección, entre contenido
- Tamaños típicos: 728x90, 970x90, adaptable
- Usado en: Landing, Home Header, Gráfico Comparativo

### Rectangle (Rectángulo)
- Ideal para: Finales de página, sidebars
- Tamaños típicos: 300x250, 336x280, adaptable
- Usado en: Home Footer

### Auto (Adaptable)
- Se ajusta automáticamente al espacio disponible
- Recomendado para diseños responsive
- **Ya configurado** en todos los anuncios con `fullWidthResponsive="true"`

---

## Verificación antes del despliegue

Antes de desplegar a producción, verifica:

- [ ] Has creado las 4 unidades de anuncios en AdSense
- [ ] Has copiado los 4 slot IDs reales
- [ ] Has reemplazado los slots temporales en los archivos:
  - [ ] `landing.component.html`
  - [ ] `home.component.ts` (2 slots)
  - [ ] `grafico-comparativo.component.html`
- [ ] El código de publicador `ca-pub-1445179381452360` está correcto en:
  - [ ] `index.html`
  - [ ] `adsense.component.ts`

---

## Testing local

Durante el desarrollo local, los anuncios aparecerán como placeholders grises con el texto "Publicidad".

Los anuncios reales SOLO se mostrarán:
1. En un dominio de producción (no localhost)
2. Después de que Google apruebe tu sitio
3. Con slot IDs reales

---

## Políticas importantes

⚠️ **Lee las políticas de AdSense antes de solicitar revisión:**
- [Políticas del programa AdSense](https://support.google.com/adsense/answer/48182)
- No hagas clic en tus propios anuncios
- No solicites clics a otros
- Asegúrate de que tu contenido cumple con las políticas

---

## Monitoreo de rendimiento

Una vez aprobado, monitorea tus anuncios en:
- **Panel de AdSense**: Ingresos, impresiones, clics
- **Por unidad**: Rendimiento de cada slot individual
- **Optimización**: Experimenta con diferentes formatos y ubicaciones

---

## Soporte

Si tienes problemas:
1. Verifica que los slot IDs sean correctos
2. Revisa la consola del navegador para errores
3. Asegúrate de estar en producción (no localhost)
4. Contacta al soporte de Google AdSense

---

## Resumen de archivos modificados

✅ **Archivos con anuncios agregados:**
1. `src/index.html` - Script de AdSense
2. `src/app/components/shared/adsense.component.ts` - Componente reutilizable
3. `src/app/components/landing/landing.component.ts` - Import del componente
4. `src/app/components/landing/landing.component.html` - 1 anuncio
5. `src/app/components/landing/landing.component.css` - Estilos para ads
6. `src/app/components/home/home.component.ts` - 2 anuncios
7. `src/app/components/reportes/grafico-comparativo/grafico-comparativo.component.ts` - Import
8. `src/app/components/reportes/grafico-comparativo/grafico-comparativo.component.html` - 1 anuncio

**Total de anuncios**: 4 ubicaciones estratégicas
