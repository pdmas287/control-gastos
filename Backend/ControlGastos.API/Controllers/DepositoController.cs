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
    public class DepositoController : ControllerBase
    {
        private readonly IDepositoService _service;

        public DepositoController(IDepositoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepositoDto>>> GetAll()
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var depositos = await _service.GetAllAsync(usuarioId, esAdmin);
                return Ok(depositos);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepositoDto>> GetById(int id)
        {
            try
            {
                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var deposito = await _service.GetByIdAsync(id, usuarioId, esAdmin);
                if (deposito == null)
                    return NotFound();

                return Ok(deposito);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<ActionResult<DepositoDto>> Create([FromBody] DepositoCreateDto dto)
        {
            try
            {
                if (dto.Monto <= 0)
                    return BadRequest("El monto debe ser mayor a cero");

                var usuarioId = User.GetUsuarioId();
                var deposito = await _service.CreateAsync(dto, usuarioId);
                return CreatedAtAction(nameof(GetById), new { id = deposito.DepositoId }, deposito);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DepositoDto>> Update(int id, [FromBody] DepositoUpdateDto dto)
        {
            try
            {
                if (dto.Monto <= 0)
                    return BadRequest("El monto debe ser mayor a cero");

                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var deposito = await _service.UpdateAsync(id, dto, usuarioId, esAdmin);
                if (deposito == null)
                    return NotFound();

                return Ok(deposito);
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
