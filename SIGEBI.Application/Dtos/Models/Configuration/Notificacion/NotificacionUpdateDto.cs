
namespace SIGEBI.Application.Dtos.Models.Configuration.Notificacion
{
    public record NotificacionUpdateDto
    {
        public int Id { get; set; }
        public bool Enviado { get; set; }
    }
}
