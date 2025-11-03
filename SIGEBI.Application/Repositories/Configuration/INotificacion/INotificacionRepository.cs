using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration.INotificacion
{
    public interface INotificacionRepository : IBaseRepository<Notificacion>
    {
       
        Task<OperationResult<IEnumerable<Notificacion>>> ObtenerNotificacionesPorUsuarioAsync(int usuarioId);
        Task<OperationResult<IEnumerable<Notificacion>>> ObtenerNotificacionesNoLeidasPorUsuarioAsync(int usuarioId);
        Task<OperationResult<IEnumerable<Notificacion>>> ObtenerPendientesAsync();
        Task<OperationResult<IEnumerable<Notificacion>>> ObtenerPorTipoAsync(string tipo);

        Task<OperationResult<bool>> MarcarComoEnviadaAsync(int notificacionId);

        Task<OperationResult<int>> MarcarTodasComoEnviadasPorUsuarioAsync(int usuarioId);
    }
}

