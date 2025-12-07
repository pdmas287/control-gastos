using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("registrogastoencabezado")]
    public class RegistroGastoEncabezado
    {
        [Key]
        [Column("registrogastoid")]
        public int RegistroGastoId { get; set; }

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [Column("fondomonetarioid")]
        public int FondoMonetarioId { get; set; }

        [StringLength(200)]
        [Column("nombrecomercio")]
        public string? NombreComercio { get; set; }

        [StringLength(50)]
        [Column("tipodocumento")]
        public string? TipoDocumento { get; set; }

        [StringLength(500)]
        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("montototal", TypeName = "decimal(18,2)")]
        public decimal MontoTotal { get; set; }

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
        [ForeignKey("FondoMonetarioId")]
        public virtual FondoMonetario? FondoMonetario { get; set; }

        public virtual ICollection<RegistroGastoDetalle> Detalles { get; set; } = new List<RegistroGastoDetalle>();
    }
}
