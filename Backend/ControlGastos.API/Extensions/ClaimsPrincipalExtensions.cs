using System.Security.Claims;

namespace ControlGastos.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Obtiene el ID del usuario autenticado desde los claims del token JWT
        /// </summary>
        public static int GetUsuarioId(this ClaimsPrincipal user)
        {
            var usuarioIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }
            return usuarioId;
        }

        /// <summary>
        /// Obtiene el rol del usuario autenticado desde los claims del token JWT
        /// </summary>
        public static string GetUserRole(this ClaimsPrincipal user)
        {
            var rolClaim = user.FindFirst(ClaimTypes.Role);
            if (rolClaim == null || string.IsNullOrEmpty(rolClaim.Value))
            {
                throw new UnauthorizedAccessException("Rol de usuario no encontrado");
            }
            return rolClaim.Value;
        }

        /// <summary>
        /// Verifica si el usuario autenticado es un administrador
        /// </summary>
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            try
            {
                var rol = user.GetUserRole();
                return rol.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si el usuario autenticado es un usuario normal (no admin)
        /// </summary>
        public static bool IsUsuario(this ClaimsPrincipal user)
        {
            try
            {
                var rol = user.GetUserRole();
                return rol.Equals("Usuario", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
