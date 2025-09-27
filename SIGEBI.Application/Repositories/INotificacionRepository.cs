using SIGEBI.Domain.Entitines.Configuration.Notificaciones;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories
{
    public interface INotificacionRepository : IBaseRepository<Notificacion>
    {
        Task<IEnumerable<Notificacion>> ObtenerNotificacionesPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Notificacion>> ObtenerNotificacionesNoLeidasPorUsuarioAsync(int usuarioId);
      
        Task<IEnumerable<Notificacion>> ObtenerPendientesAsync();
        Task<IEnumerable<Notificacion>> ObtenerPorTipoAsync(string tipo);
        Task<bool> MarcarComoEnviadaAsync(int notificacionId);
    }
}
