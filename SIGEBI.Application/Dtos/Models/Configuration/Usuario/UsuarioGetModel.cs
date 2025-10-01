using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Entitines.Configuration.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Persistence.Models.Configuration.Usuario
{
    public record UsuarioGetModel
    {
        public int Id { get; set; }

        public string Estado { get; set; } = "Activo"; // Activo, Inactivo

        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? PhoneNumber { get; set; }

        public string? Role { get; set; } = UserRole.User.ToString();

        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    }

}

