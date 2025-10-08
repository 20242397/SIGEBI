namespace SIGEBI.Domain.Entitines.Configuration.Biblioteca
{
    public enum EstadoEjemplar
    {
        Disponible,
        Prestado,
        Reservado,
        Perdido,
        Dañado
    }

    public sealed class Ejemplar : Base.BaseEntity
    {
        public string CodigoBarras { get; set; } = null!;
        public EstadoEjemplar Estado { get; set; } = EstadoEjemplar.Disponible;
        public int LibroId { get; set; }
        public Libro Libro { get; set; } = null!;
    }
}
