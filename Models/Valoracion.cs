using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PGDCP.Models
{
    public class Valoracion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una obra")]
        [Display(Name = "Obra")]
        public int ObraId { get; set; }

        [ForeignKey("ObraId")]
        public Obra? Obra { get; set; }

        [Required(ErrorMessage = "El valor es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser mayor a cero")]
        [Display(Name = "Valor Determinado (USD)")]
        public decimal ValorEstimado { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha de Valoración")]
        [DataType(DataType.Date)]
        public DateTime FechaValoracion { get; set; } = DateTime.Today;

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "ID del Perito")]
        public string? PeritoId { get; set; }

        public string MetodoValoracion { get; set; } = "Comparativo"; // Comparativo, Reposición, Histórico
        public string EstadoAutenticidad { get; set; } = "Confirmada"; // Confirmada, Atribuida, En Estudio
        public string? FactoresAjuste { get; set; } // Notas sobre plusvalía o minusvalía
    }
}
