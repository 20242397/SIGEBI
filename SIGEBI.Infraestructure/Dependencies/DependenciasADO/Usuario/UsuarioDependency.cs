using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.ISecurity;
using SIGEBI.Application.Services.SecuritySer;
using SIGEBI.Persistence.Repositories.RepositoriesAdo.Security;


namespace SIGEBI.Infraestructure.Dependencies.DependenciasADO.Usuario
{
    public static class UsuarioDependency
    {
        public static void AddUsuarioDependency(this IServiceCollection services)
        {
            // Add UsuarioRepositoryAdo dependency
         services.AddScoped<IUsuarioRepository, UsuarioRepositoryAdo>();
         services.AddTransient<IUsuarioService, UsuarioService>();
        }
    }
}
