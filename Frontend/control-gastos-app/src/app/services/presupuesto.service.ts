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

  getAll(): Observable<Presupuesto[]> {
    return this.http.get<Presupuesto[]>(this.apiUrl);
  }

  getById(id: number): Observable<Presupuesto> {
    return this.http.get<Presupuesto>(`${this.apiUrl}/${id}`);
  }

  getPresupuestosPorMes(mes: number, anio: number): Observable<PresupuestosPorMes> {
    return this.http.get<PresupuestosPorMes>(`${this.apiUrl}/mes/${mes}/anio/${anio}`);
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
