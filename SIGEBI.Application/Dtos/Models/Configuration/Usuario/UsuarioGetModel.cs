

namespace SIGEBI.Application.Dtos.Models.Configuration.Usuario
{
    public  class UsuarioGetModel
    {
        public int Id { get; internal set; }
        public string Nombre { get; internal set; }
        public string Apellido { get; internal set; }
        public string? PhoneNumber { get; internal set; }
        public string Email { get; internal set; }
        public string? Role { get; internal set; }
        public string Estado { get; internal set; }

        public record UsuarioCreateDto(
            string Nombre,
            string Apellido,
            string Email,
            string PasswordHash,
            string? PhoneNumber,
            string? Role,
            string Estado = "Activo"
        )
        {
            internal Domain.Entitines.Configuration.Security.Usuario ToEntity()
            {
                throw new NotImplementedException();
            }
        }

        public record UsuarioUpdateDto(
            int Id,
            string Nombre,
            string Apellido,
            string Email,
            string? PhoneNumber,
            string? Role,
            string Estado
        )
        {
            internal Domain.Entitines.Configuration.Security.Usuario ToEntity()
            {
                throw new NotImplementedException();
            }
        }

        public record UsuarioRoleDto(int UsuarioId, string Rol);

        public record UsuarioEstadoDto(int UsuarioId, string Estado); // Activo o Inactivo

        public record UsuarioHistorialPrestamosDto(
            int UsuarioId,
            IEnumerable<int> PrestamoIds
        );
    }

}
