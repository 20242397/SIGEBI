using SIGEBI.Domain.Entitines.Configuration.Reportes;
using SIGEBI.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Application.Repositories.Configuration
{
    public interface IReporteRepository : IBaseRepository<Reporte>
    {
        Task<IEnumerable<Reporte>> ObtenerReportesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Reporte>> ObtenerReportesPorTipoAsync(string tipo);
        Task<IEnumerable<Reporte>> ObtenerReportesPendientesAsync();
        Task<Reporte> GenerarReportePrestamosAsync(DateTime inicio, DateTime fin);
        Task<Reporte> GenerarReporteLibrosMasPrestadosAsync(int topN);
        Task<Reporte> GenerarReporteUsuariosActivosAsync();
        Task<bool> MarcarComoResueltoAsync(int reporteId);
    }
}
