using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration.Reportes
{
    public interface IReporteRepository : IBaseRepository<Reporte>
    {

        Task<OperationResult<IEnumerable<Reporte>>> ObtenerReportesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<OperationResult<IEnumerable<Reporte>>> ObtenerReportesPorTipoAsync(string tipo);
        Task<OperationResult<IEnumerable<Reporte>>> ObtenerReportesPendientesAsync();


        Task<OperationResult<Reporte>> GenerarReportePrestamosAsync(DateTime fechaInicio, DateTime fechaFin, int usuarioId);
        Task<OperationResult<Reporte>> GenerarReporteUsuariosActivosAsync(int usuarioId);
        Task<OperationResult<Reporte>> GenerarReportePenalizacionesAsync(DateTime fechaInicio, DateTime fechaFin, int usuarioId);
        Task<OperationResult<Reporte>> GenerarReporteDevolucionesAsync(DateTime fechaInicio, DateTime fechaFin, int usuarioId);

        Task<OperationResult<Reporte>> GenerarReporteLibrosMasPrestadosAsync(DateTime inicio, DateTime fin);


        Task<OperationResult<bool>> MarcarComoResueltoAsync(int reporteId);
    }
}