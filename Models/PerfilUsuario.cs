#nullable disable
using System.ComponentModel.DataAnnotations;
namespace PGDCP.Models
{
    public class MayorDeEdadAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime fecha)
            {
                
                if (fecha.Date >= DateTime.Today)
                    return new ValidationResult("La fecha de nacimiento no puede ser hoy ni una fecha futura.");

                var edad = DateTime.Today.Year - fecha.Year;
                if (fecha.Date > DateTime.Today.AddYears(-edad)) edad--;

                if (edad < 18)
                    return new ValidationResult("Debes ser mayor de 18 años para registrarte.");
            }

            return ValidationResult.Success!;
        }
    }

    public class PerfilUsuario
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [MayorDeEdad] // Validación personalizada
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio.")]
        public string Sexo { get; set; }

        [Phone(ErrorMessage = "Formato de teléfono no válido.")]
        public string Telefono { get; set; }

        public string Rol { get; set; }
    }
}