namespace SIGEBI.Domain.Entitines.Configuration.Biblioteca
{
    public enum EstadoEjemplar
    {
        Disponible,
        Prestado,
        Reservado
    }

    public sealed class Ejemplar : Base.BaseEntity
    {
        public string CodigoBarras { get; set; } = null!;
        public string Estado { get; set; } = "Disponible"; // Disponible, Prestado, Reservado.
        public int LibroId { get; set; }
        public Libro Libro { get; set; } = null!;
    }
}
