using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;

namespace SIGEBI.Application.Validators
{
    public static class PrestamoValidator
    {
        public static OperationResult<Prestamo> Validar(Prestamo prestamo)
        {
            if (prestamo.UsuarioId <= 0)
                return new OperationResult<Prestamo> { Success = false, Message = "El usuario es obligatorio" };

            if (prestamo.EjemplarId <= 0)
                return new OperationResult<Prestamo> { Success = false, Message = "El ejemplar es obligatorio" };

            if (string.IsNullOrWhiteSpace(prestamo.LibroId))
                return new OperationResult<Prestamo> { Success = false, Message = "El libro es obligatorio" };


            if (prestamo.FechaVencimiento <= prestamo.FechaPrestamo)
                return new OperationResult<Prestamo> { Success = false, Message = "La fecha de vencimiento debe ser mayor que la de préstamo" };

            return new OperationResult<Prestamo> { Success = true, Data = prestamo };
        }
    }
}

