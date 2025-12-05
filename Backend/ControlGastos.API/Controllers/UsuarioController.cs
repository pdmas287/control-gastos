using ControlGastos.API.DTOs;
using ControlGastos.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlGastos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Solo administradores pueden acceder
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema (solo admin)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioAdminDto>>> GetAll()
        {
            try
            {
                var usuarios = await _service.GetAllUsuariosAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener usuarios", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un usuario por ID (solo admin)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioAdminDto>> GetById(int id)
        {
            try
            {
                var usuario = await _service.GetUsuarioByIdAsync(id);
                if (usuario == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un usuario (solo admin)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioAdminDto>> Update(int id, [FromBody] UsuarioUpdateAdminDto dto)
        {
            try
            {
                var usuario = await _service.UpdateUsuarioAsync(id, dto);
                if (usuario == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(usuario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Activa un usuario (solo admin)
        /// </summary>
        [HttpPut("{id}/activar")]
        public async Task<ActionResult> Activar(int id)
        {
            try
            {
                var result = await _service.ActivarUsuarioAsync(id);
                if (!result)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(new { message = "Usuario activado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al activar usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Desactiva un usuario (solo admin)
        /// </summary>
        [HttpPut("{id}/desactivar")]
        public async Task<ActionResult> Desactivar(int id)
        {
            try
            {
                var result = await _service.DesactivarUsuarioAsync(id);
                if (!result)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(new { message = "Usuario desactivado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al desactivar usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Cambia el rol de un usuario (solo admin)
        /// </summary>
        [HttpPut("{id}/cambiar-rol")]
        public async Task<ActionResult> CambiarRol(int id, [FromBody] CambiarRolDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.NuevoRol))
                    return BadRequest(new { message = "Debe especificar el nuevo rol" });

                if (dto.NuevoRol != "Admin" && dto.NuevoRol != "Usuario")
                    return BadRequest(new { message = "El rol debe ser 'Admin' o 'Usuario'" });

                var result = await _service.CambiarRolAsync(id, dto.NuevoRol);
                if (!result)
                    return NotFound(new { message = "Usuario o rol no encontrado" });

                return Ok(new { message = $"Rol cambiado a {dto.NuevoRol} exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cambiar rol", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene estadísticas de usuarios (solo admin)
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<EstadisticasUsuariosDto>> GetEstadisticas()
        {
            try
            {
                var estadisticas = await _service.GetEstadisticasAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener estadísticas", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un usuario (solo admin) - usar con precaución
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteUsuarioAsync(id);
                if (!result)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(new { message = "Usuario eliminado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar usuario", error = ex.Message });
            }
        }
    }
}
