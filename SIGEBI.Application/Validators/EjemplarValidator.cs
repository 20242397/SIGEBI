using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using System;
using System.Linq;

namespace SIGEBI.Application.Validators
{
    public static class EjemplarValidator
    {
        public static OperationResult<Ejemplar> Validar(Ejemplar entity)
        {
            if (entity == null)
                return new OperationResult<Ejemplar> { Success = false, Message = "El ejemplar no puede ser nulo." };


            if (string.IsNullOrWhiteSpace(entity.CodigoBarras))
                return new OperationResult<Ejemplar> { Success = false, Message = "El código de barras es obligatorio." };

            // Validar formato del código de barras (ejemplo: alfanumérico entre 8 y 13 caracteres)
            if (!System.Text.RegularExpressions.Regex.IsMatch(entity.CodigoBarras, @"^[A-Za-z0-9]{8,13}$"))
                return new OperationResult<Ejemplar>
                {
                    Success = false,
                    Message = "El formato del código de barras no es válido. Debe tener entre 8 y 13 caracteres alfanuméricos."
                };

            if (!new[]
{
                  EstadoEjemplar.Disponible,
                  EstadoEjemplar.Prestado,
                  EstadoEjemplar.Reservado,
                  EstadoEjemplar.Dañado,
                  EstadoEjemplar.Perdido
            }.Contains(entity.Estado))
                return new OperationResult<Ejemplar>
                {
                    Success = false,
                    Message = "El estado del ejemplar no es válido."
                };



            // Fix: Use EstadoEjemplar enum array for comparison
            if (!new[] { EstadoEjemplar.Disponible, EstadoEjemplar.Prestado, EstadoEjemplar.Reservado }.Contains(entity.Estado))
                return new OperationResult<Ejemplar> { Success = false, Message = "El estado del ejemplar no es válido." };

            if (entity.LibroId <= 0)
                return new OperationResult<Ejemplar> { Success = false, Message = "El ejemplar debe estar asociado a un libro válido." };

            return new OperationResult<Ejemplar> { Success = true, Data = entity };
        }
    }
}
