using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Notificacion;

namespace SIGEBI.Application.Interfaces
{
    public interface INotificacionService
    {
        // RF4.1 - Registrar una nueva notificación (por penalización, aviso o recordatorio)
        Task<ServiceResult<T>> RegistrarNotificacionAsync<T>(NotificacionCreateDto dto);

        Task<ServiceResult<T>> EnviarNotificacionAsync<T>(NotificacionCreateDto dto);

        // RF4.2 - Obtener notificaciones de un usuario
        Task<ServiceResult<T>> ObtenerPorUsuarioAsync<T>(int usuarioId);

        // RF4.2 - Obtener notificaciones no leídas de un usuario
        Task<ServiceResult<T>> ObtenerNoLeidasAsync<T>(int usuarioId);

        // RF4.3 - Obtener notificaciones pendientes de envío
        Task<ServiceResult<T>> ObtenerPendientesAsync<T>();

        // RF4.3 - Marcar una notificación como enviada
        Task<ServiceResult<T>> MarcarComoEnviadaAsync<T>(int notificacionId);

        // RF4.3 - Filtrar notificaciones por tipo (Aviso, Recordatorio, Penalización)
        Task<ServiceResult<T>> ObtenerPorTipoAsync<T>(string tipo);
        Task<ServiceResult<T>> MarcarTodasComoEnviadasPorUsuarioAsync<T>(int usuarioId);


    }
}

