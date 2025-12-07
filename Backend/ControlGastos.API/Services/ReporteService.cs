using ControlGastos.API.Data;
using ControlGastos.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ControlGastos.API.Services
{
    public class ReporteService : IReporteService
    {
        private readonly ApplicationDbContext _context;

        public ReporteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovimientoDto>> GetMovimientosAsync(DateTime fechaInicio, DateTime fechaFin, int usuarioId, bool esAdmin)
        {
            var queryGastos = _context.RegistrosGastoEncabezado
                .Include(e => e.FondoMonetario)
                .Where(e => e.Fecha >= fechaInicio && e.Fecha <= fechaFin);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                queryGastos = queryGastos.Where(e => e.UsuarioId == usuarioId);
            }

            var gastos = await queryGastos
                .Select(e => new MovimientoDto
                {
                    Fecha = e.Fecha,
                    TipoMovimiento = "Gasto",
                    FondoMonetario = e.FondoMonetario!.Nombre,
                    Descripcion = e.NombreComercio,
                    Monto = e.MontoTotal,
                    TipoDocumento = e.TipoDocumento,
                    Observaciones = e.Observaciones
                })
                .ToListAsync();

            var queryDepositos = _context.Depositos
                .Include(d => d.FondoMonetario)
                .Where(d => d.Fecha >= fechaInicio && d.Fecha <= fechaFin);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                queryDepositos = queryDepositos.Where(d => d.UsuarioId == usuarioId);
            }

            var depositos = await queryDepositos
                .Select(d => new MovimientoDto
                {
                    Fecha = d.Fecha,
                    TipoMovimiento = "Depósito",
                    FondoMonetario = d.FondoMonetario!.Nombre,
                    Descripcion = d.Descripcion ?? "Depósito",
                    Monto = d.Monto,
                    TipoDocumento = null,
                    Observaciones = null
                })
                .ToListAsync();

            return gastos.Concat(depositos).OrderByDescending(m => m.Fecha);
        }

        public async Task<IEnumerable<MovimientoDto>> GetMovimientosByUsuariosAsync(DateTime fechaInicio, DateTime fechaFin, List<int> usuariosIds)
        {
            var gastos = await _context.RegistrosGastoEncabezado
                .Include(e => e.FondoMonetario)
                .Include(e => e.Usuario)
                .Where(e => e.Fecha >= fechaInicio && e.Fecha <= fechaFin)
                .Where(e => usuariosIds.Contains(e.UsuarioId ?? 0))
                .Select(e => new MovimientoDto
                {
                    Fecha = e.Fecha,
                    TipoMovimiento = "Gasto",
                    FondoMonetario = e.FondoMonetario!.Nombre,
                    Descripcion = e.NombreComercio,
                    Monto = e.MontoTotal,
                    TipoDocumento = e.TipoDocumento,
                    Observaciones = e.Observaciones,
                    UsuarioId = e.UsuarioId ?? 0,
                    NombreUsuario = e.Usuario != null ? e.Usuario.NombreCompleto : null
                })
                .ToListAsync();

            var depositos = await _context.Depositos
                .Include(d => d.FondoMonetario)
                .Include(d => d.Usuario)
                .Where(d => d.Fecha >= fechaInicio && d.Fecha <= fechaFin)
                .Where(d => usuariosIds.Contains(d.UsuarioId ?? 0))
                .Select(d => new MovimientoDto
                {
                    Fecha = d.Fecha,
                    TipoMovimiento = "Depósito",
                    FondoMonetario = d.FondoMonetario!.Nombre,
                    Descripcion = d.Descripcion ?? "Depósito",
                    Monto = d.Monto,
                    TipoDocumento = null,
                    Observaciones = null,
                    UsuarioId = d.UsuarioId ?? 0,
                    NombreUsuario = d.Usuario != null ? d.Usuario.NombreCompleto : null
                })
                .ToListAsync();

            return gastos.Concat(depositos).OrderByDescending(m => m.Fecha);
        }

        public async Task<IEnumerable<ComparativoPresupuestoDto>> GetComparativoPresupuestoAsync(DateTime fechaInicio, DateTime fechaFin, int usuarioId, bool esAdmin)
        {
            var queryTiposGasto = _context.TiposGasto.Where(t => t.Activo);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                queryTiposGasto = queryTiposGasto.Where(t => t.UsuarioId == usuarioId);
            }

            var tiposGasto = await queryTiposGasto.ToListAsync();
            var resultado = new List<ComparativoPresupuestoDto>();

            foreach (var tipo in tiposGasto)
            {
                var queryPresupuestos = _context.Presupuestos
                    .Where(p => p.TipoGastoId == tipo.TipoGastoId)
                    .Where(p => (p.Anio > fechaInicio.Year || (p.Anio == fechaInicio.Year && p.Mes >= fechaInicio.Month))
                             && (p.Anio < fechaFin.Year || (p.Anio == fechaFin.Year && p.Mes <= fechaFin.Month)));

                // Si NO es admin, filtrar solo por su usuarioId
                if (!esAdmin)
                {
                    queryPresupuestos = queryPresupuestos.Where(p => p.UsuarioId == usuarioId);
                }

                var presupuestoTotal = await queryPresupuestos.SumAsync(p => (decimal?)p.MontoPresupuestado) ?? 0;

                var queryEjecutado = _context.RegistrosGastoDetalle
                    .Where(d => d.TipoGastoId == tipo.TipoGastoId);

                // Si NO es admin, filtrar encabezados solo por su usuarioId
                if (esAdmin)
                {
                    queryEjecutado = queryEjecutado.Where(d => _context.RegistrosGastoEncabezado
                        .Any(e => e.RegistroGastoId == d.RegistroGastoId
                               && e.Fecha >= fechaInicio
                               && e.Fecha <= fechaFin));
                }
                else
                {
                    queryEjecutado = queryEjecutado.Where(d => _context.RegistrosGastoEncabezado
                        .Any(e => e.RegistroGastoId == d.RegistroGastoId
                               && e.Fecha >= fechaInicio
                               && e.Fecha <= fechaFin
                               && e.UsuarioId == usuarioId));
                }

                var ejecutadoTotal = await queryEjecutado.SumAsync(d => (decimal?)d.Monto) ?? 0;

                var diferencia = presupuestoTotal - ejecutadoTotal;

                // Si hay presupuesto, calcular porcentaje normal
                // Si no hay presupuesto pero hay gastos, mostrar 999% (indica sobregiro crítico)
                // Si no hay ni presupuesto ni gastos, mostrar 0%
                decimal porcentaje;
                if (presupuestoTotal > 0)
                {
                    porcentaje = (ejecutadoTotal / presupuestoTotal) * 100;
                }
                else if (ejecutadoTotal > 0)
                {
                    porcentaje = 999; // Indica sobregiro sin presupuesto
                }
                else
                {
                    porcentaje = 0;
                }

                resultado.Add(new ComparativoPresupuestoDto
                {
                    TipoGasto = tipo.Descripcion,
                    MontoPresupuestado = presupuestoTotal,
                    MontoEjecutado = ejecutadoTotal,
                    Diferencia = diferencia,
                    PorcentajeEjecucion = porcentaje
                });
            }

            return resultado.OrderBy(r => r.TipoGasto);
        }

        public async Task<IEnumerable<ComparativoPresupuestoDto>> GetComparativoPresupuestoByUsuariosAsync(DateTime fechaInicio, DateTime fechaFin, List<int> usuariosIds)
        {
            // Obtener todos los tipos de gasto de los usuarios seleccionados
            var tiposGasto = await _context.TiposGasto
                .Where(t => t.Activo && usuariosIds.Contains(t.UsuarioId ?? 0))
                .GroupBy(t => t.Descripcion)
                .Select(g => g.First())
                .ToListAsync();

            var resultado = new List<ComparativoPresupuestoDto>();

            foreach (var tipo in tiposGasto)
            {
                // Presupuestos consolidados por tipo de gasto
                var presupuestoTotal = await _context.Presupuestos
                    .Where(p => p.TipoGasto!.Descripcion == tipo.Descripcion)
                    .Where(p => usuariosIds.Contains(p.UsuarioId ?? 0))
                    .Where(p => (p.Anio > fechaInicio.Year || (p.Anio == fechaInicio.Year && p.Mes >= fechaInicio.Month))
                             && (p.Anio < fechaFin.Year || (p.Anio == fechaFin.Year && p.Mes <= fechaFin.Month)))
                    .SumAsync(p => (decimal?)p.MontoPresupuestado) ?? 0;

                // Gastos ejecutados consolidados
                var ejecutadoTotal = await _context.RegistrosGastoDetalle
                    .Where(d => d.TipoGasto!.Descripcion == tipo.Descripcion)
                    .Where(d => _context.RegistrosGastoEncabezado
                        .Any(e => e.RegistroGastoId == d.RegistroGastoId
                               && e.Fecha >= fechaInicio
                               && e.Fecha <= fechaFin
                               && usuariosIds.Contains(e.UsuarioId ?? 0)))
                    .SumAsync(d => (decimal?)d.Monto) ?? 0;

                var diferencia = presupuestoTotal - ejecutadoTotal;

                decimal porcentaje;
                if (presupuestoTotal > 0)
                {
                    porcentaje = (ejecutadoTotal / presupuestoTotal) * 100;
                }
                else if (ejecutadoTotal > 0)
                {
                    porcentaje = 999;
                }
                else
                {
                    porcentaje = 0;
                }

                resultado.Add(new ComparativoPresupuestoDto
                {
                    TipoGasto = tipo.Descripcion,
                    MontoPresupuestado = presupuestoTotal,
                    MontoEjecutado = ejecutadoTotal,
                    Diferencia = diferencia,
                    PorcentajeEjecucion = porcentaje
                });
            }

            return resultado.OrderBy(r => r.TipoGasto);
        }
    }
}
