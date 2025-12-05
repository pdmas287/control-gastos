using ControlGastos.API.Data;
using ControlGastos.API.DTOs;
using ControlGastos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlGastos.API.Services
{
    public class TipoGastoService : ITipoGastoService
    {
        private readonly ApplicationDbContext _context;

        public TipoGastoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoGastoDto>> GetAllAsync(int usuarioId, bool esAdmin)
        {
            var query = _context.TiposGasto
                .Include(t => t.Usuario)
                .Where(t => t.Activo);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(t => t.UsuarioId == usuarioId);
            }
            // Si ES admin, obtener TODOS los tipos de gasto de todos los usuarios

            return await query
                .Select(t => new TipoGastoDto
                {
                    TipoGastoId = t.TipoGastoId,
                    Codigo = t.Codigo,
                    Descripcion = t.Descripcion,
                    Activo = t.Activo,
                    UsuarioId = t.UsuarioId ?? 0,
                    NombreUsuario = t.Usuario != null ? t.Usuario.NombreCompleto : null
                })
                .ToListAsync();
        }

        public async Task<TipoGastoDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.TiposGasto
                .Include(t => t.Usuario)
                .Where(t => t.TipoGastoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(t => t.UsuarioId == usuarioId);
            }

            var tipoGasto = await query.FirstOrDefaultAsync();

            if (tipoGasto == null) return null;

            return new TipoGastoDto
            {
                TipoGastoId = tipoGasto.TipoGastoId,
                Codigo = tipoGasto.Codigo,
                Descripcion = tipoGasto.Descripcion,
                Activo = tipoGasto.Activo,
                UsuarioId = tipoGasto.UsuarioId ?? 0,
                NombreUsuario = tipoGasto.Usuario?.NombreCompleto
            };
        }

        public async Task<TipoGastoDto> CreateAsync(TipoGastoCreateDto dto, int usuarioId, bool esAdmin)
        {
            var codigo = await GetNextCodigoAsync(usuarioId, esAdmin);

            var tipoGasto = new TipoGasto
            {
                Codigo = codigo,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo,
                UsuarioId = usuarioId,
                FechaCreacion = DateTime.Now
            };

            _context.TiposGasto.Add(tipoGasto);
            await _context.SaveChangesAsync();

            // Recargar con usuario para obtener NombreCompleto
            await _context.Entry(tipoGasto).Reference(t => t.Usuario).LoadAsync();

            return new TipoGastoDto
            {
                TipoGastoId = tipoGasto.TipoGastoId,
                Codigo = tipoGasto.Codigo,
                Descripcion = tipoGasto.Descripcion,
                Activo = tipoGasto.Activo,
                UsuarioId = tipoGasto.UsuarioId ?? 0,
                NombreUsuario = tipoGasto.Usuario?.NombreCompleto
            };
        }

        public async Task<TipoGastoDto?> UpdateAsync(int id, TipoGastoUpdateDto dto, int usuarioId, bool esAdmin)
        {
            var query = _context.TiposGasto
                .Include(t => t.Usuario)
                .Where(t => t.TipoGastoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(t => t.UsuarioId == usuarioId);
            }

            var tipoGasto = await query.FirstOrDefaultAsync();

            if (tipoGasto == null) return null;

            tipoGasto.Descripcion = dto.Descripcion;
            tipoGasto.Activo = dto.Activo;
            tipoGasto.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return new TipoGastoDto
            {
                TipoGastoId = tipoGasto.TipoGastoId,
                Codigo = tipoGasto.Codigo,
                Descripcion = tipoGasto.Descripcion,
                Activo = tipoGasto.Activo,
                UsuarioId = tipoGasto.UsuarioId ?? 0,
                NombreUsuario = tipoGasto.Usuario?.NombreCompleto
            };
        }

        public async Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.TiposGasto.Where(t => t.TipoGastoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(t => t.UsuarioId == usuarioId);
            }

            var tipoGasto = await query.FirstOrDefaultAsync();

            if (tipoGasto == null) return false;

            // Verificar si tiene gastos registrados
            var tieneGastos = await _context.RegistrosGastoDetalle
                .AnyAsync(d => d.TipoGastoId == id);

            if (tieneGastos)
            {
                throw new InvalidOperationException(
                    $"No se puede eliminar el tipo de gasto '{tipoGasto.Descripcion}' porque tiene gastos registrados. " +
                    "Los tipos de gasto con historial de gastos deben mantenerse activos para preservar la integridad de los datos."
                );
            }

            // Verificar si tiene presupuestos asignados
            var tienePresupuestos = await _context.Presupuestos
                .AnyAsync(p => p.TipoGastoId == id && p.UsuarioId == usuarioId);

            if (tienePresupuestos)
            {
                throw new InvalidOperationException(
                    $"No se puede eliminar el tipo de gasto '{tipoGasto.Descripcion}' porque tiene presupuestos asignados. " +
                    "Elimine primero los presupuestos asociados o use esta categoría para gastos futuros."
                );
            }

            // Si no tiene gastos ni presupuestos, permitir inactivar
            tipoGasto.Activo = false;
            tipoGasto.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetNextCodigoAsync(int usuarioId, bool esAdmin)
        {
            // SIEMPRE calcular el siguiente código basándose en los tipos de gasto del usuario actual
            // Esto permite que cada usuario tenga su propio set de códigos independientes
            // La restricción UNIQUE en BD es (UsuarioId, Codigo), así que usuarios diferentes pueden tener el mismo código
            var todosTiposGasto = await _context.TiposGasto
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();

            if (!todosTiposGasto.Any())
            {
                return "TG-001";
            }

            // Extraer los números de los códigos existentes del usuario
            var numerosUsados = todosTiposGasto
                .Select(t => int.Parse(t.Codigo.Substring(3)))
                .OrderBy(n => n)
                .ToList();

            // Obtener el número más alto y sumar 1
            int siguienteNumero = numerosUsados.Max() + 1;

            return $"TG-{siguienteNumero:D3}";
        }
    }
}
