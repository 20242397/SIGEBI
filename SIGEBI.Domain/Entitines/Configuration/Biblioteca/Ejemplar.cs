using System.ComponentModel.DataAnnotations.Schema;

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

   [Table("Ejemplar")]

    public sealed class Ejemplar : Base.BaseEntity
    {
        
        public int LibroId { get; set; }
        public string CodigoBarras { get; set; } = string.Empty;

        public EstadoEjemplar Estado { get; set; } = EstadoEjemplar.Disponible;

        // Propiedad de navegación opcional (solo lectura)
        public Libro? Libro { get; set; }
    }
}

