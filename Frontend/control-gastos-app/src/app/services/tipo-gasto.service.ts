import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TipoGasto, TipoGastoCreate, TipoGastoUpdate } from '../models/tipo-gasto.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TipoGastoService {
  private apiUrl = `${environment.apiUrl}/TipoGasto`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<TipoGasto[]> {
    return this.http.get<TipoGasto[]>(this.apiUrl);
  }

  getById(id: number): Observable<TipoGasto> {
    return this.http.get<TipoGasto>(`${this.apiUrl}/${id}`);
  }

  getSiguienteCodigo(): Observable<{ codigo: string }> {
    return this.http.get<{ codigo: string }>(`${this.apiUrl}/siguiente-codigo`);
  }

  create(tipoGasto: TipoGastoCreate): Observable<TipoGasto> {
    return this.http.post<TipoGasto>(this.apiUrl, tipoGasto);
  }

  update(id: number, tipoGasto: TipoGastoUpdate): Observable<TipoGasto> {
    return this.http.put<TipoGasto>(`${this.apiUrl}/${id}`, tipoGasto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
