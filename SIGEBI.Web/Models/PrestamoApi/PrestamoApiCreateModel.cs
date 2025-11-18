namespace SIGEBI.Web.Models.PrestamoApi
{
    public class PrestamoApiCreateModel
    {
        public int UsuarioId { get; set; }
        public int EjemplarId { get; set; }
        public int LibroId { get; set; }

        public DateTime FechaPrestamo { get; set; } = DateTime.Now;

        public DateTime FechaVencimiento { get; set; }
    }
}
