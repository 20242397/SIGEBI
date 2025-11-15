using System.ComponentModel.DataAnnotations;

namespace SIGEBI.Web.Models.EjemplarApi
{
    public class EjemplarApiCreateModel
    {
        [Required(ErrorMessage = "El código de barras es obligatorio.")]
        [Display(Name = "Código de Barras")]
        public string CodigoBarras { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Disponible";

        [Required(ErrorMessage = "Debe asociar un libro.")]
        [Display(Name = "Libro Id")]
        public int LibroId { get; set; }
    }
}
