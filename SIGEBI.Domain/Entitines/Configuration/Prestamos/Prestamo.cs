namespace SIGEBI.Domain.Entitines.Configuration.Prestamos
{
    public sealed class Prestamo : Base.BaseEntity
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EjemplarId { get; set; }

        public DateTime FechaPrestamo { get; set; } = DateTime.Now;
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }

        public decimal? Penalizacion { get; set; }
        public string Estado { get; set; } = "Activo"; // Activo / Vencido / Devuelto
        public string? LibroId { get; set; }
        public object Usuario { get; set; }
        public object Ejemplar { get; set; }
    }
}
