using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.IPrestamo;
using SIGEBI.Application.Services.PrestamosSer;
using SIGEBI.Persistence.Repositories.RepositoriesAdo.Prestamos;

namespace SIGEBI.Infraestructure.Dependencies.DependenciasADO.Prestamo
{
    public static class PrestamoDependency
    {
        public static void AddPrestamoDependency(this IServiceCollection services)
        {
            
          services.AddScoped<IPrestamoRepository, PrestamoRepositoryAdo>();
          services.AddTransient<IPrestamoService, PrestamoService>();
        }
    }
}
