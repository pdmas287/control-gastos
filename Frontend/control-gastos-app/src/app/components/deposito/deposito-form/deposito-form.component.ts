import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DepositoService } from '../../../services/deposito.service';
import { FondoMonetarioService } from '../../../services/fondo-monetario.service';
import { Deposito, DepositoCreate } from '../../../models/deposito.model';
import { FondoMonetario } from '../../../models/fondo-monetario.model';

@Component({
  selector: 'app-deposito-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './deposito-form.component.html',
  styleUrls: ['./deposito-form.component.css']
})
export class DepositoFormComponent implements OnInit {
  depositos: Deposito[] = [];
  fondosMonetarios: FondoMonetario[] = [];
  showForm: boolean = false;

  formData: DepositoCreate = {
    fecha: new Date(),
    fondoMonetarioId: 0,
    monto: 0,
    descripcion: ''
  };

  constructor(
    private depositoService: DepositoService,
    private fondoMonetarioService: FondoMonetarioService
  ) { }

  ngOnInit(): void {
    this.loadDepositos();
    this.loadFondosMonetarios();
  }

  loadDepositos(): void {
    this.depositoService.getAll().subscribe({
      next: (data) => {
        this.depositos = data;
      },
      error: (error) => {
        console.error('Error al cargar depósitos:', error);
        alert('Error al cargar depósitos');
      }
    });
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

  showCreateForm(): void {
    this.formData = {
      fecha: new Date(),
      fondoMonetarioId: 0,
      monto: 0,
      descripcion: ''
    };
    this.showForm = true;
  }

  onSubmit(): void {
    if (this.formData.fondoMonetarioId === 0) {
      alert('Debe seleccionar un fondo monetario');
      return;
    }

    if (this.formData.monto <= 0) {
      alert('El monto debe ser mayor a cero');
      return;
    }

    this.depositoService.create(this.formData).subscribe({
      next: () => {
        alert('Depósito registrado exitosamente');
        this.cancelForm();
        this.loadDepositos();
        this.loadFondosMonetarios(); // Actualizar saldos
      },
      error: (error) => {
        console.error('Error al crear depósito:', error);
        alert('Error al crear depósito: ' + (error.error?.message || error.message));
      }
    });
  }

  cancelForm(): void {
    this.showForm = false;
    this.formData = {
      fecha: new Date(),
      fondoMonetarioId: 0,
      monto: 0,
      descripcion: ''
    };
  }

  deleteDeposito(id: number, nombreFondo: string, monto: number): void {
    const mensaje = `¿Está seguro de eliminar este depósito?\n\nFondo: ${nombreFondo}\nMonto: $${monto}\n\nEsto reducirá el saldo del fondo.`;

    if (confirm(mensaje)) {
      this.depositoService.delete(id).subscribe({
        next: () => {
          alert('Depósito eliminado exitosamente');
          this.loadDepositos();
          this.loadFondosMonetarios(); // Actualizar saldos
        },
        error: (error) => {
          console.error('Error al eliminar:', error);
          alert('Error al eliminar depósito');
        }
      });
    }
  }

  formatDate(date: Date): string {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  getFondoById(id: number): FondoMonetario | undefined {
    return this.fondosMonetarios.find(f => f.fondoMonetarioId === id);
  }

  calcularTotalDepositos(): number {
    return this.depositos.reduce((sum, d) => sum + d.monto, 0);
  }
}
