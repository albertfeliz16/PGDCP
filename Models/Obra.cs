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

        [Display(Name = "Época")]
        public string? Epoca { get; set; }

        [Display(Name = "Estilo")]
        public string? Estilo { get; set; }

        [Display(Name = "Material")]
        public string? Material { get; set; }

        [Display(Name = "Fecha de Adquisición")]
        [DataType(DataType.Date)]
        public DateTime FechaAdquisicion { get; set; } = DateTime.Today;

        [Display(Name = "Valor Estimado (USD)")]
        [Range(0, double.MaxValue, ErrorMessage = "El valor debe ser positivo")]
        public decimal ValorEstimado { get; set; }

        [Display(Name = "URL de Imagen")]
        public string? ImagenUrl { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
    }
}
