using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;
using SIGEBI.Domain.Repository;
using SIGEBI.Infrastructure.Logging;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Repositories.Configuration.NotificacionesRepositories
{
    public sealed class NotificacionRepository : BaseRepository<Notificacion>, IBaseRepository<Notificacion>
    {
        public NotificacionRepository(SIGEBIContext context, ILoggerService logger)
            : base(context, logger) { }

        public override async Task<OperationResult<Notificacion>> AddAsync(Notificacion entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Mensaje))
                return new OperationResult<Notificacion> { Success = false, Message = "El mensaje es obligatorio." };

            if (entity.FechaEnvio < DateTime.UtcNow)
                return new OperationResult<Notificacion> { Success = false, Message = "La fecha no puede estar en el pasado." };

            if (!new[] { "Aviso", "Vencimiento", "Penalizacion" }.Contains(entity.Tipo))
                return new OperationResult<Notificacion> { Success = false, Message = "El tipo de notificación no es válido." };

            return await base.AddAsync(entity);
        }

        public override async Task<OperationResult<Notificacion>> UpdateAsync(Notificacion entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Mensaje))
                return new OperationResult<Notificacion> { Success = false, Message = "El mensaje es obligatorio." };

            return await base.UpdateAsync(entity);
        }
    }
}
