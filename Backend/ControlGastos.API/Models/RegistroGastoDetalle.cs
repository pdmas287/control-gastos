using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("registrogastodetalle")]
    public class RegistroGastoDetalle
    {
        [Key]
        [Column("registroGastoDetalleId")]

        public int RegistroGastoDetalleId { get; set; }

        [Required]
        [Column("registroGastoId")]

        public int RegistroGastoId { get; set; }

        [Required]
        [Column("tipoGastoId")]

        public int TipoGastoId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Column("monto")]

        public decimal Monto { get; set; }

        [StringLength(300)]
        [Column("descripcion")]

        public string? Descripcion { get; set; }

        // Navegación
        [ForeignKey("RegistroGastoId")]
        public virtual RegistroGastoEncabezado? RegistroGasto { get; set; }

        [ForeignKey("TipoGastoId")]
        public virtual TipoGasto? TipoGasto { get; set; }
    }
}
