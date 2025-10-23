using Microsoft.EntityFrameworkCore;
using SIGEBI.Infraestructure.Dependencies.DependenciasADO.Libro;
using SIGEBI.Infraestructure.Dependencies.DependenciasADO.Prestamo;
using SIGEBI.Infraestructure.Dependencies.DependenciasADO.Usuario;
using SIGEBI.Infraestructure.Dependencies.DependenciasEF.Ejemplar;
using SIGEBI.Infraestructure.Dependencies.DependenciasEF.Notificacion;
using SIGEBI.Infraestructure.Dependencies.DependenciasEF.Reporte;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.RepositoriesAdo;

namespace SIGEBI.Configuracion.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ JSON Config
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            // ✅ Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ✅ Logging oficial de .NET
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // ✅ Database Helper (ADO)
            builder.Services.AddTransient<DbHelper>();

            // ✅ Dependencias ADO
            builder.Services.AddUsuarioDependency();
            builder.Services.AddLibroDependency();
            builder.Services.AddPrestamoDependency();

            // ✅ Entity Framework Core
            builder.Services.AddDbContext<SIGEBIContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SIGEBIConnString")));

            // ✅ Dependencias EF
            builder.Services.AddEjemplarDependency();
            builder.Services.AddNotificacionDependency();
            builder.Services.AddReporteDependency();

            // ✅ Logger personalizado (solo para capa Persistence)
            builder.Services.AddSingleton(typeof(SIGEBI.Persistence.Logging.ILoggerService<>),
                                          typeof(SIGEBI.Persistence.Logging.LoggerService<>));

            var app = builder.Build();

            // ✅ Swagger en entorno Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ✅ Routing
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
