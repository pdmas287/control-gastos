using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("usuarioId")]

        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("nombreUsuario")]

        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        [Column("email")]

        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("passwordHash")]

        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("nombreCompleto")]

        public string NombreCompleto { get; set; } = string.Empty;

        [Column("activo")]


        public bool Activo { get; set; } = true;

        [Column("fechaCreacion")]


        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fechaModificacion")]


        public DateTime? FechaModificacion { get; set; }

        [Column("ultimoAcceso")]


        public DateTime? UltimoAcceso { get; set; }

        [Required]
        [Column("rolId")]

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
