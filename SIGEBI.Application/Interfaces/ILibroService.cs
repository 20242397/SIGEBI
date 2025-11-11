using NPOI.SS.Formula.Functions;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using System.Threading.Tasks;

namespace SIGEBI.Application.Interfaces
{
    public interface ILibroService
    {
        
        Task<ServiceResult<T>> RegistrarLibroAsync<T>(LibroCreateDto dto);
        Task<ServiceResult<T>> ModificarLibroAsync<T>(LibroUpdateDto dto);
        Task<OperationResult<bool>> RemoveAsync(int id);
        Task<ServiceResult<T>> BuscarPorTituloAsync<T>(string titulo);
        Task<ServiceResult<T>> BuscarPorAutorAsync<T>(string autor);
        Task<ServiceResult<T>> BuscarPorCategoriaAsync<T>(string categoria);
        Task<OperationResult<Libro>> BuscarPorISBNAsync(string isbn);
        Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id);
        Task<ServiceResult<T>> ObtenerTodosAsync<T>();

        Task<ServiceResult<T>> FiltrarAsync<T>(
        string? titulo, string? autor, string? categoria, int? año, string? estado);
    }
}

