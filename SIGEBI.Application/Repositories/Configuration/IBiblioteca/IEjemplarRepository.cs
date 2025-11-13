using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration.IBiblioteca
{
    public interface IEjemplarRepository : IBaseRepository<Ejemplar>
    {
        Task<IEnumerable<Ejemplar>> ObtenerPorLibroAsync(int libroId);
        Task<IEnumerable<Ejemplar>> ObtenerDisponiblesPorLibroAsync(int libroId);
        Task<IEnumerable<Ejemplar>> ObtenerReservadosAsync();
        Task<IEnumerable<Ejemplar>> ObtenerPrestadosAsync();
        Task<OperationResult<bool>> MarcarComoPerdidoAsync(int ejemplarId);
    }
}