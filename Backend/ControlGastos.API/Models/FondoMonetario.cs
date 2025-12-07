using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("fondomonetario")]
    public class FondoMonetario
    {
        [Key]
        [Column("fondoMonetarioId")]

        public int FondoMonetarioId { get; set; }

        [Required]
        [StringLength(200)]
        [Column("nombre")]

        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("tipoFondo")]

        public string TipoFondo { get; set; } = string.Empty; // "Cuenta Bancaria" o "Caja Menuda"

        [StringLength(500)]
        [Column("descripcion")]

        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Column("saldoActual")]

        public decimal SaldoActual { get; set; } = 0;

        [Column("activo")]


        public bool Activo { get; set; } = true;

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
        public virtual ICollection<RegistroGastoEncabezado> RegistrosGasto { get; set; } = new List<RegistroGastoEncabezado>();
        public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();
    }
}
