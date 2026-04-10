using System.ComponentModel.DataAnnotations.Schema;

namespace PGDCP.Models
{
    public class ObraImagen
    {
        public int Id { get; set; }
        public int ObraId { get; set; }
        [ForeignKey("ObraId")]
        public virtual Obra? Obra { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; }
        public DateTime FechaSubida { get; set; } = DateTime.Now;
    }
}