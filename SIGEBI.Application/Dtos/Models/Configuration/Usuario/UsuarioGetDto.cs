

namespace SIGEBI.Application.Dtos.Models.Configuration.Usuario
{
    public record UsuarioGetDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public bool Activo { get; set; }
        public string Estado { get; set; } = "Activo";
    }
}
