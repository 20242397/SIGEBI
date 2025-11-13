using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration.IPrestamo
{
    public interface IPrestamoRepository : IBaseRepository<Prestamo>
    {

        Task<OperationResult<Prestamo>> RegistrarPrestamoAsync(Prestamo prestamo);
        Task<OperationResult<Prestamo>> RegistrarDevolucionAsync(int prestamoId, DateTime fechaDevolucion, decimal? penalizacion);
        Task<OperationResult<IEnumerable<Prestamo>>> GetPrestamosActivosAsync();
        Task<OperationResult<IEnumerable<Prestamo>>> GetPrestamosConPenalizacionAsync();
        Task<OperationResult<bool>> CalcularPenalizacionAsync(int prestamoId);
        Task<OperationResult<IEnumerable<Prestamo>>> GetHistorialPorUsuarioAsync(int usuarioId);
        Task<OperationResult<bool>> RestringirPrestamoSiPenalizadoAsync(int usuarioId);

    }
}