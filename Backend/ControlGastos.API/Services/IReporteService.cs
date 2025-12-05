using ControlGastos.API.DTOs;

namespace ControlGastos.API.Services
{
    public interface IReporteService
    {
        Task<IEnumerable<MovimientoDto>> GetMovimientosAsync(DateTime fechaInicio, DateTime fechaFin, int usuarioId, bool esAdmin);
        Task<IEnumerable<ComparativoPresupuestoDto>> GetComparativoPresupuestoAsync(DateTime fechaInicio, DateTime fechaFin, int usuarioId, bool esAdmin);
    }
}
