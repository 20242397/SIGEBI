using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Notificacion;

namespace SIGEBI.Application.Interfaces
{
    public interface INotificacionService
    {
       
        Task<ServiceResult<T>> RegistrarNotificacionAsync<T>(NotificacionCreateDto dto);

        Task<ServiceResult<T>> EnviarNotificacionAsync<T>(NotificacionCreateDto dto);

        Task<ServiceResult<T>> ObtenerPorUsuarioAsync<T>(int usuarioId);

        Task<ServiceResult<T>> ObtenerNoLeidasAsync<T>(int usuarioId);

        Task<ServiceResult<T>> ObtenerPendientesAsync<T>();

        Task<ServiceResult<T>> MarcarComoEnviadaAsync<T>(int notificacionId);

        Task<ServiceResult<T>> ObtenerPorTipoAsync<T>(string tipo);

        Task<ServiceResult<T>> MarcarTodasComoEnviadasPorUsuarioAsync<T>(int usuarioId);


    }
}

