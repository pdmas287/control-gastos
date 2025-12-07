import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Presupuesto, PresupuestoCreate, PresupuestoUpdate, PresupuestosPorMes } from '../models/presupuesto.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PresupuestoService {
  private apiUrl = `${environment.apiUrl}/Presupuesto`;

  constructor(private http: HttpClient) { }

  getAll(usuariosIds?: number[]): Observable<Presupuesto[]> {
    let url = this.apiUrl;

    if (usuariosIds && usuariosIds.length > 0) {
      const params = usuariosIds.map(id => `usuariosIds=${id}`).join('&');
      url = `${this.apiUrl}?${params}`;
    }

    return this.http.get<Presupuesto[]>(url);
  }

  getById(id: number): Observable<Presupuesto> {
    return this.http.get<Presupuesto>(`${this.apiUrl}/${id}`);
  }

  getPresupuestosPorMes(mes: number, anio: number, usuariosIds?: number[]): Observable<PresupuestosPorMes> {
    let url = `${this.apiUrl}/mes/${mes}/anio/${anio}`;

    if (usuariosIds && usuariosIds.length > 0) {
      const params = usuariosIds.map(id => `usuariosIds=${id}`).join('&');
      url = `${url}?${params}`;
    }

    return this.http.get<PresupuestosPorMes>(url);
  }

  create(presupuesto: PresupuestoCreate): Observable<Presupuesto> {
    return this.http.post<Presupuesto>(this.apiUrl, presupuesto);
  }

  update(id: number, presupuesto: PresupuestoUpdate): Observable<Presupuesto> {
    return this.http.put<Presupuesto>(`${this.apiUrl}/${id}`, presupuesto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
