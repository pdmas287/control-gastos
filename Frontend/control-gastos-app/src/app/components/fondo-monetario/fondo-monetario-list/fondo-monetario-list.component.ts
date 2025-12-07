import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FondoMonetarioService } from '../../../services/fondo-monetario.service';
import { AuthService } from '../../../services/auth.service';
import { FondoMonetario, FondoMonetarioCreate, FondoMonetarioUpdate } from '../../../models/fondo-monetario.model';
import { FiltroUsuarioAdminComponent } from '../../shared/filtro-usuario-admin.component';

@Component({
  selector: 'app-fondo-monetario-list',
  standalone: true,
  imports: [CommonModule, FormsModule, FiltroUsuarioAdminComponent],
  templateUrl: './fondo-monetario-list.component.html',
  styleUrls: ['./fondo-monetario-list.component.css']
})
export class FondoMonetarioListComponent implements OnInit {
  fondosMonetarios: FondoMonetario[] = [];
  showForm: boolean = false;
  isEditing: boolean = false;
  editingId: number | null = null;
  isAdmin: boolean = false;
  usuariosFiltrados: number[] = [];

  formData: FondoMonetarioCreate = {
    nombre: '',
    tipoFondo: 'Cuenta Bancaria',
    descripcion: '',
    saldoActual: 0,
    activo: true
  };

  constructor(
    private fondoMonetarioService: FondoMonetarioService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.loadFondosMonetarios();
  }

  onFiltroChange(usuariosIds: number[]): void {
    this.usuariosFiltrados = usuariosIds;
    this.loadFondosMonetarios();
  }

  loadFondosMonetarios(): void {
    const filtro = this.usuariosFiltrados.length > 0 ? this.usuariosFiltrados : undefined;

    this.fondoMonetarioService.getAll(filtro).subscribe({
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
    this.isEditing = false;
    this.editingId = null;
    this.formData = {
      nombre: '',
      tipoFondo: 'Cuenta Bancaria',
      descripcion: '',
      saldoActual: 0,
      activo: true
    };
    this.showForm = true;
  }

  showEditForm(fondo: FondoMonetario): void {
    this.isEditing = true;
    this.editingId = fondo.fondoMonetarioId;
    this.formData = {
      nombre: fondo.nombre,
      tipoFondo: fondo.tipoFondo,
      descripcion: fondo.descripcion || '',
      saldoActual: fondo.saldoActual,
      activo: fondo.activo
    };
    this.showForm = true;
  }

  onSubmit(): void {
    if (this.isEditing && this.editingId) {
      const updateData: FondoMonetarioUpdate = {
        nombre: this.formData.nombre,
        tipoFondo: this.formData.tipoFondo,
        descripcion: this.formData.descripcion,
        activo: this.formData.activo
      };

      this.fondoMonetarioService.update(this.editingId, updateData).subscribe({
        next: () => {
          alert('Fondo monetario actualizado exitosamente');
          this.cancelForm();
          this.loadFondosMonetarios();
        },
        error: (error) => {
          console.error('Error al actualizar:', error);
          alert('Error al actualizar fondo monetario');
        }
      });
    } else {
      this.fondoMonetarioService.create(this.formData).subscribe({
        next: () => {
          alert('Fondo monetario creado exitosamente');
          this.cancelForm();
          this.loadFondosMonetarios();
        },
        error: (error) => {
          console.error('Error al crear:', error);
          alert('Error al crear fondo monetario');
        }
      });
    }
  }

  cancelForm(): void {
    this.showForm = false;
    this.formData = {
      nombre: '',
      tipoFondo: 'Cuenta Bancaria',
      descripcion: '',
      saldoActual: 0,
      activo: true
    };
    this.isEditing = false;
    this.editingId = null;
  }

  deleteFondoMonetario(id: number): void {
    if (confirm('¿Está seguro de eliminar este fondo monetario?')) {
      this.fondoMonetarioService.delete(id).subscribe({
        next: () => {
          alert('Fondo monetario eliminado exitosamente');
          this.loadFondosMonetarios();
        },
        error: (error) => {
          console.error('Error al eliminar:', error);
          alert('Error al eliminar fondo monetario');
        }
      });
    }
  }
}
