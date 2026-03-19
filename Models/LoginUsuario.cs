#nullable disable
using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class LoginUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Referencia a AspNetUsers

        [Required]
        public string Email { get; set; }

        // Contador de intentos fallidos
        public int IntentosFallidos { get; set; } = 0;

        // Fecha hasta la que está bloqueado (null = no bloqueado)
        public DateTime? BloqueadoHasta { get; set; }

        // Fecha del último intento
        public DateTime? UltimoIntento { get; set; }
    }
}