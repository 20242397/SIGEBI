using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar;
using SIGEBI.Application.Repositories.Configuration.IBiblioteca;
using SIGEBI.Application.Services.BibliotecaSer;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using Xunit;

namespace SIGEBI.Application.Test.Services
{
    public class EjemplarServiceTests
    {
        private readonly Mock<IEjemplarRepository> _repoMock;
        private readonly Mock<ILogger<EjemplarService>> _loggerMock;
        private readonly EjemplarService _service;

        public EjemplarServiceTests()
        {
            _repoMock = new Mock<IEjemplarRepository>();
            _loggerMock = new Mock<ILogger<EjemplarService>>();
            _service = new EjemplarService(_repoMock.Object, _loggerMock.Object);
        }

        #region  RegistrarEjemplarAsync

        [Fact]
        public async Task RegistrarEjemplarAsync_Should_Fail_When_Validation_Fails()
        {
            var dto = new EjemplarCreateDto
            {
                LibroId = 0, // causa error de validación
                CodigoBarras = ""
            };

            var result = await _service.RegistrarEjemplarAsync<Ejemplar>(dto);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("El código de barras es obligatorio.");
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Ejemplar>()), Times.Never);

        }


        [Fact]
        public async Task RegistrarEjemplarAsync_Should_Return_Error_When_Dto_Is_Null()
        {
            // ACT
            var result = await _service.RegistrarEjemplarAsync<Ejemplar>(null!);

            // ASSERT
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Referencia a objeto no establecida"); // BaseService captura la excepción
        }

        [Fact]
        public async Task RegistrarEjemplarAsync_Should_Add_Ejemplar_When_Valid()
        {
            // ARRANGE
            var dto = new EjemplarCreateDto
            {
                LibroId = 1,
                CodigoBarras = "EJEMP0012"
            };

            var entity = new Ejemplar
            {
                LibroId = dto.LibroId,
                CodigoBarras = dto.CodigoBarras,
                Estado = EstadoEjemplar.Disponible
            };

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<Ejemplar>()))
                .ReturnsAsync(new OperationResult<Ejemplar>
                {
                    Success = true,
                    Message = "Ejemplar registrado correctamente.",
                    Data = entity
                });

            // ACT
            var result = await _service.RegistrarEjemplarAsync<Ejemplar>(dto);

            // ASSERT
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("correctamente");
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Ejemplar>()), Times.Once);
        }

        #endregion

        #region  ActualizarEjemplarAsync

        [Fact]
        public async Task ActualizarEjemplarAsync_Should_Return_Error_When_NotFound()
        {
            // ARRANGE
            var dto = new EjemplarUpdateDto { Id = 1, Estado = "Prestado" };

            _repoMock.Setup(r => r.GetByIdAsync(dto.Id))
                     .ReturnsAsync(new OperationResult<Ejemplar>
                     {
                         Success = false,
                         Data = null,
                         Message = "Ejemplar no encontrado."
                     });

            // ACT
            var result = await _service.ActualizarEjemplarAsync(dto);

            // ASSERT
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no encontrado");
        }

        [Fact]
        public async Task ActualizarEjemplarAsync_Should_Update_When_Valid()
        {
            // ARRANGE
            var dto = new EjemplarUpdateDto { Id = 1, Estado = "Disponible" };

            var ejemplar = new Ejemplar
            {
                Id = 1,
                LibroId = 1,
                CodigoBarras = "EJEMP0012",
                Estado = EstadoEjemplar.Reservado
            };

            _repoMock.Setup(r => r.GetByIdAsync(dto.Id))
                     .ReturnsAsync(new OperationResult<Ejemplar> { Success = true, Data = ejemplar });

            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Ejemplar>()))
                     .ReturnsAsync(new OperationResult<Ejemplar> { Success = true, Data = ejemplar });

            // ACT
            var result = await _service.ActualizarEjemplarAsync(dto);

            // ASSERT
            result.Success.Should().BeTrue();
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Ejemplar>()), Times.Once);
        }

        #endregion

        #region  Consultas

        [Fact]
        public async Task ObtenerPrestadosAsync_Should_Return_List()
        {
            var lista = new List<Ejemplar> { new Ejemplar { Estado = EstadoEjemplar.Prestado } };

            _repoMock.Setup(r => r.ObtenerPrestadosAsync()).ReturnsAsync(lista);

            var result = await _service.ObtenerPrestadosAsync<IEnumerable<Ejemplar>>();

            result.Success.Should().BeTrue();
            (result.Data as IEnumerable<Ejemplar>)!.First().Estado.Should().Be(EstadoEjemplar.Prestado);
        }

        [Fact]
        public async Task ObtenerReservadosAsync_Should_Return_List()
        {
            var lista = new List<Ejemplar> { new Ejemplar { Estado = EstadoEjemplar.Reservado } };

            _repoMock.Setup(r => r.ObtenerReservadosAsync()).ReturnsAsync(lista);

            var result = await _service.ObtenerReservadosAsync<IEnumerable<Ejemplar>>();

            result.Success.Should().BeTrue();
            (result.Data as IEnumerable<Ejemplar>)!.First().Estado.Should().Be(EstadoEjemplar.Reservado);
        }


        [Fact]
        public async Task ObtenerDisponiblesPorLibroAsync_Should_Return_Only_Disponibles()
        {
            var ejemplares = new List<Ejemplar>
            {
               new Ejemplar { Estado = EstadoEjemplar.Disponible },
               new Ejemplar { Estado = EstadoEjemplar.Prestado }
            };

            _repoMock.Setup(r => r.ObtenerDisponiblesPorLibroAsync(1))
                     .ReturnsAsync(ejemplares.Where(e => e.Estado == EstadoEjemplar.Disponible));

            var result = await _service.ObtenerDisponiblesPorLibroAsync<IEnumerable<Ejemplar>>(1);

            result.Success.Should().BeTrue();
            (result.Data as IEnumerable<Ejemplar>)!.All(e => e.Estado == EstadoEjemplar.Disponible).Should().BeTrue();
        }


        [Fact]
        public async Task ObtenerPorLibroAsync_Should_Return_List()
        {
            var lista = new List<Ejemplar> { new Ejemplar { Id = 1 }, new Ejemplar { Id = 2 } };

            _repoMock.Setup(r => r.ObtenerPorLibroAsync(1))
                     .ReturnsAsync(lista);

            var result = await _service.ObtenerPorLibroAsync<IEnumerable<Ejemplar>>(1);

            result.Success.Should().BeTrue();
            (result.Data as IEnumerable<Ejemplar>)!.Count().Should().Be(2);
        }

        [Fact]
        public async Task MarcarComoPerdidoAsync_Should_Succeed()
        {
            _repoMock.Setup(r => r.MarcarComoPerdidoAsync(1))
                     .ReturnsAsync(new OperationResult<bool> { Success = true, Message = "Marcado como perdido" });

            var result = await _service.MarcarComoPerdidoAsync<bool>(1);

            result.Success.Should().BeTrue();
            result.Message.Should().Contain("perdido");
        }

        #endregion
    }
}
