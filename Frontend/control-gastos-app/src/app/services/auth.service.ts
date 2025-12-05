import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResponse, Login, RegistroUsuario, Usuario, CambiarPassword } from '../models/auth.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(this.getUserFromStorage());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  /**
   * Obtiene el usuario almacenado en localStorage
   */
  private getUserFromStorage(): AuthResponse | null {
    const userJson = localStorage.getItem('currentUser');
    return userJson ? JSON.parse(userJson) : null;
  }

  /**
   * Registra un nuevo usuario en el sistema
   */
  registro(datos: RegistroUsuario): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/registro`, datos).pipe(
      tap(response => this.setCurrentUser(response))
    );
  }

  /**
   * Inicia sesión con un usuario existente
   */
  login(datos: Login): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, datos).pipe(
      tap(response => this.setCurrentUser(response))
    );
  }

  /**
   * Cierra la sesión del usuario actual
   */
  logout(): void {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  /**
   * Establece el usuario actual y lo guarda en localStorage
   */
  private setCurrentUser(user: AuthResponse): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  /**
   * Obtiene el token JWT del usuario actual
   */
  getToken(): string | null {
    const user = this.currentUserSubject.value;
    return user ? user.token : null;
  }

  /**
   * Verifica si el usuario está autenticado y el token no ha expirado
   */
  isAuthenticated(): boolean {
    const user = this.currentUserSubject.value;
    if (!user) return false;

    const expiration = new Date(user.fechaExpiracion);
    return expiration > new Date();
  }

  /**
   * Obtiene el ID del usuario actual
   */
  getCurrentUserId(): number | null {
    const user = this.currentUserSubject.value;
    return user ? user.usuarioId : null;
  }

  /**
   * Obtiene el nombre del usuario actual
   */
  getCurrentUserName(): string | null {
    const user = this.currentUserSubject.value;
    return user ? user.nombreCompleto : null;
  }

  /**
   * Obtiene el rol del usuario actual
   */
  getCurrentUserRole(): string | null {
    const user = this.currentUserSubject.value;
    return user ? user.rol : null;
  }

  /**
   * Verifica si el usuario actual es administrador
   */
  isAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return user ? user.rol === 'Admin' : false;
  }

  /**
   * Verifica si el usuario actual es un usuario normal
   */
  isUsuario(): boolean {
    const user = this.currentUserSubject.value;
    return user ? user.rol === 'Usuario' : false;
  }

  /**
   * Obtiene el perfil completo del usuario autenticado
   */
  getPerfil(): Observable<Usuario> {
    return this.http.get<Usuario>(`${this.apiUrl}/perfil`);
  }

  /**
   * Cambia la contraseña del usuario autenticado
   */
  cambiarPassword(datos: CambiarPassword): Observable<any> {
    return this.http.put(`${this.apiUrl}/cambiar-password`, datos);
  }

  /**
   * Verifica si el token es válido (útil para validación manual)
   */
  verificarToken(): Observable<any> {
    return this.http.get(`${this.apiUrl}/verificar-token`);
  }
}
