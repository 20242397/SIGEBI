using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration.IBiblioteca
{
    public interface ILibroRepository : IBaseRepository<Libro>
    {
       
        Task<OperationResult<Libro>> AddAsync(Libro libro);
        Task<OperationResult<IEnumerable<Libro>>> GetAllAsync();
        Task<OperationResult<Libro>> GetByIdAsync(int id);
        Task<OperationResult<IEnumerable<Libro>>> GetByAuthorAsync(string autor);
        Task<OperationResult<IEnumerable<Libro>>> GetByCategoryAsync(string categoria);
        Task<OperationResult<IEnumerable<Libro>>> SearchByTitleAsync(string titulo);
        Task<OperationResult<Libro>> UpdateAsync(Libro libro);
        Task<OperationResult<bool>> RemoveAsync(int id);
        Task<OperationResult<Libro>> GetByISBNAsync(string isbn);
    }
}
