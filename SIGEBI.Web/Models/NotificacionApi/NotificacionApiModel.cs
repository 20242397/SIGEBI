namespace SIGEBI.Web.Models.NotificacionApi
{
    public class NotificacionApiModel
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "Aviso";
        public int UsuarioId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public bool Enviado { get; set; }
    }
}
