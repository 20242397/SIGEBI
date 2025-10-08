using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration.IPrestamo
{
    public interface IPrestamoRepository : IBaseRepository<Prestamo>
    {
        // 🔹 Registrar un nuevo préstamo
        Task<OperationResult<Prestamo>> RegistrarPrestamoAsync(Prestamo prestamo);

        // 🔹 Registrar la devolución de un préstamo (con penalización opcional)
        Task<OperationResult<Prestamo>> RegistrarDevolucionAsync(int prestamoId, DateTime fechaDevolucion, decimal? penalizacion);

        // 🔹 Obtener préstamos activos (sin devolución registrada)
        Task<OperationResult<IEnumerable<Prestamo>>> GetPrestamosActivosAsync();

        // 🔹 Obtener préstamos con penalización
        Task<OperationResult<IEnumerable<Prestamo>>> GetPrestamosConPenalizacionAsync();

        // 🔹 Calcular penalización para un préstamo específico
        Task<OperationResult<bool>> CalcularPenalizacionAsync(int prestamoId);

        // 🔹 Obtener historial de préstamos de un usuario
        Task<OperationResult<IEnumerable<Prestamo>>> GetHistorialPorUsuarioAsync(int usuarioId);

        // 🔹 Verificar si el usuario está restringido por penalización
        Task<OperationResult<bool>> RestringirPrestamoSiPenalizadoAsync(int usuarioId);
    }
}

