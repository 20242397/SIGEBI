namespace SIGEBI.Web.Models.ReporteApi
{
    public class ReporteApiCreateModel
    {
        public int UsuarioId { get; set; }
        public string Tipo { get; set; } = null!;
        public string Contenido { get; set; } = null!;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
