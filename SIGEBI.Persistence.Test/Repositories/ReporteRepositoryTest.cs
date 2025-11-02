using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Logging;
using SIGEBI.Persistence.Repositories.RepositoriesEF.NotificacionesRepository;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using SIGEBI.Application.Repositories.Configuration.Reportes;
using SIGEBI.Persistence.Repositories.RepositoriesEF.Reportes;

namespace SIGEBI.Persistence.Test.Repositories
{
    public class ReporteRepositoryTest
    {
        private readonly IReporteRepository _reporteRepository;
        private readonly SIGEBIContext _context;
        private readonly ILogger<Reporte> _logger;

        public ReporteRepositoryTest()
        {
            // ✅ Configurar base de datos en memoria única por prueba
            var options = new DbContextOptionsBuilder<SIGEBIContext>()
                .UseInMemoryDatabase(databaseName: $"SIGEBI_TestDB_{Guid.NewGuid()}")
                .Options; // <- importante

            // ✅ Crear contexto con las opciones configuradas
            _context = new SIGEBIContext(options);

            // ✅ Configurar logger real
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                       .AddConsole();
            });

            _logger = loggerFactory.CreateLogger<Reporte>();

            // ✅ Crear instancia del repositorio a probar
            _reporteRepository = new ReporteRepository(_context, new LoggerService<Reporte>(_logger));
        }

        private async Task ResetDatabaseAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }


    }
}