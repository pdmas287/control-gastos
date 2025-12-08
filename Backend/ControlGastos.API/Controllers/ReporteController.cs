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
            [FromQuery] DateTime fechaFin,
            [FromQuery] List<int>? usuariosIds = null)
        {
            try
            {
                // Convert to UTC to avoid PostgreSQL timezone issues
                fechaInicio = DateTime.SpecifyKind(fechaInicio, DateTimeKind.Utc);
                fechaFin = DateTime.SpecifyKind(fechaFin, DateTimeKind.Utc);

                if (fechaFin < fechaInicio)
                    return BadRequest("La fecha fin debe ser mayor o igual a la fecha inicio.");

                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();

                if (esAdmin && usuariosIds != null && usuariosIds.Count > 0)
                {
                    var movimientos = await _service.GetMovimientosByUsuariosAsync(fechaInicio, fechaFin, usuariosIds);
                    return Ok(movimientos);
                }

                var movimientosNormal = await _service.GetMovimientosAsync(fechaInicio, fechaFin, usuarioId, esAdmin);
                return Ok(movimientosNormal);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("comparativo-presupuesto")]
        public async Task<ActionResult<IEnumerable<ComparativoPresupuestoDto>>> GetComparativoPresupuesto(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            [FromQuery] List<int>? usuariosIds = null)
        {
            try
            {
                // Convert to UTC to avoid PostgreSQL timezone issues
                fechaInicio = DateTime.SpecifyKind(fechaInicio, DateTimeKind.Utc);
                fechaFin = DateTime.SpecifyKind(fechaFin, DateTimeKind.Utc);

                if (fechaFin < fechaInicio)
                    return BadRequest("La fecha fin debe ser mayor o igual a la fecha inicio.");

                var usuarioId = User.GetUsuarioId();
                var esAdmin = User.IsAdmin();

                if (esAdmin && usuariosIds != null && usuariosIds.Count > 0)
                {
                    var comparativo = await _service.GetComparativoPresupuestoByUsuariosAsync(fechaInicio, fechaFin, usuariosIds);
                    return Ok(comparativo);
                }

                var comparativoNormal = await _service.GetComparativoPresupuestoAsync(fechaInicio, fechaFin, usuarioId, esAdmin);
                return Ok(comparativoNormal);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
