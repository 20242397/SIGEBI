using Microsoft.Extensions.Logging;
using Moq;
using SIGEBI.Application.Dtos.Models.Configuration.Notificacion;
using SIGEBI.Application.Services.NotificacionesSer;
using SIGEBI.Application.Repositories.Configuration.INotificacion;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;
using FluentAssertions;

namespace SIGEBI.Application.Test.Services
{
    public class NotificacionServiceTests
    {
        private readonly Mock<INotificacionRepository> _repoMock;
        private readonly Mock<ILogger<NotificacionService>> _loggerMock;
        private readonly NotificacionService _service;

        public NotificacionServiceTests()
        {
            _repoMock = new Mock<INotificacionRepository>();
            _loggerMock = new Mock<ILogger<NotificacionService>>();
            _service = new NotificacionService(_repoMock.Object, _loggerMock.Object);
        }


        #region RegistrarNotificacionAsync

        
        [Fact]
        public async Task RegistrarNotificacionAsync_Should_Return_Error_When_Dto_Is_Null()
        {
            var result = await _service.RegistrarNotificacionAsync<Notificacion>(null!);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Excepción");
        }

        [Fact]
        public async Task RegistrarNotificacionAsync_Should_Fail_When_Validation_Fails()
        {
            var dto = new NotificacionCreateDto
            {
                UsuarioId = 0,
                Tipo = "",
                Mensaje = "",
                FechaEnvio = null
            };

            var result = await _service.RegistrarNotificacionAsync<Notificacion>(dto);

            result.Success.Should().BeFalse();
            result.Message.Should().MatchRegex("(obligatorio|válido)");
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }


        [Fact]
        public async Task RegistrarNotificacionAsync_Should_Add_When_Valid()
        {
            var dto = new NotificacionCreateDto
            {
                UsuarioId = 1,
                Tipo = "Préstamo",
                Mensaje = "Notificación de prueba",
                FechaEnvio = DateTime.Now
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync(new OperationResult<Notificacion>
                {
                    Success = true,
                    Message = "Registrada correctamente.",
                    Data = new Notificacion()
                });

            var result = await _service.RegistrarNotificacionAsync<Notificacion>(dto);

            result.Success.Should().BeTrue();
            result.Message.Should().Contain("correctamente");
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Notificacion>()), Times.Once);
        }


        #endregion

        #region ObtenerPorUsuarioAsync

        [Fact]
        public async Task ObtenerPorUsuarioAsync_Should_Return_Empty_When_NoData()
        {
            _repoMock.Setup(r => r.ObtenerNotificacionesPorUsuarioAsync(99))
                .ReturnsAsync(new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = true,
                    Data = Enumerable.Empty<Notificacion>()
                });

            var result = await _service.ObtenerPorUsuarioAsync<IEnumerable<Notificacion>>(99);

            result.Success.Should().BeTrue();
            ((IEnumerable<Notificacion>)result.Data).Should().BeEmpty();
        }


        [Fact]
        public async Task ObtenerPorUsuarioAsync_Should_Return_List()
        {
            _repoMock.Setup(r => r.ObtenerNotificacionesPorUsuarioAsync(1))
                .ReturnsAsync(new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = true,
                    Data = new List<Notificacion> { new Notificacion() }
                });

            var result = await _service.ObtenerPorUsuarioAsync<IEnumerable<Notificacion>>(1);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }


        [Fact]
        public async Task ObtenerNoLeidasAsync_Should_Return_List()
        {
            _repoMock.Setup(r => r.ObtenerNotificacionesNoLeidasPorUsuarioAsync(1))
                .ReturnsAsync(new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = true,
                    Data = new List<Notificacion> { new Notificacion { Enviado = false } }
                });

            var result = await _service.ObtenerNoLeidasAsync<IEnumerable<Notificacion>>(1);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }


        [Fact]
        public async Task ObtenerPendientesAsync_Should_Return_List()
        {
            _repoMock.Setup(r => r.ObtenerPendientesAsync())
                .ReturnsAsync(new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = true,
                    Data = new List<Notificacion> { new Notificacion { Enviado = false } }
                });

            var result = await _service.ObtenerPendientesAsync<IEnumerable<Notificacion>>();

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }


        [Fact]
        public async Task ObtenerPorTipoAsync_Should_Return_ByType()
        {
            _repoMock.Setup(r => r.ObtenerPorTipoAsync("Préstamo"))
                .ReturnsAsync(new OperationResult<IEnumerable<Notificacion>>
                {
                    Success = true,
                    Data = new List<Notificacion> { new Notificacion { Tipo = "Préstamo" } }
                });

            var result = await _service.ObtenerPorTipoAsync<IEnumerable<Notificacion>>("Préstamo");

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        #endregion

        #region MarcarComoEnviadaAsync y MarcarTodasComoEnviadasPorUsuarioAsync


        [Fact]
        public async Task MarcarTodasComoEnviadasPorUsuarioAsync_Should_Fail_When_IdInvalid()
        {
            var result = await _service.MarcarTodasComoEnviadasPorUsuarioAsync<int>(0);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no es válido");
        }


        [Fact]
        public async Task MarcarComoEnviadaAsync_Should_Fail_When_RepoFails()
        {
            _repoMock.Setup(r => r.MarcarComoEnviadaAsync(1))
                .ReturnsAsync(new OperationResult<bool>
                {
                    Success = false,
                    Message = "Error interno"
                });

            var result = await _service.MarcarComoEnviadaAsync<bool>(1);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Error");
        }


        [Fact]
        public async Task MarcarComoEnviadaAsync_Should_Succeed()
        {
            _repoMock.Setup(r => r.MarcarComoEnviadaAsync(1))
                .ReturnsAsync(new OperationResult<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Marcada como enviada."
                });

            var result = await _service.MarcarComoEnviadaAsync<bool>(1);

            result.Success.Should().BeTrue();
            result.Message.Should().Contain("enviada");
        }


        [Fact]
        public async Task MarcarTodasComoEnviadasPorUsuarioAsync_Should_Succeed()
        {
            _repoMock.Setup(r => r.MarcarTodasComoEnviadasPorUsuarioAsync(1))
                .ReturnsAsync(new OperationResult<int>
                {
                    Success = true,
                    Data = 5,
                    Message = "5 notificaciones marcadas como enviadas."
                });

            var result = await _service.MarcarTodasComoEnviadasPorUsuarioAsync<int>(1);

            result.Success.Should().BeTrue();
            result.Message.Should().Contain("enviadas");
            result.Data.Should().Be(5);
        }

        #endregion

        #region EnviarNotificacionAsync

        [Fact]
        public async Task EnviarNotificacionAsync_Should_Send_Successfully()
        {
            var dto = new NotificacionCreateDto
            {
                UsuarioId = 1,
                Tipo = "Recordatorio",
                Mensaje = "Tu préstamo vence hoy"
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync(new OperationResult<Notificacion>
                {
                    Success = true,
                    Data = new Notificacion { UsuarioId = 1 },
                    Message = "Notificación enviada correctamente."
                });

            var result = await _service.EnviarNotificacionAsync<Notificacion>(dto);

            result.Success.Should().BeTrue();
            result.Message.Should().Contain("enviada");
        }

        [Fact]
        public async Task EnviarNotificacionAsync_Should_Fail_When_Invalid()
        {
            var dto = new NotificacionCreateDto
            {
                UsuarioId = 0,
                Tipo = "",
                Mensaje = ""
            };

            var result = await _service.EnviarNotificacionAsync<Notificacion>(dto);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("obligatorio");
        }

        #endregion

    }
}
