#!/usr/bin/env python3
"""Script para agregar métodos GetByUsuariosAsync a los servicios backend."""

# Agregar GetByUsuariosAsync a RegistroGastoService
registro_gasto_method = '''
        public async Task<IEnumerable<RegistroGastoDto>> GetByUsuariosAsync(List<int> usuariosIds)
        {
            return await _context.RegistrosGastoEncabezado
                .Include(r => r.FondoMonetario)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.TipoGasto)
                .Include(r => r.Usuario)
                .Where(r => usuariosIds.Contains(r.UsuarioId ?? 0))
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
                    UsuarioId = r.UsuarioId ?? 0,
                    NombreUsuario = r.Usuario != null ? r.Usuario.NombreCompleto : null,
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
'''

print("Método para RegistroGastoService:")
print(registro_gasto_method)
