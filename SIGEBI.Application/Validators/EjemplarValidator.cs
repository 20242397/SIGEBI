using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;

public static class EjemplarValidator
{
    public static OperationResult<Ejemplar> Validar(Ejemplar entity)
    {
        if (entity == null)
            return new OperationResult<Ejemplar> { Success = false, Message = "El ejemplar no puede ser nulo." };

       
        if (string.IsNullOrWhiteSpace(entity.CodigoBarras))
            return new OperationResult<Ejemplar> { Success = false, Message = "El código de barras es obligatorio." };

        if (!System.Text.RegularExpressions.Regex.IsMatch(entity.CodigoBarras, @"^[A-Za-z0-9\-]{4,20}$"))
            return new OperationResult<Ejemplar>
            {
                Success = false,
                Message = "El formato del código de barras no es válido."
            };

       
        var estadosValidos = new[]
        {
            EstadoEjemplar.Disponible,
            EstadoEjemplar.Prestado,
            EstadoEjemplar.Reservado,
            EstadoEjemplar.Dañado,
            EstadoEjemplar.Perdido
        };

        if (!estadosValidos.Contains(entity.Estado))
            return new OperationResult<Ejemplar>
            {
                Success = false,
                Message = "El estado del ejemplar no es válido."
            };

        
        if (entity.LibroId <= 0)
            return new OperationResult<Ejemplar> { Success = false, Message = "Debe asociarse a un libro válido." };

        return new OperationResult<Ejemplar> { Success = true, Data = entity };
    }
}
