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
    public class FondoMonetarioController : ControllerBase
    {
        private readonly IFondoMonetarioService _service;

        public FondoMonetarioController(IFondoMonetarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FondoMonetarioDto>>> GetAll([FromQuery] List<int>? usuariosIds = null)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();

                if (esAdmin && usuariosIds != null && usuariosIds.Count > 0)
                {
                    var fondos = await _service.GetByUsuariosAsync(usuariosIds);
                    return Ok(fondos);
                }

                var fondosNormal = await _service.GetAllAsync(usuarioId, esAdmin);
                return Ok(fondosNormal);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FondoMonetarioDto>> GetById(int id)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var fondo = await _service.GetByIdAsync(id, usuarioId, esAdmin);
                if (fondo == null)
                    return NotFound();

                return Ok(fondo);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<ActionResult<FondoMonetarioDto>> Create([FromBody] FondoMonetarioCreateDto dto)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var fondo = await _service.CreateAsync(dto, usuarioId);
                return CreatedAtAction(nameof(GetById), new { id = fondo.FondoMonetarioId }, fondo);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FondoMonetarioDto>> Update(int id, [FromBody] FondoMonetarioUpdateDto dto)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var fondo = await _service.UpdateAsync(id, dto, usuarioId, esAdmin);
                if (fondo == null)
                    return NotFound();

                return Ok(fondo);
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
