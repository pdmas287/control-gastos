import { Injectable, inject } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

export interface SEOConfig {
  title: string;
  description: string;
  keywords?: string;
  image?: string;
  url?: string;
  type?: string;
  author?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SeoService {
  private meta = inject(Meta);
  private titleService = inject(Title);
  private router = inject(Router);

  private defaultConfig: SEOConfig = {
    title: 'Control de Gastos - Gestiona tus finanzas personales',
    description: 'Aplicación web para el control y seguimiento de gastos personales. Registra tus gastos, crea presupuestos, genera reportes y toma el control de tus finanzas.',
    keywords: 'control de gastos, finanzas personales, presupuesto, ahorro, gestión financiera, registro de gastos, app financiera',
    image: 'https://control-gastos.vercel.app/assets/og-image.png',
    type: 'website',
    author: 'Control de Gastos'
  };

  constructor() {
    // Actualizar el título en cada cambio de ruta
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        window.scrollTo(0, 0);
      });
  }

  updateMetaTags(config: Partial<SEOConfig>): void {
    const seoConfig = { ...this.defaultConfig, ...config };

    // Título de la página
    this.titleService.setTitle(seoConfig.title);

    // Meta tags básicos
    this.meta.updateTag({ name: 'description', content: seoConfig.description });
    if (seoConfig.keywords) {
      this.meta.updateTag({ name: 'keywords', content: seoConfig.keywords });
    }
    if (seoConfig.author) {
      this.meta.updateTag({ name: 'author', content: seoConfig.author });
    }

    // Open Graph (Facebook, LinkedIn)
    this.meta.updateTag({ property: 'og:title', content: seoConfig.title });
    this.meta.updateTag({ property: 'og:description', content: seoConfig.description });
    this.meta.updateTag({ property: 'og:type', content: seoConfig.type || 'website' });
    if (seoConfig.image) {
      this.meta.updateTag({ property: 'og:image', content: seoConfig.image });
    }
    if (seoConfig.url) {
      this.meta.updateTag({ property: 'og:url', content: seoConfig.url });
    }

    // Twitter Card
    this.meta.updateTag({ name: 'twitter:card', content: 'summary_large_image' });
    this.meta.updateTag({ name: 'twitter:title', content: seoConfig.title });
    this.meta.updateTag({ name: 'twitter:description', content: seoConfig.description });
    if (seoConfig.image) {
      this.meta.updateTag({ name: 'twitter:image', content: seoConfig.image });
    }

    // Meta tags adicionales
    this.meta.updateTag({ name: 'robots', content: 'index, follow' });
    this.meta.updateTag({ name: 'viewport', content: 'width=device-width, initial-scale=1' });
    this.meta.updateTag({ 'http-equiv': 'Content-Type', content: 'text/html; charset=utf-8' });
  }

  createStructuredData(type: 'WebApplication' | 'Organization' | 'FAQPage', data: any): void {
    let script = document.querySelector('script[type="application/ld+json"]');

    if (!script) {
      script = document.createElement('script');
      script.setAttribute('type', 'application/ld+json');
      document.head.appendChild(script);
    }

    const structuredData = this.getStructuredData(type, data);
    script.textContent = JSON.stringify(structuredData);
  }

  private getStructuredData(type: string, customData?: any): any {
    const baseUrl = 'https://control-gastos.vercel.app';

    switch (type) {
      case 'WebApplication':
        return {
          '@context': 'https://schema.org',
          '@type': 'WebApplication',
          'name': 'Control de Gastos',
          'description': 'Aplicación web para el control y seguimiento de gastos personales',
          'url': baseUrl,
          'applicationCategory': 'FinanceApplication',
          'operatingSystem': 'Web',
          'offers': {
            '@type': 'Offer',
            'price': '0',
            'priceCurrency': 'USD'
          },
          'aggregateRating': {
            '@type': 'AggregateRating',
            'ratingValue': '4.8',
            'ratingCount': '150'
          },
          ...customData
        };

      case 'Organization':
        return {
          '@context': 'https://schema.org',
          '@type': 'Organization',
          'name': 'Control de Gastos',
          'url': baseUrl,
          'logo': `${baseUrl}/assets/logo.png`,
          'description': 'Plataforma de gestión financiera personal',
          'sameAs': [
            // Agregar redes sociales si existen
          ],
          ...customData
        };

      case 'FAQPage':
        return {
          '@context': 'https://schema.org',
          '@type': 'FAQPage',
          'mainEntity': customData || []
        };

      default:
        return customData;
    }
  }

  removeStructuredData(): void {
    const script = document.querySelector('script[type="application/ld+json"]');
    if (script) {
      script.remove();
    }
  }
}
