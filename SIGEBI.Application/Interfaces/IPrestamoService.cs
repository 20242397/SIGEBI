using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;

namespace SIGEBI.Application.Interfaces
{
    public interface IPrestamoService
    {
        // RF3.1 - Registrar préstamo
        Task<ServiceResult<T>> RegistrarPrestamoAsync<T>(PrestamoCreateDto dto);

        // RF3.2 - Extender préstamo
        Task<ServiceResult<T>> ExtenderPrestamoAsync<T>(PrestamoUpdateDto dto);

        // RF3.3 - Registrar devolución
        Task<ServiceResult<T>> RegistrarDevolucionAsync<T>(int prestamoId, DateTime fechaDevolucion);

        // RF3.4 - Calcular penalización
        Task<ServiceResult<T>> CalcularPenalizacionAsync<T>(int prestamoId);

        // RF3.5 - Restringir préstamo si hay penalizaciones activas
        Task<ServiceResult<T>> RestringirPrestamoSiPenalizadoAsync<T>(int usuarioId);

        // RF2.5 - Historial de préstamos del usuario
        Task<ServiceResult<T>> ObtenerHistorialUsuarioAsync<T>(int usuarioId);
    }
}
