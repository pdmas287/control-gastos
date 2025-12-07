import os
import re

models_dir = r"Backend\ControlGastos.API\Models"

def clean_duplicate_columns(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    cleaned_lines = []
    i = 0
    while i < len(lines):
        line = lines[i]

        # Si encontramos un atributo [Column]
        if '[Column(' in line:
            # Verificar si hay otro [Column] en las siguientes líneas antes de la propiedad
            next_columns = []
            j = i + 1

            # Buscar más atributos [Column] antes de llegar a una línea que no sea atributo o espacio
            while j < len(lines):
                next_line = lines[j].strip()
                if '[Column(' in next_line:
                    next_columns.append((j, next_line))
                    j += 1
                elif next_line == '' or next_line.startswith('[') and '[Column(' not in next_line:
                    j += 1
                elif next_line.startswith('public '):
                    break
                else:
                    j += 1
                    if '[' not in next_line:
                        break

            # Si hay duplicados
            if next_columns:
                # Mantener solo el último [Column("nombre")] y descartar [Column(TypeName=...)]
                columns_with_name = [line] + [lines[idx] for idx, _ in next_columns]

                # Buscar el que tiene nombre de columna (no TypeName)
                final_column = None
                for col_line in columns_with_name:
                    if '[Column("' in col_line and 'TypeName' not in col_line:
                        final_column = col_line
                        break

                if final_column:
                    cleaned_lines.append(final_column)
                else:
                    # Si no hay uno con nombre, mantener el primero
                    cleaned_lines.append(line)

                # Saltar todas las líneas duplicadas
                i = j
                continue

        cleaned_lines.append(line)
        i += 1

    # Guardar archivo limpio
    with open(filepath, 'w', encoding='utf-8') as f:
        f.writelines(cleaned_lines)

    print(f"[OK] Limpiado: {filepath}")

# Procesar todos los modelos
for filename in os.listdir(models_dir):
    if filename.endswith('.cs'):
        filepath = os.path.join(models_dir, filename)
        print(f"\nProcesando: {filename}")
        clean_duplicate_columns(filepath)

print("\n[OK] Proceso completado")
