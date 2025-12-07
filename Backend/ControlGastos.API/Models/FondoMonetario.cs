using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("fondomonetario")]
    public class FondoMonetario
    {
        [Key]
        [Column("fondomonetarioid")]
        public int FondoMonetarioId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("tipofondo")]
        public string TipoFondo { get; set; } = string.Empty;

        [StringLength(200)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("saldoactual", TypeName = "decimal(18,2)")]
        public decimal SaldoActual { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

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
        public virtual ICollection<RegistroGastoEncabezado> RegistrosGasto { get; set; } = new List<RegistroGastoEncabezado>();
        public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();
    }
}
