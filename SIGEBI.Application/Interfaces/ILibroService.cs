using NPOI.SS.Formula.Functions;
using SIGEBI.Application.Base;

using static SIGEBI.Application.Dtos.Models.Configuration.Libro.LibroGetModel;

namespace SIGEBI.Application.Interfaces
{
    public interface ILibroService
    {
        Task<ServiceResult<T> > RegistrarLibroAsync<T>(LibroCreateDto dto);
        Task<ServiceResult<T>> ModificarLibroAsync<T>(LibroUpdateDto dto);
        Task<ServiceResult<T>> EliminarLibroAsync<T>(int id);
        Task<ServiceResult<T>> BuscarPorTituloAsync<T>(string titulo);
        Task<ServiceResult<T> >BuscarPorAutorAsync<T>(string autor);
        Task<ServiceResult<T> >BuscarPorCategoriaAsync<T>(string categoria);
        Task<ServiceResult<T> > ObtenerPorIdAsync<T>(int id);
        Task<ServiceResult<T> >ObtenerTodosAsync<T>();
    }
}
