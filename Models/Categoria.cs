using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }
        public virtual ICollection<Obra>? Obras { get; set; }
    }
}
