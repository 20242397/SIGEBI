using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;

namespace SIGEBI.Application.Validators
{
    public static class NotificacionValidator
    {
        public static OperationResult<Notificacion> Validar(Notificacion entity)
        {
            if (entity == null)
                return new OperationResult<Notificacion> { Success = false, Message = "La notificación no puede ser nula." };

            if (string.IsNullOrWhiteSpace(entity.Tipo))
                return new OperationResult<Notificacion> { Success = false, Message = "El tipo de notificación es obligatorio." };

            if (entity.UsuarioId <= 0)
                return new OperationResult<Notificacion> { Success = false, Message = "Debe asociar la notificación a un usuario válido." };

            if (string.IsNullOrWhiteSpace(entity.Mensaje))
                return new OperationResult<Notificacion> { Success = false, Message = "El mensaje no puede estar vacío." };

            if (entity.FechaEnvio == default)
                return new OperationResult<Notificacion> { Success = false, Message = "La fecha de envío no es válida." };

            var tiposValidos = new[] { "Préstamo", "Devolución", "Penalización", "Recordatorio" };
            if (!tiposValidos.Contains(entity.Tipo))
                return new OperationResult<Notificacion>
                {
                    Success = false,
                    Message = "El tipo de notificación no es válido. Debe ser: Préstamo, Devolución, Penalización o Recordatorio."
                };


            return new OperationResult<Notificacion> { Success = true, Data = entity };
        }
    }
}
