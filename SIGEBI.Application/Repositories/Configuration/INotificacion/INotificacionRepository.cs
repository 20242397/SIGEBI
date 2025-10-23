using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration.INotificacion
{
    public interface INotificacionRepository : IBaseRepository<Notificacion>
    {
        // Obtener notificaciones de un usuario
        Task<OperationResult<IEnumerable<Notificacion>>> ObtenerNotificacionesPorUsuarioAsync(int usuarioId);

        // Obtener notificaciones no leídas
        Task<OperationResult<IEnumerable<Notificacion>>> ObtenerNotificacionesNoLeidasPorUsuarioAsync(int usuarioId);

        // Obtener todas las notificaciones pendientes de envío
        Task<OperationResult<IEnumerable<Notificacion>>> ObtenerPendientesAsync();

        // Obtener notificaciones filtradas por tipo (Aviso, Recordatorio, etc.)
        Task<OperationResult<IEnumerable<Notificacion>>> ObtenerPorTipoAsync(string tipo);

        // Marcar una notificación como enviada
        Task<OperationResult<bool>> MarcarComoEnviadaAsync(int notificacionId);

        Task<OperationResult<int>> MarcarTodasComoEnviadasPorUsuarioAsync(int usuarioId);
    }
}

