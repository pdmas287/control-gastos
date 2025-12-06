using ControlGastos.API.Data;
using ControlGastos.API.DTOs;
using ControlGastos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlGastos.API.Services
{
    public class PresupuestoService : IPresupuestoService
    {
        private readonly ApplicationDbContext _context;

        public PresupuestoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PresupuestoDto>> GetAllAsync(int usuarioId, bool esAdmin)
        {
            var query = _context.Presupuestos.Include(p => p.TipoGasto).AsQueryable();

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(p => p.UsuarioId == usuarioId);
            }

            return await query
                .Select(p => new PresupuestoDto
                {
                    PresupuestoId = p.PresupuestoId,
                    TipoGastoId = p.TipoGastoId,
                    TipoGastoDescripcion = p.TipoGasto!.Descripcion,
                    Mes = p.Mes,
                    Anio = p.Anio,
                    MontoPresupuestado = p.MontoPresupuestado
                })
                .OrderByDescending(p => p.Anio)
                .ThenByDescending(p => p.Mes)
                .ToListAsync();
        }

        public async Task<PresupuestoDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.Presupuestos
                .Include(p => p.TipoGasto)
                .Where(p => p.PresupuestoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(p => p.UsuarioId == usuarioId);
            }

            var presupuesto = await query.FirstOrDefaultAsync();

            if (presupuesto == null) return null;

            return new PresupuestoDto
            {
                PresupuestoId = presupuesto.PresupuestoId,
                TipoGastoId = presupuesto.TipoGastoId,
                TipoGastoDescripcion = presupuesto.TipoGasto!.Descripcion,
                Mes = presupuesto.Mes,
                Anio = presupuesto.Anio,
                MontoPresupuestado = presupuesto.MontoPresupuestado
            };
        }

        public async Task<PresupuestosPorMesDto> GetPresupuestosPorMesAsync(int mes, int anio, int usuarioId, bool esAdmin)
        {
            var query = _context.Presupuestos
                .Include(p => p.TipoGasto)
                .Where(p => p.Mes == mes && p.Anio == anio)
                .Where(p => p.TipoGasto!.Activo); // Solo presupuestos de tipos de gasto activos

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(p => p.UsuarioId == usuarioId);
            }

            var presupuestos = await query
                .Select(p => new PresupuestoItemDto
                {
                    PresupuestoId = p.PresupuestoId,
                    TipoGastoId = p.TipoGastoId,
                    TipoGastoDescripcion = p.TipoGasto!.Descripcion,
                    MontoPresupuestado = p.MontoPresupuestado
                })
                .ToListAsync();

            return new PresupuestosPorMesDto
            {
                Mes = mes,
                Anio = anio,
                Items = presupuestos
            };
        }

        public async Task<PresupuestoDto> CreateAsync(PresupuestoCreateDto dto, int usuarioId)
        {
            // Verificar si ya existe un presupuesto para este tipo de gasto, mes y año
            var existente = await _context.Presupuestos
                .FirstOrDefaultAsync(p => p.TipoGastoId == dto.TipoGastoId
                    && p.Mes == dto.Mes
                    && p.Anio == dto.Anio
                    && p.UsuarioId == usuarioId);

            if (existente != null)
            {
                throw new InvalidOperationException(
                    $"Ya existe un presupuesto para este tipo de gasto en {dto.Mes}/{dto.Anio}"
                );
            }

            var presupuesto = new Presupuesto
            {
                TipoGastoId = dto.TipoGastoId,
                Mes = dto.Mes,
                Anio = dto.Anio,
                MontoPresupuestado = dto.MontoPresupuestado,
                UsuarioId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Presupuestos.Add(presupuesto);
            await _context.SaveChangesAsync();

            // Cargar el tipo de gasto para devolverlo
            await _context.Entry(presupuesto)
                .Reference(p => p.TipoGasto)
                .LoadAsync();

            return new PresupuestoDto
            {
                PresupuestoId = presupuesto.PresupuestoId,
                TipoGastoId = presupuesto.TipoGastoId,
                TipoGastoDescripcion = presupuesto.TipoGasto!.Descripcion,
                Mes = presupuesto.Mes,
                Anio = presupuesto.Anio,
                MontoPresupuestado = presupuesto.MontoPresupuestado
            };
        }

        public async Task<PresupuestoDto?> UpdateAsync(int id, PresupuestoUpdateDto dto, int usuarioId, bool esAdmin)
        {
            var query = _context.Presupuestos
                .Include(p => p.TipoGasto)
                .Where(p => p.PresupuestoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(p => p.UsuarioId == usuarioId);
            }

            var presupuesto = await query.FirstOrDefaultAsync();

            if (presupuesto == null) return null;

            presupuesto.MontoPresupuestado = dto.MontoPresupuestado;
            presupuesto.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PresupuestoDto
            {
                PresupuestoId = presupuesto.PresupuestoId,
                TipoGastoId = presupuesto.TipoGastoId,
                TipoGastoDescripcion = presupuesto.TipoGasto!.Descripcion,
                Mes = presupuesto.Mes,
                Anio = presupuesto.Anio,
                MontoPresupuestado = presupuesto.MontoPresupuestado
            };
        }

        public async Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.Presupuestos.Where(p => p.PresupuestoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(p => p.UsuarioId == usuarioId);
            }

            var presupuesto = await query.FirstOrDefaultAsync();

            if (presupuesto == null) return false;

            _context.Presupuestos.Remove(presupuesto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
