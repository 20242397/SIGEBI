

namespace SIGEBI.Application.Dtos.Models.Configuration.Usuario
{
    public record UsuarioUpdateDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; } = null; 

        public string? Estado { get; set; }
        public dynamic Activo { get; internal set; }
    }
}
