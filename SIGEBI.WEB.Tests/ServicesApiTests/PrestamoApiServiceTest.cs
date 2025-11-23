using Moq;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.PrestamoApi;
using SIGEBI.Web.Refactory;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Test.ServicesApiTests
{
    public class PrestamoApiServiceTests
    {
        private readonly Mock<IApiClient> _mockApi;
        private readonly PrestamoApiService _service;

        public PrestamoApiServiceTests()
        {
            _mockApi = new Mock<IApiClient>();
            _service = new PrestamoApiService(_mockApi.Object);
        }

        
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var lista = new List<PrestamoApiModel>
            {
                new PrestamoApiModel { Id = 1 },
                new PrestamoApiModel { Id = 2 }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<PrestamoApiModel>>("Prestamo/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<PrestamoApiModel>>.Ok(lista));

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

       
        [Fact]
        public async Task GetByIdAsync_ReturnsPrestamo()
        {
            var lista = new List<PrestamoApiModel>
            {
                new PrestamoApiModel { Id = 10, UsuarioId = 1 },
                new PrestamoApiModel { Id = 20, UsuarioId = 2 }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<PrestamoApiModel>>("Prestamo/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<PrestamoApiModel>>.Ok(lista));

            var result = await _service.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<PrestamoApiModel>>("Prestamo/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<PrestamoApiModel>>.Ok(new List<PrestamoApiModel>()));

            var result = await _service.GetByIdAsync(999);

            Assert.Null(result);
        }

        
        [Fact]
        public async Task VerificarRestriccionesAsync_ReturnsBool()
        {
            _mockApi.Setup(api =>
                api.GetAsync<bool>("Prestamo/restringir/5"))
                .ReturnsAsync(ApiResponse<bool>.Ok(true));

            var result = await _service.VerificarRestriccionesAsync(5);

            Assert.True(result.Data);
        }

     
        [Fact]
        public async Task CreateAsync_CallsApiCorrectly()
        {
            var model = new PrestamoApiCreateModel
            {
                UsuarioId = 1,
                EjemplarId = 2
            };

            _mockApi.Setup(api =>
                api.PostAsync<PrestamoApiModel>("Prestamo/registrar", model))
                .ReturnsAsync(ApiResponse<PrestamoApiModel>.Ok(new PrestamoApiModel()));

            var result = await _service.CreateAsync(model);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PostAsync<PrestamoApiModel>("Prestamo/registrar", model), Times.Once);
        }

       
        [Fact]
        public async Task UpdateAsync_CallsApiCorrectly()
        {
            var model = new PrestamoApiUpdateModel
            {
                Id = 15,
                FechaVencimiento = DateTime.Now.AddDays(3)
            };

            _mockApi.Setup(api =>
                api.PutAsync<PrestamoApiModel>("Prestamo/extender", model))
                .ReturnsAsync(ApiResponse<PrestamoApiModel>.Ok(new PrestamoApiModel()));

            var result = await _service.UpdateAsync(model);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PutAsync<PrestamoApiModel>("Prestamo/extender", model), Times.Once);
        }

      
        [Fact]
        public async Task RegistrarDevolucionAsync_CallsApiCorrectly()
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");

            _mockApi.Setup(api =>
                api.PutAsync<object>($"Prestamo/devolucion/7?fechaDevolucion={today}", It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.RegistrarDevolucionAsync(7);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PutAsync<object>($"Prestamo/devolucion/7?fechaDevolucion={today}", It.IsAny<object>()),
                Times.Once);
        }

        
        [Fact]
        public async Task CalcularPenalizacionAsync_CallsApiCorrectly()
        {
            _mockApi.Setup(api =>
                api.PutAsync<object>("Prestamo/penalizacion/12", It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.CalcularPenalizacionAsync(12);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PutAsync<object>("Prestamo/penalizacion/12", It.IsAny<object>()), Times.Once);
        }

      
        [Fact]
        public async Task HistorialAsync_ReturnsPrestamos()
        {
            var lista = new List<PrestamoApiModel>
            {
                new PrestamoApiModel { Id = 1, UsuarioId = 10 }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<PrestamoApiModel>>("Prestamo/historial/10"))
                .ReturnsAsync(ApiResponse<IEnumerable<PrestamoApiModel>>.Ok(lista));

            var result = await _service.HistorialAsync(10);

            Assert.Single(result);
        }

      
        [Fact]
        public async Task DeleteAsync_CallsApiCorrectly()
        {
            _mockApi.Setup(api =>
                api.DeleteAsync<object>("Prestamo/5"))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.DeleteAsync(5);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.DeleteAsync<object>("Prestamo/5"), Times.Once);
        }
    }
}
