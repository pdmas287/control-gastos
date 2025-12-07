import re

# Leer el archivo
with open(r'Backend\ControlGastos.API\Services\TipoGastoService.cs', 'r', encoding='utf-8') as f:
    content = f.read()

# Método a insertar
new_method = '''
        public async Task<IEnumerable<TipoGastoDto>> GetByUsuariosAsync(List<int> usuariosIds)
        {
            return await _context.TiposGasto
                .Include(t => t.Usuario)
                .Where(t => t.Activo && usuariosIds.Contains(t.UsuarioId ?? 0))
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
'''

# Buscar dónde insertar (después del método GetAllAsync)
pattern = r'(public async Task<IEnumerable<TipoGastoDto>> GetAllAsync.*?\n        \})\n'
match = re.search(pattern, content, re.DOTALL)

if match:
    # Insertar el nuevo método
    insert_pos = match.end()
    new_content = content[:insert_pos] + new_method + content[insert_pos:]

    # Escribir el archivo
    with open(r'Backend\ControlGastos.API\Services\TipoGastoService.cs', 'w', encoding='utf-8') as f:
        f.write(new_content)

    print("[OK] Método GetByUsuariosAsync agregado exitosamente")
else:
    print("[ERROR] No se pudo encontrar el método GetAllAsync")
