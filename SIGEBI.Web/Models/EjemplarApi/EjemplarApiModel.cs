using System.ComponentModel.DataAnnotations;

namespace SIGEBI.Web.Models.EjemplarApi
{
    public class EjemplarApiModel
    {
        public int Id { get; set; }

        
        public string CodigoBarras { get; set; } = string.Empty;

       
        public string Estado { get; set; } = string.Empty;


        public int LibroId { get; set; }
    }
}
