import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Deposito, DepositoCreate, DepositoUpdate } from '../models/deposito.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DepositoService {
  private apiUrl = `${environment.apiUrl}/Deposito`;

  constructor(private http: HttpClient) { }

  getAll(usuariosIds?: number[]): Observable<Deposito[]> {
    let url = this.apiUrl;

    if (usuariosIds && usuariosIds.length > 0) {
      const params = usuariosIds.map(id => `usuariosIds=${id}`).join('&');
      url = `${this.apiUrl}?${params}`;
    }

    return this.http.get<Deposito[]>(url);
  }

  getById(id: number): Observable<Deposito> {
    return this.http.get<Deposito>(`${this.apiUrl}/${id}`);
  }

  create(deposito: DepositoCreate): Observable<Deposito> {
    return this.http.post<Deposito>(this.apiUrl, deposito);
  }

  update(id: number, deposito: DepositoUpdate): Observable<Deposito> {
    return this.http.put<Deposito>(`${this.apiUrl}/${id}`, deposito);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
