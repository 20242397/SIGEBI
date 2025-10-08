namespace SIGEBI.Domain.Entitines.Configuration.Biblioteca
{
    public sealed  class Libro : Base.BaseEntity
    {
        public string Titulo { get; set; } = null!;
        public string Autor { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public string Editorial { get; set; } = null!;
        public int AñoPublicacion { get; set; }

        public string ? Categoria { get; set; }

        //relacion con ejemplares
        public ICollection<Ejemplar> Ejemplares { get; set; } = new List<Ejemplar>();
        public string Estado { get; set; } 
    }
}
