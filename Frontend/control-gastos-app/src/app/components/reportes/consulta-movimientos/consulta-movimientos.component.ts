import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReporteService } from '../../../services/reporte.service';
import { Movimiento } from '../../../models/reporte.model';

@Component({
  selector: 'app-consulta-movimientos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './consulta-movimientos.component.html',
  styleUrls: ['./consulta-movimientos.component.css']
})
export class ConsultaMovimientosComponent implements OnInit {
  fechaInicio: string = '';
  fechaFin: string = '';
  movimientos: Movimiento[] = [];
  movimientosFiltrados: Movimiento[] = [];
  cargando: boolean = false;
  consultado: boolean = false;

  // Filtros
  filtroTipo: string = 'Todos';
  tiposMovimiento: string[] = ['Todos', 'Gasto', 'Depósito'];

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

  consultarMovimientos(): void {
    if (!this.fechaInicio || !this.fechaFin) {
      alert('Debe seleccionar ambas fechas');
      return;
    }

    if (new Date(this.fechaFin) < new Date(this.fechaInicio)) {
      alert('La fecha fin debe ser mayor o igual a la fecha inicio');
      return;
    }

    this.cargando = true;

    this.reporteService.getMovimientos(this.fechaInicio, this.fechaFin).subscribe({
      next: (data) => {
        this.movimientos = data;
        this.aplicarFiltro();
        this.consultado = true;
        this.cargando = false;
      },
      error: (error) => {
        console.error('Error al consultar movimientos:', error);
        alert('Error al consultar movimientos');
        this.cargando = false;
      }
    });
  }

  aplicarFiltro(): void {
    if (this.filtroTipo === 'Todos') {
      this.movimientosFiltrados = this.movimientos;
    } else {
      this.movimientosFiltrados = this.movimientos.filter(
        m => m.tipoMovimiento === this.filtroTipo
      );
    }
  }

  onFiltroChange(): void {
    this.aplicarFiltro();
  }

  calcularTotalGastos(): number {
    return this.movimientosFiltrados
      .filter(m => m.tipoMovimiento === 'Gasto')
      .reduce((sum, m) => sum + m.monto, 0);
  }

  calcularTotalDepositos(): number {
    return this.movimientosFiltrados
      .filter(m => m.tipoMovimiento === 'Depósito')
      .reduce((sum, m) => sum + m.monto, 0);
  }

  calcularBalance(): number {
    return this.calcularTotalDepositos() - this.calcularTotalGastos();
  }

  limpiarConsulta(): void {
    const hoy = new Date();
    const primerDia = new Date(hoy.getFullYear(), hoy.getMonth(), 1);

    this.fechaInicio = this.formatDate(primerDia);
    this.fechaFin = this.formatDate(hoy);
    this.movimientos = [];
    this.movimientosFiltrados = [];
    this.consultado = false;
    this.filtroTipo = 'Todos';
  }

  exportarCSV(): void {
    if (this.movimientosFiltrados.length === 0) {
      alert('No hay datos para exportar');
      return;
    }

    let csv = 'Fecha,Tipo,Fondo Monetario,Descripción,Monto,Documento,Observaciones\n';

    this.movimientosFiltrados.forEach(m => {
      const fecha = new Date(m.fecha).toLocaleDateString();
      const monto = m.monto.toFixed(2);
      const documento = m.tipoDocumento || '';
      const observaciones = m.observaciones || '';

      csv += `${fecha},${m.tipoMovimiento},${m.fondoMonetario},"${m.descripcion}",${monto},${documento},"${observaciones}"\n`;
    });

    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);

    link.setAttribute('href', url);
    link.setAttribute('download', `movimientos_${this.fechaInicio}_${this.fechaFin}.csv`);
    link.style.visibility = 'hidden';

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}
