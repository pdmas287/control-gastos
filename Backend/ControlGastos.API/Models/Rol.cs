using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlGastos.API.Models
{
    [Table("rol")]
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("rolId")]

        public int RolId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("nombre")]

        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        [Column("descripcion")]

        public string? Descripcion { get; set; }

        [Column("activo")]


        public bool Activo { get; set; } = true;

        [Column("fechaCreacion")]


        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relaciones
        public virtual ICollection<Usuario>? Usuarios { get; set; }
    }
}
