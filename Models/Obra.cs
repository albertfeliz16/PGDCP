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
        public Categoria? Categoria { get; set; }

        [Display(Name = "Época")]
        public int? EpocaId { get; set; }
        [ForeignKey("EpocaId")]
        public Epoca? Epoca { get; set; }

        [Display(Name = "Estilo")]
        public int? EstiloId { get; set; }
        [ForeignKey("EstiloId")]
        public Estilo? Estilo { get; set; }

        [Display(Name = "Ubicación")]
        public int? UbicacionId { get; set; }
        [ForeignKey("UbicacionId")]
        public Ubicacion? Ubicacion { get; set; }

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Fecha de Adquisición")]
        [DataType(DataType.Date)]
        public DateTime FechaAdquisicion { get; set; } = DateTime.Today;

        [Display(Name = "Valor Estimado (USD)")]
        [Range(0, double.MaxValue, ErrorMessage = "El valor debe ser positivo")]
        public decimal ValorEstimado { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }

        public ICollection<ObraImagen>? Imagenes { get; set; }
        public ICollection<ObraMaterial>? ObraMateriales { get; set; }
    }

    // ── Catálogos ──

    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class Epoca
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public short? SigloDesde { get; set; }
        public short? SigloHasta { get; set; }
        public string? Descripcion { get; set; }
    }

    public class Estilo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class Material
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class Ubicacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Direccion { get; set; }
    }

    // ── Imágenes (1 obra : N imágenes) ──

    public class ObraImagen
    {
        public int Id { get; set; }
        public int ObraId { get; set; }

        [ForeignKey("ObraId")]
        public Obra? Obra { get; set; }

        public string Url { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; } = false;
        public DateTime FechaSubida { get; set; } = DateTime.Now;
    }

    // ── Materiales N:M ──

    public class ObraMaterial
    {
        public int ObraId { get; set; }
        public int MaterialId { get; set; }

        [ForeignKey("ObraId")]
        public Obra? Obra { get; set; }

        [ForeignKey("MaterialId")]
        public Material? Material { get; set; }
    }
}