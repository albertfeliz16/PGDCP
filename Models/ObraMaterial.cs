using System.ComponentModel.DataAnnotations.Schema;

namespace PGDCP.Models
{
    public class ObraMaterial
    {
        public int ObraId { get; set; }
        [ForeignKey("ObraId")]
        public virtual Obra? Obra { get; set; }

        public int MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material? Material { get; set; }
    }
}