using Moq;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.EjemplarApi;
using SIGEBI.Web.Models.LibroApi;
using SIGEBI.Web.Models.NotificacionApi;
using SIGEBI.Web.Models.PrestamoApi;
using SIGEBI.Web.Models.ReporteApi;
using SIGEBI.Web.Models.UsuarioApi;
using SIGEBI.Web.Refactory;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Test.ServicesApiTests
{
    public class DashboardAdmApiServiceTests
    {
        private readonly Mock<IApiClient> _mockApi;
        private readonly DashboardApiService _service;

        public DashboardAdmApiServiceTests()
        {
            _mockApi = new Mock<IApiClient>();
            _service = new DashboardApiService(_mockApi.Object);
        }

       
        [Fact]
        public async Task GetUsuariosAsync_ReturnsList()
        {
            var items = new List<UsuarioApiModel>
            {
                new UsuarioApiModel { Id = 1 },
                new UsuarioApiModel { Id = 2 }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<UsuarioApiModel>>("Usuario/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<UsuarioApiModel>>.Ok(items));

            var result = await _service.GetUsuariosAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUsuariosAsync_WhenNull_ReturnsEmpty()
        {
            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<UsuarioApiModel>>("Usuario/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<UsuarioApiModel>>.Ok(null));

            var result = await _service.GetUsuariosAsync();

            Assert.Empty(result);
        }

       
        [Fact]
        public async Task GetLibrosAsync_ReturnsList()
        {
            var items = new List<LibroApiModel> { new LibroApiModel { Id = 1 } };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<LibroApiModel>>("Libro/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<LibroApiModel>>.Ok(items));

            var result = await _service.GetLibrosAsync();

            Assert.Single(result);
        }

      
        [Fact]
        public async Task GetEjemplaresAsync_ReturnsList()
        {
            var items = new List<EjemplarApiModel> { new EjemplarApiModel { Id = 1 } };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<EjemplarApiModel>>("Ejemplar/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<EjemplarApiModel>>.Ok(items));

            var result = await _service.GetEjemplaresAsync();

            Assert.Single(result);
        }

        
        [Fact]
        public async Task GetNotificacionesAsync_ReturnsList()
        {
            var items = new List<NotificacionApiModel> { new NotificacionApiModel { Id = 1 } };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<NotificacionApiModel>>("Notificacion/todas"))
                .ReturnsAsync(ApiResponse<IEnumerable<NotificacionApiModel>>.Ok(items));

            var result = await _service.GetNotificacionesAsync();

            Assert.Single(result);
        }

       
        [Fact]
        public async Task GetPrestamosAsync_ReturnsList()
        {
            var items = new List<PrestamoApiModel> { new PrestamoApiModel { Id = 1 } };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<PrestamoApiModel>>("Prestamo/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<PrestamoApiModel>>.Ok(items));

            var result = await _service.GetPrestamosAsync();

            Assert.Single(result);
        }

       
        [Fact]
        public async Task GetReportesAsync_ReturnsList()
        {
            var items = new List<ReporteApiModel> { new ReporteApiModel { Id = 1 } };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<ReporteApiModel>>("Reporte/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<ReporteApiModel>>.Ok(items));

            var result = await _service.GetReportesAsync();

            Assert.Single(result);
        }
    }
}
