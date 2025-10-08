using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro;

namespace SIGEBI.Application.Interfaces
{
    public interface ILibroService
    {
        // RF1.1 - Registrar libro
        Task<ServiceResult<T>> RegistrarLibroAsync<T>(LibroCreateDto dto);

        // RF1.2 - Modificar datos
        Task<ServiceResult<T>> ModificarLibroAsync<T>(LibroUpdateDto dto);

        // RF1.3 - Eliminar lógicamente un libro
        Task<ServiceResult<T>> EliminarLibroAsync<T>(int id);

        // RF1.4 - Búsquedas
        Task<ServiceResult<T>> BuscarPorTituloAsync<T>(string titulo);
        Task<ServiceResult<T>> BuscarPorAutorAsync<T>(string autor);
        Task<ServiceResult<T>> BuscarPorCategoriaAsync<T>(string categoria);
        Task<ServiceResult<T>> BuscarPorISBNAsync<T>(string isbn);

        // RF1.5 - Mostrar estado del libro
        Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id);
        Task<ServiceResult<T>> ObtenerTodosAsync<T>();
    }
}

