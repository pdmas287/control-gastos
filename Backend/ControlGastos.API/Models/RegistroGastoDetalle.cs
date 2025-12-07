using System.ComponentModel.DataAnnotations;
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
