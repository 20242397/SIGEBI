using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Dependencies
{
    public static class UsuarioApiDependency
    {
        public static  void AddUsuarioApiDependency( this IServiceCollection services)
        {
            services.AddScoped<IUsuarioApiService, UsuarioApiService>();
        }
    }
}

