using ControlGastos.API.Data;
using ControlGastos.API.DTOs;
using ControlGastos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlGastos.API.Services
{
    public class FondoMonetarioService : IFondoMonetarioService
    {
        private readonly ApplicationDbContext _context;

        public FondoMonetarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FondoMonetarioDto>> GetAllAsync(int usuarioId, bool esAdmin)
        {
            var query = _context.FondosMonetarios.Where(f => f.Activo);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(f => f.UsuarioId == usuarioId);
            }
            // Si ES admin, obtener TODOS los fondos de todos los usuarios

            return await query
                .Select(f => new FondoMonetarioDto
                {
                    FondoMonetarioId = f.FondoMonetarioId,
                    Nombre = f.Nombre,
                    TipoFondo = f.TipoFondo,
                    Descripcion = f.Descripcion,
                    SaldoActual = f.SaldoActual,
                    Activo = f.Activo
                })
                .ToListAsync();
        }

        public async Task<FondoMonetarioDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.FondosMonetarios.Where(f => f.FondoMonetarioId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(f => f.UsuarioId == usuarioId);
            }

            var fondo = await query.FirstOrDefaultAsync();

            if (fondo == null) return null;

            return new FondoMonetarioDto
            {
                FondoMonetarioId = fondo.FondoMonetarioId,
                Nombre = fondo.Nombre,
                TipoFondo = fondo.TipoFondo,
                Descripcion = fondo.Descripcion,
                SaldoActual = fondo.SaldoActual,
                Activo = fondo.Activo
            };
        }

        public async Task<FondoMonetarioDto> CreateAsync(FondoMonetarioCreateDto dto, int usuarioId)
        {
            var fondo = new FondoMonetario
            {
                Nombre = dto.Nombre,
                TipoFondo = dto.TipoFondo,
                Descripcion = dto.Descripcion,
                SaldoActual = dto.SaldoActual,
                Activo = dto.Activo,
                UsuarioId = usuarioId,
                FechaCreacion = DateTime.Now
            };

            _context.FondosMonetarios.Add(fondo);
            await _context.SaveChangesAsync();

            return new FondoMonetarioDto
            {
                FondoMonetarioId = fondo.FondoMonetarioId,
                Nombre = fondo.Nombre,
                TipoFondo = fondo.TipoFondo,
                Descripcion = fondo.Descripcion,
                SaldoActual = fondo.SaldoActual,
                Activo = fondo.Activo
            };
        }

        public async Task<FondoMonetarioDto?> UpdateAsync(int id, FondoMonetarioUpdateDto dto, int usuarioId, bool esAdmin)
        {
            var query = _context.FondosMonetarios.Where(f => f.FondoMonetarioId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(f => f.UsuarioId == usuarioId);
            }

            var fondo = await query.FirstOrDefaultAsync();

            if (fondo == null) return null;

            fondo.Nombre = dto.Nombre;
            fondo.TipoFondo = dto.TipoFondo;
            fondo.Descripcion = dto.Descripcion;
            fondo.Activo = dto.Activo;
            fondo.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return new FondoMonetarioDto
            {
                FondoMonetarioId = fondo.FondoMonetarioId,
                Nombre = fondo.Nombre,
                TipoFondo = fondo.TipoFondo,
                Descripcion = fondo.Descripcion,
                SaldoActual = fondo.SaldoActual,
                Activo = fondo.Activo
            };
        }

        public async Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.FondosMonetarios.Where(f => f.FondoMonetarioId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(f => f.UsuarioId == usuarioId);
            }

            var fondo = await query.FirstOrDefaultAsync();

            if (fondo == null) return false;

            // Soft delete: solo marcar como inactivo
            fondo.Activo = false;
            fondo.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
