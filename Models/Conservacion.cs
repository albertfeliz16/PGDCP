using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PGDCP.Models;

public class Conservacion
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una obra")]
    [Display(Name = "Obra")]
    public int ObraId { get; set; }

    [ForeignKey("ObraId")]
    public Obra? Obra { get; set; }

    [Required(ErrorMessage = "El diagnóstico es obligatorio")]
    [Display(Name = "Diagnóstico")]
    public string Diagnostico { get; set; } = string.Empty;

    [Display(Name = "Tratamiento aplicado")]
    public string? Tratamiento { get; set; }

    [Display(Name = "Fecha de Intervención")]
    [DataType(DataType.Date)]
    public DateTime FechaIntervencion { get; set; } = DateTime.Today;

    [Display(Name = "ID del Restaurador")]
    public string? RestauradorId { get; set; }
}
