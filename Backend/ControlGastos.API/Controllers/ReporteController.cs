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
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _service;

        public ReporteController(IReporteService service)
        {
            _service = service;
        }

        [HttpGet("movimientos")]
        public async Task<ActionResult<IEnumerable<MovimientoDto>>> GetMovimientos(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaFin < fechaInicio)
                    return BadRequest("La fecha fin debe ser mayor o igual a la fecha inicio.");

                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var movimientos = await _service.GetMovimientosAsync(fechaInicio, fechaFin, usuarioId, esAdmin);
                return Ok(movimientos);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("comparativo-presupuesto")]
        public async Task<ActionResult<IEnumerable<ComparativoPresupuestoDto>>> GetComparativoPresupuesto(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                if (fechaFin < fechaInicio)
                    return BadRequest("La fecha fin debe ser mayor o igual a la fecha inicio.");

                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();
                var comparativo = await _service.GetComparativoPresupuestoAsync(fechaInicio, fechaFin, usuarioId, esAdmin);
                return Ok(comparativo);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
