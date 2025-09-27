namespace SIGEBI.Domain.Entitines.Configuration.Reportes
{
    public sealed class Reporte : Base.BaseEntity
    {
        public string Tipo { get; set; } = null!; // Ejemplos: "Libros Prestados", "Usuarios Activos"
        public string Contenido { get; set; } = null!; 
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
    }
}
