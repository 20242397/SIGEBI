
namespace SIGEBI.Application.Dtos.Models.Configuration.Usuario
{
    public record UsuarioCreateDto
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; } = "User";

        public bool Activo { get; set; } = true;
    }
}