using Moq;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.EjemplarApi;
using SIGEBI.Web.Refactory;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Test.ServicesApiTests
{
    public class EjemplarApiServiceTests
    {
        private readonly Mock<IApiClient> _mockApiClient;
        private readonly EjemplarApiService _service;

        public EjemplarApiServiceTests()
        {
            _mockApiClient = new Mock<IApiClient>();
            _service = new EjemplarApiService(_mockApiClient.Object);
        }

       
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            
            var fakeEjemplares = new List<EjemplarApiModel>
            {
                new EjemplarApiModel { Id = 1, CodigoBarras = "A1" },
                new EjemplarApiModel { Id = 2, CodigoBarras = "B2" }
            };

            _mockApiClient.Setup(api =>
                api.GetAsync<IEnumerable<EjemplarApiModel>>("Ejemplar/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<EjemplarApiModel>>.Ok(fakeEjemplares));

           
            var result = await _service.GetAllAsync();

            
            Assert.Equal(2, result.Count());
            Assert.Contains(result, e => e.Id == 1);
        }

       
        [Fact]
        public async Task GetAllAsync_WhenNullData_ReturnsEmptyList()
        {
            
            _mockApiClient.Setup(api =>
                api.GetAsync<IEnumerable<EjemplarApiModel>>("Ejemplar/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<EjemplarApiModel>>.Ok(null));

            
            var result = await _service.GetAllAsync();

          
            Assert.Empty(result);
        }

       
        [Fact]
        public async Task GetByIdAsync_ReturnsEjemplar()
        {
            var ejemplar = new EjemplarApiModel { Id = 10, Estado = "Disponible" };

            _mockApiClient.Setup(api =>
                api.GetAsync<ApiResponse<EjemplarApiModel>>("Ejemplar/10"))
                .ReturnsAsync(ApiResponse<ApiResponse<EjemplarApiModel>>.Ok(
                    new ApiResponse<EjemplarApiModel>
                    {
                        Success = true,
                        Data = ejemplar
                    }
                ));

           
            var result = await _service.GetByIdAsync(10);

         
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
        }

        
        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            _mockApiClient.Setup(api =>
                api.GetAsync<ApiResponse<EjemplarApiModel>>("Ejemplar/999"))
                .ReturnsAsync(ApiResponse<ApiResponse<EjemplarApiModel>>.Ok(
                    new ApiResponse<EjemplarApiModel> { Data = null }
                ));

            
            var result = await _service.GetByIdAsync(999);

           
            Assert.Null(result);
        }

        
        [Fact]
        public async Task CreateAsync_CallsApiClientCorrectly()
        {
            var model = new EjemplarApiCreateModel { LibroId = 1 };

            _mockApiClient.Setup(api =>
                api.PostAsync<object>("Ejemplar/registrar", model))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

           
            var result = await _service.CreateAsync(model);

          
            Assert.True(result.Success);
            _mockApiClient.Verify(api =>
                api.PostAsync<object>("Ejemplar/registrar", model), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsApiClientCorrectly()
        {
            var model = new EjemplarApiUpdateModel { Id = 5, Estado = "Malo" };

            _mockApiClient.Setup(api =>
                api.PutAsync<object>("Ejemplar/actualizar", model))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.UpdateAsync(model);

            Assert.True(result.Success);
            _mockApiClient.Verify(api =>
                api.PutAsync<object>("Ejemplar/actualizar", model), Times.Once);
        }

       
        [Fact]
        public async Task MarcarComoPerdidoAsync_CallsApiClientCorrectly()
        {
            _mockApiClient.Setup(api =>
                api.PutAsync<object>("Ejemplar/perdido/7", It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.MarcarComoPerdidoAsync(7);

            Assert.True(result.Success);

            _mockApiClient.Verify(api =>
                api.PutAsync<object>("Ejemplar/perdido/7", It.IsAny<object>()),
                Times.Once);
        }
    }
}
