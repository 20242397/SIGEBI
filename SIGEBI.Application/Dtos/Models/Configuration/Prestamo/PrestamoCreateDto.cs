
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;

namespace SIGEBI.Application.Dtos.Models.Configuration.Prestamo
{
    public record PrestamoCreateDto
    {
        public int UsuarioId { get; set; }
        public int EjemplarId { get; set; }
        public int LibroId { get; set; } // Relación con el libro
        public DateTime FechaPrestamo { get; set; } = DateTime.Now;
        public DateTime FechaVencimiento { get; set; }

    }
}
