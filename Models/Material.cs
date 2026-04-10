using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class Material
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del material es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        // Relación inversa: Un material puede estar en muchas obras
        public virtual ICollection<ObraMaterial>? ObraMateriales { get; set; }
    }
}