import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FondoMonetario, FondoMonetarioCreate, FondoMonetarioUpdate } from '../models/fondo-monetario.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FondoMonetarioService {
  private apiUrl = `${environment.apiUrl}/FondoMonetario`;

  constructor(private http: HttpClient) { }

  getAll(usuariosIds?: number[]): Observable<FondoMonetario[]> {
    let url = this.apiUrl;

    if (usuariosIds && usuariosIds.length > 0) {
      const params = usuariosIds.map(id => `usuariosIds=${id}`).join('&');
      url = `${this.apiUrl}?${params}`;
    }

    return this.http.get<FondoMonetario[]>(url);
  }

  getById(id: number): Observable<FondoMonetario> {
    return this.http.get<FondoMonetario>(`${this.apiUrl}/${id}`);
  }

  create(fondo: FondoMonetarioCreate): Observable<FondoMonetario> {
    return this.http.post<FondoMonetario>(this.apiUrl, fondo);
  }

  update(id: number, fondo: FondoMonetarioUpdate): Observable<FondoMonetario> {
    return this.http.put<FondoMonetario>(`${this.apiUrl}/${id}`, fondo);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
