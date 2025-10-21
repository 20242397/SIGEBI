using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar;

namespace SIGEBI.Application.Interfaces
{
    public interface IEjemplarService
    {
        // Registrar un nuevo ejemplar (para un libro)
        Task<ServiceResult<T>> RegistrarEjemplarAsync<T>(EjemplarCreateDto dto);

        // Actualizar estado o información del ejemplar
        Task<ServiceResult<T>> ActualizarEjemplarAsync<T>(EjemplarUpdateDto dto);

        // Obtener todos los ejemplares de un libro
        Task<ServiceResult<T>> ObtenerPorLibroAsync<T>(int libroId);

        // Obtener ejemplares disponibles para préstamo
        Task<ServiceResult<T>> ObtenerDisponiblesPorLibroAsync<T>(int libroId);

        // Listar ejemplares prestados
        Task<ServiceResult<T>> ObtenerPrestadosAsync<T>();

        // Listar ejemplares reservados
        Task<ServiceResult<T>> ObtenerReservadosAsync<T>();

        // Marcar ejemplar como perdido o dañado
        Task<ServiceResult<T>> MarcarComoPerdidoAsync<T>(int ejemplarId);
        Task<ServiceResult<T>> MarcarComoDañadoAsync<T>(int ejemplarId);
    }
}

