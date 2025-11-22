using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Dependencies
{
    public static class PrestamoApiDependency
    {
        public static void AddPrestamoApiDependency(this IServiceCollection services)
        {
            services.AddScoped<IPrestamoApiService, PrestamoApiService>();
        }
    }
}
