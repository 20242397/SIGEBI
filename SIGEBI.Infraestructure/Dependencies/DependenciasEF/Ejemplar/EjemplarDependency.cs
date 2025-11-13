using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.IBiblioteca;
using SIGEBI.Application.Services.BibliotecaSer;
using SIGEBI.Persistence.Repositories.RepositoriesEF.Biblioteca;

namespace SIGEBI.Infraestructure.Dependencies.DependenciasEF.Ejemplar
{
    public static class EjemplarDependency
    {
        public static void AddEjemplarDependency(this IServiceCollection services)
        {

            services.AddScoped<IEjemplarRepository, EjemplarRepository>();
            services.AddTransient<IEjemplarService, EjemplarService>();
        }
    }
}