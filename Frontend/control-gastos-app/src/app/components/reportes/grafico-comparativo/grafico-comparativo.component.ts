import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReporteService } from '../../../services/reporte.service';
import { ComparativoPresupuesto } from '../../../models/reporte.model';

@Component({
  selector: 'app-grafico-comparativo',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './grafico-comparativo.component.html',
  styleUrls: ['./grafico-comparativo.component.css']
})
export class GraficoComparativoComponent implements OnInit {
  fechaInicio: string = '';
  fechaFin: string = '';
  datos: ComparativoPresupuesto[] = [];
  cargando: boolean = false;
  consultado: boolean = false;

  // Configuración del gráfico (simulación básica sin Chart.js para evitar dependencias)
  mostrarGrafico: boolean = false;

  constructor(private reporteService: ReporteService) { }

  ngOnInit(): void {
    // Configurar fechas por defecto: primer día del mes actual hasta hoy
    const hoy = new Date();
    const primerDia = new Date(hoy.getFullYear(), hoy.getMonth(), 1);

    this.fechaInicio = this.formatDate(primerDia);
    this.fechaFin = this.formatDate(hoy);
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  generarGrafico(): void {
    if (!this.fechaInicio || !this.fechaFin) {
      alert('Debe seleccionar ambas fechas');
      return;
    }

    if (new Date(this.fechaFin) < new Date(this.fechaInicio)) {
      alert('La fecha fin debe ser mayor o igual a la fecha inicio');
      return;
    }

    this.cargando = true;

    this.reporteService.getComparativoPresupuesto(this.fechaInicio, this.fechaFin).subscribe({
      next: (data) => {
        this.datos = data;
        this.consultado = true;
        this.mostrarGrafico = true;
        this.cargando = false;
      },
      error: (error) => {
        console.error('Error al generar gráfico:', error);
        alert('Error al generar gráfico comparativo');
        this.cargando = false;
      }
    });
  }

  limpiarGrafico(): void {
    const hoy = new Date();
    const primerDia = new Date(hoy.getFullYear(), hoy.getMonth(), 1);

    this.fechaInicio = this.formatDate(primerDia);
    this.fechaFin = this.formatDate(hoy);
    this.datos = [];
    this.consultado = false;
    this.mostrarGrafico = false;
  }

  calcularTotalPresupuestado(): number {
    return this.datos.reduce((sum, d) => sum + d.montoPresupuestado, 0);
  }

  calcularTotalEjecutado(): number {
    return this.datos.reduce((sum, d) => sum + d.montoEjecutado, 0);
  }

  calcularDiferenciaTotal(): number {
    return this.calcularTotalPresupuestado() - this.calcularTotalEjecutado();
  }

  getPorcentajeEjecucionPromedio(): number {
    if (this.datos.length === 0) return 0;
    const suma = this.datos.reduce((sum, d) => sum + d.porcentajeEjecucion, 0);
    return suma / this.datos.length;
  }

  getBarWidth(monto: number, maxMonto: number): number {
    if (maxMonto === 0) return 0;
    return (monto / maxMonto) * 100;
  }

  getMaxMonto(): number {
    let max = 0;
    this.datos.forEach(d => {
      if (d.montoPresupuestado > max) max = d.montoPresupuestado;
      if (d.montoEjecutado > max) max = d.montoEjecutado;
    });
    return max;
  }

  getColorPorcentaje(porcentaje: number): string {
    if (porcentaje <= 70) return '#27ae60'; // Verde - Bien
    if (porcentaje <= 90) return '#f39c12'; // Naranja - Cuidado
    if (porcentaje <= 100) return '#e67e22'; // Naranja oscuro - Alerta
    return '#e74c3c'; // Rojo - Sobregiro
  }

  exportarPDF(): void {
    alert('Función de exportar a PDF disponible con librerías adicionales como jsPDF.\n\nPor ahora, puede usar la función de impresión del navegador (Ctrl+P) para guardar como PDF.');
    window.print();
  }
}
