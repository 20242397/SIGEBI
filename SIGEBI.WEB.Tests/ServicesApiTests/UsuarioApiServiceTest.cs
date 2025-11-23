using Moq;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.UsuarioApi;
using SIGEBI.Web.Refactory;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Test.ServicesApiTests
{
    public class UsuarioApiServiceTests
    {
        private readonly Mock<IApiClient> _mockApi;
        private readonly UsuarioApiService _service;

        public UsuarioApiServiceTests()
        {
            _mockApi = new Mock<IApiClient>();
            _service = new UsuarioApiService(_mockApi.Object);
        }

      
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var items = new List<UsuarioApiModel>
            {
                new UsuarioApiModel { Id = 1, Nombre = "Juan" },
                new UsuarioApiModel { Id = 2, Nombre = "Ana" }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<UsuarioApiModel>>("Usuario/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<UsuarioApiModel>>.Ok(items));

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

      
        [Fact]
        public async Task GetByIdAsync_ReturnsUser()
        {
            var user = new UsuarioApiModel
            {
                Id = 10,
                Nombre = "Pedro",
                Email = "pedro@test.com"
            };

            _mockApi.Setup(api =>
                api.GetAsync<UsuarioApiModel>("Usuario/10"))
                .ReturnsAsync(ApiResponse<UsuarioApiModel>.Ok(user));

            var result = await _service.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNull_ReturnsNull()
        {
            _mockApi.Setup(api =>
                api.GetAsync<UsuarioApiModel>("Usuario/999"))
                .ReturnsAsync(ApiResponse<UsuarioApiModel>.Ok(null));

            var result = await _service.GetByIdAsync(999);

            Assert.Null(result);
        }

       
        [Fact]
        public async Task CreateAsync_CallsApiCorrectly()
        {
            var model = new UsuarioApiCreateModel
            {
                Nombre = "Nuevo",
                Apellido = "Test",
                Email = "nuevo@test.com"
            };

            _mockApi.Setup(api =>
                api.PostAsync<object>("Usuario/registrar", model))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.CreateAsync(model);

            Assert.True(result.Success);
            _mockApi.Verify(api =>
                api.PostAsync<object>("Usuario/registrar", model), Times.Once);
        }

        
        [Fact]
        public async Task UpdateAsync_CallsApiCorrectly()
        {
            var model = new UsuarioApiUpdateModel
            {
                Id = 1,
                Nombre = "Actualizado",
                Email = "upd@test.com"
            };

            _mockApi.Setup(api =>
                api.PutAsync<object>("Usuario/editar", model))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.UpdateAsync(model);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PutAsync<object>("Usuario/editar", model), Times.Once);
        }

      
        [Fact]
        public async Task GetByEmailAsync_ReturnsUser()
        {
            var user = new UsuarioApiModel
            {
                Id = 3,
                Email = "fher@test.com"
            };

            _mockApi.Setup(api =>
                api.GetAsync<UsuarioApiModel>("Usuario/email/test@test.com"))
                .ReturnsAsync(ApiResponse<UsuarioApiModel>.Ok(user));

            var result = await _service.GetByEmailAsync("test@test.com");

            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_WhenNull_ReturnsNull()
        {
            _mockApi.Setup(api =>
                api.GetAsync<UsuarioApiModel>("Usuario/email/noexiste@test.com"))
                .ReturnsAsync(ApiResponse<UsuarioApiModel>.Ok(null));

            var result = await _service.GetByEmailAsync("noexiste@test.com");

            Assert.Null(result);
        }

       
        [Fact]
        public async Task CambiarEstadoAsync_CallsApiCorrectly()
        {
            _mockApi.Setup(api =>
                api.PutAsync<object>("Usuario/1/estado?activo=True", It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.CambiarEstadoAsync(1, true);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PutAsync<object>("Usuario/1/estado?activo=True", It.IsAny<object>()),
                Times.Once);
        }

      
        [Fact]
        public async Task AsignarRolAsync_CallsApiCorrectly()
        {
            _mockApi.Setup(api =>
                api.PutAsync<object>("Usuario/1/rol?rol=Admin", It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.AsignarRolAsync(1, "Admin");

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PutAsync<object>("Usuario/1/rol?rol=Admin", It.IsAny<object>()),
                Times.Once);
        }

       
        [Fact]
        public async Task DeleteAsync_CallsApiCorrectly()
        {
            _mockApi.Setup(api =>
                api.DeleteAsync<object>("Usuario/10"))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.DeleteAsync(10);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.DeleteAsync<object>("Usuario/10"), Times.Once);
        }
    }
}
