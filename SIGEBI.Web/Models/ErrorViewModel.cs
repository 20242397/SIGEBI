namespace SIGEBI.Web.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // Agregar esta propiedad para almacenar el mensaje de error
        public string? Message { get; set; }
    }
}