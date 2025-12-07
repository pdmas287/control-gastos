using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("presupuesto")]
    public class Presupuesto
    {
        [Key]
        [Column("presupuestoId")]

        public int PresupuestoId { get; set; }

        [Required]
        [Column("tipoGastoId")]

        public int TipoGastoId { get; set; }

        [Required]
        [Range(1, 12)]
        [Column("mes")]

        public int Mes { get; set; }

        [Required]
        [Column("anio")]

        public int Anio { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Column("montoPresupuestado")]

        public decimal MontoPresupuestado { get; set; }

        [Column("fechaCreacion")]


        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fechaModificacion")]


        public DateTime? FechaModificacion { get; set; }

        // Relación con Usuario
        [Column("usuarioId")]

        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Navegación
        [ForeignKey("TipoGastoId")]
        public virtual TipoGasto? TipoGasto { get; set; }
    }
}
