import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RegistroGastoService } from '../../../services/registro-gasto.service';
import { FondoMonetarioService } from '../../../services/fondo-monetario.service';
import { TipoGastoService } from '../../../services/tipo-gasto.service';
import { RegistroGastoCreate, RegistroGastoDetalleCreate, ValidacionPresupuesto } from '../../../models/registro-gasto.model';
import { FondoMonetario } from '../../../models/fondo-monetario.model';
import { TipoGasto } from '../../../models/tipo-gasto.model';

interface DetalleTemp {
  tipoGastoId: number;
  monto: number;
  descripcion: string;
}

@Component({
  selector: 'app-registro-gasto-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './registro-gasto-form.component.html',
  styleUrls: ['./registro-gasto-form.component.css']
})
export class RegistroGastoFormComponent implements OnInit {
  // Encabezado
  fecha: Date = new Date();
  fondoMonetarioId: number = 0;
  nombreComercio: string = '';
  tipoDocumento: string = 'Comprobante';
  observaciones: string = '';

  // Detalles
  detalles: DetalleTemp[] = [];

  // Listas para dropdowns
  fondosMonetarios: FondoMonetario[] = [];
  tiposGasto: TipoGasto[] = [];

  // Control
  guardando: boolean = false;

  constructor(
    private registroGastoService: RegistroGastoService,
    private fondoMonetarioService: FondoMonetarioService,
    private tipoGastoService: TipoGastoService
  ) { }

  ngOnInit(): void {
    this.loadFondosMonetarios();
    this.loadTiposGasto();
    this.agregarDetalle(); // Agregar primera línea automáticamente
  }

  loadFondosMonetarios(): void {
    this.fondoMonetarioService.getAll().subscribe({
      next: (data) => {
        this.fondosMonetarios = data;
      },
      error: (error) => {
        console.error('Error al cargar fondos monetarios:', error);
        alert('Error al cargar fondos monetarios');
      }
    });
  }

  loadTiposGasto(): void {
    this.tipoGastoService.getAll().subscribe({
      next: (data) => {
        this.tiposGasto = data;
      },
      error: (error) => {
        console.error('Error al cargar tipos de gasto:', error);
        alert('Error al cargar tipos de gasto');
      }
    });
  }

  agregarDetalle(): void {
    this.detalles.push({
      tipoGastoId: 0,
      monto: 0,
      descripcion: ''
    });
  }

  eliminarDetalle(index: number): void {
    if (this.detalles.length > 1) {
      this.detalles.splice(index, 1);
    } else {
      alert('Debe haber al menos un detalle');
    }
  }

  calcularTotal(): number {
    return this.detalles.reduce((sum, d) => sum + (d.monto || 0), 0);
  }

  validarFormulario(): boolean {
    // Validar encabezado
    if (!this.fecha) {
      alert('La fecha es obligatoria');
      return false;
    }

    if (this.fondoMonetarioId === 0) {
      alert('Debe seleccionar un fondo monetario');
      return false;
    }

    if (!this.nombreComercio.trim()) {
      alert('El nombre del comercio es obligatorio');
      return false;
    }

    if (!this.tipoDocumento) {
      alert('Debe seleccionar un tipo de documento');
      return false;
    }

    // Validar detalles
    if (this.detalles.length === 0) {
      alert('Debe agregar al menos un detalle');
      return false;
    }

    // Validar cada detalle
    for (let i = 0; i < this.detalles.length; i++) {
      const detalle = this.detalles[i];

      if (detalle.tipoGastoId === 0) {
        alert(`Línea ${i + 1}: Debe seleccionar un tipo de gasto`);
        return false;
      }

      if (!detalle.monto || detalle.monto <= 0) {
        alert(`Línea ${i + 1}: El monto debe ser mayor a cero`);
        return false;
      }
    }

    return true;
  }

  onSubmit(): void {
    if (!this.validarFormulario()) {
      return;
    }

    if (this.guardando) {
      return; // Evitar doble submit
    }

    this.guardando = true;

    // Construir objeto de registro
    const detallesCreate: RegistroGastoDetalleCreate[] = this.detalles.map(d => ({
      tipoGastoId: d.tipoGastoId,
      monto: d.monto,
      descripcion: d.descripcion || undefined
    }));

    const registro: RegistroGastoCreate = {
      fecha: this.fecha,
      fondoMonetarioId: this.fondoMonetarioId,
      nombreComercio: this.nombreComercio,
      tipoDocumento: this.tipoDocumento,
      observaciones: this.observaciones || undefined,
      detalles: detallesCreate
    };

    this.registroGastoService.create(registro).subscribe({
      next: (response) => {
        this.guardando = false;

        // Verificar si hay sobregiros
        if (response.validacion.haySobregiro) {
          this.mostrarAlertaSobregiro(response.validacion);
        } else {
          alert('✅ Gasto registrado exitosamente\n\nNo hay sobregiros de presupuesto.');
        }

        this.limpiarFormulario();
      },
      error: (error) => {
        this.guardando = false;
        console.error('Error al registrar gasto:', error);

        let mensaje = 'Error al registrar gasto';
        if (error.error?.message) {
          mensaje += ':\n' + error.error.message;
        } else if (error.message) {
          mensaje += ':\n' + error.message;
        }

        alert(mensaje);
      }
    });
  }

  mostrarAlertaSobregiro(validacion: ValidacionPresupuesto): void {
    let mensaje = '⚠️ ALERTA: PRESUPUESTO SOBREGIRADO\n\n';
    mensaje += 'El gasto ha sido registrado, pero ha excedido el presupuesto en los siguientes tipos de gasto:\n\n';

    validacion.sobregiros.forEach((sobregiro, index) => {
      mensaje += `${index + 1}. ${sobregiro.tipoGastoDescripcion}\n`;
      mensaje += `   • Presupuestado: $${sobregiro.montoPresupuestado.toFixed(2)}\n`;
      mensaje += `   • Ejecutado: $${sobregiro.montoEjecutado.toFixed(2)}\n`;
      mensaje += `   • Sobregiro: $${sobregiro.montoSobregiro.toFixed(2)}\n\n`;
    });

    mensaje += '💡 Considere ajustar su presupuesto mensual o reducir gastos en estas categorías.';

    alert(mensaje);
  }

  limpiarFormulario(): void {
    this.fecha = new Date();
    this.fondoMonetarioId = 0;
    this.nombreComercio = '';
    this.tipoDocumento = 'Comprobante';
    this.observaciones = '';
    this.detalles = [];
    this.agregarDetalle();
  }

  formatDate(date: Date): string {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  getTipoGastoNombre(id: number): string {
    const tipo = this.tiposGasto.find(t => t.tipoGastoId === id);
    return tipo ? tipo.descripcion : '';
  }
}
