using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("RegistroGastoEncabezado")]
    public class RegistroGastoEncabezado
    {
        [Key]
        public int RegistroGastoId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; }

        [Required]
        public int FondoMonetarioId { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreComercio { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TipoDocumento { get; set; } = string.Empty; // "Comprobante", "Factura", "Otro"

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoTotal { get; set; } = 0;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        // Relación con Usuario
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Navegación
        [ForeignKey("FondoMonetarioId")]
        public virtual FondoMonetario? FondoMonetario { get; set; }

        public virtual ICollection<RegistroGastoDetalle> Detalles { get; set; } = new List<RegistroGastoDetalle>();
    }
}
