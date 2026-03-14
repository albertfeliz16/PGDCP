using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class PerfilUsuario
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50)]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio.")]
        public string? Sexo { get; set; }

        [Phone(ErrorMessage = "Formato de teléfono no válido.")]
        public string? Telefono { get; set; }

        public string? Rol { get; set; }
    }
}