using ControlGastos.API.DTOs;
using ControlGastos.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControlGastos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        [HttpPost("registro")]
        public async Task<ActionResult<AuthResponseDto>> Registro([FromBody] RegistroUsuarioDto registroDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _authService.RegistrarUsuarioAsync(registroDto);

            if (resultado == null)
            {
                return BadRequest(new { message = "El nombre de usuario o email ya está en uso" });
            }

            return Ok(resultado);
        }

        /// <summary>
        /// Inicia sesión con un usuario existente
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _authService.LoginAsync(loginDto);

            if (resultado == null)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            return Ok(resultado);
        }

        /// <summary>
        /// Obtiene la información del usuario autenticado
        /// </summary>
        [HttpGet("perfil")]
        [Authorize]
        public async Task<ActionResult<UsuarioDto>> ObtenerPerfil()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
            {
                return Unauthorized();
            }

            var usuario = await _authService.ObtenerUsuarioActualAsync(usuarioId);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(usuario);
        }

        /// <summary>
        /// Cambia la contraseña del usuario autenticado
        /// </summary>
        [HttpPut("cambiar-password")]
        [Authorize]
        public async Task<ActionResult> CambiarPassword([FromBody] CambiarPasswordDto cambiarPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
            {
                return Unauthorized();
            }

            var resultado = await _authService.CambiarPasswordAsync(usuarioId, cambiarPasswordDto);

            if (!resultado)
            {
                return BadRequest(new { message = "La contraseña actual es incorrecta" });
            }

            return Ok(new { message = "Contraseña actualizada exitosamente" });
        }

        /// <summary>
        /// Verifica si el token es válido (útil para el frontend)
        /// </summary>
        [HttpGet("verificar-token")]
        [Authorize]
        public ActionResult VerificarToken()
        {
            return Ok(new { valid = true });
        }
    }
}
