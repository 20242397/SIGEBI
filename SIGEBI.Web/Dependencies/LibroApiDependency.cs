
using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Dependencies
{
    public static class LibroApiDependency
    {
        public static void AddLibroApiDependency(this IServiceCollection services)
        {
            services.AddScoped<ILibroApiService, LibroApiService>();
        }
    }
}

