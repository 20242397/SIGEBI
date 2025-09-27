using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories
{
    public interface ILibroRepository : IBaseRepository<Libro>
    {
        
        Task<IEnumerable<Libro>> BuscarPorTituloAsync(string titulo);
        Task<IEnumerable<Libro>> BuscarPorAutorAsync(string autor);
        Task<IEnumerable<Libro>> BuscarPorCategoriaAsync(string categoria);
        Task<IEnumerable<Libro>> GetLibrosMasPrestadosAsync(int topN);

    }
}
