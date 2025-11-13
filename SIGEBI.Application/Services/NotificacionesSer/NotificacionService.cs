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

       
        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.ObtenerTodosAsync();

                if (!result.Success)
                    return new OperationResult<T> { Success = false, Message = result.Message };

                var lista = result.Data.Select(n => new NotificacionGetDto
                {
                    Id = n.Id,
                    UsuarioId = n.UsuarioId,
                    Tipo = n.Tipo,
                    Mensaje = n.Mensaje,
                    FechaEnvio = n.FechaEnvio,
                    Enviado = n.Enviado
                }).ToList();

                return new OperationResult<T> { Success = true, Data = (T)(object)lista };
            });

       
        public Task<ServiceResult<T>> EnviarNotificacionAsync<T>(NotificacionCreateDto dto) =>
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

                var validacion = NotificacionValidator.Validar(entity);
                if (!validacion.Success)
                    return new OperationResult<T> { Success = false, Message = validacion.Message };

                var result = await _notificacionRepository.AddAsync(entity);
                if (!result.Success)
                    return new OperationResult<T> { Success = false, Message = result.Message };

                var dtoResult = new NotificacionGetDto
                {
                    Id = result.Data.Id,
                    UsuarioId = result.Data.UsuarioId,
                    Tipo = result.Data.Tipo,
                    Mensaje = result.Data.Mensaje,
                    FechaEnvio = result.Data.FechaEnvio,
                    Enviado = result.Data.Enviado
                };

                return new OperationResult<T>
                {
                    Success = true,
                    Data = (T)(object)dtoResult,
                    Message = "Notificación registrada correctamente."
                };
            });

       
        public Task<ServiceResult<T>> ObtenerPorUsuarioAsync<T>(int usuarioId) =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.ObtenerNotificacionesPorUsuarioAsync(usuarioId);
                if (!result.Success)
                    return new OperationResult<T> { Success = false, Message = result.Message };

                var lista = result.Data.Select(n => new NotificacionGetDto
                {
                    Id = n.Id,
                    UsuarioId = n.UsuarioId,
                    Tipo = n.Tipo,
                    Mensaje = n.Mensaje,
                    FechaEnvio = n.FechaEnvio,
                    Enviado = n.Enviado
                }).ToList();

                return new OperationResult<T> { Success = true, Data = (T)(object)lista };
            });

       
        public Task<ServiceResult<T>> ObtenerNoLeidasAsync<T>(int usuarioId) =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.ObtenerNotificacionesNoLeidasPorUsuarioAsync(usuarioId);
                if (!result.Success)
                    return new OperationResult<T> { Success = false, Message = result.Message };

                var lista = result.Data.Select(n => new NotificacionGetDto
                {
                    Id = n.Id,
                    UsuarioId = n.UsuarioId,
                    Tipo = n.Tipo,
                    Mensaje = n.Mensaje,
                    FechaEnvio = n.FechaEnvio,
                    Enviado = n.Enviado
                }).ToList();

                return new OperationResult<T> { Success = true, Data = (T)(object)lista };
            });

        
        public Task<ServiceResult<T>> ObtenerPendientesAsync<T>() =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.ObtenerPendientesAsync();
                if (!result.Success)
                    return new OperationResult<T> { Success = false, Message = result.Message };

                var lista = result.Data.Select(n => new NotificacionGetDto
                {
                    Id = n.Id,
                    UsuarioId = n.UsuarioId,
                    Tipo = n.Tipo,
                    Mensaje = n.Mensaje,
                    FechaEnvio = n.FechaEnvio,
                    Enviado = n.Enviado
                }).ToList();

                return new OperationResult<T> { Success = true, Data = (T)(object)lista };
            });

       
        public Task<ServiceResult<T>> ObtenerPorTipoAsync<T>(string tipo) =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.ObtenerPorTipoAsync(tipo);
                if (!result.Success)
                    return new OperationResult<T> { Success = false, Message = result.Message };

                var lista = result.Data.Select(n => new NotificacionGetDto
                {
                    Id = n.Id,
                    UsuarioId = n.UsuarioId,
                    Tipo = n.Tipo,
                    Mensaje = n.Mensaje,
                    FechaEnvio = n.FechaEnvio,
                    Enviado = n.Enviado
                }).ToList();

                return new OperationResult<T> { Success = true, Data = (T)(object)lista };
            });

       
        public Task<ServiceResult<T>> MarcarTodasComoEnviadasPorUsuarioAsync<T>(int usuarioId) =>
            ExecuteAsync(async () =>
            {
                var result = await _notificacionRepository.MarcarTodasComoEnviadasPorUsuarioAsync(usuarioId);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success ? (T)(object)result.Data! : default
                };
            });

     
        public Task<ServiceResult<bool>> GenerarNotificacionesAutomaticasAsync() =>
            ExecuteAsync(async () =>
            {
                try
                {
                    var previas = await _notificacionRepository.GenerarNotificacionesPreviasAsync();
                    var vencidas = await _notificacionRepository.GenerarNotificacionesDiaVencimientoAsync();
                    var penalizaciones = await _notificacionRepository.GenerarNotificacionesPorPenalizacionAsync();

                    int total = 0;
                    if (previas.Success) total += previas.Data;
                    if (vencidas.Success) total += vencidas.Data;
                    if (penalizaciones.Success) total += penalizaciones.Data;

                    _logger.LogInformation($"Se generaron {total} notificaciones automáticas en total.");

                    return new OperationResult<bool>
                    {
                        Success = true,
                        Data = true,
                        Message = $"Se generaron {total} notificaciones automáticas correctamente."
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al generar notificaciones automáticas.");
                    return new OperationResult<bool>
                    {
                        Success = false,
                        Message = "Error al generar notificaciones automáticas."
                    };
                }
            });
    }
}
