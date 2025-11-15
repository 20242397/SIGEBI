using System.ComponentModel.DataAnnotations;

namespace SIGEBI.Web.Models.EjemplarApi
{
    
        public class EjemplarApiUpdateModel
        {
            public int Id { get; set; }

            public string Estado { get; set; } = string.Empty;
        }

    
}
