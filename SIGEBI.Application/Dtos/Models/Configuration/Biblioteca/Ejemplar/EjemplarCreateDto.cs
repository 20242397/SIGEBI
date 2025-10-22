

using SIGEBI.Domain.Entitines.Configuration.Biblioteca;

namespace SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar
{
    public record EjemplarCreateDto
    {
        public string CodigoBarras { get; set; } = null!;
        public int LibroId { get; set; }

        public string Estado { get; set; } = "Disponible";
    }
}
