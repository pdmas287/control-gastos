using ControlGastos.API.Data;
using ControlGastos.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ControlGastos.API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _context;

        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioAdminDto>> GetAllUsuariosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Select(u => new UsuarioAdminDto
                {
                    UsuarioId = u.UsuarioId,
                    NombreUsuario = u.NombreUsuario,
                    Email = u.Email,
                    NombreCompleto = u.NombreCompleto,
                    Rol = u.Rol!.Nombre,
                    Activo = u.Activo,
                    FechaCreacion = u.FechaCreacion,
                    FechaModificacion = u.FechaModificacion,
                    UltimoAcceso = u.UltimoAcceso
                })
                .OrderByDescending(u => u.FechaCreacion)
                .ToListAsync();
        }

        public async Task<UsuarioAdminDto?> GetUsuarioByIdAsync(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null) return null;

            return new UsuarioAdminDto
            {
                UsuarioId = usuario.UsuarioId,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto,
                Rol = usuario.Rol!.Nombre,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion,
                FechaModificacion = usuario.FechaModificacion,
                UltimoAcceso = usuario.UltimoAcceso
            };
        }

        public async Task<UsuarioAdminDto?> UpdateUsuarioAsync(int id, UsuarioUpdateAdminDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null) return null;

            // Actualizar campos si se proporcionan
            if (!string.IsNullOrEmpty(dto.NombreUsuario))
            {
                // Verificar que el nombre de usuario no esté en uso
                var existeUsuario = await _context.Usuarios
                    .AnyAsync(u => u.NombreUsuario == dto.NombreUsuario && u.UsuarioId != id);

                if (existeUsuario)
                    throw new InvalidOperationException("El nombre de usuario ya está en uso");

                usuario.NombreUsuario = dto.NombreUsuario;
            }

            if (!string.IsNullOrEmpty(dto.Email))
            {
                // Verificar que el email no esté en uso
                var existeEmail = await _context.Usuarios
                    .AnyAsync(u => u.Email == dto.Email && u.UsuarioId != id);

                if (existeEmail)
                    throw new InvalidOperationException("El email ya está en uso");

                usuario.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.NombreCompleto))
                usuario.NombreCompleto = dto.NombreCompleto;

            if (!string.IsNullOrEmpty(dto.Rol))
            {
                var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == dto.Rol);
                if (rol != null)
                    usuario.RolId = rol.RolId;
            }

            if (dto.Activo.HasValue)
                usuario.Activo = dto.Activo.Value;

            usuario.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new UsuarioAdminDto
            {
                UsuarioId = usuario.UsuarioId,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto,
                Rol = usuario.Rol!.Nombre,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion,
                FechaModificacion = usuario.FechaModificacion,
                UltimoAcceso = usuario.UltimoAcceso
            };
        }

        public async Task<bool> ActivarUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            usuario.Activo = true;
            usuario.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DesactivarUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            usuario.Activo = false;
            usuario.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CambiarRolAsync(int id, string nuevoRol)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == nuevoRol);
            if (rol == null) return false;

            usuario.RolId = rol.RolId;
            usuario.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EstadisticasUsuariosDto> GetEstadisticasAsync()
        {
            var totalUsuarios = await _context.Usuarios.CountAsync();
            var usuariosActivos = await _context.Usuarios.CountAsync(u => u.Activo);
            var usuariosInactivos = totalUsuarios - usuariosActivos;

            var administradores = await _context.Usuarios
                .Include(u => u.Rol)
                .CountAsync(u => u.Rol!.Nombre == "Admin");

            var usuariosNormales = await _context.Usuarios
                .Include(u => u.Rol)
                .CountAsync(u => u.Rol!.Nombre == "Usuario");

            return new EstadisticasUsuariosDto
            {
                TotalUsuarios = totalUsuarios,
                UsuariosActivos = usuariosActivos,
                UsuariosInactivos = usuariosInactivos,
                Administradores = administradores,
                UsuariosNormales = usuariosNormales
            };
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            // Verificar que no sea el último administrador
            var esAdmin = await _context.Usuarios
                .Include(u => u.Rol)
                .AnyAsync(u => u.UsuarioId == id && u.Rol!.Nombre == "Admin");

            if (esAdmin)
            {
                var cantidadAdmins = await _context.Usuarios
                    .Include(u => u.Rol)
                    .CountAsync(u => u.Rol!.Nombre == "Admin" && u.Activo);

                if (cantidadAdmins <= 1)
                    throw new InvalidOperationException("No se puede eliminar el último administrador activo del sistema");
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
