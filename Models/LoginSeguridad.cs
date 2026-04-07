#nullable disable
using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class LoginSeguridad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public int IntentosFallidos { get; set; } = 0;

        public DateTime? BloqueadoHasta { get; set; }

        public DateTime? UltimoIntento { get; set; }
    }
}