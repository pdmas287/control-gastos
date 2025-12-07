using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("deposito")]
    public class Deposito
    {
        [Key]
        [Column("depositoId")]

        [Column("depositoId")]


        public int DepositoId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        [Column("fecha")]

        public DateTime Fecha { get; set; }

        [Required]
        [Column("fondoMonetarioId")]

        [Column("fondoMonetarioId")]


        public int FondoMonetarioId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Column("monto")]

        [Column("monto")]


        public decimal Monto { get; set; }

        [StringLength(300)]
        [Column("descripcion")]

        [Column("descripcion")]


        public string? Descripcion { get; set; }

        [Column("fechaCreacion")]


        [Column("fechaCreacion")]



        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fechaModificacion")]


        [Column("fechaModificacion")]



        public DateTime? FechaModificacion { get; set; }

        // Relación con Usuario
        [Column("usuarioId")]

        [Column("usuarioId")]


        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Navegación
        [ForeignKey("FondoMonetarioId")]
        public virtual FondoMonetario? FondoMonetario { get; set; }
    }
}
