using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class MayorDeEdadAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime fechaNacimiento)
            {
                if (fechaNacimiento <= DateTime.Now.AddYears(-18))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Debes ser mayor de 18 años.");
        }
    }
}