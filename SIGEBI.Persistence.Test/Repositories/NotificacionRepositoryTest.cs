using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIGEBI.Application.Repositories.Configuration.INotificacion;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Logging;
using SIGEBI.Persistence.Repositories.RepositoriesEF.NotificacionesRepository;

namespace SIGEBI.Persistence.Test.Repositories
{
    public class NotificacionRepositoryTest
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly SIGEBIContext _context;
        private readonly ILogger<Notificacion> _logger;

        public NotificacionRepositoryTest()
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

            _logger = loggerFactory.CreateLogger<Notificacion>();


            _notificacionRepository = new NotificacionRepository(_context, new LoggerService<Notificacion>(_logger));
        }

        private async Task ResetDatabaseAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }

        #region "Pruebas para AddAsync, UpdateAsync"

        [Fact]
        public async Task AddAsync_Should_Add_Valid_Notificacion()
        {
            await ResetDatabaseAsync();

            var notificacion = new Notificacion
            {
                Tipo = "Préstamo",
                UsuarioId = 1,
                Mensaje = "Tu préstamo vence mañana.",
                FechaEnvio = DateTime.Now,
                Enviado = false
            };

            var result = await _notificacionRepository.AddAsync(notificacion);

            result.Success.Should().BeTrue("la notificación cumple todas las validaciones y debe guardarse en la base de datos");
            result.Data.Should().NotBeNull();
            (await _context.Notificacion.CountAsync()).Should().Be(1);
        }

        [Fact]
        public async Task AddAsync_Should_Fail_When_Tipo_Is_Invalid()
        {
            await ResetDatabaseAsync();

            var notificacion = new Notificacion
            {
                Tipo = "Alerta",
                UsuarioId = 1,
                Mensaje = "Mensaje inválido.",
                FechaEnvio = DateTime.Now,
                Enviado = false
            };

            var result = await _notificacionRepository.AddAsync(notificacion);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no es válido");
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Notificacion_When_Valid()
        {
            await ResetDatabaseAsync();

            var notificacion = new Notificacion
            {
                Tipo = "Recordatorio",
                UsuarioId = 2,
                Mensaje = "Recuerda devolver tu libro.",
                FechaEnvio = DateTime.Now,
                Enviado = false
            };

            await _context.Notificacion.AddAsync(notificacion);
            await _context.SaveChangesAsync();

            notificacion.Mensaje = "Recordatorio actualizado.";
            var result = await _notificacionRepository.UpdateAsync(notificacion);

            result.Success.Should().BeTrue();
            result.Data.Mensaje.Should().Be("Recordatorio actualizado.");
        }

        #endregion

        #region "Pruebas de Metodos Específicos"

        [Fact]
        public async Task MarcarComoEnviadaAsync_Should_Mark_As_True()
        {
            await ResetDatabaseAsync();

            var notificacion = new Notificacion
            {
                Tipo = "Recordatorio",
                UsuarioId = 1,
                Mensaje = "Devuelve tu libro pendiente.",
                FechaEnvio = DateTime.Now,
                Enviado = false
            };

            await _context.Notificacion.AddAsync(notificacion);
            await _context.SaveChangesAsync();

            var result = await _notificacionRepository.MarcarComoEnviadaAsync(notificacion.Id);

            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();

            var actualizado = await _context.Notificacion.FindAsync(notificacion.Id);
            actualizado!.Enviado.Should().BeTrue();
        }

        [Fact]
        public async Task MarcarTodasComoEnviadasPorUsuarioAsync_Should_Mark_All_As_True()
        {
            await ResetDatabaseAsync();

            var notificaciones = new[]
            {
                new Notificacion { Tipo = "Préstamo", UsuarioId = 10, Mensaje = "A", FechaEnvio = DateTime.Now, Enviado = false },
                new Notificacion { Tipo = "Recordatorio", UsuarioId = 10, Mensaje = "B", FechaEnvio = DateTime.Now, Enviado = false },
                new Notificacion { Tipo = "Penalización", UsuarioId = 5, Mensaje = "C", FechaEnvio = DateTime.Now, Enviado = false }
            };

            await _context.Notificacion.AddRangeAsync(notificaciones);
            await _context.SaveChangesAsync();

            var result = await _notificacionRepository.MarcarTodasComoEnviadasPorUsuarioAsync(10);

            result.Success.Should().BeTrue();
            result.Data.Should().Be(2);
        }

      
        [Fact]
        public async Task ObtenerNotificacionesPorUsuarioAsync_Should_Return_User_Notifications()
        {
            await ResetDatabaseAsync();

            var notificaciones = new[]
            {
                new Notificacion { Tipo = "Préstamo", UsuarioId = 3, Mensaje = "Libro A", FechaEnvio = DateTime.Now },
                new Notificacion { Tipo = "Recordatorio", UsuarioId = 3, Mensaje = "Libro B", FechaEnvio = DateTime.Now }
            };

            await _context.Notificacion.AddRangeAsync(notificaciones);
            await _context.SaveChangesAsync();

            var result = await _notificacionRepository.ObtenerNotificacionesPorUsuarioAsync(3);

            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }

        
        [Fact]
        public async Task ObtenerNotificacionesNoLeidasPorUsuarioAsync_Should_Return_Unsent_Only()
        {
            await ResetDatabaseAsync();

            var notificaciones = new[]
            {
                new Notificacion { Tipo = "Préstamo", UsuarioId = 2, Mensaje = "Sin enviar", FechaEnvio = DateTime.Now, Enviado = false },
                new Notificacion { Tipo = "Penalización", UsuarioId = 2, Mensaje = "Ya enviada", FechaEnvio = DateTime.Now, Enviado = true }
            };

            await _context.Notificacion.AddRangeAsync(notificaciones);
            await _context.SaveChangesAsync();

            var result = await _notificacionRepository.ObtenerNotificacionesNoLeidasPorUsuarioAsync(2);

            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().Enviado.Should().BeFalse();
        }

      
        [Fact]
        public async Task ObtenerPendientesAsync_Should_Return_Unsent_Notifications()
        {
            await ResetDatabaseAsync();

            var notificaciones = new[]
            {
                new Notificacion { Tipo = "Recordatorio", UsuarioId = 1, Mensaje = "Pendiente", FechaEnvio = DateTime.Now, Enviado = false },
                new Notificacion { Tipo = "Penalización", UsuarioId = 1, Mensaje = "Ya enviada", FechaEnvio = DateTime.Now, Enviado = true }
            };

            await _context.Notificacion.AddRangeAsync(notificaciones);
            await _context.SaveChangesAsync();

            var result = await _notificacionRepository.ObtenerPendientesAsync();

            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(1);
        }

      
        [Fact]
        public async Task ObtenerPorTipoAsync_Should_Return_Filtered_By_Type()
        {
            await ResetDatabaseAsync();

            var notificaciones = new[]
            {
                new Notificacion { Tipo = "Préstamo", UsuarioId = 1, Mensaje = "N1", FechaEnvio = DateTime.Now },
                new Notificacion { Tipo = "Recordatorio", UsuarioId = 1, Mensaje = "N2", FechaEnvio = DateTime.Now }
            };

            await _context.Notificacion.AddRangeAsync(notificaciones);
            await _context.SaveChangesAsync();

            var result = await _notificacionRepository.ObtenerPorTipoAsync("Préstamo");

            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().Tipo.Should().Be("Préstamo");
        }

        [Fact]
        public async Task ObtenerTodosAsync_Should_Return_All_Notifications()
        {
            await ResetDatabaseAsync();

            var notificaciones = new[]
            {
        new Notificacion { Tipo = "Préstamo", UsuarioId = 1, Mensaje = "A", FechaEnvio = DateTime.Now, Enviado = false },
        new Notificacion { Tipo = "Recordatorio", UsuarioId = 2, Mensaje = "B", FechaEnvio = DateTime.Now, Enviado = true }
    };

            await _context.Notificacion.AddRangeAsync(notificaciones);
            await _context.SaveChangesAsync();

            var result = await _notificacionRepository.ObtenerTodosAsync();

            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GenerarNotificacionesPreviasAsync_Should_Create_Notifications_For_Prestamos_2_Days_Before()
        {
            await ResetDatabaseAsync();

            var libroA = new Domain.Entitines.Configuration.Biblioteca.Libro { Titulo = "Libro A" };
            var libroB = new Domain.Entitines.Configuration.Biblioteca.Libro { Titulo = "Libro B" };

            await _context.Libro.AddRangeAsync(libroA, libroB);
            await _context.SaveChangesAsync();

            var prestamos = new[]
            {
        new Prestamo
        {
            UsuarioId = 1,
            LibroId = libroA.Id,
            Libro = libroA,
            FechaVencimiento = DateTime.Today.AddDays(2),
            Estado = "Activo"
        },
        new Prestamo
        {
            UsuarioId = 2,
            LibroId = libroB.Id,
            Libro = libroB,
            FechaVencimiento = DateTime.Today.AddDays(5),
            Estado = "Activo"
        }
    };

            await _context.Prestamo.AddRangeAsync(prestamos);
            await _context.SaveChangesAsync();

            var result = await _notificacionRepository.GenerarNotificacionesPreviasAsync();

            result.Success.Should().BeTrue();
            result.Data.Should().Be(1);
            (await _context.Notificacion.CountAsync()).Should().Be(1);
        }


        [Fact]
        public async Task GenerarNotificacionesDiaVencimientoAsync_Should_Create_Notifications_For_Today()
        {
            await ResetDatabaseAsync();

            var libroX = new Domain.Entitines.Configuration.Biblioteca.Libro { Titulo = "Libro X" };
            var libroY = new Domain.Entitines.Configuration.Biblioteca.Libro { Titulo = "Libro Y" };

            await _context.Libro.AddRangeAsync(libroX, libroY);
            await _context.SaveChangesAsync();

            var prestamos = new[]
            {
        new Prestamo
        {
            UsuarioId = 3,
            LibroId = libroX.Id,
            Libro = libroX,
            FechaVencimiento = DateTime.Today,
            Estado = "Activo"
        },
        new Prestamo
        {
            UsuarioId = 4,
            LibroId = libroY.Id,
            Libro = libroY,
            FechaVencimiento = DateTime.Today.AddDays(1),
            Estado = "Activo"
        }
    };

            await _context.Prestamo.AddRangeAsync(prestamos);
            await _context.SaveChangesAsync();

            var result = await _notificacionRepository.GenerarNotificacionesDiaVencimientoAsync();

            result.Success.Should().BeTrue();
            result.Data.Should().Be(1);
            (await _context.Notificacion.CountAsync()).Should().Be(1);
        }


        [Fact]
        public async Task GenerarNotificacionesPorPenalizacionAsync_Should_Create_Notifications_For_Overdue_Prestamos()
        {
            await ResetDatabaseAsync();

            var libroR = new Domain.Entitines.Configuration.Biblioteca.Libro { Titulo = "Libro Retrasado" };
            var libroNo = new Domain.Entitines.Configuration.Biblioteca.Libro { Titulo = "Libro No Retrasado" };

            await _context.Libro.AddRangeAsync(libroR, libroNo);
            await _context.SaveChangesAsync();

            var prestamos = new[]
            {
        new Prestamo
        {
            UsuarioId = 5,
            LibroId = libroR.Id,
            Libro = libroR,
            FechaVencimiento = DateTime.Today.AddDays(-3),
            Estado = "Activo"
        },
        new Prestamo
        {
            UsuarioId = 6,
            LibroId = libroNo.Id,
            Libro = libroNo,
            FechaVencimiento = DateTime.Today.AddDays(2),
            Estado = "Activo"
        }
    };

            await _context.Prestamo.AddRangeAsync(prestamos);
            await _context.SaveChangesAsync();

            var result = await _notificacionRepository.GenerarNotificacionesPorPenalizacionAsync();

            result.Success.Should().BeTrue();
            result.Data.Should().Be(1);
            (await _context.Notificacion.CountAsync()).Should().Be(1);
        }

        #endregion
    }
}