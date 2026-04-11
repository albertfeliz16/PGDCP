using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PGDCP.Models
{
    [Table("PerfilesUsuario")]
    public class PerfilUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(10)]
        public string Sexo { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }
    }
}