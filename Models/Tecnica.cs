using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class Tecnica
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public virtual ICollection<Obra>? Obras { get; set; }
    }
}