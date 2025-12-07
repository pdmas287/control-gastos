using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("tipogasto")]
    public class TipoGasto
    {
        [Key]
        [Column("tipogastoid")]
        public int TipoGastoId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

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
        public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
        public virtual ICollection<RegistroGastoDetalle> DetallesGasto { get; set; } = new List<RegistroGastoDetalle>();
    }
}
