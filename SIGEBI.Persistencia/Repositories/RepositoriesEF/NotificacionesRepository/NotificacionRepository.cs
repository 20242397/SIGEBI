using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;
using SIGEBI.Application.Repositories.Configuration.INotificacion;
using SIGEBI.Application.Validators;
using SIGEBI.Infrastructure.Logging;

namespace SIGEBI.Persistence.Repositories.RepositoriesEF.NotificacionesRepository
{
    public sealed class NotificacionRepository : BaseRepository<Notificacion>, INotificacionRepository
    {
        private readonly SIGEBIContext _context;
        private readonly ILoggerService<Notificacion> _logger;

        public NotificacionRepository(SIGEBIContext context, ILoggerService<Notificacion> logger)
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        #region CRUD con validación

        public override async Task<OperationResult<Notificacion>> AddAsync(Notificacion entity)
        {
            var validacion = NotificacionValidator.Validar(entity);
            if (!validacion.Success)
                return validacion;

            return await base.AddAsync(entity);
        }

        public override async Task<OperationResult<Notificacion>> UpdateAsync(Notificacion entity)
        {
            var validacion = NotificacionValidator.Validar(entity);
            if (!validacion.Success)
                return validacion;

            return await base.UpdateAsync(entity);
        }

        #endregion

        #region Métodos personalizados

        public async Task<OperationResult<IEnumerable<Notificacion>>> ObtenerNotificacionesPorUsuarioAsync(int usuarioId)
        {
            try
            {
                var notificaciones = await _context.Notificacion
                    .Where(n => n.UsuarioId == usuarioId)
                    .OrderByDescending(n => n.FechaEnvio)
                    .ToListAsync();

                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = true,
                    Data = notificaciones
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener notificaciones del usuario {usuarioId}: {ex.Message}");
                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = false,
                    Message = "Error al obtener notificaciones por usuario"
                };
            }
        }

        public async Task<OperationResult<IEnumerable<Notificacion>>> ObtenerNotificacionesNoLeidasPorUsuarioAsync(int usuarioId)
        {
            try
            {
                var notificaciones = await _context.Notificacion
                    .Where(n => n.UsuarioId == usuarioId && !n.Enviado)
                    .OrderByDescending(n => n.FechaEnvio)
                    .ToListAsync();

                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = true,
                    Data = notificaciones
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener notificaciones no leídas del usuario {usuarioId}: {ex.Message}");
                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = false,
                    Message = "Error al obtener notificaciones no leídas"
                };
            }
        }

        public async Task<OperationResult<IEnumerable<Notificacion>>> ObtenerPendientesAsync()
        {
            try
            {
                var pendientes = await _context.Notificacion
                    .Where(n => !n.Enviado)
                    .OrderBy(n => n.FechaEnvio)
                    .ToListAsync();

                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = true,
                    Data = pendientes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener notificaciones pendientes: {ex.Message}");
                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = false,
                    Message = "Error al obtener notificaciones pendientes"
                };
            }
        }

        public async Task<OperationResult<IEnumerable<Notificacion>>> ObtenerPorTipoAsync(string tipo)
        {
            try
            {
                var notificaciones = await _context.Notificacion
                    .Where(n => n.Tipo == tipo)
                    .OrderByDescending(n => n.FechaEnvio)
                    .ToListAsync();

                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = true,
                    Data = notificaciones
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener notificaciones del tipo {tipo}: {ex.Message}");
                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = false,
                    Message = "Error al obtener notificaciones por tipo"
                };
            }
        }

        public async Task<OperationResult<bool>> MarcarComoEnviadaAsync(int notificacionId)
        {
            try
            {
                var notificacion = await _context.Notificacion.FindAsync(notificacionId);

                if (notificacion == null)
                {
                    return new OperationResult<bool>
                    {
                        Success = false,
                        Message = "Notificación no encontrada"
                    };
                }

                notificacion.Enviado = true;
                await _context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Notificación marcada como enviada"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al marcar notificación {notificacionId} como enviada: {ex.Message}");
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = "Error al marcar la notificación como enviada"
                };
            }
        }

        #endregion
    }
}
