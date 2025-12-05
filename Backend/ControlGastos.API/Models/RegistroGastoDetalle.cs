using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("RegistroGastoDetalle")]
    public class RegistroGastoDetalle
    {
        [Key]
        public int RegistroGastoDetalleId { get; set; }

        [Required]
        public int RegistroGastoId { get; set; }

        [Required]
        public int TipoGastoId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [StringLength(300)]
        public string? Descripcion { get; set; }

        // Navegación
        [ForeignKey("RegistroGastoId")]
        public virtual RegistroGastoEncabezado? RegistroGasto { get; set; }

        [ForeignKey("TipoGastoId")]
        public virtual TipoGasto? TipoGasto { get; set; }
    }
}
