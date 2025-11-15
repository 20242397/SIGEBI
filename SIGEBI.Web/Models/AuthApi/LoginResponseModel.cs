namespace SIGEBI.Web.Models.AuthApi
{
    public class LoginResponseModel
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
