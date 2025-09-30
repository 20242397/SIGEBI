using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration
{
    public interface IPrestamoRepository : IBaseRepository<Prestamo>
    {
        Task<IEnumerable<Prestamo>> ObtenerPrestamosActivosAsync();
        Task<IEnumerable<Prestamo>> ObtenerPrestamosActivosPorUsuarioAsync(int usuarioId);
        Task<Prestamo?> ObtenerPrestamoActivoPorEjemplarAsync(int ejemplarId);
        Task<bool> RegistrarDevolucionAsync(int prestamoId, DateTime fechaDevolucion);
        Task<decimal> CalcularPenalizacionAsync(int prestamoId);
    }
}
