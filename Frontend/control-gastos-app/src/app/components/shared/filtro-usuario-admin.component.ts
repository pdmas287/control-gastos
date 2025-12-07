import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UsuarioService } from '../../services/usuario.service';
import { AuthService } from '../../services/auth.service';

export interface Usuario {
  usuarioId: number;
  nombreCompleto: string;
  nombreUsuario: string;
  email: string;
}

@Component({
  selector: 'app-filtro-usuario-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="filtro-container" *ngIf="isAdmin">
      <div class="filtro-header">
        <h4>Filtrar por Usuario</h4>
        <button class="btn btn-link" (click)="toggleExpanded()">
          {{ expanded ? 'Ocultar' : 'Mostrar' }} Filtros
        </button>
      </div>

      <div class="filtro-content" *ngIf="expanded">
        <div class="filtro-opciones">
          <label class="filtro-opcion">
            <input
              type="radio"
              name="tipoFiltro"
              value="todos"
              [(ngModel)]="tipoFiltro"
              (change)="onTipoFiltroChange()">
            Ver todos los usuarios (consolidado)
          </label>

          <label class="filtro-opcion">
            <input
              type="radio"
              name="tipoFiltro"
              value="seleccionados"
              [(ngModel)]="tipoFiltro"
              (change)="onTipoFiltroChange()">
            Filtrar por usuarios específicos
          </label>
        </div>

        <div class="usuarios-lista" *ngIf="tipoFiltro === 'seleccionados'">
          <div class="usuarios-grid">
            <label
              *ngFor="let usuario of usuarios"
              class="usuario-checkbox">
              <input
                type="checkbox"
                [checked]="usuariosSeleccionados.includes(usuario.usuarioId)"
                (change)="toggleUsuario(usuario.usuarioId)">
              {{ usuario.nombreCompleto }} ({{ usuario.nombreUsuario }})
            </label>
          </div>

          <div class="filtro-acciones">
            <button class="btn btn-primary" (click)="aplicarFiltro()">
              Aplicar Filtro
            </button>
            <button class="btn btn-secondary" (click)="limpiarFiltro()">
              Limpiar
            </button>
          </div>
        </div>

        <div class="usuarios-seleccionados" *ngIf="usuariosSeleccionados.length > 0 && tipoFiltro === 'seleccionados'">
          <strong>Usuarios seleccionados ({{ usuariosSeleccionados.length }}):</strong>
          <div class="tags">
            <span
              *ngFor="let id of usuariosSeleccionados"
              class="tag">
              {{ getNombreUsuario(id) }}
              <button (click)="toggleUsuario(id)" class="tag-remove">×</button>
            </span>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .filtro-container {
      background-color: #f8f9fa;
      border: 1px solid #dee2e6;
      border-radius: 4px;
      padding: 15px;
      margin-bottom: 20px;
    }

    .filtro-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 10px;
    }

    .filtro-header h4 {
      margin: 0;
      font-size: 16px;
    }

    .btn-link {
      background: none;
      border: none;
      color: #007bff;
      cursor: pointer;
      text-decoration: underline;
      padding: 0;
    }

    .filtro-content {
      margin-top: 15px;
    }

    .filtro-opciones {
      display: flex;
      flex-direction: column;
      gap: 10px;
      margin-bottom: 15px;
    }

    .filtro-opcion {
      display: flex;
      align-items: center;
      gap: 8px;
      font-weight: 500;
      cursor: pointer;
    }

    .filtro-opcion input[type="radio"] {
      cursor: pointer;
    }

    .usuarios-lista {
      border-top: 1px solid #dee2e6;
      padding-top: 15px;
    }

    .usuarios-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
      gap: 10px;
      margin-bottom: 15px;
      max-height: 300px;
      overflow-y: auto;
      padding: 10px;
      background-color: white;
      border: 1px solid #dee2e6;
      border-radius: 4px;
    }

    .usuario-checkbox {
      display: flex;
      align-items: center;
      gap: 8px;
      cursor: pointer;
      padding: 5px;
      border-radius: 4px;
      transition: background-color 0.2s;
    }

    .usuario-checkbox:hover {
      background-color: #f8f9fa;
    }

    .usuario-checkbox input[type="checkbox"] {
      cursor: pointer;
    }

    .filtro-acciones {
      display: flex;
      gap: 10px;
      margin-top: 10px;
    }

    .usuarios-seleccionados {
      margin-top: 15px;
      padding: 10px;
      background-color: white;
      border: 1px solid #dee2e6;
      border-radius: 4px;
    }

    .tags {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      margin-top: 10px;
    }

    .tag {
      display: inline-flex;
      align-items: center;
      gap: 5px;
      background-color: #007bff;
      color: white;
      padding: 5px 10px;
      border-radius: 20px;
      font-size: 14px;
    }

    .tag-remove {
      background: none;
      border: none;
      color: white;
      font-size: 18px;
      cursor: pointer;
      padding: 0;
      width: 20px;
      height: 20px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 50%;
      transition: background-color 0.2s;
    }

    .tag-remove:hover {
      background-color: rgba(255, 255, 255, 0.2);
    }

    @media (max-width: 768px) {
      .usuarios-grid {
        grid-template-columns: 1fr;
      }

      .filtro-acciones {
        flex-direction: column;
      }

      .filtro-acciones button {
        width: 100%;
      }
    }
  `]
})
export class FiltroUsuarioAdminComponent implements OnInit {
  @Output() filtroChange = new EventEmitter<number[]>();

  isAdmin: boolean = false;
  expanded: boolean = false;
  tipoFiltro: 'todos' | 'seleccionados' = 'todos';
  usuarios: Usuario[] = [];
  usuariosSeleccionados: number[] = [];

  constructor(
    private usuarioService: UsuarioService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    if (this.isAdmin) {
      this.cargarUsuarios();
      // Emitir filtro inicial (todos)
      this.filtroChange.emit([]);
    }
  }

  cargarUsuarios(): void {
    this.usuarioService.getAll().subscribe({
      next: (data) => {
        this.usuarios = data;
      },
      error: (error) => {
        console.error('Error al cargar usuarios:', error);
      }
    });
  }

  toggleExpanded(): void {
    this.expanded = !this.expanded;
  }

  onTipoFiltroChange(): void {
    if (this.tipoFiltro === 'todos') {
      this.usuariosSeleccionados = [];
      this.filtroChange.emit([]);
    }
  }

  toggleUsuario(usuarioId: number): void {
    const index = this.usuariosSeleccionados.indexOf(usuarioId);
    if (index > -1) {
      this.usuariosSeleccionados.splice(index, 1);
    } else {
      this.usuariosSeleccionados.push(usuarioId);
    }
  }

  aplicarFiltro(): void {
    if (this.usuariosSeleccionados.length === 0) {
      alert('Por favor selecciona al menos un usuario');
      return;
    }
    this.filtroChange.emit(this.usuariosSeleccionados);
  }

  limpiarFiltro(): void {
    this.usuariosSeleccionados = [];
    this.tipoFiltro = 'todos';
    this.filtroChange.emit([]);
  }

  getNombreUsuario(usuarioId: number): string {
    const usuario = this.usuarios.find(u => u.usuarioId === usuarioId);
    return usuario ? usuario.nombreCompleto : '';
  }
}
