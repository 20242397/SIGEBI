
namespace SIGEBI.Application.Dtos.Models.Configuration.Reporte
{
    public record ReporteUpdateDto
    {
        public int Id { get; set; }
        public bool Resuelto { get; set; }
        public bool MarcarComoResuelto { get; set; }
        public object Contenido { get;  set; }
    }
}
