using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("registrogastoencabezado")]
    public class RegistroGastoEncabezado
    {
        [Key]
        [Column("registroGastoId")]

        public int RegistroGastoId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        [Column("fecha")]

        public DateTime Fecha { get; set; }

        [Required]
        [Column("fondoMonetarioId")]

        public int FondoMonetarioId { get; set; }

        [Required]
        [StringLength(200)]
        [Column("nombreComercio")]

        public string NombreComercio { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("tipoDocumento")]

        public string TipoDocumento { get; set; } = string.Empty; // "Comprobante", "Factura", "Otro"

        [StringLength(500)]
        [Column("observaciones")]

        public string? Observaciones { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Column("montoTotal")]

        public decimal MontoTotal { get; set; } = 0;

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
        [ForeignKey("FondoMonetarioId")]
        public virtual FondoMonetario? FondoMonetario { get; set; }

        public virtual ICollection<RegistroGastoDetalle> Detalles { get; set; } = new List<RegistroGastoDetalle>();
    }
}
