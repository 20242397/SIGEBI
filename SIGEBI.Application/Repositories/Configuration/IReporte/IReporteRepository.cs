using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration.Reportes
{
    public interface IReporteRepository : IBaseRepository<Reporte>
    {
        // 🔹 Consultas
        Task<OperationResult<IEnumerable<Reporte>>> ObtenerReportesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<OperationResult<IEnumerable<Reporte>>> ObtenerReportesPorTipoAsync(string tipo);
        Task<OperationResult<IEnumerable<Reporte>>> ObtenerReportesPendientesAsync();

        // 🔹 Generación de reportes
        Task<OperationResult<Reporte>> GenerarReportePrestamosAsync(DateTime inicio, DateTime fin);
        
        Task<OperationResult<Reporte>> GenerarReporteUsuariosActivosAsync();

        // 🔹 Actualización
        Task<OperationResult<bool>> MarcarComoResueltoAsync(int reporteId);
        Task<OperationResult<Reporte>> GenerarReportePrestamosAsync(object fechaInicio, object fechaFin);
    }
}

