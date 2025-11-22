using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Dependencies
{
    public static class DashboardApiDependency
    {
        public static void AddDashboardApiDependency(this IServiceCollection services)
        {
            services.AddScoped<IDashboardApiService, DashboardApiService>();
        }
    }
}
