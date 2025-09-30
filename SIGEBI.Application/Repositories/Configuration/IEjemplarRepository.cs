using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration
{
    public interface IEjemplarRepository : IBaseRepository<Ejemplar>
    {
        Task<IEnumerable<Ejemplar>> ObtenerPorLibroAsync(int libroId);
        Task<IEnumerable<Ejemplar>> ObtenerDisponiblesPorLibroAsync(int libroId);
        Task<IEnumerable<Ejemplar>> ObtenerReservadosAsync();
        Task<IEnumerable<Ejemplar>> ObtenerPrestadosAsync();
        Task<bool> MarcarComoPerdidoAsync(int ejemplarId);
        Task<bool> MarcarComoDaniadoAsync(int ejemplarId);


    }
}
