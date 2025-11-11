using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;

namespace SIGEBI.Application.Interfaces
{
    public interface IPrestamoService
    {
       
        Task<ServiceResult<T>> RegistrarPrestamoAsync<T>(PrestamoCreateDto dto);

        Task<ServiceResult<T>> ExtenderPrestamoAsync<T>(PrestamoUpdateDto dto);

        Task<ServiceResult<T>> RegistrarDevolucionAsync<T>(int prestamoId, DateTime fechaDevolucion);

        Task<ServiceResult<T>> CalcularPenalizacionAsync<T>(int prestamoId);

        Task<ServiceResult<T>> RestringirPrestamoSiPenalizadoAsync<T>(int usuarioId);

        Task<ServiceResult<T>> ObtenerHistorialUsuarioAsync<T>(int usuarioId);

        Task<ServiceResult<T>> ObtenerTodosAsync<T>();


    }
}
