using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Persistence.Models.Configuration.Libro
{
    public record LibroGetModel
    {
       
            public int Id { get; set; }
            public string Titulo { get; set; } 
            public string Autor { get; set; } 
            public string ISBN { get; set; } 
            public string Editorial { get; set; } 
            public int AñoPublicacion { get; set; }
            public string? Categoria { get; set; }
        
    }

}
