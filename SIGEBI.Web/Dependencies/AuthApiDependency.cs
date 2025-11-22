using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Dependencies
{
    public static class AuthApiDependency
    {
        public static void AddAuthApiDependency(this IServiceCollection services)
        {
            services.AddScoped<IAuthApiService, AuthApiService>();
        }
    }
}
