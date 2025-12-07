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
        <div class="navbar-container">
          <div class="navbar-header">
            <div class="navbar-brand">
              <h1>Control de Gastos</h1>
            </div>
            <button class="mobile-menu-btn" (click)="toggleMobileMenu()">
              <span></span>
              <span></span>
              <span></span>
            </button>
          </div>
          <div class="navbar-content" [class.mobile-open]="mobileMenuOpen">
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
      padding: 15px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .navbar-container {
      max-width: 1200px;
      margin: 0 auto;
    }

    .navbar-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .navbar-brand h1 {
      margin: 0;
      font-size: 20px;
    }

    .mobile-menu-btn {
      display: none;
      background: none;
      border: none;
      cursor: pointer;
      padding: 10px;
    }

    .mobile-menu-btn span {
      display: block;
      width: 25px;
      height: 3px;
      background-color: white;
      margin: 5px 0;
      transition: 0.3s;
    }

    .navbar-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-top: 15px;
    }

    .nav-menu {
      list-style: none;
      display: flex;
      gap: 20px;
      margin: 0;
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

    /* ============================================ */
    /* Responsive Design - Mobile */
    /* ============================================ */

    @media (max-width: 768px) {
      .mobile-menu-btn {
        display: block;
      }

      .navbar-content {
        display: none;
        flex-direction: column;
        align-items: flex-start;
        margin-top: 0;
      }

      .navbar-content.mobile-open {
        display: flex;
        margin-top: 15px;
      }

      .nav-menu {
        flex-direction: column;
        width: 100%;
        gap: 0;
      }

      .nav-item {
        width: 100%;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
      }

      .nav-item a,
      .nav-item span {
        padding: 15px;
        width: 100%;
      }

      .dropdown-menu {
        position: static;
        display: none;
        width: 100%;
        box-shadow: none;
        background-color: #1a252f;
        padding-left: 20px;
      }

      .nav-item.dropdown:hover .dropdown-menu {
        display: none;
      }

      .nav-item.dropdown.active .dropdown-menu {
        display: block;
      }

      .user-menu {
        flex-direction: column;
        align-items: flex-start;
        width: 100%;
        margin-left: 0;
        padding: 15px;
        border-top: 1px solid rgba(255, 255, 255, 0.1);
      }

      .user-name {
        margin-bottom: 10px;
      }

      .logout-btn {
        width: 100%;
      }

      .navbar-brand h1 {
        font-size: 18px;
      }

      .main-content {
        padding: 15px 0;
      }
    }
  `]
})
export class AppComponent {
  title = 'Control de Gastos';
  isAuthenticated = false;
  isAdmin = false;
  userName = '';
  isAdminRoute = false;
  mobileMenuOpen = false;

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
      this.mobileMenuOpen = false; // Cerrar menú al cambiar de ruta
    });
  }

  toggleMobileMenu(): void {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
