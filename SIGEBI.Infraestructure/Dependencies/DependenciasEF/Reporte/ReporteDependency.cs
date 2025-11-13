using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.Reportes;
using SIGEBI.Application.Services.ReportesSer;
using SIGEBI.Persistence.Repositories.RepositoriesEF.Reportes;

namespace SIGEBI.Infraestructure.Dependencies.DependenciasEF.Reporte
{
    public static class ReporteDependency
    {
        public static void AddReporteDependency(this IServiceCollection services)
        {

            services.AddScoped<IReporteRepository, ReporteRepository>();
            services.AddTransient<IReporteService, ReporteService>();
        }
    }
}