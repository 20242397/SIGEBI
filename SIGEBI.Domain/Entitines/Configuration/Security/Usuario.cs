using SIGEBI.Domain.Entitines.Configuration.Prestamos;

namespace SIGEBI.Domain.Entitines.Configuration.Security
{
    public enum UserRole
    {
        Admin,
        Estudiante,
        Docente,
    }
    public sealed class Usuario : Base.BaseEntity
    {
        public int Id { get; set; }

        // Datos personales
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        // Control de acceso
        public string Rol { get; set; } = "Estudiante"; // Admin / Docente / Estudiante
        public string Estado { get; set; } = "Activo";  // Activo / Inactivo
        public bool Activo { get; set; } = true;
    }
}
