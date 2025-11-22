
using SIGEBI.Web.Dependencies;
using SIGEBI.Web.Refactory;
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

            builder.Services.AddHttpClient("SIGEBIApi", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5286/api/");
               
            });

            builder.Services.AddScoped<IApiClient, ApiClient>();

            builder.Services.AddAuthApiDependency();
            builder.Services.AddDashboardApiDependency();
            builder.Services.AddEjemplarApiDependency();
            builder.Services.AddLibroApiDependency();
            builder.Services.AddNotificacionApiDependency();
            builder.Services.AddPrestamoApiDependency();
            builder.Services.AddReporteApiDependency();
            builder.Services.AddUsuarioApiDependency();

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