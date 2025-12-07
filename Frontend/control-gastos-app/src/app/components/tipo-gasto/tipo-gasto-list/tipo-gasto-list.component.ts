import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TipoGastoService } from '../../../services/tipo-gasto.service';
import { AuthService } from '../../../services/auth.service';
import { TipoGasto, TipoGastoCreate } from '../../../models/tipo-gasto.model';
import { FiltroUsuarioAdminComponent } from '../../shared/filtro-usuario-admin.component';

@Component({
  selector: 'app-tipo-gasto-list',
  standalone: true,
  imports: [CommonModule, FormsModule, FiltroUsuarioAdminComponent],
  templateUrl: './tipo-gasto-list.component.html',
  styleUrls: ['./tipo-gasto-list.component.css']
})
export class TipoGastoListComponent implements OnInit {
  tiposGasto: TipoGasto[] = [];
  showForm: boolean = false;
  isEditing: boolean = false;
  editingId: number | null = null;
  siguienteCodigo: string = '';
  isAdmin: boolean = false;
  usuariosFiltrados: number[] = [];

  formData: TipoGastoCreate = {
    descripcion: '',
    activo: true
  };

  constructor(
    private tipoGastoService: TipoGastoService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.loadTiposGasto();
  }

  onFiltroChange(usuariosIds: number[]): void {
    this.usuariosFiltrados = usuariosIds;
    this.loadTiposGasto();
  }

  loadTiposGasto(): void {
    // Si hay usuarios filtrados, pasar el filtro
    const filtro = this.usuariosFiltrados.length > 0 ? this.usuariosFiltrados : undefined;

    this.tipoGastoService.getAll(filtro).subscribe({
      next: (data) => {
        this.tiposGasto = data;
      },
      error: (error) => {
        console.error('Error al cargar tipos de gasto:', error);
        alert('Error al cargar tipos de gasto');
      }
    });
  }

  loadSiguienteCodigo(): void {
    this.tipoGastoService.getSiguienteCodigo().subscribe({
      next: (data) => {
        this.siguienteCodigo = data.codigo;
      },
      error: (error) => {
        console.error('Error al obtener siguiente código:', error);
      }
    });
  }

  showCreateForm(): void {
    this.isEditing = false;
    this.editingId = null;
    this.formData = { descripcion: '', activo: true };
    this.showForm = true;
    this.loadSiguienteCodigo();
  }

  showEditForm(tipoGasto: TipoGasto): void {
    this.isEditing = true;
    this.editingId = tipoGasto.tipoGastoId;
    this.siguienteCodigo = tipoGasto.codigo;
    this.formData = {
      descripcion: tipoGasto.descripcion,
      activo: tipoGasto.activo
    };
    this.showForm = true;
  }

  onSubmit(): void {
    if (this.isEditing && this.editingId) {
      this.tipoGastoService.update(this.editingId, this.formData).subscribe({
        next: () => {
          alert('Tipo de gasto actualizado exitosamente');
          this.cancelForm();
          this.loadTiposGasto();
        },
        error: (error) => {
          console.error('Error al actualizar:', error);
          alert('Error al actualizar tipo de gasto');
        }
      });
    } else {
      this.tipoGastoService.create(this.formData).subscribe({
        next: () => {
          alert('Tipo de gasto creado exitosamente');
          this.cancelForm();
          this.loadTiposGasto();
        },
        error: (error) => {
          console.error('Error al crear:', error);
          alert('Error al crear tipo de gasto');
        }
      });
    }
  }

  cancelForm(): void {
    this.showForm = false;
    this.formData = { descripcion: '', activo: true };
    this.isEditing = false;
    this.editingId = null;
  }

  deleteTipoGasto(id: number): void {
    if (confirm('¿Está seguro de eliminar este tipo de gasto?\n\nNota: Solo se pueden eliminar tipos de gasto que no tengan gastos o presupuestos registrados.')) {
      this.tipoGastoService.delete(id).subscribe({
        next: () => {
          alert('Tipo de gasto eliminado exitosamente');
          this.loadTiposGasto();
        },
        error: (error) => {
          console.error('Error al eliminar:', error);

          // Extraer el mensaje de error del backend
          let mensajeError = 'Error al eliminar tipo de gasto';
          if (error.error && typeof error.error === 'string') {
            mensajeError = error.error;
          } else if (error.error && error.error.message) {
            mensajeError = error.error.message;
          } else if (error.message) {
            mensajeError = error.message;
          }

          alert(mensajeError);
        }
      });
    }
  }
}
