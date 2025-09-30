namespace SIGEBI.Persistence.Models.Configuration
{
    public record class PrestamoGetModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EjemplarId { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public decimal? Penalizacion { get; set; }
    }
}

