using SIGEBI.Domain.Entitines.Configuration.Security;

namespace SIGEBI.Domain.Entitines.Configuration.Notificaciones
{
    public sealed class Notificacion : Base.BaseEntity
    {
        public string Tipo { get; set; } = "Aviso";

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public string Mensaje { get; set; } = null!;
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
        public bool Enviado { get; set; } = false;
    }
}
