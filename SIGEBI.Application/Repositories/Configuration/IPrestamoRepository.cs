using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;

namespace SIGEBI.Domain.Repository
{
    public interface IPrestamoRepository : IBaseRepository<Prestamo>
    {
        Task<OperationResult<bool>> RegistrarPrestamoAsync(Prestamo prestamo);
        Task<OperationResult<bool>> RegistrarDevolucionAsync(int prestamoId, DateTime fechaDevolucion, decimal? penalizacion);
        Task<OperationResult<IEnumerable<Prestamo>>> GetPrestamosActivosAsync();
        Task<OperationResult<IEnumerable<Prestamo>>> GetPrestamosConPenalizacionAsync();
        Task<OperationResult<bool>> AddAsync(Prestamo prestamo);
        Task<OperationResult<bool>> CalcularPenalizacionAsync(int prestamoId);
        Task<OperationResult<bool>> GetHistorialPorUsuarioAsync(int usuarioId);
        Task<OperationResult<bool>> RestringirPrestamoSiPenalizadoAsync(int usuarioId);
    }
}

