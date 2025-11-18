namespace SIGEBI.Web.Models.PrestamoApi
{
    public class PrestamoApiUpdateModel
    {
        public int Id { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public DateTime? FechaDevolucion { get; set; }

        public decimal? Penalizacion { get; set; }
    }
}
