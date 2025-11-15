using System.ComponentModel.DataAnnotations;

namespace SIGEBI.Web.Models.LibroApi
{
    public class LibroApiUpdateModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio.")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ISBN es obligatorio.")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "La editorial es obligatoria.")]
        public string Editorial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(1500, 2100)]
        public int AnioPublicacion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public string Categoria { get; set; } = string.Empty;

        public string Estado { get; set; } = "Disponible";
    }
}
