using ControlGastos.API.DTOs;
using ControlGastos.API.Extensions;
using ControlGastos.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlGastos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegistroGastoController : ControllerBase
    {
        private readonly IRegistroGastoService _service;

        public RegistroGastoController(IRegistroGastoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistroGastoDto>>> GetAll([FromQuery] List<int>? usuariosIds = null)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();

                if (esAdmin && usuariosIds != null && usuariosIds.Count > 0)
                {
                    var registros = await _service.GetByUsuariosAsync(usuariosIds);
                    return Ok(registros);
                }

                var registrosNormal = await _service.GetAllAsync(usuarioId, esAdmin);
                return Ok(registrosNormal);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegistroGastoDto>> GetById(int id)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var registro = await _service.GetByIdAsync(id, usuarioId, esAdmin);
                if (registro == null)
                    return NotFound();

                return Ok(registro);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("validar")]
        public async Task<ActionResult<ValidacionPresupuestoDto>> ValidarPresupuesto([FromBody] RegistroGastoCreateDto dto)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var validacion = await _service.ValidarPresupuestoAsync(dto, usuarioId);
                return Ok(validacion);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<ActionResult<RegistroGastoDto>> Create([FromBody] RegistroGastoCreateDto dto)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();

                if (dto.Detalles == null || !dto.Detalles.Any())
                    return BadRequest("No se puede guardar un registro de gastos sin detalles.");

                var (registro, validacion) = await _service.CreateAsync(dto, usuarioId);

                var response = new
                {
                    registro,
                    validacion
                };

                return CreatedAtAction(nameof(GetById), new { id = registro.RegistroGastoId }, response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var result = await _service.DeleteAsync(id, usuarioId, esAdmin);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
