


namespace SIGEBI.Application.Dtos.Models.Configuration.Notificacion
{
    public record NotificacionCreateDto
    {
        public int UsuarioId { get; set; }
        public string Tipo { get; set; } = "Aviso";
        public string Mensaje { get; set; } = null!;
        public DateTime? FechaEnvio { get;  set; }

      
    }
}
