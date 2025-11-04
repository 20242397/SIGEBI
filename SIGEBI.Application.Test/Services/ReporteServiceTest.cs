
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SIGEBI.Application.Dtos.Models.Configuration.Reporte;
using SIGEBI.Application.Services.ReportesSer;
using SIGEBI.Application.Repositories.Configuration.Reportes;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using Xunit;

namespace SIGEBI.Application.Test.Services
{
    public class ReporteServiceTests
    {
        private readonly Mock<IReporteRepository> _repoMock;
        private readonly Mock<ILogger<ReporteService>> _loggerMock;
        private readonly ReporteService _service;

        public ReporteServiceTests()
        {
            _repoMock = new Mock<IReporteRepository>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<ReporteService>>();
            _service = new ReporteService(_repoMock.Object, _loggerMock.Object);
        }

        #region Helpers
        private static OperationResult<T> Ok<T>(T data, string? message = null) =>
            new OperationResult<T> { Success = true, Data = data, Message = message ?? string.Empty };

        private static OperationResult<T> Fail<T>(string message) =>
            new OperationResult<T> { Success = false, Message = message, Data = default! };
        #endregion

        #region GenerarReporteAsync Tests

        [Fact]
        public async Task GenerarReporteAsync_Should_Fail_When_Dto_Is_Null()
        {
            var result = await _service.GenerarReporteAsync<Reporte>(null!);

            result.Success.Should().BeFalse();
            result.Message.Should().StartWith("Excepción:");
        }

        [Fact]
        public async Task GenerarReporteAsync_Should_Fail_When_Tipo_Is_Invalid()
        {
            var dto = new ReporteCreateDto
            {
                UsuarioId = 1,
                Tipo = "desconocido",
                Contenido = "algo"
            };

            // No se debe llamar al repo — el validador rechaza antes
            var result = await _service.GenerarReporteAsync<Reporte>(dto);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Tipo de reporte no válido");
            _repoMock.Invocations.Should().BeEmpty(); // nada del repo
        }

        [Fact]
        public async Task GenerarReporteAsync_Should_Call_LibrosMasPrestados_When_Tipo_Prestamos()
        {
            var dto = new ReporteCreateDto
            {
                UsuarioId = 10,
                Tipo = "prestamos",
                Contenido = ""
            };

            var reporte = new Reporte { Id = 1, Tipo = "Reporte de Préstamos", UsuarioId = 10, Contenido = "ok" };

            _repoMock
                .Setup(r => r.GenerarReporteLibrosMasPrestadosAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(Ok(reporte, "generado"));

            var result = await _service.GenerarReporteAsync<Reporte>(dto);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            _repoMock.Verify(r => r.GenerarReporteLibrosMasPrestadosAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GenerarReporteAsync_Should_Call_LibrosMasPrestados_When_Tipo_LibrosMasPrestados()
        {
            var dto = new ReporteCreateDto
            {
                UsuarioId = 5,
                Tipo = "libros mas prestados",
                Contenido = ""
            };

            var reporte = new Reporte { Id = 2, Tipo = "Libros más prestados", UsuarioId = 5, Contenido = "Top 10" };

            _repoMock
                .Setup(r => r.GenerarReporteLibrosMasPrestadosAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(Ok(reporte, "ok"));

            var result = await _service.GenerarReporteAsync<Reporte>(dto);

            result.Success.Should().BeTrue();
            result.Data.Tipo.Should().Be("Libros más prestados");
            _repoMock.Verify(r => r.GenerarReporteLibrosMasPrestadosAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GenerarReporteAsync_Should_Call_UsuariosActivos_When_Tipo_UsuariosActivos()
        {
            var dto = new ReporteCreateDto
            {
                UsuarioId = 3,
                Tipo = "usuarios activos",
                Contenido = ""
            };

            var reporte = new Reporte { Id = 3, Tipo = "Usuarios Activos", UsuarioId = 3, Contenido = "Activos: 12" };

            _repoMock
                .Setup(r => r.GenerarReporteUsuariosActivosAsync(dto.UsuarioId))
                .ReturnsAsync(Ok(reporte, "listo"));

            var result = await _service.GenerarReporteAsync<Reporte>(dto);

            result.Success.Should().BeTrue();
            result.Data.Tipo.Should().Be("Usuarios Activos");
            _repoMock.Verify(r => r.GenerarReporteUsuariosActivosAsync(3), Times.Once);
        }

        [Fact]
        public async Task GenerarReporteAsync_Should_Call_Penalizaciones_When_Tipo_Penalizaciones()
        {
            var dto = new ReporteCreateDto
            {
                UsuarioId = 9,
                Tipo = "penalizaciones",
                Contenido = "",
                FechaInicio = DateTime.UtcNow.AddDays(-7),
                FechaFin = DateTime.UtcNow
            };

            var reporte = new Reporte { Id = 4, Tipo = "Penalizaciones", UsuarioId = 9, Contenido = "Total: 3" };

            _repoMock
                .Setup(r => r.GenerarReportePenalizacionesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), dto.UsuarioId))
                .ReturnsAsync(Ok(reporte, "ok"));

            var result = await _service.GenerarReporteAsync<Reporte>(dto);

            result.Success.Should().BeTrue();
            result.Data.Tipo.Should().Be("Penalizaciones");
            _repoMock.Verify(r => r.GenerarReportePenalizacionesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 9), Times.Once);
        }

        [Fact]
       public async Task GenerarReporteAsync_Should_Call_Devoluciones_When_Tipo_Devoluciones()
        {
            var dto = new ReporteCreateDto
            {
                UsuarioId = 11,
                Tipo = "devoluciones",
                Contenido = "",
                FechaInicio = DateTime.UtcNow.AddDays(-2),
                FechaFin = DateTime.UtcNow
            };

            var reporte = new Reporte { Id = 5, Tipo = "Devoluciones", UsuarioId = 11, Contenido = "Devoluciones: 5" };

            _repoMock
                .Setup(r => r.GenerarReporteDevolucionesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), dto.UsuarioId))
                .ReturnsAsync(Ok(reporte, "ok"));

            var result = await _service.GenerarReporteAsync<Reporte>(dto);

            result.Success.Should().BeTrue();
            result.Data.Tipo.Should().Be("Devoluciones");
            _repoMock.Verify(r => r.GenerarReporteDevolucionesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 11), Times.Once);
        }

