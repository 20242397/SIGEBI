using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Dependencies
{
    public static class ReporteApiDependency
    {
        public static void AddReporteApiDependency( this IServiceCollection services)
        {
            services.AddScoped<IReporteApiService, ReporteApiService>();
        }
    }
}
