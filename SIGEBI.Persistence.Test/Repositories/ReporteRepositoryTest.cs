using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Entitines.Configuration.Security;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Logging;
using SIGEBI.Application.Repositories.Configuration.Reportes;
using SIGEBI.Persistence.Repositories.RepositoriesEF.Reportes;
using FluentAssertions;

namespace SIGEBI.Persistence.Test.Repositories
{
    public class ReporteRepositoryTest
    {
        private readonly IReporteRepository _reporteRepository;
        private readonly SIGEBIContext _context;
        private readonly ILogger<Reporte> _logger;

        public ReporteRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<SIGEBIContext>()
                .UseInMemoryDatabase(databaseName: $"SIGEBI_TestDB_{Guid.NewGuid()}")
                .Options;

            _context = new SIGEBIContext(options);

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                       .AddConsole();
            });

            _logger = loggerFactory.CreateLogger<Reporte>();
            _reporteRepository = new ReporteRepository(_context, new LoggerService<Reporte>(_logger));
        }

        private async Task ResetDatabaseAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }

        #region 🧩 AddAsync y UpdateAsync

        [Fact]
        public async Task AddAsync_Should_Add_Valid_Reporte()
        {
            await ResetDatabaseAsync();

            var reporte = new Reporte
            {
                Tipo = "Prestamos",
                UsuarioId = 1,
                Contenido = "Contenido de prueba",
                FechaGeneracion = DateTime.Now
            };

            var result = await _reporteRepository.AddAsync(reporte);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            (await _context.Reporte.CountAsync()).Should().Be(1);
        }

        [Fact]
        public async Task AddAsync_Should_Fail_When_Tipo_Is_Invalid()
        {
            await ResetDatabaseAsync();

            var reporte = new Reporte
            {
                Tipo = "Inventario",
                UsuarioId = 1,
                Contenido = "No válido",
                FechaGeneracion = DateTime.Now
            };

            var result = await _reporteRepository.AddAsync(reporte);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Tipo de reporte no válido");
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Reporte_When_Valid()
        {
            await ResetDatabaseAsync();

            var reporte = new Reporte
            {
                Tipo = "Usuarios Activos",
                UsuarioId = 1,
                Contenido = "Original",
                FechaGeneracion = DateTime.Now
            };

            await _context.Reporte.AddAsync(reporte);
            await _context.SaveChangesAsync();

            reporte.Contenido = "Actualizado";
            var result = await _reporteRepository.UpdateAsync(reporte);

            result.Success.Should().BeTrue();
            result.Data.Contenido.Should().Be("Actualizado");
        }

        #endregion

        #region 🔍 Consultas generales

        [Fact]
        public async Task ObtenerReportesPorTipoAsync_Should_Return_Only_Matching_Type()
        {
            await ResetDatabaseAsync();

            await _context.Reporte.AddRangeAsync(
                new Reporte { Tipo = "Prestamos", UsuarioId = 1, FechaGeneracion = DateTime.Now },
                new Reporte { Tipo = "Usuarios Activos", UsuarioId = 1, FechaGeneracion = DateTime.Now }
            );
            await _context.SaveChangesAsync();

            var result = await _reporteRepository.ObtenerReportesPorTipoAsync("Prestamos");

            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().Tipo.Should().Be("Prestamos");
        }

        [Fact]
        public async Task ObtenerReportesPorFechaAsync_Should_Return_Reportes_In_Range()
        {
            await ResetDatabaseAsync();

            await _context.Reporte.AddRangeAsync(
                new Reporte { Tipo = "Prestamos", UsuarioId = 1, FechaGeneracion = new DateTime(2025, 1, 1) },
                new Reporte { Tipo = "Prestamos", UsuarioId = 1, FechaGeneracion = new DateTime(2025, 3, 1) },
                new Reporte { Tipo = "Prestamos", UsuarioId = 1, FechaGeneracion = new DateTime(2025, 5, 1) }
            );
            await _context.SaveChangesAsync();

            var result = await _reporteRepository.ObtenerReportesPorFechaAsync(
                new DateTime(2025, 2, 1),
                new DateTime(2025, 4, 1)
            );

            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(1);
        }

        [Fact]
        public async Task ObtenerReportesPendientesAsync_Should_Return_Empty_Content_Only()
        {
            await ResetDatabaseAsync();

            await _context.Reporte.AddRangeAsync(
                new Reporte { Tipo = "Prestamos", UsuarioId = 1, Contenido = "", FechaGeneracion = DateTime.Now },
                new Reporte { Tipo = "Prestamos", UsuarioId = 1, Contenido = "OK", FechaGeneracion = DateTime.Now }
            );
            await _context.SaveChangesAsync();

            var result = await _reporteRepository.ObtenerReportesPendientesAsync();

            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(1);
        }

        #endregion

        #region 📊 Generación de reportes automáticos

        [Fact]
        public async Task GenerarReportePrestamosAsync_Should_Create_Reporte()
        {
            await ResetDatabaseAsync();

            // Arrange: crear usuario y ejemplar
            var usuario = new Usuario { Id = 1, Nombre = "Juan", Apellido = "Perez", Email = "juan@correo.com", Estado = "Activo" };
            var libro = new Libro { Id = 1, Titulo = "C# Básico" };
            var ejemplar = new Ejemplar { Id = 1, LibroId = 1, CodigoBarras = "ABC12345" };

            await _context.Usuario.AddAsync(usuario);
            await _context.Libro.AddAsync(libro);
            await _context.Ejemplar.AddAsync(ejemplar);
            await _context.Prestamo.AddAsync(new Prestamo
            {
                UsuarioId = 1,
                EjemplarId = 1,
                FechaPrestamo = DateTime.Now.AddDays(-2)
            });

            await _context.SaveChangesAsync();

            // Act
            var result = await _reporteRepository.GenerarReportePrestamosAsync(
                DateTime.Now.AddDays(-5), DateTime.Now, 1
            );

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Contenido.Should().Contain("REPORTE DE PRÉSTAMOS");
        }

        [Fact]
        public async Task GenerarReporteUsuariosActivosAsync_Should_Create_Reporte()
        {
            await ResetDatabaseAsync();

            await _context.Usuario.AddRangeAsync(
                new Usuario { Id = 1, Nombre = "Ana", Apellido = "Gomez", Email = "ana@correo.com", Estado = "Activo" },
                new Usuario { Id = 2, Nombre = "Luis", Apellido = "Diaz", Email = "luis@correo.com", Estado = "Inactivo" }
            );
            await _context.SaveChangesAsync();

            var result = await _reporteRepository.GenerarReporteUsuariosActivosAsync(1);

            result.Success.Should().BeTrue();
            result.Data.Contenido.Should().Contain("REPORTE DE USUARIOS ACTIVOS");
            result.Data.Contenido.Should().Contain("Ana Gomez");
        }

        [Fact]
        public async Task GenerarReportePenalizacionesAsync_Should_Create_Reporte()
        {
            await ResetDatabaseAsync();

            await _context.Prestamo.AddRangeAsync(
                new Prestamo { UsuarioId = 1, Penalizacion = 5, FechaPrestamo = DateTime.Now.AddDays(-3) },
                new Prestamo { UsuarioId = 2, Penalizacion = 0, FechaPrestamo = DateTime.Now.AddDays(-1) }
            );
            await _context.SaveChangesAsync();

            var result = await _reporteRepository.GenerarReportePenalizacionesAsync(
                DateTime.Now.AddDays(-7),
                DateTime.Now,
                1
            );

            result.Success.Should().BeTrue();
            result.Data.Contenido.Should().Contain("Penalizaciones registradas");
        }

        [Fact]
        public async Task GenerarReporteDevolucionesAsync_Should_Create_Reporte()
        {
            await ResetDatabaseAsync();

            await _context.Prestamo.AddAsync(new Prestamo
            {
                UsuarioId = 1,
                FechaPrestamo = DateTime.Now.AddDays(-5),
                FechaDevolucion = DateTime.Now.AddDays(-2)
            });
            await _context.SaveChangesAsync();

            var result = await _reporteRepository.GenerarReporteDevolucionesAsync(
                DateTime.Now.AddDays(-10),
                DateTime.Now,
                1
            );

            result.Success.Should().BeTrue();
            result.Data.Contenido.Should().Contain("devoluciones");
        }

        [Fact]
        public async Task GenerarReporteLibrosMasPrestadosAsync_Should_Create_Reporte()
        {
            await ResetDatabaseAsync();

            var libro = new Libro { Id = 1, Titulo = "C# Intermedio" };
            var prestamo = new Prestamo
            {
                LibroId = 1,
                FechaPrestamo = DateTime.Now.AddDays(-2)
            };

            await _context.Libro.AddAsync(libro);
            await _context.Prestamo.AddAsync(prestamo);
            await _context.SaveChangesAsync();

            var result = await _reporteRepository.GenerarReporteLibrosMasPrestadosAsync(
                DateTime.Now.AddDays(-5),
                DateTime.Now
            );

            result.Success.Should().BeTrue();
            result.Data.Contenido.Should().Contain("📚 REPORTE DE LIBROS MÁS PRESTADOS");
        }

        [Fact]
        public async Task MarcarComoResueltoAsync_Should_Update_Contenido()
        {
            await ResetDatabaseAsync();

            var reporte = new Reporte
            {
                Tipo = "Prestamos",
                UsuarioId = 1,
                Contenido = "Reporte base",
                FechaGeneracion = DateTime.Now
            };

            await _context.Reporte.AddAsync(reporte);
            await _context.SaveChangesAsync();

            var result = await _reporteRepository.MarcarComoResueltoAsync(reporte.Id);

            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();

            var updated = await _context.Reporte.FindAsync(reporte.Id);
            updated.Contenido.Should().Contain("✅");
        }

        #endregion
    }
}