        #endregion


        #region ExportarReporteAsync Tests

        [Fact]
        public async Task ExportarReporteAsync_Should_Fail_When_Format_Is_Empty()
        {
            var result = await _service.ExportarReporteAsync<string>(1, "");

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Debe especificar un formato");
        }

        [Fact]
        public async Task ExportarReporteAsync_Should_Fail_When_Format_Is_Invalid()
        {
            var result = await _service.ExportarReporteAsync<string>(1, "csv");

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Formato no permitido");
        }

        [Fact]
        public async Task ExportarReporteAsync_Should_Fail_When_Report_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(Fail<Reporte>("no existe"));

            var result = await _service.ExportarReporteAsync<string>(99, "txt");

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no encontrado");
            _repoMock.Verify(r => r.GetByIdAsync(99), Times.Once);
        }

        [Fact]
        public async Task ExportarReporteAsync_Should_Export_Successfully()
        {
            var reporte = new Reporte { Id = 7, Contenido = "contenido de prueba", Tipo = "Usuarios Activos", UsuarioId = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(Ok(reporte));

            var result = await _service.ExportarReporteAsync<string>(7, "txt");

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNullOrEmpty();

            var path = result.Data;
            File.Exists(path).Should().BeTrue("el servicio debe haber creado el archivo de exportación");

            // Limpieza
            try
            {
                if (File.Exists(path)) File.Delete(path);
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
                {
                    // intenta eliminar si quedó vacío
                    if (!Directory.EnumerateFileSystemEntries(dir).Any())
                        Directory.Delete(dir);
                }
            }
            catch { /* ignorar limpieza en CI */ }
        }
        #endregion
       

        #region ActualizarReporteAsync Tests

        [Fact]
        public async Task ActualizarReporteAsync_Should_Fail_When_NotFound()
        {
            var dto = new ReporteUpdateDto { Id = 100, Contenido = "append", MarcarComoResuelto = false, Resuelto = false };

            _repoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(Fail<Reporte>("no existe"));

            var result = await _service.ActualizarReporteAsync<Reporte>(dto);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no encontrado");
            _repoMock.Verify(r => r.GetByIdAsync(dto.Id), Times.Once);
        }

        [Fact]
        public async Task ActualizarReporteAsync_Should_Update_Content()
        {
            var dto = new ReporteUpdateDto { Id = 10, Contenido = "Nueva línea", MarcarComoResuelto = false, Resuelto = false };

            var existente = new Reporte { Id = 10, Contenido = "Original", Tipo = "Usuarios Activos", UsuarioId = 1 };

            _repoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(Ok(existente));
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Reporte>()))
          .ReturnsAsync((Reporte r) => new OperationResult<Reporte>
          {
              Success = true,
              Message = "actualizado",
              Data = r
          });

            var result = await _service.ActualizarReporteAsync<Reporte>(dto);

