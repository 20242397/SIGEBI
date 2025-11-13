using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.IBiblioteca;
using SIGEBI.Application.Services.BibliotecaSer;
using SIGEBI.Persistence.Repositories.RepositoriesAdo.Biblioteca;

namespace SIGEBI.Infraestructure.Dependencies.DependenciasADO.Libro
{
    public static class LibroDependency
    {
        public static void AddLibroDependency(this IServiceCollection services)
        {

            services.AddScoped<ILibroRepository, LibroRepositoryAdo>();
            services.AddTransient<ILibroService, LibroService>();
        }
    }
}