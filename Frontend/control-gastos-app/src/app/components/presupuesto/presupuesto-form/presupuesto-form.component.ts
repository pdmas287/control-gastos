import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PresupuestoService } from '../../../services/presupuesto.service';
import { TipoGastoService } from '../../../services/tipo-gasto.service';
import { PresupuestoItem, PresupuestoCreate, PresupuestoUpdate } from '../../../models/presupuesto.model';
import { TipoGasto } from '../../../models/tipo-gasto.model';

@Component({
  selector: 'app-presupuesto-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './presupuesto-form.component.html',
  styleUrls: ['./presupuesto-form.component.css']
})
export class PresupuestoFormComponent implements OnInit {
  mes: number = new Date().getMonth() + 1;
  anio: number = new Date().getFullYear();

  presupuestos: PresupuestoItem[] = [];
  tiposGasto: TipoGasto[] = [];
  loaded: boolean = false;

  meses = [
    { valor: 1, nombre: 'Enero' },
    { valor: 2, nombre: 'Febrero' },
    { valor: 3, nombre: 'Marzo' },
    { valor: 4, nombre: 'Abril' },
    { valor: 5, nombre: 'Mayo' },
    { valor: 6, nombre: 'Junio' },
    { valor: 7, nombre: 'Julio' },
    { valor: 8, nombre: 'Agosto' },
    { valor: 9, nombre: 'Septiembre' },
    { valor: 10, nombre: 'Octubre' },
    { valor: 11, nombre: 'Noviembre' },
    { valor: 12, nombre: 'Diciembre' }
  ];

  anios: number[] = [];

  constructor(
    private presupuestoService: PresupuestoService,
    private tipoGastoService: TipoGastoService
  ) {
    const currentYear = new Date().getFullYear();
    for (let i = currentYear - 2; i <= currentYear + 2; i++) {
      this.anios.push(i);
    }
  }

