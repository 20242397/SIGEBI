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

            // --------------------------------------------
            // 🔥 NECESARIO PARA HttpContext en filtros y vistas
            // --------------------------------------------
            builder.Services.AddHttpContextAccessor();

            // --------------------------------------------
            // MVC + JSON config
            // --------------------------------------------
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        JsonIgnoreCondition.WhenWritingNull;
                });

            // Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Database Helper ADO.NET
            builder.Services.AddTransient<DbHelper>();

            // Dependencias ADO
            builder.Services.AddUsuarioDependency();
            builder.Services.AddLibroDependency();
            builder.Services.AddPrestamoDependency();

            // EF Core
            builder.Services.AddDbContext<SIGEBIContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SIGEBIConnString")));

            // Dependencias EF
            builder.Services.AddEjemplarDependency();
            builder.Services.AddNotificacionDependency();
            builder.Services.AddReporteDependency();

            // Logger Persistence
            builder.Services.AddSingleton(
                typeof(SIGEBI.Persistence.Logging.ILoggerService<>),
                typeof(SIGEBI.Persistence.Logging.LoggerService<>));

            // --------------------------------------------
            // 🔥 ACTIVAR SESSION
            // --------------------------------------------
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // --------------------------------------------
            // MIDDLEWARE PIPELINE
            // --------------------------------------------
            app.UseStaticFiles();

            app.UseRouting();

            // 🔥 NECESARIO para usar HttpContext.Session
            app.UseSession();

            // Si usas filtros con roles -> dejar Authorization activado
            app.UseAuthorization();

            // --------------------------------------------
            // RUTA POR DEFECTO → LOGIN
            // --------------------------------------------
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
