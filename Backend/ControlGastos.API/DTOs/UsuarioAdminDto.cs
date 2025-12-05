namespace ControlGastos.API.DTOs
{
    // DTO para mostrar información de usuario (admin)
    public class UsuarioAdminDto
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }

    // DTO para actualizar usuario (admin)
    public class UsuarioUpdateAdminDto
    {
        public string? NombreUsuario { get; set; }
        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Rol { get; set; }
        public bool? Activo { get; set; }
    }

    // DTO para cambiar rol
    public class CambiarRolDto
    {
        public string NuevoRol { get; set; } = string.Empty;
    }

    // DTO para estadísticas de usuarios
    public class EstadisticasUsuariosDto
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosInactivos { get; set; }
        public int Administradores { get; set; }
        public int UsuariosNormales { get; set; }
    }
}
