using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;

namespace SIGEBI.Application.Validators
{
    public static class ReporteValidator
    {
        public static OperationResult<Reporte> Validar(Reporte entity)
        {
            if (entity == null)
                return new OperationResult<Reporte> { Success = false, Message = "El reporte no puede ser nulo." };

            if (entity.UsuarioId <= 0)
                return new OperationResult<Reporte> { Success = false, Message = "Debe indicar el usuario que generó el reporte." };

            if (string.IsNullOrWhiteSpace(entity.Tipo))
                return new OperationResult<Reporte> { Success = false, Message = "El tipo de reporte es obligatorio." };

            if (string.IsNullOrWhiteSpace(entity.Contenido))
                return new OperationResult<Reporte> { Success = false, Message = "El contenido del reporte no puede estar vacío." };

            if (entity.FechaGeneracion == default)
                return new OperationResult<Reporte> { Success = false, Message = "La fecha de generación no es válida." };

            return new OperationResult<Reporte> { Success = true, Data = entity };
        }
    }
}
