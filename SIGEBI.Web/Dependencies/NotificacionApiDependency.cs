using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Dependencies
{
    public static class NotificacionApiDependency
    {
        public static void AddNotificacionApiDependency(this IServiceCollection services)
        {
            services.AddScoped<INotificacionApiService, NotificacionApiService>();
        }
    }
}
