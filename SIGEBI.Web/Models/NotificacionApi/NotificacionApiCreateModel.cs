namespace SIGEBI.Web.Models.NotificacionApi
{
    public class NotificacionApiCreateModel
    {
        public int UsuarioId { get; set; }
        public string Tipo { get; set; } = "Aviso";
        public string Mensaje { get; set; } = string.Empty;
        public DateTime? FechaEnvio { get; set; }
    }
}
