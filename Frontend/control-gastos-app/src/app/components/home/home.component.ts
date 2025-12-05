import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="home-container">
      <div class="card">
        <h1>Bienvenido al Sistema de Control de Gastos</h1>
        <p class="subtitle">Gestiona tus finanzas personales de manera eficiente</p>

        <div class="features">
          <div class="feature-card">
            <h3>Mantenimientos</h3>
            <p>Administra tipos de gastos y fondos monetarios</p>
            <ul>
              <li>Tipos de Gasto</li>
              <li>Fondos Monetarios (Cuentas Bancarias y Caja Menuda)</li>
            </ul>
          </div>

          <div class="feature-card">
            <h3>Movimientos</h3>
            <p>Registra y controla tus transacciones financieras</p>
            <ul>
              <li>Presupuestos Mensuales</li>
              <li>Registro de Gastos con Detalle</li>
              <li>Depósitos</li>
            </ul>
          </div>

          <div class="feature-card">
            <h3>Consultas y Reportes</h3>
            <p>Analiza y visualiza tu información financiera</p>
            <ul>
              <li>Consulta de Movimientos por Fecha</li>
              <li>Gráfico Comparativo Presupuesto vs Ejecución</li>
            </ul>
          </div>
        </div>

        <div class="alert alert-info mt-3">
          <strong>Nota:</strong> Este sistema te permite llevar un control detallado de tus ingresos y egresos,
          con alertas automáticas cuando excedas tu presupuesto mensual.
        </div>
      </div>
    </div>
  `,
  styles: [`
    .home-container {
      max-width: 1200px;
      margin: 0 auto;
    }

    h1 {
      color: #2c3e50;
      margin-bottom: 10px;
    }

    .subtitle {
      color: #7f8c8d;
      font-size: 18px;
      margin-bottom: 30px;
    }

    .features {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 20px;
      margin-top: 30px;
    }

    .feature-card {
      background: #f8f9fa;
      padding: 20px;
      border-radius: 8px;
      border-left: 4px solid #3498db;
    }

    .feature-card h3 {
      color: #3498db;
      margin-bottom: 10px;
    }

    .feature-card p {
      color: #555;
      margin-bottom: 15px;
    }

    .feature-card ul {
      list-style: none;
      padding-left: 0;
    }

    .feature-card ul li {
      padding: 5px 0;
      color: #666;
    }

    .feature-card ul li:before {
      content: "✓ ";
      color: #27ae60;
      font-weight: bold;
      margin-right: 8px;
    }

    .alert-info {
      background-color: #d1ecf1;
      color: #0c5460;
      border: 1px solid #bee5eb;
    }
  `]
})
export class HomeComponent {
}
