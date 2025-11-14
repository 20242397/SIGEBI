using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Repositories.RepositoriesEF.Biblioteca;
using SIGEBI.Persistence.Logging;
using FluentAssertions;

namespace SIGEBI.Persistence.Test.Repositories
{
    public class EjemplarRepositoryTests
    {
        private readonly SIGEBIContext _context;
        private readonly EjemplarRepository _ejemplarRepository;
        private readonly ILogger<Ejemplar> _logger;

        public EjemplarRepositoryTests()
        {

            var options = new DbContextOptionsBuilder<SIGEBIContext>()
                .UseInMemoryDatabase(databaseName: $"SIGEBI_TestDB_{Guid.NewGuid()}")
                .Options;

            _context = new SIGEBIContext(options);


            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddConsole();
            });

            _logger = loggerFactory.CreateLogger<Ejemplar>();


            _ejemplarRepository = new EjemplarRepository(_context, new LoggerService<Ejemplar>(_logger));
        }
        #region "Pruebas para AddAsync"

        [Fact]
        public async Task AddAsync_Should_Add_Valid_Ejemplar()
        {
            
            var libro = new Libro { Id = 1, Titulo = "C# Avanzado" };
            await _context.Libro.AddAsync(libro);
            await _context.SaveChangesAsync();

            var ejemplar = new Ejemplar
            {
                CodigoBarras = "ABC12345",
                Estado = EstadoEjemplar.Disponible,
                LibroId = 1
            };

           
            var result = await _ejemplarRepository.AddAsync(ejemplar);

           
            result.Success.Should().BeTrue("porque el ejemplar es válido y el libro existe");
            result.Message.Should().Contain("registrado correctamente");
            (await _context.Ejemplar.AnyAsync(e => e.CodigoBarras == "ABC12345")).Should().BeTrue();
        }

        [Fact]
        public async Task AddAsync_Should_Fail_When_CodigoBarras_Duplicated()
        {
           
            var libro = new Libro { Id = 2, Titulo = "Programación en Java" };
            await _context.Libro.AddAsync(libro);
            await _context.SaveChangesAsync();

            var ejemplar1 = new Ejemplar
            {
                CodigoBarras = "DUPL1234",
                Estado = EstadoEjemplar.Disponible,
                LibroId = 2
            };

            var ejemplar2 = new Ejemplar
            {
                CodigoBarras = "DUPL1234",
                Estado = EstadoEjemplar.Disponible,
                LibroId = 2
            };

            await _ejemplarRepository.AddAsync(ejemplar1);

           
            var result = await _ejemplarRepository.AddAsync(ejemplar2);

           
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("ya está registrado");
        }

        [Fact]
        public async Task AddAsync_Should_Fail_When_Libro_Does_Not_Exist()
        {
          
            var ejemplar = new Ejemplar
            {
                CodigoBarras = "LIBRO999",
                Estado = EstadoEjemplar.Disponible,
                LibroId = 999 
            };

           
            var result = await _ejemplarRepository.AddAsync(ejemplar);

            
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("libro asociado no existe");
        }

        [Fact]
        public async Task AddAsync_Should_Fail_When_CodigoBarras_Invalid()
        {
           
            var libro = new Libro { Id = 3, Titulo = "SQL Server Essentials" };
            await _context.Libro.AddAsync(libro);
            await _context.SaveChangesAsync();

            var ejemplar = new Ejemplar
            {
                CodigoBarras = "##@@@", 
                Estado = EstadoEjemplar.Disponible,
                LibroId = 3
            };

          
            var result = await _ejemplarRepository.AddAsync(ejemplar);

           
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("formato del código de barras");
        }

        [Fact]
        public async Task AddAsync_Should_Fail_When_Estado_Invalid()
        {
           
            var libro = new Libro { Id = 4, Titulo = "Introducción a la Programación" };
            await _context.Libro.AddAsync(libro);
            await _context.SaveChangesAsync();

            var ejemplar = new Ejemplar
            {
                CodigoBarras = "EST12345",
                Estado = (EstadoEjemplar)999, 
                LibroId = 4
            };

            
            var result = await _ejemplarRepository.AddAsync(ejemplar);

          
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("estado del ejemplar no es válido");
        }





        #endregion

        #region "Pruebas de UpdateAsync"


        [Fact]
        public async Task UpdateAsync_Should_Update_Ejemplar_When_Valid()
        {
            await ResetDatabaseAsync();

           
            var libro = new Libro { Id = 1, Titulo = "EF Core Básico" };
            await _context.Libro.AddAsync(libro);
            await _context.SaveChangesAsync();

            var ejemplar = new Ejemplar
            {
                CodigoBarras = "UPD002AA",
                Estado = EstadoEjemplar.Disponible,
                LibroId = 1
            };

            await _context.Ejemplar.AddAsync(ejemplar);
            await _context.SaveChangesAsync();

           
            var ejemplarToUpdate = await _context.Ejemplar.FirstAsync(e => e.CodigoBarras == "UPD002AA");
            ejemplarToUpdate.Estado = EstadoEjemplar.Prestado;

            
            var result = await _ejemplarRepository.UpdateAsync(ejemplarToUpdate);

           
            result.Success.Should().BeTrue("porque el ejemplar fue actualizado correctamente");
            result.Message.Should().Contain("actualizado correctamente");

            var actualizado = await _context.Ejemplar.FirstAsync(e => e.CodigoBarras == "UPD002AA");
            actualizado.Estado.Should().Be(EstadoEjemplar.Prestado);
        }




        [Fact]
        public async Task UpdateAsync_Should_Fail_When_Estado_Is_The_Same()
        {
            await ResetDatabaseAsync();

           
            var libro = new Libro { Id = 1, Titulo = "EF Core Avanzado" };
            var ejemplar = new Ejemplar
            {
                Id = 1,
                CodigoBarras = "UPD002AA", 
                Estado = EstadoEjemplar.Reservado,
                LibroId = 1
            };

            await _context.Libro.AddAsync(libro);
            await _context.Ejemplar.AddAsync(ejemplar);
            await _context.SaveChangesAsync();

           
            var result = await _ejemplarRepository.UpdateAsync(new Ejemplar
            {
                Id = 1,
                CodigoBarras = "UPD002AA", 
                Estado = EstadoEjemplar.Reservado, 
                LibroId = 1
            });

           
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("ya se encuentra en ese estado");
        }

        [Fact]
        public async Task UpdateAsync_Should_Fail_When_Validator_Fails()
        {
            await ResetDatabaseAsync();

            var libro = new Libro { Id = 1, Titulo = "Prueba Validación Update" };
            var ejemplar = new Ejemplar
            {
                Id = 1,
                CodigoBarras = "UPD003",
                Estado = EstadoEjemplar.Disponible,
                LibroId = 1
            };

            await _context.Libro.AddAsync(libro);
            await _context.Ejemplar.AddAsync(ejemplar);
            await _context.SaveChangesAsync();

           
            ejemplar.CodigoBarras = ""; 
            var result = await _ejemplarRepository.UpdateAsync(ejemplar);

          
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("código de barras es obligatorio");
        }

        private async Task ResetDatabaseAsync()
        {

            await _context.Database.EnsureDeletedAsync();  
            await _context.Database.EnsureCreatedAsync();  

        }



        #endregion

        #region "Pruebas de métodos personalizados"


        [Fact]
        public async Task ObtenerPorLibroAsync_Should_Return_Ejemplares_For_Given_Libro()
        {
            await ResetDatabaseAsync();

           
            var libro = new Libro { Id = 1, Titulo = "Test Libro" };
            await _context.Libro.AddAsync(libro);

            var ejemplares = new[]
            {
              new Ejemplar { CodigoBarras = "EJP001AA", Estado = EstadoEjemplar.Disponible, LibroId = 1 },
              new Ejemplar { CodigoBarras = "EJP002AA", Estado = EstadoEjemplar.Prestado, LibroId = 1 },
            };

            await _context.Ejemplar.AddRangeAsync(ejemplares);
            await _context.SaveChangesAsync();

           
            var result = await _ejemplarRepository.ObtenerPorLibroAsync(1);

          
            result.Should().HaveCount(2);
            result.All(e => e.LibroId == 1).Should().BeTrue();
        }


        [Fact]
        public async Task ObtenerDisponiblesPorLibroAsync_Should_Return_Only_Disponibles()
        {
            await ResetDatabaseAsync();

           
            var libro = new Libro { Id = 1, Titulo = "Libro con Disponibles" };
            await _context.Libro.AddAsync(libro);

            var ejemplares = new[]
            {
               new Ejemplar { CodigoBarras = "DISP001A", Estado = EstadoEjemplar.Disponible, LibroId = 1 },
               new Ejemplar { CodigoBarras = "RESV001A", Estado = EstadoEjemplar.Reservado, LibroId = 1 }
            };

            await _context.Ejemplar.AddRangeAsync(ejemplares);
            await _context.SaveChangesAsync();

            
            var result = await _ejemplarRepository.ObtenerDisponiblesPorLibroAsync(1);

           
            result.Should().HaveCount(1);
            result.First().Estado.Should().Be(EstadoEjemplar.Disponible);
        }


        [Fact]
        public async Task ObtenerReservadosAsync_Should_Return_Only_Reservados()
        {
            await ResetDatabaseAsync();

            
            var libro = new Libro { Id = 1, Titulo = "Libro de Prueba Reservados" };
            await _context.Libro.AddAsync(libro);
            await _context.SaveChangesAsync();

          
            var ejemplares = new[]
            {
               new Ejemplar { CodigoBarras = "RES001AA", Estado = EstadoEjemplar.Reservado, LibroId = 1 },
               new Ejemplar { CodigoBarras = "DIS001AA", Estado = EstadoEjemplar.Disponible, LibroId = 1 }
            };

            await _context.Ejemplar.AddRangeAsync(ejemplares);
            await _context.SaveChangesAsync();

           
            var result = (await _ejemplarRepository.ObtenerReservadosAsync()).ToList();

          
            result.Should().HaveCount(1, "porque solo uno tiene estado 'Reservado'");
            result.First().Estado.Should().Be(EstadoEjemplar.Reservado);
        }



        [Fact]

        public async Task ObtenerPrestadosAsync_Should_Return_Only_Prestados()
        {
            await ResetDatabaseAsync();

            var libro = new Libro { Id = 1, Titulo = "Libro de Prueba" };
            await _context.Libro.AddAsync(libro);
            await _context.SaveChangesAsync();

           
            var ejemplares = new[]
            {
              new Ejemplar { CodigoBarras = "PRS001AA", Estado = EstadoEjemplar.Prestado, LibroId = 1 },
              new Ejemplar { CodigoBarras = "DIS002AA", Estado = EstadoEjemplar.Disponible, LibroId = 1 }
            };

            await _context.Ejemplar.AddRangeAsync(ejemplares);
            await _context.SaveChangesAsync();

           
            var result = (await _ejemplarRepository.ObtenerPrestadosAsync()).ToList();

            
            result.Should().HaveCount(1, "porque solo uno de los ejemplares tiene estado Prestado");
            result.First().Estado.Should().Be(EstadoEjemplar.Prestado);
        }



        [Fact]

        public async Task MarcarComoPerdidoAsync_Should_Mark_Ejemplar_As_Perdido()
        {
            await ResetDatabaseAsync();

           
            var ejemplar = new Ejemplar
            {
                Id = 1,
                CodigoBarras = "PERD001A",
                Estado = EstadoEjemplar.Disponible,
                LibroId = 1
            };

            await _context.Ejemplar.AddAsync(ejemplar);
            await _context.SaveChangesAsync();

          
            var result = await _ejemplarRepository.MarcarComoPerdidoAsync(1);

         
            result.Success.Should().BeTrue();
            ((bool)result.Data).Should().BeTrue();
            (await _context.Ejemplar.FindAsync(1))!.Estado.Should().Be(EstadoEjemplar.Perdido);
        }


        #endregion

    }
}