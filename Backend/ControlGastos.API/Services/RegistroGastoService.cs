using ControlGastos.API.Data;
using ControlGastos.API.DTOs;
using ControlGastos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlGastos.API.Services
{
    public class RegistroGastoService : IRegistroGastoService
    {
        private readonly ApplicationDbContext _context;

        public RegistroGastoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RegistroGastoDto>> GetAllAsync(int usuarioId, bool esAdmin)
        {
            var query = _context.RegistrosGastoEncabezado.AsQueryable();

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(r => r.UsuarioId == usuarioId);
            }
            // Si ES admin, obtener TODOS los registros de todos los usuarios

            return await query
                .Include(r => r.FondoMonetario)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.TipoGasto)
                .OrderByDescending(r => r.Fecha)
                .Select(r => new RegistroGastoDto
                {
                    RegistroGastoId = r.RegistroGastoId,
                    Fecha = r.Fecha,
                    FondoMonetarioId = r.FondoMonetarioId,
                    FondoMonetarioNombre = r.FondoMonetario!.Nombre,
                    NombreComercio = r.NombreComercio,
                    TipoDocumento = r.TipoDocumento,
                    Observaciones = r.Observaciones,
                    MontoTotal = r.MontoTotal,
                    Detalles = r.Detalles.Select(d => new RegistroGastoDetalleDto
                    {
                        RegistroGastoDetalleId = d.RegistroGastoDetalleId,
                        TipoGastoId = d.TipoGastoId,
                        TipoGastoDescripcion = d.TipoGasto!.Descripcion,
                        Monto = d.Monto,
                        Descripcion = d.Descripcion
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<RegistroGastoDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.RegistrosGastoEncabezado
                .Include(r => r.FondoMonetario)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.TipoGasto)
                .Where(r => r.RegistroGastoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(r => r.UsuarioId == usuarioId);
            }

            var registro = await query.FirstOrDefaultAsync();

            if (registro == null) return null;

            return new RegistroGastoDto
            {
                RegistroGastoId = registro.RegistroGastoId,
                Fecha = registro.Fecha,
                FondoMonetarioId = registro.FondoMonetarioId,
                FondoMonetarioNombre = registro.FondoMonetario!.Nombre,
                NombreComercio = registro.NombreComercio,
                TipoDocumento = registro.TipoDocumento,
                Observaciones = registro.Observaciones,
                MontoTotal = registro.MontoTotal,
                Detalles = registro.Detalles.Select(d => new RegistroGastoDetalleDto
                {
                    RegistroGastoDetalleId = d.RegistroGastoDetalleId,
                    TipoGastoId = d.TipoGastoId,
                    TipoGastoDescripcion = d.TipoGasto!.Descripcion,
                    Monto = d.Monto,
                    Descripcion = d.Descripcion
                }).ToList()
            };
        }

        public async Task<(RegistroGastoDto registro, ValidacionPresupuestoDto validacion)> CreateAsync(RegistroGastoCreateDto dto, int usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar que el FondoMonetario pertenece al usuario
                var fondoExiste = await _context.FondosMonetarios
                    .AnyAsync(f => f.FondoMonetarioId == dto.FondoMonetarioId && f.UsuarioId == usuarioId);

                if (!fondoExiste)
                {
                    throw new InvalidOperationException("El fondo monetario no existe o no pertenece al usuario.");
                }

                // Validar que todos los TipoGasto pertenecen al usuario
                var tiposGastoIds = dto.Detalles.Select(d => d.TipoGastoId).Distinct().ToList();
                var tiposGastoValidos = await _context.TiposGasto
                    .Where(t => tiposGastoIds.Contains(t.TipoGastoId) && t.UsuarioId == usuarioId)
                    .Select(t => t.TipoGastoId)
                    .ToListAsync();

                if (tiposGastoValidos.Count != tiposGastoIds.Count)
                {
                    throw new InvalidOperationException("Uno o más tipos de gasto no existen o no pertenecen al usuario.");
                }

                var validacion = await ValidarPresupuestoAsync(dto, usuarioId);

                var montoTotal = dto.Detalles.Sum(d => d.Monto);

                var encabezado = new RegistroGastoEncabezado
                {
                    Fecha = dto.Fecha,
                    FondoMonetarioId = dto.FondoMonetarioId,
                    NombreComercio = dto.NombreComercio,
                    TipoDocumento = dto.TipoDocumento,
                    Observaciones = dto.Observaciones,
                    MontoTotal = montoTotal,
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.RegistrosGastoEncabezado.Add(encabezado);
                await _context.SaveChangesAsync();

                foreach (var detalleDto in dto.Detalles)
                {
                    var detalle = new RegistroGastoDetalle
                    {
                        RegistroGastoId = encabezado.RegistroGastoId,
                        TipoGastoId = detalleDto.TipoGastoId,
                        Monto = detalleDto.Monto,
                        Descripcion = detalleDto.Descripcion
                    };
                    _context.RegistrosGastoDetalle.Add(detalle);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var registroCreado = await GetByIdAsync(encabezado.RegistroGastoId, usuarioId, false);
                return (registroCreado!, validacion);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.RegistrosGastoEncabezado.Where(r => r.RegistroGastoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(r => r.UsuarioId == usuarioId);
            }

            var registro = await query.FirstOrDefaultAsync();

            if (registro == null) return false;

            _context.RegistrosGastoEncabezado.Remove(registro);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ValidacionPresupuestoDto> ValidarPresupuestoAsync(RegistroGastoCreateDto dto, int usuarioId)
        {
            var mes = dto.Fecha.Month;
            var anio = dto.Fecha.Year;
            var validacion = new ValidacionPresupuestoDto { HaySobregiro = false };

            var gastosPorTipo = dto.Detalles.GroupBy(d => d.TipoGastoId);

            foreach (var grupo in gastosPorTipo)
            {
                var tipoGastoId = grupo.Key;
                var montoGasto = grupo.Sum(d => d.Monto);

                var presupuesto = await _context.Presupuestos
                    .Include(p => p.TipoGasto)
                    .FirstOrDefaultAsync(p => p.TipoGastoId == tipoGastoId
                        && p.Mes == mes
                        && p.Anio == anio
                        && p.UsuarioId == usuarioId);

                if (presupuesto == null) continue;

                var totalEjecutado = await _context.RegistrosGastoDetalle
                    .Where(d => d.TipoGastoId == tipoGastoId)
                    .Where(d => _context.RegistrosGastoEncabezado
                        .Any(e => e.RegistroGastoId == d.RegistroGastoId
                            && e.Fecha.Month == mes
                            && e.Fecha.Year == anio
                            && e.UsuarioId == usuarioId))
                    .SumAsync(d => (decimal?)d.Monto) ?? 0;

                var nuevoTotal = totalEjecutado + montoGasto;

                if (nuevoTotal > presupuesto.MontoPresupuestado)
                {
                    validacion.HaySobregiro = true;
                    validacion.Sobregiros.Add(new SobregiroDetalleDto
                    {
                        TipoGastoId = tipoGastoId,
                        TipoGastoDescripcion = presupuesto.TipoGasto!.Descripcion,
                        MontoPresupuestado = presupuesto.MontoPresupuestado,
                        MontoEjecutado = nuevoTotal,
                        MontoSobregiro = nuevoTotal - presupuesto.MontoPresupuestado
                    });
                }
            }

            return validacion;
        }
    }
}
