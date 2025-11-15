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


            builder.Services.AddHttpContextAccessor();


            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        JsonIgnoreCondition.WhenWritingNull;
                });

            builder.Services.AddHttpClient("SIGEBIApi", c =>
            {
                c.BaseAddress = new Uri("http://localhost:5286/api/");
            });


            /*
                       
                        builder.Logging.ClearProviders();
                        builder.Logging.AddConsole();
                        builder.Logging.AddDebug();


                        builder.Services.AddTransient<DbHelper>();


                        builder.Services.AddUsuarioDependency();
                        builder.Services.AddLibroDependency();
                        builder.Services.AddPrestamoDependency();


                        builder.Services.AddDbContext<SIGEBIContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("SIGEBIConnString")));


                        builder.Services.AddEjemplarDependency();
                        builder.Services.AddNotificacionDependency();
                        builder.Services.AddReporteDependency();


                        builder.Services.AddSingleton(
                            typeof(SIGEBI.Persistence.Logging.ILoggerService<>),
                            typeof(SIGEBI.Persistence.Logging.LoggerService<>));
            */

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();


            app.UseStaticFiles();

            app.UseRouting();


            app.UseSession();


            app.UseAuthorization();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=AuthApi}/{action=Login}/{id?}");

            app.Run();
        }
    }
}