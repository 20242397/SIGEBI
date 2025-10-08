using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Reporte;

namespace SIGEBI.Application.Interfaces
{
    public interface IReporteService
    {
        // RF5.1–RF5.3 - Generar un nuevo reporte según tipo o parámetros
        Task<ServiceResult<T>> GenerarReporteAsync<T>(ReporteCreateDto dto);

        // RF5.4 - Exportar reportes en PDF o Excel
        Task<ServiceResult<T>> ExportarReporteAsync<T>(int reporteId, string formato);

        // Editar un reporte existente (por ejemplo, marcarlo como resuelto)
        Task<ServiceResult<T>> ActualizarReporteAsync<T>(ReporteUpdateDto dto);

        // Obtener reportes según filtros
        Task<ServiceResult<T>> ObtenerPorFechaAsync<T>(DateTime inicio, DateTime fin);
        Task<ServiceResult<T>> ObtenerPorTipoAsync<T>(string tipo);

        // Consultar todos los reportes o uno específico
        Task<ServiceResult<T>> ObtenerTodosAsync<T>();
        Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id);
    }
}

