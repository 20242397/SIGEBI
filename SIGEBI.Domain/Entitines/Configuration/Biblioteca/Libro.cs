namespace SIGEBI.Domain.Entitines.Configuration.Biblioteca
{
    public sealed  class Libro : Base.BaseEntity
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string? Editorial { get; set; }
        public int? AñoPublicacion { get; set; }

        // Estado de disponibilidad
        public string Estado { get; set; } = "Disponible"; // Disponible / Prestado / Reservado / Dañado
        
    }
}
