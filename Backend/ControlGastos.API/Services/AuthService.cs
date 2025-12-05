using ControlGastos.API.Data;
using ControlGastos.API.DTOs;
using ControlGastos.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ControlGastos.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> RegistrarUsuarioAsync(RegistroUsuarioDto registroDto)
        {
            // Verificar si el usuario ya existe
            var usuarioExistente = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == registroDto.NombreUsuario || u.Email == registroDto.Email);

            if (usuarioExistente)
            {
                return null;
            }

            // Obtener el rol "Usuario" por defecto
            var rolUsuario = await _context.Roles
                .FirstOrDefaultAsync(r => r.Nombre == "Usuario");

            if (rolUsuario == null)
            {
                throw new InvalidOperationException("El rol 'Usuario' no existe en la base de datos");
            }

            // Crear hash del password
            var passwordHash = HashPassword(registroDto.Password);

            // Crear nuevo usuario
            var nuevoUsuario = new Usuario
            {
                NombreUsuario = registroDto.NombreUsuario,
                Email = registroDto.Email,
                PasswordHash = passwordHash,
                NombreCompleto = registroDto.NombreCompleto,
                RolId = rolUsuario.RolId,
                Activo = true,
                FechaCreacion = DateTime.Now,
                UltimoAcceso = DateTime.Now
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Recargar el usuario con su rol para generar el token
            var usuarioConRol = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == nuevoUsuario.UsuarioId);

            // Generar token JWT
            var token = GenerarToken(usuarioConRol!);

            return new AuthResponseDto
            {
                UsuarioId = usuarioConRol!.UsuarioId,
                NombreUsuario = usuarioConRol.NombreUsuario,
                Email = usuarioConRol.Email,
                NombreCompleto = usuarioConRol.NombreCompleto,
                Rol = usuarioConRol.Rol!.Nombre,
                Token = token,
                FechaExpiracion = DateTime.Now.AddDays(7)
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Buscar usuario por nombre de usuario o email, incluyendo el rol
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u =>
                    (u.NombreUsuario == loginDto.NombreUsuarioOEmail || u.Email == loginDto.NombreUsuarioOEmail)
                    && u.Activo);

            if (usuario == null)
            {
                return null;
            }

            // Verificar password
            if (!VerificarPassword(loginDto.Password, usuario.PasswordHash))
            {
                return null;
            }

            // Actualizar último acceso
            await ActualizarUltimoAccesoAsync(usuario.UsuarioId);

            // Generar token JWT
            var token = GenerarToken(usuario);

            return new AuthResponseDto
            {
                UsuarioId = usuario.UsuarioId,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto,
                Rol = usuario.Rol!.Nombre,
                Token = token,
                FechaExpiracion = DateTime.Now.AddDays(7)
            };
        }

        public async Task<UsuarioDto?> ObtenerUsuarioActualAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId && u.Activo);

            if (usuario == null)
            {
                return null;
            }

            return new UsuarioDto
            {
                UsuarioId = usuario.UsuarioId,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto,
                Rol = usuario.Rol!.Nombre,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion,
                UltimoAcceso = usuario.UltimoAcceso
            };
        }

        public async Task<bool> CambiarPasswordAsync(int usuarioId, CambiarPasswordDto cambiarPasswordDto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId && u.Activo);

            if (usuario == null)
            {
                return false;
            }

            // Verificar password actual
            if (!VerificarPassword(cambiarPasswordDto.PasswordActual, usuario.PasswordHash))
            {
                return false;
            }

            // Actualizar password
            usuario.PasswordHash = HashPassword(cambiarPasswordDto.NuevaPassword);
            usuario.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ActualizarUltimoAccesoAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario != null)
            {
                usuario.UltimoAcceso = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // Métodos privados para hash y verificación de password
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerificarPassword(string password, string passwordHash)
        {
            var hash = HashPassword(password);
            return hash == passwordHash;
        }

        // Método para generar token JWT
        private string GenerarToken(Usuario usuario)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "ClaveSecretaMuySeguraParaControlDeGastos2024!";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ControlGastosAPI";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "ControlGastosApp";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol!.Nombre),
                new Claim("NombreCompleto", usuario.NombreCompleto)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
