import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { SeoService } from '../../services/seo.service';
import { AdsenseComponent } from '../shared/adsense.component';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink, AdsenseComponent],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit {
  private seoService = inject(SeoService);
  private router = inject(Router);

  currentYear = new Date().getFullYear();

  features = [
    {
      icon: '📊',
      title: 'Registro de Gastos',
      description: 'Registra todos tus gastos de forma rápida y organizada. Categoriza cada transacción para un mejor control.'
    },
    {
      icon: '💰',
      title: 'Presupuestos Inteligentes',
      description: 'Crea presupuestos mensuales por categoría y recibe alertas cuando te acerques a tu límite.'
    },
    {
      icon: '📈',
      title: 'Reportes Detallados',
      description: 'Visualiza tus patrones de gasto con gráficos interactivos y reportes personalizados.'
    },
    {
      icon: '🏦',
      title: 'Múltiples Fondos',
      description: 'Gestiona diferentes cuentas bancarias, tarjetas y efectivo en un solo lugar.'
    },
    {
      icon: '📅',
      title: 'Consulta Histórica',
      description: 'Accede a todo tu historial de movimientos y analiza tu evolución financiera.'
    },
    {
      icon: '🔒',
      title: 'Seguro y Privado',
      description: 'Tus datos están protegidos con encriptación y solo tú tienes acceso a ellos.'
    }
  ];

  faqs = [
    {
      question: '¿Es gratis usar la aplicación?',
      answer: 'Sí, Control de Gastos es completamente gratuita. Puedes registrarte y empezar a gestionar tus finanzas sin ningún costo.',
      isOpen: false
    },
    {
      question: '¿Mis datos están seguros?',
      answer: 'Absolutamente. Utilizamos encriptación de nivel bancario para proteger tu información. Solo tú tienes acceso a tus datos financieros.',
      isOpen: false
    },
    {
      question: '¿Puedo usar la app en mi teléfono?',
      answer: 'Sí, la aplicación es totalmente responsive y funciona perfectamente en dispositivos móviles, tablets y computadoras.',
      isOpen: false
    },
    {
      question: '¿Necesito conocimientos técnicos?',
      answer: 'No, la interfaz es intuitiva y fácil de usar. Cualquier persona puede empezar a registrar sus gastos en minutos.',
      isOpen: false
    },
    {
      question: '¿Puedo exportar mis datos?',
      answer: 'Sí, puedes generar reportes y exportar tu información financiera en cualquier momento.',
      isOpen: false
    }
  ];

  ngOnInit(): void {
    // Configurar SEO
    this.seoService.updateMetaTags({
      title: 'Control de Gastos - Gestiona tus Finanzas Personales Gratis',
      description: 'Controla tus gastos, crea presupuestos inteligentes y genera reportes detallados. Aplicación web gratuita para la gestión de finanzas personales. Registra tus gastos, ahorra dinero y toma el control de tu economía.',
      keywords: 'control de gastos, finanzas personales, presupuesto, ahorro, gestión financiera, app gastos, registro gastos, control económico, finanzas gratis',
      url: 'https://control-gastos.vercel.app'
    });

    // Configurar Structured Data para WebApplication
    this.seoService.createStructuredData('WebApplication', {
      'screenshot': 'https://control-gastos.vercel.app/assets/screenshot.png',
      'browserRequirements': 'Requires JavaScript. Requires HTML5.',
      'softwareVersion': '1.0',
      'author': {
        '@type': 'Organization',
        'name': 'Control de Gastos'
      }
    });

    // Agregar FAQ Structured Data
    this.addFAQStructuredData();
  }

  private addFAQStructuredData(): void {
    const faqSchema = this.faqs.map(faq => ({
      '@type': 'Question',
      'name': faq.question,
      'acceptedAnswer': {
        '@type': 'Answer',
        'text': faq.answer
      }
    }));

    // Crear un script adicional para FAQ
    const script = document.createElement('script');
    script.setAttribute('type', 'application/ld+json');
    script.textContent = JSON.stringify({
      '@context': 'https://schema.org',
      '@type': 'FAQPage',
      'mainEntity': faqSchema
    });
    document.head.appendChild(script);
  }

  toggleFAQ(index: number): void {
    this.faqs[index].isOpen = !this.faqs[index].isOpen;
  }

  scrollToSection(sectionId: string): void {
    const element = document.getElementById(sectionId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }

  goToRegister(): void {
    this.router.navigate(['/registro']);
  }
}
