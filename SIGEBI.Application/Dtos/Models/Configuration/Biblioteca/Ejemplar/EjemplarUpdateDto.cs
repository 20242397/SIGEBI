

namespace SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar
{
    public record EjemplarUpdateDto
    {
        public int Id { get; set; }
        public string Estado { get; set; } = "Disponible";
    }
}
