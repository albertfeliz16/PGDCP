using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class Epoca
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public short? SigloDesde { get; set; }
        public short? SigloHasta { get; set; }
        public virtual ICollection<Obra>? Obras { get; set; }
    }
}