

namespace SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar
{
    public record EjemplarGetDto
    {
        public int Id { get; set; }
        public string CodigoBarras { get; set; } = null!;
        public string Estado { get; set; } = "Disponible";
        public int LibroId { get; set; }
    }
}
