import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-reportes-globales',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reportes-globales.component.html',
  styleUrls: ['./reportes-globales.component.css']
})
export class ReportesGlobalesComponent implements OnInit {
  fechaInicio: string = '';
  fechaFin: string = '';
  cargando: boolean = false;

  // Datos de ejemplo - Aquí conectarías con tu servicio de reportes
  resumen = {
    totalGastos: 0,
    totalDepositos: 0,
    balance: 0,
    cantidadTransacciones: 0
  };

  constructor(public authService: AuthService) {}

  ngOnInit(): void {
    // Establecer fechas por defecto (mes actual)
    const hoy = new Date();
    const primerDia = new Date(hoy.getFullYear(), hoy.getMonth(), 1);

    this.fechaInicio = this.formatearFecha(primerDia);
    this.fechaFin = this.formatearFecha(hoy);

    this.cargarReportes();
  }

  formatearFecha(fecha: Date): string {
    return fecha.toISOString().split('T')[0];
  }

  cargarReportes(): void {
    this.cargando = true;

    // TODO: Implementar llamada al servicio de reportes
    // this.reporteService.getReportesGlobales(this.fechaInicio, this.fechaFin).subscribe({
    //   next: (data) => {
    //     this.resumen = data;
    //     this.cargando = false;
    //   },
    //   error: (err) => {
    //     console.error('Error:', err);
    //     this.cargando = false;
    //   }
    // });

    // Simulación de carga
    setTimeout(() => {
      this.resumen = {
        totalGastos: 15000,
        totalDepositos: 20000,
        balance: 5000,
        cantidadTransacciones: 45
      };
      this.cargando = false;
    }, 500);
  }

  aplicarFiltros(): void {
    if (!this.fechaInicio || !this.fechaFin) {
      alert('Por favor selecciona ambas fechas');
      return;
    }

    if (new Date(this.fechaFin) < new Date(this.fechaInicio)) {
      alert('La fecha fin debe ser mayor o igual a la fecha inicio');
      return;
    }

    this.cargarReportes();
  }
}
