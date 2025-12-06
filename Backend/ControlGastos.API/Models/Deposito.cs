using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("Deposito")]
    public class Deposito
    {
        [Key]
        public int DepositoId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; }

        [Required]
        public int FondoMonetarioId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [StringLength(300)]
        public string? Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaModificacion { get; set; }

        // Relación con Usuario
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Navegación
        [ForeignKey("FondoMonetarioId")]
        public virtual FondoMonetario? FondoMonetario { get; set; }
    }
}
