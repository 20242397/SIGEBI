
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.INotificacion;
using SIGEBI.Application.Services.NotificacionesSer;
using SIGEBI.Persistence.Repositories.RepositoriesEF.NotificacionesRepository;

namespace SIGEBI.Infraestructure.Dependencies.DependenciasEF.Notificacion
{
    public static class NotificacionDependency
    {
        public static void AddNotificacionDependency(this IServiceCollection services)
        {
            // Add NotificacionRepositoryEF dependency
            services.AddScoped<INotificacionRepository, NotificacionRepository>();
           services.AddTransient<INotificacionService, NotificacionService>();
        }
    }
}
