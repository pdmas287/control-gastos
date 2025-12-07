using ControlGastos.API.Data;
using ControlGastos.API.DTOs;
using ControlGastos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlGastos.API.Services
{
    public class DepositoService : IDepositoService
    {
        private readonly ApplicationDbContext _context;

        public DepositoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepositoDto>> GetAllAsync(int usuarioId, bool esAdmin)
        {
            var query = _context.Depositos.Include(d => d.FondoMonetario).AsQueryable();

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(d => d.UsuarioId == usuarioId);
            }
            // Si ES admin, obtener TODOS los depósitos de todos los usuarios

            return await query
                .Include(d => d.Usuario)
                .OrderByDescending(d => d.Fecha)
                .Select(d => new DepositoDto
                {
                    DepositoId = d.DepositoId,
                    Fecha = d.Fecha,
                    FondoMonetarioId = d.FondoMonetarioId,
                    FondoMonetarioNombre = d.FondoMonetario!.Nombre,
                    Monto = d.Monto,
                    Descripcion = d.Descripcion,
                    UsuarioId = d.UsuarioId ?? 0,
                    NombreUsuario = d.Usuario != null ? d.Usuario.NombreCompleto : null
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DepositoDto>> GetByUsuariosAsync(List<int> usuariosIds)
        {
            return await _context.Depositos
                .Include(d => d.FondoMonetario)
                .Include(d => d.Usuario)
                .Where(d => usuariosIds.Contains(d.UsuarioId ?? 0))
                .OrderByDescending(d => d.Fecha)
                .Select(d => new DepositoDto
                {
                    DepositoId = d.DepositoId,
                    Fecha = d.Fecha,
                    FondoMonetarioId = d.FondoMonetarioId,
                    FondoMonetarioNombre = d.FondoMonetario!.Nombre,
                    Monto = d.Monto,
                    Descripcion = d.Descripcion,
                    UsuarioId = d.UsuarioId ?? 0,
                    NombreUsuario = d.Usuario != null ? d.Usuario.NombreCompleto : null
                })
                .ToListAsync();
        }

        public async Task<DepositoDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.Depositos
                .Include(d => d.FondoMonetario)
                .Where(d => d.DepositoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(d => d.UsuarioId == usuarioId);
            }

            var deposito = await query.FirstOrDefaultAsync();

            if (deposito == null) return null;

            return new DepositoDto
            {
                DepositoId = deposito.DepositoId,
                Fecha = deposito.Fecha,
                FondoMonetarioId = deposito.FondoMonetarioId,
                FondoMonetarioNombre = deposito.FondoMonetario!.Nombre,
                Monto = deposito.Monto,
                Descripcion = deposito.Descripcion
            };
        }

        public async Task<DepositoDto> CreateAsync(DepositoCreateDto dto, int usuarioId)
        {
            var deposito = new Deposito
            {
                Fecha = dto.Fecha,
                FondoMonetarioId = dto.FondoMonetarioId,
                Monto = dto.Monto,
                Descripcion = dto.Descripcion,
                UsuarioId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Depositos.Add(deposito);
            await _context.SaveChangesAsync();

            // Cargar el fondo monetario para devolverlo
            await _context.Entry(deposito)
                .Reference(d => d.FondoMonetario)
                .LoadAsync();

            return new DepositoDto
            {
                DepositoId = deposito.DepositoId,
                Fecha = deposito.Fecha,
                FondoMonetarioId = deposito.FondoMonetarioId,
                FondoMonetarioNombre = deposito.FondoMonetario!.Nombre,
                Monto = deposito.Monto,
                Descripcion = deposito.Descripcion
            };
        }

        public async Task<DepositoDto?> UpdateAsync(int id, DepositoUpdateDto dto, int usuarioId, bool esAdmin)
        {
            var query = _context.Depositos
                .Include(d => d.FondoMonetario)
                .Where(d => d.DepositoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(d => d.UsuarioId == usuarioId);
            }

            var deposito = await query.FirstOrDefaultAsync();

            if (deposito == null) return null;

            // Guardar el monto anterior y el fondo anterior para ajustar saldos
            var montoAnterior = deposito.Monto;
            var fondoAnteriorId = deposito.FondoMonetarioId;

            // Si cambió el fondo o el monto, necesitamos ajustar los saldos manualmente
            if (fondoAnteriorId != dto.FondoMonetarioId || montoAnterior != dto.Monto)
            {
                // Revertir el depósito del fondo anterior
                var fondoAnterior = await _context.FondosMonetarios
                    .FirstOrDefaultAsync(f => f.FondoMonetarioId == fondoAnteriorId && f.UsuarioId == usuarioId);
                if (fondoAnterior != null)
                {
                    fondoAnterior.SaldoActual -= montoAnterior;
                }

                // Aplicar el nuevo depósito al nuevo fondo
                var fondoNuevo = await _context.FondosMonetarios
                    .FirstOrDefaultAsync(f => f.FondoMonetarioId == dto.FondoMonetarioId && f.UsuarioId == usuarioId);
                if (fondoNuevo != null)
                {
                    fondoNuevo.SaldoActual += dto.Monto;
                }
            }

            deposito.Fecha = dto.Fecha;
            deposito.FondoMonetarioId = dto.FondoMonetarioId;
            deposito.Monto = dto.Monto;
            deposito.Descripcion = dto.Descripcion;
            deposito.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Recargar el fondo monetario actualizado
            await _context.Entry(deposito)
                .Reference(d => d.FondoMonetario)
                .LoadAsync();

            return new DepositoDto
            {
                DepositoId = deposito.DepositoId,
                Fecha = deposito.Fecha,
                FondoMonetarioId = deposito.FondoMonetarioId,
                FondoMonetarioNombre = deposito.FondoMonetario!.Nombre,
                Monto = deposito.Monto,
                Descripcion = deposito.Descripcion
            };
        }

        public async Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.Depositos.Where(d => d.DepositoId == id);

            // Si NO es admin, filtrar solo por su usuarioId
            if (!esAdmin)
            {
                query = query.Where(d => d.UsuarioId == usuarioId);
            }

            var deposito = await query.FirstOrDefaultAsync();
            if (deposito == null) return false;

            _context.Depositos.Remove(deposito);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
