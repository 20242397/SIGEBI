using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Dependencies
{
    public static class EjemplarApiDependency
    {
        public static void AddEjemplarApiDependency(this IServiceCollection services)
        {
            services.AddScoped<IEjemplarApiService, EjemplarApiService>();
        }
    }

}
