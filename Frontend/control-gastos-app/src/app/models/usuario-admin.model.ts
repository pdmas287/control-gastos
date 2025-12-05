// Modelo extendido de usuario para administración
export interface UsuarioAdmin {
  usuarioId: number;
  nombreUsuario: string;
  email: string;
  nombreCompleto: string;
  rol: string;
  activo: boolean;
  fechaCreacion: Date;
  fechaModificacion?: Date;
  ultimoAcceso?: Date;
}

// DTO para actualizar usuario (admin)
export interface UsuarioUpdateAdmin {
  nombreUsuario?: string;
  email?: string;
  nombreCompleto?: string;
  rol?: string;
  activo?: boolean;
}

// DTO para cambiar rol de usuario
export interface CambiarRolDto {
  usuarioId: number;
  nuevoRol: string;
}

// Estadísticas de usuarios
export interface EstadisticasUsuarios {
  totalUsuarios: number;
  usuariosActivos: number;
  usuariosInactivos: number;
  administradores: number;
  usuariosNormales: number;
}
