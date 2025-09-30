using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
namespace SIGEBI.Domain.Repository
{
    public interface ILibroRepository : IBaseRepository<Libro>
    {
        Task<OperationResult<IEnumerable<Libro>>> BuscarPorTituloAsync(string titulo);
        Task<OperationResult<IEnumerable<Libro>>> BuscarPorAutorAsync(string autor);
        Task<OperationResult<IEnumerable<Libro>>> BuscarPorCategoriaAsync(string categoria);
        Task<OperationResult<IEnumerable<Libro>>> GetLibrosMasPrestadosAsync(int topN);
        Task<OperationResult<bool>> AddAsync(Libro libro);
    }
}
