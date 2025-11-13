using Microsoft.EntityFrameworkCore;
using SIGEBI.Application.Repositories.Configuration.INotificacion;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Logging;

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

        #region Obtener todas
        public async Task<OperationResult<IEnumerable<Notificacion>>> ObtenerTodosAsync()
        {
            try
            {
                var notificaciones = await _context.Notificacion
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
                _logger.LogError(ex, "Error al obtener todas las notificaciones.");
                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = false,
                    Message = "Error al obtener todas las notificaciones."
                };
            }
        }
        #endregion

        #region RF4.1 - Notificación 2 días antes del vencimiento
        public async Task<OperationResult<int>> GenerarNotificacionesPreviasAsync()
        {
            try
            {
                var fechaObjetivo = DateTime.Today.AddDays(2);

                var prestamos = await _context.Prestamo
                    .Include(p => p.Usuario)
                    .Include(p => p.Libro)
                    .Where(p => p.Estado == "Activo" && p.FechaVencimiento.Date == fechaObjetivo)
                    .ToListAsync();

                if (!prestamos.Any())
                    return new OperationResult<int> { Success = false, Message = "No hay préstamos próximos a vencer." };

                foreach (var p in prestamos)
                {
                    var notificacion = new Notificacion
                    {
                        UsuarioId = p.UsuarioId,
                        Tipo = "Recordatorio",
                        Mensaje = $"El préstamo del libro '{p.Libro.Titulo}' vence en 2 días.",
                        FechaEnvio = DateTime.Now,
                        Enviado = false
                    };
                    await _context.Notificacion.AddAsync(notificacion);
                }

                var count = await _context.SaveChangesAsync();

                return new OperationResult<int>
                {
                    Success = true,
                    Data = count,
                    Message = $"{count} notificaciones generadas por vencimientos próximos."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar notificaciones previas.");
                return new OperationResult<int>
                {
                    Success = false,
                    Message = "Error al generar notificaciones previas."
                };
            }
        }
        #endregion

        #region RF4.2 - Notificación el día del vencimiento
        public async Task<OperationResult<int>> GenerarNotificacionesDiaVencimientoAsync()
        {
            try
            {
                var prestamos = await _context.Prestamo
                    .Include(p => p.Usuario)
                    .Include(p => p.Libro)
                    .Where(p => p.Estado == "Activo" && p.FechaVencimiento.Date == DateTime.Today)
                    .ToListAsync();

                if (!prestamos.Any())
                    return new OperationResult<int> { Success = false, Message = "No hay préstamos que venzan hoy." };

                foreach (var p in prestamos)
                {
                    var notificacion = new Notificacion
                    {
                        UsuarioId = p.UsuarioId,
                        Tipo = "Vencimiento",
                        Mensaje = $"El préstamo del libro '{p.Libro.Titulo}' vence hoy.",
                        FechaEnvio = DateTime.Now,
                        Enviado = false
                    };
                    await _context.Notificacion.AddAsync(notificacion);
                }

                var count = await _context.SaveChangesAsync();

                return new OperationResult<int>
                {
                    Success = true,
                    Data = count,
                    Message = $"{count} notificaciones de vencimiento generadas."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar notificaciones del día de vencimiento.");
                return new OperationResult<int>
                {
                    Success = false,
                    Message = "Error al generar notificaciones del día de vencimiento."
                };
            }
        }
        #endregion

        #region RF4.3 - Notificación por penalización (retraso)
        public async Task<OperationResult<int>> GenerarNotificacionesPorPenalizacionAsync()
        {
            try
            {
                var prestamos = await _context.Prestamo
                    .Include(p => p.Usuario)
                    .Include(p => p.Libro)
                    .Where(p => p.Estado == "Activo" && p.FechaVencimiento.Date < DateTime.Today)
                    .ToListAsync();

                if (!prestamos.Any())
                    return new OperationResult<int> { Success = false, Message = "No hay préstamos con retraso." };

                foreach (var p in prestamos)
                {
                    var diasRetraso = (DateTime.Today - p.FechaVencimiento.Date).Days;

                    var notificacion = new Notificacion
                    {
                        UsuarioId = p.UsuarioId,
                        Tipo = "Penalización",
                        Mensaje = $"Tiene {diasRetraso} día(s) de retraso en la devolución del libro '{p.Libro.Titulo}'.",
                        FechaEnvio = DateTime.Now,
                        Enviado = false
                    };
                    await _context.Notificacion.AddAsync(notificacion);
                }

                var count = await _context.SaveChangesAsync();

                return new OperationResult<int>
                {
                    Success = true,
                    Data = count,
                    Message = $"{count} notificaciones de penalización generadas."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar notificaciones por penalización.");
                return new OperationResult<int>
                {
                    Success = false,
                    Message = "Error al generar notificaciones por penalización."
                };
            }
        }

        #region Métodos de consulta y actualización general

       
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
                    Data = notificaciones,
                    Message = $"Se encontraron {notificaciones.Count} notificaciones del usuario {usuarioId}."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener notificaciones del usuario {usuarioId}.");
                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = false,
                    Message = "Error al obtener notificaciones por usuario."
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
                    Data = notificaciones,
                    Message = $"Se encontraron {notificaciones.Count} notificaciones no leídas del usuario {usuarioId}."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener no leídas del usuario {usuarioId}.");
                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = false,
                    Message = "Error al obtener notificaciones no leídas."
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
                    Data = pendientes,
                    Message = $"Se encontraron {pendientes.Count} notificaciones pendientes."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notificaciones pendientes.");
                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = false,
                    Message = "Error al obtener notificaciones pendientes."
                };
            }
        }

       
        public async Task<OperationResult<IEnumerable<Notificacion>>> ObtenerPorTipoAsync(string tipo)
        {
            try
            {
                var notificaciones = await _context.Notificacion
                    .Where(n => n.Tipo.ToLower() == tipo.ToLower())
                    .OrderByDescending(n => n.FechaEnvio)
                    .ToListAsync();

                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = true,
                    Data = notificaciones,
                    Message = $"Se encontraron {notificaciones.Count} notificaciones de tipo '{tipo}'."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener notificaciones del tipo {tipo}.");
                return new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = false,
                    Message = "Error al obtener notificaciones por tipo."
                };
            }
        }

      
        public async Task<OperationResult<bool>> MarcarComoEnviadaAsync(int notificacionId)
        {
            try
            {
                var notificacion = await _context.Notificacion.FindAsync(notificacionId);

                if (notificacion == null)
                    return new OperationResult<bool> { Success = false, Message = "Notificación no encontrada." };

                if (notificacion.Enviado)
                    return new OperationResult<bool> { Success = false, Message = "La notificación ya está marcada como enviada." };

                notificacion.Enviado = true;
                await _context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Notificación marcada como enviada."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al marcar como enviada la notificación {notificacionId}.");
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = "Error al marcar la notificación como enviada."
                };
            }
        }

        public async Task<OperationResult<int>> MarcarTodasComoEnviadasPorUsuarioAsync(int usuarioId)
        {
            try
            {
                var pendientes = await _context.Notificacion
                    .Where(n => n.UsuarioId == usuarioId && !n.Enviado)
                    .ToListAsync();

                if (!pendientes.Any())
                    return new OperationResult<int> { Success = false, Message = "No hay notificaciones pendientes para este usuario." };

                pendientes.ForEach(n => n.Enviado = true);
                var count = await _context.SaveChangesAsync();

                return new OperationResult<int>
                {
                    Success = true,
                    Data = count,
                    Message = $"{count} notificaciones marcadas como enviadas para el usuario {usuarioId}."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al marcar todas como enviadas para usuario {usuarioId}.");
                return new OperationResult<int>
                {
                    Success = false,
                    Message = "Error al actualizar notificaciones pendientes."
                };
            }
        }

        #endregion

        #endregion
    }
}
