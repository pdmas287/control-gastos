using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("deposito")]
    public class Deposito
    {
        [Key]
        [Column("depositoid")]
        public int DepositoId { get; set; }

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [Column("fondomonetarioid")]
        public int FondoMonetarioId { get; set; }

        [Required]
        [Column("monto", TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [StringLength(500)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

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
    }
}
