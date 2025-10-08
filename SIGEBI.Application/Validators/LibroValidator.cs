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

            return new OperationResult<Libro> { Success = true, Data = libro };
        }
    }
}
