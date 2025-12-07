import os
import re

# Directorio de modelos
models_dir = r"Backend\ControlGastos.API\Models"

# Mapeo de nombres de tabla a minúsculas
table_mappings = {
    "TipoGasto": "tipogasto",
    "FondoMonetario": "fondomonetario",
    "Presupuesto": "presupuesto",
    "RegistroGastoEncabezado": "registrogastoencabezado",
    "RegistroGastoDetalle": "registrogastodetalle",
    "Deposito": "deposito",
    "Usuario": "usuario",
    "Rol": "rol"
}

def fix_model_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    original_content = content

    # Detectar el nombre de la tabla actual
    table_match = re.search(r'\[Table\("(\w+)"\)\]', content)
    if not table_match:
        print(f"No se encontró [Table] en {filepath}")
        return

    current_table = table_match.group(1)

    # Reemplazar nombre de tabla a minúsculas
    if current_table in table_mappings:
        lowercase_table = table_mappings[current_table]
        content = content.replace(f'[Table("{current_table}")]', f'[Table("{lowercase_table}")]')
        print(f"Tabla actualizada: {current_table} -> {lowercase_table}")

    # Encontrar todas las propiedades públicas
    property_pattern = r'(\s+)(public\s+(?:virtual\s+)?(?:int|string|bool|DateTime|decimal|double|float|ICollection<\w+>)(?:\?)?)\s+(\w+)\s*\{'

    properties = re.findall(property_pattern, content)

    for indent, prop_type, prop_name in properties:
        # Ignorar propiedades de navegación (ICollection)
        if 'ICollection' in prop_type or 'virtual' in prop_type:
            continue

        # Generar nombre de columna en minúsculas
        column_name = prop_name[0].lower() + prop_name[1:]

        # Buscar si ya tiene atributo [Column]
        column_attr_pattern = rf'(\[Column\("[^"]+"\)\]\s+)?{re.escape(indent)}{re.escape(prop_type)}\s+{re.escape(prop_name)}\s*\{{'

        if not re.search(rf'\[Column\("[^"]+"\)\]\s+{re.escape(indent)}{re.escape(prop_type)}\s+{re.escape(prop_name)}', content):
            # Agregar atributo [Column] si no existe
            pattern = rf'{re.escape(indent)}{re.escape(prop_type)}\s+{re.escape(prop_name)}\s*\{{'
            replacement = f'{indent}[Column("{column_name}")]\n{indent}{prop_type} {prop_name} {{'
            content = re.sub(pattern, replacement, content)
            print(f"  Agregado [Column(\"{column_name}\")] para {prop_name}")

    # Guardar solo si hubo cambios
    if content != original_content:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        print(f"[OK] Archivo actualizado: {filepath}\n")
    else:
        print(f"Sin cambios en: {filepath}\n")

# Procesar todos los archivos de modelos
for filename in os.listdir(models_dir):
    if filename.endswith('.cs'):
        filepath = os.path.join(models_dir, filename)
        print(f"\nProcesando: {filename}")
        fix_model_file(filepath)

print("\n[OK] Proceso completado")
