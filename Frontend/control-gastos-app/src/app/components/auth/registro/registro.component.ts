import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { RegistroUsuario } from '../../../models/auth.model';

@Component({
  selector: 'app-registro',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './registro.component.html',
  styleUrls: ['./registro.component.css']
})
export class RegistroComponent implements OnInit {
  registroData: RegistroUsuario = {
    nombreUsuario: '',
    email: '',
    password: '',
    nombreCompleto: ''
  };

  confirmPassword = '';
  error = '';
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Si ya está autenticado, redirigir
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/home']);
    }
  }

  onSubmit(): void {
    // Validaciones
    if (!this.registroData.nombreUsuario || !this.registroData.email ||
        !this.registroData.password || !this.registroData.nombreCompleto) {
      this.error = 'Por favor complete todos los campos';
      return;
    }

    if (this.registroData.password !== this.confirmPassword) {
      this.error = 'Las contraseñas no coinciden';
      return;
    }

    if (this.registroData.password.length < 6) {
      this.error = 'La contraseña debe tener al menos 6 caracteres';
      return;
    }

    this.error = '';
    this.loading = true;

    this.authService.registro(this.registroData).subscribe({
      next: (response) => {
        console.log('Registro exitoso:', response);
        this.router.navigate(['/home']);
      },
      error: (err) => {
        console.error('Error en registro:', err);
        this.error = err.error?.message || 'Error al registrar el usuario';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}