  ngOnInit(): void {
    this.loadTiposGasto();
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

  cargarPresupuestos(): void {
    this.presupuestoService.getPresupuestosPorMes(this.mes, this.anio).subscribe({
      next: (data) => {
        this.presupuestos = data.items;
        this.loaded = true;

        if (this.presupuestos.length === 0) {
          // Caso 1: No hay presupuestos para este mes, inicializar todos
          this.inicializarPresupuestos();
        } else {
          // Caso 2: Ya hay presupuestos, pero verificar si hay nuevos tipos de gasto
          this.agregarTiposGastoFaltantes();
        }
      },
      error: (error) => {
        console.error('Error al cargar presupuestos:', error);
        alert('Error al cargar presupuestos');
      }
    });
  }

  agregarTiposGastoFaltantes(): void {
    // Obtener IDs de tipos de gasto que ya tienen presupuesto
    const tiposConPresupuesto = this.presupuestos.map(p => p.tipoGastoId);

    // Encontrar tipos de gasto que NO tienen presupuesto en este mes
    const tiposFaltantes = this.tiposGasto.filter(
      tipo => !tiposConPresupuesto.includes(tipo.tipoGastoId)
    );

    // Agregar los tipos de gasto faltantes con monto 0
    tiposFaltantes.forEach(tipo => {
      this.presupuestos.push({
        presupuestoId: 0,
        tipoGastoId: tipo.tipoGastoId,
        tipoGastoDescripcion: tipo.descripcion,
        montoPresupuestado: 0
      });
    });

    // Ordenar por descripción para mantener consistencia
    this.presupuestos.sort((a, b) =>
      a.tipoGastoDescripcion.localeCompare(b.tipoGastoDescripcion)
    );
  }

  inicializarPresupuestos(): void {
    this.presupuestos = this.tiposGasto.map(tipo => ({
      presupuestoId: 0,
      tipoGastoId: tipo.tipoGastoId,
      tipoGastoDescripcion: tipo.descripcion,
      montoPresupuestado: 0
    }));
  }

  guardarPresupuestos(): void {
    if (!this.loaded) {
      alert('Primero debe cargar los presupuestos');
      return;
    }

    let errores = 0;
    let creados = 0;
    let actualizados = 0;

    const total = this.presupuestos.filter(p => p.montoPresupuestado > 0).length;
    let procesados = 0;

    this.presupuestos.forEach(presupuesto => {
      if (presupuesto.montoPresupuestado <= 0) {
        return;
      }

      if (presupuesto.presupuestoId === 0) {
        const crear: PresupuestoCreate = {
          tipoGastoId: presupuesto.tipoGastoId,
          mes: this.mes,
          anio: this.anio,
          montoPresupuestado: presupuesto.montoPresupuestado
        };

        this.presupuestoService.create(crear).subscribe({
          next: () => {
            creados++;
            procesados++;
            if (procesados === total) {
              this.mostrarResultado(creados, actualizados, errores);
            }
          },
          error: (error) => {
            errores++;
            procesados++;
            console.error('Error al crear:', error);
            if (procesados === total) {
              this.mostrarResultado(creados, actualizados, errores);
            }
          }
        });
      } else {
        const actualizar: PresupuestoUpdate = {
          montoPresupuestado: presupuesto.montoPresupuestado
        };

        this.presupuestoService.update(presupuesto.presupuestoId, actualizar).subscribe({
          next: () => {
            actualizados++;
            procesados++;
            if (procesados === total) {
              this.mostrarResultado(creados, actualizados, errores);
            }
          },
          error: (error) => {
            errores++;
            procesados++;
            console.error('Error al actualizar:', error);
            if (procesados === total) {
              this.mostrarResultado(creados, actualizados, errores);
            }
          }
        });
      }
    });

    if (total === 0) {
      alert('No hay presupuestos con montos mayores a cero para guardar');
    }
  }

  mostrarResultado(creados: number, actualizados: number, errores: number): void {
    let mensaje = 'Presupuestos guardados:\n\n';
    if (creados > 0) mensaje += `Creados: ${creados}\n`;
    if (actualizados > 0) mensaje += `Actualizados: ${actualizados}\n`;
    if (errores > 0) mensaje += `Errores: ${errores}\n`;

    alert(mensaje);
    this.cargarPresupuestos();
  }

  calcularTotal(): number {
    return this.presupuestos.reduce((sum, p) => sum + (p.montoPresupuestado || 0), 0);
  }

  getNombreMes(): string {
    const mesObj = this.meses.find(m => m.valor === this.mes);
    return mesObj ? mesObj.nombre : '';
  }

  resetearMontos(): void {
    if (!this.loaded) {
      alert('Primero debe cargar los presupuestos');
      return;
    }

    const confirmacion = confirm(
      `¿Está seguro que desea poner en cero TODOS los montos de ${this.getNombreMes()} ${this.anio}?\n\nNota: Esto solo resetea los valores en pantalla. Use "Guardar" para aplicar los cambios.`
    );

    if (confirmacion) {
      this.presupuestos.forEach(p => p.montoPresupuestado = 0);
      alert('Montos reseteados a cero. Recuerde hacer clic en "Guardar" para aplicar los cambios.');
    }
  }

  eliminarPresupuestosMes(): void {
    if (!this.loaded) {
      alert('Primero debe cargar los presupuestos');
      return;
    }

    const presupuestosExistentes = this.presupuestos.filter(p => p.presupuestoId > 0);

    if (presupuestosExistentes.length === 0) {
      alert('No hay presupuestos guardados en este mes para eliminar.');
      return;
    }

    const nombreMes = this.getNombreMes();
    const mesAnio = nombreMes ? `${nombreMes} ${this.anio}` : `Mes ${this.mes}/${this.anio}`;

    const confirmacion = confirm(
      `¿Está seguro que desea ELIMINAR PERMANENTEMENTE todos los presupuestos de ${mesAnio}?\n\n` +
      `Se eliminarán ${presupuestosExistentes.length} presupuesto(s) SOLO de este mes.\n\n` +
      `Esta acción NO se puede deshacer.`
    );

    if (!confirmacion) {
      return;
    }

    let eliminados = 0;
    let errores = 0;
    const total = presupuestosExistentes.length;

    presupuestosExistentes.forEach(presupuesto => {
      this.presupuestoService.delete(presupuesto.presupuestoId).subscribe({
        next: () => {
          eliminados++;
          if (eliminados + errores === total) {
            this.mostrarResultadoEliminacion(eliminados, errores);
          }
        },
        error: (error) => {
          errores++;
          console.error('Error al eliminar:', error);
          if (eliminados + errores === total) {
            this.mostrarResultadoEliminacion(eliminados, errores);
          }
        }
      });
    });
  }

  mostrarResultadoEliminacion(eliminados: number, errores: number): void {
    let mensaje = 'Resultado de la eliminación:\n\n';
    if (eliminados > 0) mensaje += `✓ Eliminados: ${eliminados}\n`;
    if (errores > 0) mensaje += `✗ Errores: ${errores}\n`;

    alert(mensaje);
    this.cargarPresupuestos(); // Recargar para mostrar el mes vacío
  }
}
