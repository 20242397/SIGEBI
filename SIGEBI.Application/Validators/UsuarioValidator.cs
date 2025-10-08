using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using System.Text.RegularExpressions;

namespace SIGEBI.Application.Validators
{
    public static class UsuarioValidator
    {
        public static OperationResult<Usuario> Validar(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                return new OperationResult<Usuario> { Success = false, Message = "El nombre es obligatorio" };

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                return new OperationResult<Usuario> { Success = false, Message = "El apellido es obligatorio" };

            if (string.IsNullOrWhiteSpace(usuario.Email) ||
                !Regex.IsMatch(usuario.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return new OperationResult<Usuario> { Success = false, Message = "El email no es válido" };

            if (string.IsNullOrWhiteSpace(usuario.PasswordHash))
                return new OperationResult<Usuario> { Success = false, Message = "La contraseña es obligatoria" };

            return new OperationResult<Usuario> { Success = true, Data = usuario };
        }
    }
}
