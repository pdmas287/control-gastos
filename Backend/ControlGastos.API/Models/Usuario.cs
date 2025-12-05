using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string NombreCompleto { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        public DateTime? UltimoAcceso { get; set; }

        [Required]
        public int RolId { get; set; }

        // Relaciones
        [ForeignKey("RolId")]
        public virtual Rol? Rol { get; set; }

        public virtual ICollection<TipoGasto>? TipoGastos { get; set; }
        public virtual ICollection<FondoMonetario>? FondosMonetarios { get; set; }
        public virtual ICollection<Presupuesto>? Presupuestos { get; set; }
        public virtual ICollection<RegistroGastoEncabezado>? RegistrosGasto { get; set; }
        public virtual ICollection<Deposito>? Depositos { get; set; }
    }
}
