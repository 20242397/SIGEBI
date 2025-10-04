using NPOI.SS.Formula.Functions;
using SIGEBI.Application.Base;
using static SIGEBI.Application.Dtos.Models.Configuration.Prestamo.PrestamoGetModel;

namespace SIGEBI.Application.Interfaces
{
    public interface IPrestamoService
    {
        Task<ServiceResult<T>> RegistrarPrestamoAsync<T>(PrestamoCreateDto dto);
        Task<ServiceResult<T>>RegistrarDevolucionAsync<T>(int prestamoId, DateTime fechaDevolucion);
        Task<ServiceResult<T>> CalcularPenalizacionAsync<T>(int prestamoId);
        Task<ServiceResult<T>> RestringirPrestamoSiPenalizadoAsync<T>(int usuarioId);
        Task<ServiceResult<T>> ObtenerHistorialUsuarioAsync<T>(int usuarioId);
    }
}
