// Interfaz para el registro de nuevos usuarios
export interface RegistroUsuario {
  nombreUsuario: string;
  email: string;
  password: string;
  nombreCompleto: string;
}

// Interfaz para el login de usuarios
export interface Login {
  nombreUsuarioOEmail: string;
  password: string;
}

// Interfaz para la respuesta de autenticación (registro y login)
export interface AuthResponse {
  usuarioId: number;
  nombreUsuario: string;
  email: string;
  nombreCompleto: string;
  rol: string;
  token: string;
  fechaExpiracion: Date;
}

// Interfaz para información del usuario autenticado
export interface Usuario {
  usuarioId: number;
  nombreUsuario: string;
  email: string;
  nombreCompleto: string;
  rol: string;
  activo: boolean;
  fechaCreacion: Date;
  ultimoAcceso?: Date;
}

// Interfaz para cambiar contraseña
export interface CambiarPassword {
  passwordActual: string;
  nuevaPassword: string;
}
