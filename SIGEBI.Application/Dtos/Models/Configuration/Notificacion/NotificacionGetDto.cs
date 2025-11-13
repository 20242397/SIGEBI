


namespace SIGEBI.Application.Dtos.Models.Configuration.Notificacion
{
    public record NotificacionGetDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "Aviso";
        public int UsuarioId { get; set; }
        public string Mensaje { get; set; } = null!;
        public DateTime FechaEnvio { get; set; }
        public bool Enviado { get; set; }
    }
}