import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="app-container">
      <nav class="navbar" *ngIf="isAuthenticated">
        <div class="container">
          <div class="navbar-brand">
            <h1>Control de Gastos</h1>
          </div>
          <ul class="nav-menu">
            <li class="nav-item">
              <a routerLink="/home" routerLinkActive="active">Inicio</a>
            </li>
            <li class="nav-item dropdown">
              <span>Mantenimientos</span>
              <ul class="dropdown-menu">
                <li><a routerLink="/tipos-gasto" routerLinkActive="active">Tipos de Gasto</a></li>
                <li><a routerLink="/fondos-monetarios" routerLinkActive="active">Fondos Monetarios</a></li>
              </ul>
            </li>
            <li class="nav-item dropdown">
              <span>Movimientos</span>
              <ul class="dropdown-menu">
                <li><a routerLink="/presupuestos" routerLinkActive="active">Presupuestos</a></li>
                <li><a routerLink="/registro-gastos" routerLinkActive="active">Registro de Gastos</a></li>
                <li><a routerLink="/depositos" routerLinkActive="active">Depósitos</a></li>
              </ul>
            </li>
            <li class="nav-item dropdown">
              <span>Consultas y Reportes</span>
              <ul class="dropdown-menu">
                <li><a routerLink="/consulta-movimientos" routerLinkActive="active">Consulta de Movimientos</a></li>
                <li><a routerLink="/grafico-comparativo" routerLinkActive="active">Gráfico Comparativo</a></li>
              </ul>
            </li>
            <li class="nav-item dropdown" *ngIf="isAdmin">
              <span class="admin-menu-title">⚙️ Administración</span>
              <ul class="dropdown-menu">
                <li><a routerLink="/admin/usuarios" routerLinkActive="active">Gestión de Usuarios</a></li>
                <li><a routerLink="/admin/reportes-globales" routerLinkActive="active">Reportes Globales</a></li>
              </ul>
            </li>
          </ul>
          <div class="user-menu">
            <span class="user-name">{{ userName }} <span class="role-badge" *ngIf="isAdmin">Admin</span></span>
            <button class="logout-btn" (click)="onLogout()">Cerrar Sesión</button>
          </div>
        </div>
      </nav>
      <main class="main-content">
        <div [class.container]="isAuthenticated && !isAdminRoute">
          <router-outlet></router-outlet>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .app-container {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
    }

    .navbar {
      background-color: #2c3e50;
      color: white;
      padding: 15px 0;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .navbar-brand h1 {
      margin: 0;
      font-size: 24px;
    }

    .nav-menu {
      list-style: none;
      display: flex;
      gap: 30px;
      margin-top: 15px;
    }

    .nav-item {
      position: relative;
    }

    .nav-item a,
    .nav-item span {
      color: white;
      text-decoration: none;
      padding: 8px 12px;
      display: block;
      cursor: pointer;
      transition: background-color 0.3s;
    }

    .nav-item a:hover,
    .nav-item span:hover {
      background-color: rgba(255, 255, 255, 0.1);
      border-radius: 4px;
    }

    .nav-item a.active {
      background-color: #3498db;
      border-radius: 4px;
    }

    .dropdown-menu {
      display: none;
      position: absolute;
      top: 100%;
      left: 0;
      background-color: #34495e;
      list-style: none;
      min-width: 200px;
      box-shadow: 0 4px 6px rgba(0,0,0,0.1);
      border-radius: 4px;
      z-index: 1000;
    }

    .nav-item.dropdown:hover .dropdown-menu {
      display: block;
    }

    .dropdown-menu li a {
      padding: 10px 15px;
      border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    }

    .dropdown-menu li:last-child a {
      border-bottom: none;
    }

    .main-content {
      flex: 1;
      padding: 30px 0;
    }

    .user-menu {
      display: flex;
      align-items: center;
      gap: 15px;
      margin-left: auto;
    }

    .user-name {
      color: white;
      font-weight: 500;
    }

    .logout-btn {
      background-color: #e74c3c;
      color: white;
      border: none;
      padding: 8px 16px;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;
      transition: background-color 0.3s;
    }

    .logout-btn:hover {
      background-color: #c0392b;
    }

    .container {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .admin-menu-title {
      color: #f39c12 !important;
      font-weight: 600;
    }

    .role-badge {
      background-color: #f39c12;
      color: #2c3e50;
      padding: 2px 8px;
      border-radius: 12px;
      font-size: 11px;
      font-weight: 600;
      margin-left: 8px;
    }
  `]
})
export class AppComponent {
  title = 'Control de Gastos';
  isAuthenticated = false;
  isAdmin = false;
  userName = '';
  isAdminRoute = false;

  constructor(
    public authService: AuthService,
    private router: Router
  ) {
    // Suscribirse a cambios de autenticación
    this.authService.currentUser$.subscribe(user => {
      this.isAuthenticated = !!user;
      this.userName = user?.nombreCompleto || '';
      this.isAdmin = this.authService.isAdmin();
    });

    // Detectar si estamos en una ruta de administración
    this.router.events.subscribe(() => {
      this.isAdminRoute = this.router.url.startsWith('/admin');
    });
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
