using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;

namespace SIGEBI.Application.Validators
{
    public static class LibroValidator
    {
        public static OperationResult<Libro> Validar(Libro libro)
        {
            if (string.IsNullOrWhiteSpace(libro.Titulo))
                return new OperationResult<Libro> { Success = false, Message = "El título es obligatorio" };

            if (string.IsNullOrWhiteSpace(libro.Autor))
                return new OperationResult<Libro> { Success = false, Message = "El autor es obligatorio" };

            if (string.IsNullOrWhiteSpace(libro.ISBN))
                return new OperationResult<Libro> { Success = false, Message = "El ISBN es obligatorio" };

            if (libro.AñoPublicacion <= 0 || libro.AñoPublicacion > DateTime.Now.Year)
                return new OperationResult<Libro> { Success = false, Message = "El año de publicación no es válido" };

            if (string.IsNullOrWhiteSpace(libro.Editorial))
                return new OperationResult<Libro> { Success = false, Message = "La editorial es obligatoria." };

            if (libro.ISBN.Length != 10 && libro.ISBN.Length != 13)
                return new OperationResult<Libro> { Success = false, Message = "El ISBN debe tener 10 o 13 caracteres." };

            var estadosValidos = new[] { "Disponible", "Prestado", "Reservado", "Dañado", "Inactivo" };
            if (string.IsNullOrWhiteSpace(libro.Estado) || !estadosValidos.Contains(libro.Estado))
                return new OperationResult<Libro> { Success = false, Message = "El estado del libro no es válido." };


            return new OperationResult<Libro> { Success = true, Data = libro };
        }
    }
}