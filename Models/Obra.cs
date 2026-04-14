using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PGDCP.Models
{
    public class Obra
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Display(Name = "Autor")]
        public string? Autor { get; set; }

        [Display(Name = "Categoría")]
        public int? CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        [Display(Name = "Época")]
        public int? EpocaId { get; set; }
        [ForeignKey("EpocaId")]
        public virtual Epoca? Epoca { get; set; }

        [Display(Name = "Estilo")]
        public int? EstiloId { get; set; }
        [ForeignKey("EstiloId")]
        public virtual Estilo? Estilo { get; set; }

        [Display(Name = "Ubicación")]
        public int? UbicacionId { get; set; }
        [ForeignKey("UbicacionId")]
        public virtual Ubicacion? Ubicacion { get; set; }

        [Display(Name = "Técnica")]
        public int? TecnicaId { get; set; }
        [ForeignKey("TecnicaId")]
        public virtual Tecnica? Tecnica { get; set; }

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Fecha de Adquisición")]
        [DataType(DataType.Date)]
        public DateTime FechaAdquisicion { get; set; } = DateTime.Today;

        [Display(Name = "Valor Estimado (USD)")]
        [Column(TypeName = "decimal(18, 2)")] 
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)] // Fuerza 2 decimales en pantalla
        [Range(0, 9999999999999999.99, ErrorMessage = "El valor debe ser positivo")]
        public decimal ValorEstimado { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }

        public virtual ICollection<ObraImagen>? Imagenes { get; set; }
        public virtual ICollection<ObraMaterial>? ObraMateriales { get; set; }
    }
}