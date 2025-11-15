using System.ComponentModel.DataAnnotations;

namespace SIGEBI.Web.Models.UsuarioApi
{
    public class UsuarioApiUpdateModel
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string Estado { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }
}