            result.Success.Should().BeTrue();
            result.Message.Should().Contain("actualizado");
            _repoMock.Verify(r => r.UpdateAsync(It.Is<Reporte>(rpt => rpt.Id == 10 && rpt.Contenido.Contains("Nueva línea"))), Times.Once);
        }

        [Fact]
        public async Task ActualizarReporteAsync_Should_Call_MarcarComoResuelto_When_Requested()
        {
            var dto = new ReporteUpdateDto { Id = 20, Contenido = "fix", MarcarComoResuelto = true, Resuelto = false };

            var existente = new Reporte { Id = 20, Contenido = "log", Tipo = "Penalizaciones", UsuarioId = 2 };

            _repoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(Ok(existente));
            _repoMock.Setup(r => r.MarcarComoResueltoAsync(dto.Id)).ReturnsAsync(Ok(true, "resuelto"));
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Reporte>()))
          .ReturnsAsync((Reporte r) => new OperationResult<Reporte>
          {
              Success = true,
              Message = "ok",
              Data = r
          });


            var result = await _service.ActualizarReporteAsync<Reporte>(dto);

            result.Success.Should().BeTrue();
            _repoMock.Verify(r => r.MarcarComoResueltoAsync(20), Times.Once);
        }

        #endregion


        #region Metodos ObtenerAsync Tests

        [Fact]
        public async Task ObtenerPorFechaAsync_Should_Return_List()
        {
            var inicio = DateTime.UtcNow.AddDays(-30);
            var fin = DateTime.UtcNow;

            var lista = new List<Reporte>
            {
                new Reporte{ Id = 1, Tipo = "Usuarios Activos", UsuarioId = 1 },
                new Reporte{ Id = 2, Tipo = "Penalizaciones", UsuarioId = 2 }
            };

            _repoMock.Setup(r => r.ObtenerReportesPorFechaAsync(inicio, fin)).ReturnsAsync(Ok<IEnumerable<Reporte>>(lista));

            var result = await _service.ObtenerPorFechaAsync<IEnumerable<Reporte>>(inicio, fin);

            result.Success.Should().BeTrue();
            (result.Data?.Count() ?? 0).Should().Be(2);
        }

        [Fact]
        public async Task ObtenerPorFechaAsync_Should_Propagate_Repo_Failure()
        {
            var inicio = DateTime.UtcNow.AddDays(-7);
            var fin = DateTime.UtcNow;

            _repoMock.Setup(r => r.ObtenerReportesPorFechaAsync(inicio, fin)).ReturnsAsync(Fail<IEnumerable<Reporte>>("error"));

            var result = await _service.ObtenerPorFechaAsync<IEnumerable<Reporte>>(inicio, fin);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("error");
        }


        [Fact]
        public async Task ObtenerPorTipoAsync_Should_Return_List_By_Type()
        {
            var tipo = "usuarios activos";
            var lista = new List<Reporte> { new Reporte { Id = 3, Tipo = "Usuarios Activos", UsuarioId = 5 } };

            _repoMock.Setup(r => r.ObtenerReportesPorTipoAsync(tipo)).ReturnsAsync(Ok<IEnumerable<Reporte>>(lista));

            var result = await _service.ObtenerPorTipoAsync<IEnumerable<Reporte>>(tipo);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Count().Should().Be(1);
        }

        [Fact]
        public async Task ObtenerPorTipoAsync_Should_Propagate_Repo_Failure()
        {
            var tipo = "penalizaciones";

            _repoMock.Setup(r => r.ObtenerReportesPorTipoAsync(tipo)).ReturnsAsync(Fail<IEnumerable<Reporte>>("falló"));

            var result = await _service.ObtenerPorTipoAsync<IEnumerable<Reporte>>(tipo);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("falló");
        }

        [Fact]
        public async Task ObtenerTodosAsync_Should_Return_All()
        {
            var lista = new List<Reporte>
            {
                new Reporte{ Id = 1, Tipo = "Usuarios Activos", UsuarioId = 1 },
                new Reporte{ Id = 2, Tipo = "Devoluciones", UsuarioId = 2 },
                new Reporte{ Id = 3, Tipo = "Penalizaciones", UsuarioId = 3 }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(Ok<IEnumerable<Reporte>>(lista));

            var result = await _service.ObtenerTodosAsync<IEnumerable<Reporte>>();

            result.Success.Should().BeTrue();
            result.Data.Count().Should().Be(3);
        }


        [Fact]
        public async Task ObtenerPorIdAsync_Should_Return_Report_When_Exists()
        {
            var rpt = new Reporte { Id = 77, Tipo = "Usuarios Activos", UsuarioId = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(77)).ReturnsAsync(Ok(rpt));

            var result = await _service.ObtenerPorIdAsync<Reporte>(77);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(77);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_Should_Inform_When_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(404)).ReturnsAsync(Fail<Reporte>("no encontrado"));

            var result = await _service.ObtenerPorIdAsync<Reporte>(404);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no encontrado");
        }

        #endregion
    }
}
