import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Movimiento, ComparativoPresupuesto } from '../models/reporte.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReporteService {
  private apiUrl = `${environment.apiUrl}/Reporte`;

  constructor(private http: HttpClient) { }

  getMovimientos(fechaInicio: string, fechaFin: string, usuariosIds?: number[]): Observable<Movimiento[]> {
    let url = `${this.apiUrl}/movimientos?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`;

    if (usuariosIds && usuariosIds.length > 0) {
      const params = usuariosIds.map(id => `usuariosIds=${id}`).join('&');
      url = `${url}&${params}`;
    }

    return this.http.get<Movimiento[]>(url);
  }

  getComparativoPresupuesto(fechaInicio: string, fechaFin: string, usuariosIds?: number[]): Observable<ComparativoPresupuesto[]> {
    let url = `${this.apiUrl}/comparativo-presupuesto?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`;

    if (usuariosIds && usuariosIds.length > 0) {
      const params = usuariosIds.map(id => `usuariosIds=${id}`).join('&');
      url = `${url}&${params}`;
    }

    return this.http.get<ComparativoPresupuesto[]>(url);
  }
}
