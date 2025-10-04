using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
namespace SIGEBI.Domain.Repository
{
    public interface ILibroRepository : IBaseRepository<Libro>
    {

        Task<OperationResult<bool>> AddAsync(Libro libro);
        Task <OperationResult<bool>>GetByAuthorAsync(string autor);
        Task <OperationResult<bool>>GetByCategoryAsync(string categoria);
        Task <OperationResult<bool>>SearchByTitleAsync(string titulo);
    }
}
