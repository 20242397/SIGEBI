
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGEBI.Domain.Entitines.Configuration.Notificaciones
{
    [Table("Notificacion")]

    public sealed class Notificacion : Base.BaseEntity
    {
       
        public int UsuarioId { get; set; }

        public string Tipo { get; set; } = "Aviso"; // Aviso, Recordatorio, Penalización
        public string Mensaje { get; set; } = string.Empty;

        public DateTime FechaEnvio { get; set; } = DateTime.Now;
        public bool Enviado { get; set; } = false;

       
    }
}
