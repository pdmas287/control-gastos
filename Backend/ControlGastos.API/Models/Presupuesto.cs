using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("Presupuesto")]
    public class Presupuesto
    {
        [Key]
        public int PresupuestoId { get; set; }

        [Required]
        public int TipoGastoId { get; set; }

        [Required]
        [Range(1, 12)]
        public int Mes { get; set; }

        [Required]
        public int Anio { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoPresupuestado { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        // Relación con Usuario
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Navegación
        [ForeignKey("TipoGastoId")]
        public virtual TipoGasto? TipoGasto { get; set; }
    }
}
