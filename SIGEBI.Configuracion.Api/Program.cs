
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.IBiblioteca;
using SIGEBI.Application.Repositories.Configuration.IPrestamo;
using SIGEBI.Application.Repositories.Configuration.ISecurity;
using SIGEBI.Application.Services.Biblioteca;
using SIGEBI.Application.Services.Prestamos;
using SIGEBI.Application.Services.Security;
using SIGEBI.Persistence.Repositories.Configuration.RepositoriesAdo;
using SIGEBI.Persistence.Repositories.Configuration.RepositoriesAdo.Biblioteca;
using SIGEBI.Persistence.Repositories.Configuration.RepositoriesAdo.Prestamos;
using SIGEBI.Persistence.Repositories.Configuration.RepositoriesAdo.Security;

namespace SIGEBI.Configuracion.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            // 1️⃣ Add Controllers and Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 2️⃣ Add Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // 3️⃣ Add Database Helper
            builder.Services.AddScoped<DbHelper>();

            // 4️⃣ Add Repositories (Ado.NET)
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryAdo>();
            builder.Services.AddScoped<ILibroRepository, LibroRepositoryAdo>();
            builder.Services.AddScoped<IPrestamoRepository, PrestamoRepositoryAdo>();

            // 5️⃣ Add Services (Application Layer)
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<ILibroService, LibroService>();
            builder.Services.AddScoped<IPrestamoService, PrestamoService>();

            var app = builder.Build();

            // 6️⃣ Enable Swagger in Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 7️⃣ Routing and Authorization
            app.UseRouting();
            app.UseAuthorization();

            // 8️⃣ Map Controllers
            app.MapControllers();

            // 9️⃣ Run the API
            app.Run();
        }
    }
}