using Moq;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.LibroApi;
using SIGEBI.Web.Refactory;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Test.ServicesApiTests
{
    public class LibroApiServiceTests
    {
        private readonly Mock<IApiClient> _mockApi;
        private readonly LibroApiService _service;

        public LibroApiServiceTests()
        {
            _mockApi = new Mock<IApiClient>();
            _service = new LibroApiService(_mockApi.Object);
        }

        
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var items = new List<LibroApiModel>
            {
                new LibroApiModel { Id = 1, Titulo = "A" },
                new LibroApiModel { Id = 2, Titulo = "B" }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<LibroApiModel>>("Libro/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<LibroApiModel>>.Ok(items));

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        
        [Fact]
        public async Task GetByIdAsync_ReturnsLibro()
        {
            var libro = new LibroApiModel { Id = 10, Titulo = "Test" };

            _mockApi.Setup(api =>
                api.GetAsync<ApiResponse<LibroApiModel>>("Libro/10"))
                .ReturnsAsync(ApiResponse<ApiResponse<LibroApiModel>>.Ok(
                    new ApiResponse<LibroApiModel>
                    {
                        Success = true,
                        Data = libro
                    }
                ));

            var result = await _service.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNull_ReturnsNull()
        {
            _mockApi.Setup(api =>
                api.GetAsync<ApiResponse<LibroApiModel>>("Libro/999"))
                .ReturnsAsync(ApiResponse<ApiResponse<LibroApiModel>>.Ok(
                    new ApiResponse<LibroApiModel> { Data = null }
                ));

            var result = await _service.GetByIdAsync(999);

            Assert.Null(result);
        }

        
        [Fact]
        public async Task CreateAsync_CallsApiCorrectly()
        {
            var model = new LibroApiCreateModel
            {
                Titulo = "Nuevo",
                Autor = "Autor"
            };

            _mockApi.Setup(api =>
                api.PostAsync<object>("Libro/registrar", model))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.CreateAsync(model);

            Assert.True(result.Success);
            _mockApi.Verify(api =>
                api.PostAsync<object>("Libro/registrar", model), Times.Once);
        }

       
        [Fact]
        public async Task UpdateAsync_CallsApiCorrectly()
        {
            var model = new LibroApiUpdateModel { Id = 5, Titulo = "Modificado" };

            _mockApi.Setup(api =>
                api.PutAsync<object>("Libro/modificar", model))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.UpdateAsync(model);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PutAsync<object>("Libro/modificar", model), Times.Once);
        }

        
       
        [Fact]
        public async Task DeleteAsync_CallsApiCorrectly()
        {
            _mockApi.Setup(api =>
                api.DeleteAsync<object>("Libro/123"))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.DeleteAsync(123);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.DeleteAsync<object>("Libro/123"), Times.Once);
        }

       
        [Fact]
        public async Task BuscarPorTituloAsync_ReturnsList()
        {
            var items = new List<LibroApiModel>
            {
                new LibroApiModel { Id = 1, Titulo = "C# Básico" }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<LibroApiModel>>("Libro/titulo/C#"))
                .ReturnsAsync(ApiResponse<IEnumerable<LibroApiModel>>.Ok(items));

            var result = await _service.BuscarPorTituloAsync("C#");

            Assert.Single(result);
        }

       
        [Fact]
        public async Task BuscarPorAutorAsync_ReturnsList()
        {
            var items = new List<LibroApiModel>
            {
                new LibroApiModel { Id = 2, Autor = "Juan" }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<LibroApiModel>>("Libro/autor/Juan"))
                .ReturnsAsync(ApiResponse<IEnumerable<LibroApiModel>>.Ok(items));

            var result = await _service.BuscarPorAutorAsync("Juan");

            Assert.Single(result);
        }

       
        [Fact]
        public async Task FiltrarAsync_BuildsCorrectQuery()
        {
            var items = new List<LibroApiModel>
            {
                new LibroApiModel { Id = 3, Titulo = "Libro X" }
            };

            string expectedUrl =
                "Libro/filtrar?titulo=X&autor=Y&categoria=Z&anio=2024&estado=Activo";

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<LibroApiModel>>(expectedUrl))
                .ReturnsAsync(ApiResponse<IEnumerable<LibroApiModel>>.Ok(items));

            var result = await _service.FiltrarAsync(
                titulo: "X",
                autor: "Y",
                categoria: "Z",
                anio: 2024,
                estado: "Activo"
            );

            Assert.Single(result);
        }

       
        [Fact]
        public async Task CambiarEstadoAsync_CallsApiCorrectly()
        {
            _mockApi.Setup(api =>
                api.PutAsync<object>("Libro/estado/10", "Disponible"))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.CambiarEstadoAsync(10, "Disponible");

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PutAsync<object>("Libro/estado/10", "Disponible"), Times.Once);
        }
    }
}
