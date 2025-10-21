﻿

namespace SIGEBI.Domain.Entitines.Configuration.Reportes
{
    public sealed class Reporte : Base.BaseEntity
    {
        
        public int? UsuarioId { get; set; }

        public string Tipo { get; set; } = string.Empty; // Prestamos, Libros, Usuarios, etc.
        public string? Contenido { get; set; }

        public DateTime FechaGeneracion { get; set; } = DateTime.Now;

    }
}
