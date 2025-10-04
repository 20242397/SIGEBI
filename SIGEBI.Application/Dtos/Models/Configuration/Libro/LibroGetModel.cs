

namespace SIGEBI.Application.Dtos.Models.Configuration.Libro
{
    public class LibroGetModel
    {

        public record LibroCreateDto(
            string Titulo,
            string Autor,
            string ISBN,
            string Editorial,
            int AñoPublicacion,
            string? Categoria
        );

       
        public record LibroUpdateDto(
            int Id,
            string Titulo,
            string Autor,
            string ISBN,
            string Editorial,
            int AñoPublicacion,
            string? Categoria
        );


        public record LibroSearchDto(string? Titulo, string? Autor, string? Categoria, string? ISBN);

        public record LibroGetDto(
            int Id,
            string Titulo,
            string Autor,
            string ISBN,
            string Editorial,
            int AñoPublicacion,
            string? Categoria,
            string Estado  // Disponible, Prestado, Reservado
        );
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string ISBN { get; set; }
        public string Editorial { get; set; }
        public int AñoPublicacion { get; set; }
        public string? Categoria { get; set; }
   
    }
}
