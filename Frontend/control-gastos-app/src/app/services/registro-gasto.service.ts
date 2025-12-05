import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegistroGasto, RegistroGastoCreate, ValidacionPresupuesto } from '../models/registro-gasto.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RegistroGastoService {
  private apiUrl = `${environment.apiUrl}/RegistroGasto`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<RegistroGasto[]> {
    return this.http.get<RegistroGasto[]>(this.apiUrl);
  }

  getById(id: number): Observable<RegistroGasto> {
    return this.http.get<RegistroGasto>(`${this.apiUrl}/${id}`);
  }

  validarPresupuesto(registroGasto: RegistroGastoCreate): Observable<ValidacionPresupuesto> {
    return this.http.post<ValidacionPresupuesto>(`${this.apiUrl}/validar`, registroGasto);
  }

  create(registroGasto: RegistroGastoCreate): Observable<{ registro: RegistroGasto, validacion: ValidacionPresupuesto }> {
    return this.http.post<{ registro: RegistroGasto, validacion: ValidacionPresupuesto }>(this.apiUrl, registroGasto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
