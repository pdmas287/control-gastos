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
                FechaCreacion = DateTime.UtcNow,
                UltimoAcceso = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Inicializar datos por defecto para el nuevo usuario
            await InicializarDatosUsuarioAsync(nuevoUsuario.UsuarioId);

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
                FechaExpiracion = DateTime.UtcNow.AddDays(7)
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
                FechaExpiracion = DateTime.UtcNow.AddDays(7)
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
            usuario.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ActualizarUltimoAccesoAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario != null)
            {
                usuario.UltimoAcceso = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private async Task InicializarDatosUsuarioAsync(int usuarioId)
        {
            var mesActual = DateTime.UtcNow.Month;
            var anioActual = DateTime.UtcNow.Year;

            // Insertar Tipos de Gasto por defecto
            var tiposGasto = new List<TipoGasto>
            {
                new TipoGasto { Codigo = "TG-001", Descripcion = "Alimentación", Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new TipoGasto { Codigo = "TG-002", Descripcion = "Transporte", Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new TipoGasto { Codigo = "TG-003", Descripcion = "Servicios Públicos", Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new TipoGasto { Codigo = "TG-004", Descripcion = "Entretenimiento", Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new TipoGasto { Codigo = "TG-005", Descripcion = "Salud", Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new TipoGasto { Codigo = "TG-006", Descripcion = "Educación", Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new TipoGasto { Codigo = "TG-007", Descripcion = "Vivienda", Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new TipoGasto { Codigo = "TG-008", Descripcion = "Vestimenta", Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new TipoGasto { Codigo = "TG-009", Descripcion = "Tecnología", Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new TipoGasto { Codigo = "TG-010", Descripcion = "Otros", Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId }
            };

            _context.TipoGastos.AddRange(tiposGasto);
            await _context.SaveChangesAsync();

            // Insertar Fondos Monetarios por defecto
            var fondos = new List<FondoMonetario>
            {
                new FondoMonetario { Nombre = "Cuenta Corriente Principal", TipoFondo = "Cuenta Bancaria", Descripcion = "Cuenta bancaria para gastos generales", SaldoActual = 0, Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new FondoMonetario { Nombre = "Cuenta de Ahorros", TipoFondo = "Cuenta Bancaria", Descripcion = "Cuenta de ahorros", SaldoActual = 0, Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new FondoMonetario { Nombre = "Caja Chica", TipoFondo = "Caja Menuda", Descripcion = "Efectivo disponible", SaldoActual = 0, Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId },
                new FondoMonetario { Nombre = "Efectivo Personal", TipoFondo = "Caja Menuda", Descripcion = "Efectivo en billetera", SaldoActual = 0, Activo = true, FechaCreacion = DateTime.UtcNow, UsuarioId = usuarioId }
            };

            _context.FondosMonetarios.AddRange(fondos);
            await _context.SaveChangesAsync();

            // Insertar Presupuestos para el mes actual
            var presupuestos = new List<Presupuesto>();
            var montosPresupuesto = new Dictionary<string, decimal>
            {
                { "TG-001", 1200000.00m },  // Alimentación
                { "TG-002", 400000.00m },   // Transporte
                { "TG-003", 500000.00m },   // Servicios Públicos
                { "TG-004", 300000.00m },   // Entretenimiento
                { "TG-005", 250000.00m },   // Salud
                { "TG-006", 200000.00m },   // Educación
                { "TG-007", 800000.00m },   // Vivienda
                { "TG-008", 200000.00m },   // Vestimenta
                { "TG-009", 150000.00m },   // Tecnología
                { "TG-010", 100000.00m }    // Otros
            };

            foreach (var tipoGasto in tiposGasto)
            {
                var monto = montosPresupuesto.ContainsKey(tipoGasto.Codigo)
                    ? montosPresupuesto[tipoGasto.Codigo]
                    : 100000.00m;

                presupuestos.Add(new Presupuesto
                {
                    TipoGastoId = tipoGasto.TipoGastoId,
                    Mes = mesActual,
                    Anio = anioActual,
                    MontoPresupuestado = monto,
                    FechaCreacion = DateTime.UtcNow,
                    UsuarioId = usuarioId
                });
            }

            _context.Presupuestos.AddRange(presupuestos);
            await _context.SaveChangesAsync();
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
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
