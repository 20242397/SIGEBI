using SIGEBI.Domain.Entitines.Configuration.Prestamos;

namespace SIGEBI.Domain.Entitines.Configuration.Security
{
    public enum UserRole
    {
        Admin,
        User,
        Bibliotecario,
    }
    public sealed class Usuario : Base.BaseEntity
    {
        public string Estado { get; set; } = "Activo"; // Activo, Inactivo

        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? PhoneNumber { get; set; }

        public string? Role { get; set; } = UserRole.User.ToString();

        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
        public bool Activo { get; set; }
    }
}
