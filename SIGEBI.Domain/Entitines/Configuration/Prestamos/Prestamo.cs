using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Entitines.Configuration.Security;

namespace SIGEBI.Domain.Entitines.Configuration.Prestamos
{
    public sealed class Prestamo : Base.BaseEntity
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int EjemplarId { get; set; }
        public Ejemplar Ejemplar { get; set; } = null!;

        public DateTime FechaPrestamo { get; set; } = DateTime.Now;
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public decimal? Penalizacion { get; set; }
    }
}
