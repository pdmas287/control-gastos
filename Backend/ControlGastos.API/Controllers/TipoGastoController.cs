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
    public class TipoGastoController : ControllerBase
    {
        private readonly ITipoGastoService _service;

        public TipoGastoController(ITipoGastoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoGastoDto>>> GetAll([FromQuery] List<int>? usuariosIds = null)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();

                // Si el admin proporciona filtro de usuarios, usarlo
                if (esAdmin && usuariosIds != null && usuariosIds.Count > 0)
                {
                    var tiposGasto = await _service.GetByUsuariosAsync(usuariosIds);
                    return Ok(tiposGasto);
                }

                // Comportamiento normal (todos para admin, solo del usuario para usuario normal)
                var tiposGastoNormal = await _service.GetAllAsync(usuarioId, esAdmin);
                return Ok(tiposGastoNormal);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TipoGastoDto>> GetById(int id)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var tipoGasto = await _service.GetByIdAsync(id, usuarioId, esAdmin);
                if (tipoGasto == null)
                    return NotFound();

                return Ok(tipoGasto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("siguiente-codigo")]
        public async Task<ActionResult<string>> GetSiguienteCodigo()
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var codigo = await _service.GetNextCodigoAsync(usuarioId, esAdmin);
                return Ok(new { codigo });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<ActionResult<TipoGastoDto>> Create([FromBody] TipoGastoCreateDto dto)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var tipoGasto = await _service.CreateAsync(dto, usuarioId, esAdmin);
                return CreatedAtAction(nameof(GetById), new { id = tipoGasto.TipoGastoId }, tipoGasto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TipoGastoDto>> Update(int id, [FromBody] TipoGastoUpdateDto dto)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var tipoGasto = await _service.UpdateAsync(id, dto, usuarioId, esAdmin);
                if (tipoGasto == null)
                    return NotFound();

                return Ok(tipoGasto);
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
