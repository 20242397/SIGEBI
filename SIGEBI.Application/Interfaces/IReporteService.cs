using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Reporte;

namespace SIGEBI.Application.Interfaces
{
    public interface IReporteService
    {
     
        Task<ServiceResult<T>> GenerarReporteAsync<T>(ReporteCreateDto dto);
        Task<ServiceResult<T>> ExportarReporteAsync<T>(int reporteId, string formato);
        Task<ServiceResult<T>> ActualizarReporteAsync<T>(ReporteUpdateDto dto);
        Task<ServiceResult<T>> ObtenerPorFechaAsync<T>(DateTime inicio, DateTime fin);
        Task<ServiceResult<T>> ObtenerPorTipoAsync<T>(string tipo);
        Task<ServiceResult<T>> ObtenerTodosAsync<T>();
        Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id);
    }
}

