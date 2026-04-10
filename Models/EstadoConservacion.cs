using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class EstadoConservacion
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Nombre { get; set; } = string.Empty; // Ej: Excelente, Bueno, Dañado
        public string? Descripcion { get; set; }

        public virtual ICollection<Conservacion>? Conservaciones { get; set; }
    }
}