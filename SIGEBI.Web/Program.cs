using Microsoft.EntityFrameworkCore;
using SIGEBI.Infraestructure.Dependencies.DependenciasADO.Libro;
using SIGEBI.Infraestructure.Dependencies.DependenciasADO.Prestamo;
using SIGEBI.Infraestructure.Dependencies.DependenciasADO.Usuario;
using SIGEBI.Infraestructure.Dependencies.DependenciasEF.Ejemplar;
using SIGEBI.Infraestructure.Dependencies.DependenciasEF.Notificacion;
using SIGEBI.Infraestructure.Dependencies.DependenciasEF.Reporte;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.RepositoriesAdo;
using System.Text.Json.Serialization;

namespace SIGEBI.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ? JSON Config (igual a API)
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        JsonIgnoreCondition.WhenWritingNull;
                });

            // ? Swagger (igual que API)
            builder.Services.AddEndpointsApiExplorer();

            // ? Logging oficial .NET (igual que API)
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // ? Database Helper (ADO)
            builder.Services.AddTransient<DbHelper>();

            // ? Dependencias ADO (igual que API)
            builder.Services.AddUsuarioDependency();
            builder.Services.AddLibroDependency();
            builder.Services.AddPrestamoDependency();

            // ? Entity Framework Core (igual que API)
            builder.Services.AddDbContext<SIGEBIContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SIGEBIConnString")));

            // ? Dependencias EF (igual que API)
            builder.Services.AddEjemplarDependency();
            builder.Services.AddNotificacionDependency();
            builder.Services.AddReporteDependency();

            // ? Logger Persistence (igual que API)
            builder.Services.AddSingleton(typeof(SIGEBI.Persistence.Logging.ILoggerService<>),
                                          typeof(SIGEBI.Persistence.Logging.LoggerService<>));

            // ? HttpClient para consumir tu API
            builder.Services.AddHttpClient("SIGEBI_API", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
            });

            var app = builder.Build();

           

            // ? Static files
            app.UseStaticFiles();

            // ? Routing y autorización
            app.UseRouting();
            app.UseAuthorization();

            // ? MVC Default Route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

