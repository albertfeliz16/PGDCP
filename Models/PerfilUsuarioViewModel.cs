using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class PerfilUsuarioViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Seleccione el sexo")]
        public string Sexo { get; set; } = string.Empty;

        public string? Telefono { get; set; }
    }
}