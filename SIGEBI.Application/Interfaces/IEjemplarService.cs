using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;

namespace SIGEBI.Application.Interfaces
{
    public interface IEjemplarService
    {

        Task<ServiceResult<T>> RegistrarEjemplarAsync<T>(EjemplarCreateDto dto);
        Task<ServiceResult<Ejemplar>> ActualizarEjemplarAsync(EjemplarUpdateDto dto);
        Task<ServiceResult<T>> ObtenerPorLibroAsync<T>(int libroId);
        Task<ServiceResult<T>> ObtenerDisponiblesPorLibroAsync<T>(int libroId);
        Task<ServiceResult<T>> ObtenerPrestadosAsync<T>();
        Task<ServiceResult<T>> ObtenerReservadosAsync<T>();
        Task<ServiceResult<T>> MarcarComoPerdidoAsync<T>(int ejemplarId);
        Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id);
        Task<ServiceResult<T>> ObtenerTodosAsync<T>();
    }
}