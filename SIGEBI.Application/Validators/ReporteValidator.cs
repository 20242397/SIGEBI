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

            // Lista normalizada (sin tildes, en minúsculas)
            var tiposValidos = new[]
            {
                "prestamos",
                "usuarios activos",
                "ejemplares",
                "penalizaciones",
                "libros mas prestados"
            };

            // Normalizar el texto recibido (minúsculas, sin tildes)
            string tipoNormalizado = RemoverTildes(entity.Tipo?.ToLower() ?? string.Empty);

            //  Validar tipo
            if (!tiposValidos.Contains(tipoNormalizado))
            {
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "Tipo de reporte no válido. Debe ser Préstamos, Usuarios Activos, Ejemplares, Libros Más Prestados o Penalizaciones."
                };
            }

            // Validar usuario
            if (entity.UsuarioId <= 0)
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "Debe indicar el usuario que generó el reporte."
                };

            // Validar tipo (no vacío)
            if (string.IsNullOrWhiteSpace(entity.Tipo))
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "El tipo de reporte es obligatorio."
                };

            // Validar contenido (solo si no es un tipo de reporte generado automáticamente)
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

            // Validar fecha
            if (entity.FechaGeneracion == default)
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "La fecha de generación no es válida."
                };

            return new OperationResult<Reporte> { Success = true, Data = entity };
        }

        // Método auxiliar para remover tildes
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
