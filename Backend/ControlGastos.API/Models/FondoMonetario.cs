using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("FondoMonetario")]
    public class FondoMonetario
    {
        [Key]
        public int FondoMonetarioId { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TipoFondo { get; set; } = string.Empty; // "Cuenta Bancaria" o "Caja Menuda"

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoActual { get; set; } = 0;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaModificacion { get; set; }

        // Relación con Usuario
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Navegación
        public virtual ICollection<RegistroGastoEncabezado> RegistrosGasto { get; set; } = new List<RegistroGastoEncabezado>();
        public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();
    }
}
