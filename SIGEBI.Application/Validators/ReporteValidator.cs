using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using System.Globalization;
using System.Text;

namespace SIGEBI.Application.Validators
{
    public static class ReporteValidator
    {
        public static OperationResult<Reporte> Validar(Reporte entity)
        {
            if (entity == null)
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "El reporte no puede ser nulo."
                };

           
            var tiposValidos = new[]
            {
                "prestamos",
                "usuarios activos",
                "ejemplares",
                "penalizaciones",
                "devoluciones",
                "libros mas prestados"
            };

          
            string tipoNormalizado = RemoverTildes(entity.Tipo?.ToLower() ?? string.Empty);

         
            if (!tiposValidos.Contains(tipoNormalizado))
            {
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "Tipo de reporte no válido. Debe ser Préstamos, Usuarios Activos, Ejemplares, Libros Más Prestados o Penalizaciones."
                };
            }

          
            if (entity.UsuarioId <= 0)
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "Debe indicar el usuario que generó el reporte."
                };

          
            if (string.IsNullOrWhiteSpace(entity.Tipo))
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "El tipo de reporte es obligatorio."
                };

           
            if (string.IsNullOrWhiteSpace(entity.Contenido) &&
                tipoNormalizado != "prestamos" &&
                tipoNormalizado != "usuarios activos" &&
                tipoNormalizado != "penalizaciones" &&
                tipoNormalizado != "devoluciones" &&
                tipoNormalizado != "libros mas prestados")
            {
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "El contenido del reporte no puede estar vacío."
                };
            }

          
            if (entity.FechaGeneracion == default)
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "La fecha de generación no es válida."
                };

            return new OperationResult<Reporte> { Success = true, Data = entity };
        }

        
        private static string RemoverTildes(string texto)
        {
            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}