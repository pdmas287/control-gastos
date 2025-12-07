import os
import re

models_dir = r"Backend\ControlGastos.API\Models"

# Contenido correcto para cada modelo
models_content = {
    "Presupuesto.cs": '''using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("presupuesto")]
    public class Presupuesto
    {
        [Key]
        [Column("presupuestoid")]
        public int PresupuestoId { get; set; }

        [Required]
        [Column("tipogastoid")]
        public int TipoGastoId { get; set; }

        [Required]
        [Range(1, 12)]
        [Column("mes")]
        public int Mes { get; set; }

        [Required]
        [Column("anio")]
        public int Anio { get; set; }

        [Required]
        [Column("montopresupuestado", TypeName = "decimal(18,2)")]
        public decimal MontoPresupuestado { get; set; }

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fechamodificacion")]
        public DateTime? FechaModificacion { get; set; }

        // Relación con Usuario
        [Column("usuarioid")]
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Navegación
        [ForeignKey("TipoGastoId")]
        public virtual TipoGasto? TipoGasto { get; set; }
    }
}
''',

    "Usuario.cs": '''using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("usuarioid")]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nombreusuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Column("passwordhash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("nombrecompleto")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fechamodificacion")]
        public DateTime? FechaModificacion { get; set; }

        [Column("ultimoacceso")]
        public DateTime? UltimoAcceso { get; set; }

        // Relación con Rol
        [Required]
        [Column("rolid")]
        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public virtual Rol? Rol { get; set; }

        // Navegación
        public virtual ICollection<TipoGasto> TiposGasto { get; set; } = new List<TipoGasto>();
        public virtual ICollection<FondoMonetario> FondosMonetarios { get; set; } = new List<FondoMonetario>();
        public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
        public virtual ICollection<RegistroGastoEncabezado> RegistrosGasto { get; set; } = new List<RegistroGastoEncabezado>();
        public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();
    }
}
''',

    "Rol.cs": '''using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("rol")]
    public class Rol
    {
        [Key]
        [Column("rolid")]
        public int RolId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
''',

    "FondoMonetario.cs": '''using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("fondomonetario")]
    public class FondoMonetario
    {
        [Key]
        [Column("fondomonetarioid")]
        public int FondoMonetarioId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("tipofondo")]
        public string TipoFondo { get; set; } = string.Empty;

        [StringLength(200)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("saldoactual", TypeName = "decimal(18,2)")]
        public decimal SaldoActual { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fechamodificacion")]
        public DateTime? FechaModificacion { get; set; }

        // Relación con Usuario
        [Column("usuarioid")]
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Navegación
        public virtual ICollection<RegistroGastoEncabezado> RegistrosGasto { get; set; } = new List<RegistroGastoEncabezado>();
        public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();
    }
}
''',

    "Deposito.cs": '''using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("deposito")]
    public class Deposito
    {
        [Key]
        [Column("depositoid")]
        public int DepositoId { get; set; }

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [Column("fondomonetarioid")]
        public int FondoMonetarioId { get; set; }

        [Required]
        [Column("monto", TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [StringLength(500)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fechamodificacion")]
        public DateTime? FechaModificacion { get; set; }

        // Relación con Usuario
        [Column("usuarioid")]
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Navegación
        [ForeignKey("FondoMonetarioId")]
        public virtual FondoMonetario? FondoMonetario { get; set; }
    }
}
''',

    "RegistroGastoEncabezado.cs": '''using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("registrogastoencabezado")]
    public class RegistroGastoEncabezado
    {
        [Key]
        [Column("registrogastoid")]
        public int RegistroGastoId { get; set; }

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [Column("fondomonetarioid")]
        public int FondoMonetarioId { get; set; }

        [StringLength(200)]
        [Column("nombrecomercio")]
        public string? NombreComercio { get; set; }

        [StringLength(50)]
        [Column("tipodocumento")]
        public string? TipoDocumento { get; set; }

        [StringLength(500)]
        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("montototal", TypeName = "decimal(18,2)")]
        public decimal MontoTotal { get; set; }

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fechamodificacion")]
        public DateTime? FechaModificacion { get; set; }

        // Relación con Usuario
        [Column("usuarioid")]
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Navegación
        [ForeignKey("FondoMonetarioId")]
        public virtual FondoMonetario? FondoMonetario { get; set; }

        public virtual ICollection<RegistroGastoDetalle> Detalles { get; set; } = new List<RegistroGastoDetalle>();
    }
}
''',

    "RegistroGastoDetalle.cs": '''using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("registrogastodetalle")]
    public class RegistroGastoDetalle
    {
        [Key]
        [Column("registrogastodetalleid")]
        public int RegistroGastoDetalleId { get; set; }

        [Required]
        [Column("registrogastoid")]
        public int RegistroGastoId { get; set; }

        [Required]
        [Column("tipogastoid")]
        public int TipoGastoId { get; set; }

        [Required]
        [Column("monto", TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [StringLength(500)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        // Navegación
        [ForeignKey("RegistroGastoId")]
        public virtual RegistroGastoEncabezado? RegistroGasto { get; set; }

        [ForeignKey("TipoGastoId")]
        public virtual TipoGasto? TipoGasto { get; set; }
    }
}
'''
}

# Escribir cada archivo
for filename, content in models_content.items():
    filepath = os.path.join(models_dir, filename)
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f"[OK] Corregido: {filename}")

print("\n[OK] Todos los modelos han sido corregidos")
