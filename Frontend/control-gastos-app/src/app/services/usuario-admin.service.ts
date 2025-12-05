import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UsuarioAdmin, UsuarioUpdateAdmin, CambiarRolDto, EstadisticasUsuarios } from '../models/usuario-admin.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UsuarioAdminService {
  private apiUrl = `${environment.apiUrl}/usuario`;

  constructor(private http: HttpClient) {}

  /**
   * Obtiene todos los usuarios del sistema (solo admin)
   */
  getAllUsuarios(): Observable<UsuarioAdmin[]> {
    return this.http.get<UsuarioAdmin[]>(this.apiUrl);
  }

  /**
   * Obtiene un usuario por ID (solo admin)
   */
  getUsuarioById(id: number): Observable<UsuarioAdmin> {
    return this.http.get<UsuarioAdmin>(`${this.apiUrl}/${id}`);
  }

  /**
   * Actualiza un usuario (solo admin)
   */
  updateUsuario(id: number, usuario: UsuarioUpdateAdmin): Observable<UsuarioAdmin> {
    return this.http.put<UsuarioAdmin>(`${this.apiUrl}/${id}`, usuario);
  }

  /**
   * Activa un usuario (solo admin)
   */
  activarUsuario(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/activar`, {});
  }

  /**
   * Desactiva un usuario (solo admin)
   */
  desactivarUsuario(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/desactivar`, {});
  }

  /**
   * Cambia el rol de un usuario (solo admin)
   */
  cambiarRol(dto: CambiarRolDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/${dto.usuarioId}/cambiar-rol`, { nuevoRol: dto.nuevoRol });
  }

  /**
   * Obtiene estadísticas de usuarios (solo admin)
   */
  getEstadisticas(): Observable<EstadisticasUsuarios> {
    return this.http.get<EstadisticasUsuarios>(`${this.apiUrl}/estadisticas`);
  }

  /**
   * Elimina un usuario (solo admin) - usar con precaución
   */
  deleteUsuario(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
