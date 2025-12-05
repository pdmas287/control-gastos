import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UsuarioAdminService } from '../../services/usuario-admin.service';
import { UsuarioAdmin, EstadisticasUsuarios } from '../../models/usuario-admin.model';

@Component({
  selector: 'app-usuarios-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './usuarios-admin.component.html',
  styleUrls: ['./usuarios-admin.component.css']
})
export class UsuariosAdminComponent implements OnInit {
  usuarios: UsuarioAdmin[] = [];
  estadisticas: EstadisticasUsuarios | null = null;
  filtroTexto: string = '';
  filtroRol: string = '';
  filtroActivo: string = '';
  cargando: boolean = false;
  error: string = '';

  constructor(private usuarioService: UsuarioAdminService) {}

  ngOnInit(): void {
    this.cargarUsuarios();
    this.cargarEstadisticas();
  }

  cargarUsuarios(): void {
    this.cargando = true;
    this.error = '';

    this.usuarioService.getAllUsuarios().subscribe({
      next: (data) => {
        this.usuarios = data;
        this.cargando = false;
      },
      error: (err) => {
        this.error = 'Error al cargar usuarios';
        console.error('Error:', err);
        this.cargando = false;
      }
    });
  }

  cargarEstadisticas(): void {
    this.usuarioService.getEstadisticas().subscribe({
      next: (data) => {
        this.estadisticas = data;
      },
      error: (err) => {
        console.error('Error al cargar estadísticas:', err);
      }
    });
  }

  get usuariosFiltrados(): UsuarioAdmin[] {
    return this.usuarios.filter(u => {
      const coincideTexto = !this.filtroTexto ||
        u.nombreUsuario.toLowerCase().includes(this.filtroTexto.toLowerCase()) ||
        u.email.toLowerCase().includes(this.filtroTexto.toLowerCase()) ||
        u.nombreCompleto.toLowerCase().includes(this.filtroTexto.toLowerCase());

      const coincideRol = !this.filtroRol || u.rol === this.filtroRol;

      const coincideActivo = !this.filtroActivo ||
        (this.filtroActivo === 'true' && u.activo) ||
        (this.filtroActivo === 'false' && !u.activo);

      return coincideTexto && coincideRol && coincideActivo;
    });
  }

  activarUsuario(id: number): void {
    if (confirm('¿Estás seguro de activar este usuario?')) {
      this.usuarioService.activarUsuario(id).subscribe({
        next: () => {
          this.cargarUsuarios();
          this.cargarEstadisticas();
        },
        error: (err) => {
          alert('Error al activar usuario');
          console.error('Error:', err);
        }
      });
    }
  }

  desactivarUsuario(id: number): void {
    if (confirm('¿Estás seguro de desactivar este usuario?')) {
      this.usuarioService.desactivarUsuario(id).subscribe({
        next: () => {
          this.cargarUsuarios();
          this.cargarEstadisticas();
        },
        error: (err) => {
          alert('Error al desactivar usuario');
          console.error('Error:', err);
        }
      });
    }
  }

  cambiarRol(usuario: UsuarioAdmin): void {
    const nuevoRol = usuario.rol === 'Admin' ? 'Usuario' : 'Admin';
    const mensaje = `¿Cambiar rol de ${usuario.nombreCompleto} a ${nuevoRol}?`;

    if (confirm(mensaje)) {
      this.usuarioService.cambiarRol({ usuarioId: usuario.usuarioId, nuevoRol }).subscribe({
        next: () => {
          this.cargarUsuarios();
          this.cargarEstadisticas();
          alert('Rol cambiado exitosamente');
        },
        error: (err) => {
          alert('Error al cambiar rol');
          console.error('Error:', err);
        }
      });
    }
  }

  limpiarFiltros(): void {
    this.filtroTexto = '';
    this.filtroRol = '';
    this.filtroActivo = '';
  }
}
