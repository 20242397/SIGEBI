using Microsoft.EntityFrameworkCore;
using SIGEBI.Configuracion.Api.Dependencies.DependenciasADO.Libro;
using SIGEBI.Configuracion.Api.Dependencies.DependenciasADO.Prestamo;
using SIGEBI.Configuracion.Api.Dependencies.DependenciasADO.Usuario;
using SIGEBI.Configuracion.Api.Dependencies.DependenciasEF.Ejemplar;
using SIGEBI.Configuracion.Api.Dependencies.DependenciasEF.Notificacion;
using SIGEBI.Configuracion.Api.Dependencies.DependenciasEF.Reporte;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.RepositoriesAdo;

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
            builder.Services.AddTransient<DbHelper>();

            builder.Services.AddUsuarioDependency();
            builder.Services.AddLibroDependency();
            builder.Services.AddPrestamoDependency();


            // 6️⃣ Add Entity Framework Core
            builder.Services.AddDbContext<SIGEBIContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SIGEBIConnString")));

            builder.Services.AddEjemplarDependency();
            builder.Services.AddNotificacionDependency();
            builder.Services.AddReporteDependency();

            builder.Services.AddSingleton(typeof(SIGEBI.Infrastructure.Logging.ILoggerService<>),
                              typeof(SIGEBI.Infrastructure.Logging.LoggerService<>));


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