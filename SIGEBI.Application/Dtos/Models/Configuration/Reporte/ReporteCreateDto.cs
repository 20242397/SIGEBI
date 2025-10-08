using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Application.Dtos.Models.Configuration.Reporte
{
    public record ReporteCreateDto : IEquatable<ReporteCreateDto>
    {
        public int UsuarioId { get; set; }
        public string Tipo { get; set; } = null!;
        public string Contenido { get; set; } = null!;
        public int? TopN { get; set; } // Add this property to fix CS1061
        public DateTime FechaFin { get;  set; }
        public DateTime FechaInicio { get;  set; }
    }
}
