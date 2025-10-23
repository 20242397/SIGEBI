using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using System.Text.RegularExpressions;

namespace SIGEBI.Application.Validators
{
    public static class UsuarioValidator
    {
        public static OperationResult<Usuario> Validar(Usuario usuario, bool esNuevo = true)
        {
            if (usuario == null)
                return new OperationResult<Usuario> { Success = false, Message = "El usuario no puede ser nulo." };

            if (string.IsNullOrWhiteSpace(usuario.Nombre) || usuario.Nombre.Length < 2)
                return new OperationResult<Usuario> { Success = false, Message = "El nombre es obligatorio y debe tener al menos 2 caracteres." };

            if (string.IsNullOrWhiteSpace(usuario.Apellido) || usuario.Apellido.Length < 2)
                return new OperationResult<Usuario> { Success = false, Message = "El apellido es obligatorio y debe tener al menos 2 caracteres." };

            if (string.IsNullOrWhiteSpace(usuario.Email) ||
                !Regex.IsMatch(usuario.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return new OperationResult<Usuario> { Success = false, Message = "El correo electrónico no es válido." };

            if (esNuevo && string.IsNullOrWhiteSpace(usuario.PasswordHash))
                return new OperationResult<Usuario> { Success = false, Message = "La contraseña es obligatoria al registrar un usuario nuevo." };

            if (!string.IsNullOrWhiteSpace(usuario.PhoneNumber) &&
                !Regex.IsMatch(usuario.PhoneNumber, @"^\+?[0-9]{8,15}$"))
                return new OperationResult<Usuario> { Success = false, Message = "El número de teléfono no tiene un formato válido." };

            var rolesValidos = new[] { "Admin", "Docente", "Estudiante" };
            if (string.IsNullOrWhiteSpace(usuario.Role) || !rolesValidos.Contains(usuario.Role))
                return new OperationResult<Usuario> { Success = false, Message = "El rol del usuario no es válido." };

            var estadosValidos = new[] { "Activo", "Inactivo" };
            if (string.IsNullOrWhiteSpace(usuario.Estado) || !estadosValidos.Contains(usuario.Estado))
                return new OperationResult<Usuario> { Success = false, Message = "El estado del usuario no es válido." };

            return new OperationResult<Usuario> { Success = true, Data = usuario };
        }
    }
}
