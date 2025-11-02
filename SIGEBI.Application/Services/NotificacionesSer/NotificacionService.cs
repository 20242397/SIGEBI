using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Notificacion;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.INotificacion;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;

namespace SIGEBI.Application.Services.NotificacionesSer
{
    public sealed class NotificacionService : BaseService, INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly ILogger<NotificacionService> _logger;

        public NotificacionService(INotificacionRepository notificacionRepository, ILogger<NotificacionService> logger)
        {
            _notificacionRepository = notificacionRepository;
            _logger = logger;
        }

        // ✅ RF4.1 - Registrar notificación
        public Task<ServiceResult<T>> RegistrarNotificacionAsync<T>(NotificacionCreateDto dto) =>
            ExecuteAsync(async () =>
            {
                var entity = new Notificacion
                {
                    UsuarioId = dto.UsuarioId,
                    Tipo = dto.Tipo,
                    Mensaje = dto.Mensaje,
                    FechaEnvio = dto.FechaEnvio ?? DateTime.Now,
                    Enviado = false
                };

                var validation = NotificacionValidator.Validar(entity);
                if (!validation.Success)
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = validation.Message
                    };

                var result = await _notificacionRepository.AddAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Notificación registrada correctamente para usuario ID {UsuarioId}", dto.UsuarioId);
                else
                    _logger.LogWarning("Error al registrar notificación para usuario ID {UsuarioId}", dto.UsuarioId);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };
            });

        // ✅ RF4.2 - Obtener notificaciones por usuario
        public Task<ServiceResult<T>> ObtenerPorUsuarioAsync<T>(int usuarioId) =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.ObtenerNotificacionesPorUsuarioAsync(usuarioId);

                _logger.LogInformation("Consulta de notificaciones para usuario {UsuarioId}: {Count}",
                    (object)usuarioId, (result.Data is IEnumerable<object> enumerable ? enumerable.Count() : 0));

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };
            });

        // ✅ RF4.2 - Obtener notificaciones no leídas
        public Task<ServiceResult<T>> ObtenerNoLeidasAsync<T>(int usuarioId) =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.ObtenerNotificacionesNoLeidasPorUsuarioAsync(usuarioId);

                _logger.LogInformation("Consulta de notificaciones no leídas del usuario {UsuarioId}: {Count}",
                    (object)usuarioId, (result.Data is IEnumerable<object> enumerable ? enumerable.Count() : 0));

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };
            });

        // ✅ RF4.3 - Obtener notificaciones pendientes
        public Task<ServiceResult<T>> ObtenerPendientesAsync<T>() =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.ObtenerPendientesAsync();

                _logger.LogInformation( "Consulta de notificaciones pendientes: {Count}", (result.Data is IEnumerable<object> enumerable ? enumerable.Count() : 0));


                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };
            });

        // ✅ RF4.3 - Filtrar notificaciones por tipo
        public Task<ServiceResult<T>> ObtenerPorTipoAsync<T>(string tipo) =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.ObtenerPorTipoAsync(tipo);

                _logger.LogInformation("Consulta de notificaciones tipo {Tipo}: {Count}",
                    (object)tipo, (result.Data is IEnumerable<object> enumerable ? enumerable.Count() : 0));

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };
            });

        // ✅ RF4.3 - Marcar notificación como enviada
        public Task<ServiceResult<T>> MarcarComoEnviadaAsync<T>(int notificacionId) =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.MarcarComoEnviadaAsync(notificacionId);

                if (result.Success)
                    _logger.LogInformation("Notificación marcada como enviada: {Id}", notificacionId);
                else
                    _logger.LogWarning("Error al marcar notificación {Id} como enviada", notificacionId);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = default!
                };
            });

        public Task<ServiceResult<T>> EnviarNotificacionAsync<T>(NotificacionCreateDto dto) =>
            ExecuteAsync(async () =>
            {
                // Aquí se implementaría la lógica para enviar la notificación (por email, SMS, etc.)
                // Por simplicidad, asumimos que la notificación se envía correctamente.
                var entity = new Notificacion
                {
                    UsuarioId = dto.UsuarioId,
                    Tipo = dto.Tipo,
                    Mensaje = dto.Mensaje,
                    FechaEnvio = DateTime.Now,
                    Enviado = true
                };
                var validation = NotificacionValidator.Validar(entity);
                if (!validation.Success)
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = validation.Message
                    };
                var result = await _notificacionRepository.AddAsync(entity);
                if (result.Success)
                    _logger.LogInformation("Notificación enviada correctamente para usuario ID {UsuarioId}", dto.UsuarioId);
                else
                    _logger.LogWarning("Error al enviar notificación para usuario ID {UsuarioId}", dto.UsuarioId);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };



            });

        // ✅ RF5.x - Marcar todas las notificaciones como enviadas por usuario
        public Task<ServiceResult<T>> MarcarTodasComoEnviadasPorUsuarioAsync<T>(int usuarioId) =>
            ExecuteAsync(async () =>
            {
                if (usuarioId <= 0)
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "El ID de usuario no es válido."
                    };

                var result = await _notificacionRepository.MarcarTodasComoEnviadasPorUsuarioAsync(usuarioId);

                if (result.Success)
                    _logger.LogInformation("Se marcaron {Cantidad} notificaciones como enviadas para el usuario {UsuarioId}.", (int)(object)result.Data, usuarioId);

                else
                    _logger.LogWarning("Error al marcar notificaciones enviadas para usuario {UsuarioId}: {Mensaje}", usuarioId, result.Message);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

    }
}
