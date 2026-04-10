namespace PGDCP.Models
{
    public class LogCambio
    {
        public int Id { get; set; }
        public string? UsuarioId { get; set; } // Quién cambió
        public string? Entidad { get; set; }   // Ej: "Obra"
        public int RegistroId { get; set; }    // ID de la obra cambiada
        public string? Accion { get; set; }    // "Editar", "Borrar"
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
    }
}