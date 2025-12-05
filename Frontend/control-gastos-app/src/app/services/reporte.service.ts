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

  getMovimientos(fechaInicio: string, fechaFin: string): Observable<Movimiento[]> {
    return this.http.get<Movimiento[]>(
      `${this.apiUrl}/movimientos?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`
    );
  }

  getComparativoPresupuesto(fechaInicio: string, fechaFin: string): Observable<ComparativoPresupuesto[]> {
    return this.http.get<ComparativoPresupuesto[]>(
      `${this.apiUrl}/comparativo-presupuesto?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`
    );
  }
}
