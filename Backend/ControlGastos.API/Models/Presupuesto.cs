using System.ComponentModel.DataAnnotations;
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
