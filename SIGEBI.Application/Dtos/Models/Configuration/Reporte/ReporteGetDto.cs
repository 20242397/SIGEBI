

namespace SIGEBI.Application.Dtos.Models.Configuration.Reporte
{
    public record ReporteGetDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Tipo { get; set; } = null!;
        public string Contenido { get; set; } = null!;
        public DateTime FechaGeneracion { get; set; }
    }
}
