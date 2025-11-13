
namespace SIGEBI.Application.Dtos.Models.Configuration.Prestamo
{
    public record PrestamoUpdateDto
    {
        public int Id { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public decimal? Penalizacion { get; set; }
    }
}