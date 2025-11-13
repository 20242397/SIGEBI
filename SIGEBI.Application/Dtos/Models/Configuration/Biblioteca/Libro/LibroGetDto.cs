namespace SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro
{

    public record LibroGetDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Autor { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public string Editorial { get; set; } = null!;
        public int AñoPublicacion { get; set; }
        public string? Categoria { get; set; }
        public string Estado { get; set; } = "Disponible";
    }
}