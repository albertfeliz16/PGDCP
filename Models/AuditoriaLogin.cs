using System.ComponentModel.DataAnnotations;

namespace PGDCP.Models
{
    public class AuditoriaLogin
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool Exitoso { get; set; }
        public string? IpAddress { get; set; }
    }
}