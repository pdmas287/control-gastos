using System.ComponentModel.DataAnnotations;
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
        public virtual ICollection<TipoGasto> TipoGastos { get; set; } = new List<TipoGasto>();
        public virtual ICollection<FondoMonetario> FondosMonetarios { get; set; } = new List<FondoMonetario>();
        public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
        public virtual ICollection<RegistroGastoEncabezado> RegistrosGasto { get; set; } = new List<RegistroGastoEncabezado>();
        public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();
    }
}
