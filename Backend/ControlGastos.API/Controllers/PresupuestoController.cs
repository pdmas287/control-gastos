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
    public class PresupuestoController : ControllerBase
    {
        private readonly IPresupuestoService _service;

        public PresupuestoController(IPresupuestoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PresupuestoDto>>> GetAll([FromQuery] List<int>? usuariosIds = null)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();

                if (esAdmin && usuariosIds != null && usuariosIds.Count > 0)
                {
                    var presupuestos = await _service.GetByUsuariosAsync(usuariosIds);
                    return Ok(presupuestos);
                }

                var presupuestosNormal = await _service.GetAllAsync(usuarioId, esAdmin);
                return Ok(presupuestosNormal);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PresupuestoDto>> GetById(int id)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var presupuesto = await _service.GetByIdAsync(id, usuarioId, esAdmin);
                if (presupuesto == null)
                    return NotFound();

                return Ok(presupuesto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("mes/{mes}/anio/{anio}")]
        public async Task<ActionResult<PresupuestosPorMesDto>> GetPresupuestosPorMes(int mes, int anio, [FromQuery] List<int>? usuariosIds = null)
        {
            try
            {
                if (mes < 1 || mes > 12)
                    return BadRequest("El mes debe estar entre 1 y 12");

                if (anio < 2000 || anio > 2100)
                    return BadRequest("El año debe estar entre 2000 y 2100");

                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();

                if (esAdmin && usuariosIds != null && usuariosIds.Count > 0)
                {
                    var presupuestos = await _service.GetPresupuestosPorMesYUsuariosAsync(mes, anio, usuariosIds);
                    return Ok(presupuestos);
                }

                var presupuestosNormal = await _service.GetPresupuestosPorMesAsync(mes, anio, usuarioId, esAdmin);
                return Ok(presupuestosNormal);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<ActionResult<PresupuestoDto>> Create([FromBody] PresupuestoCreateDto dto)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var presupuesto = await _service.CreateAsync(dto, usuarioId);
                return CreatedAtAction(nameof(GetById), new { id = presupuesto.PresupuestoId }, presupuesto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PresupuestoDto>> Update(int id, [FromBody] PresupuestoUpdateDto dto)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var presupuesto = await _service.UpdateAsync(id, dto, usuarioId, esAdmin);
                if (presupuesto == null)
                    return NotFound();

                return Ok(presupuesto);
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
