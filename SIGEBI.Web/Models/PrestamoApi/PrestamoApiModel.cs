namespace SIGEBI.Web.Models.PrestamoApi
{
    public class PrestamoApiModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int LibroId { get; set; }
        public int EjemplarId { get; set; }

        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }

        public decimal? Penalizacion { get; set; }

        public string Estado { get; set; }
    }
}
