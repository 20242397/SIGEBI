using Moq;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.ReporteApi;
using SIGEBI.Web.Refactory;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Test.ServicesApiTests
{
    public class ReporteApiServiceTests
    {
        private readonly Mock<IApiClient> _mockApi;
        private readonly ReporteApiService _service;

        public ReporteApiServiceTests()
        {
            _mockApi = new Mock<IApiClient>();
            _service = new ReporteApiService(_mockApi.Object);
        }

        
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var items = new List<ReporteApiModel>
            {
                new ReporteApiModel { Id = 1, Tipo = "General" },
                new ReporteApiModel { Id = 2, Tipo = "Mensual" }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<ReporteApiModel>>("Reporte/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<ReporteApiModel>>.Ok(items));

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

       
        [Fact]
        public async Task GetAllAsync_WhenNull_ReturnsEmptyList()
        {
            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<ReporteApiModel>>("Reporte/todos"))
                .ReturnsAsync(ApiResponse<IEnumerable<ReporteApiModel>>.Ok(null));

            var result = await _service.GetAllAsync();

            Assert.Empty(result);
        }

       
        [Fact]
        public async Task GetByTipoAsync_ReturnsFilteredList()
        {
            var items = new List<ReporteApiModel>
            {
                new ReporteApiModel { Id = 1, Tipo = "Mensual" }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<ReporteApiModel>>("Reporte/tipo/Mensual"))
                .ReturnsAsync(ApiResponse<IEnumerable<ReporteApiModel>>.Ok(items));

            var result = await _service.GetByTipoAsync("Mensual");

            Assert.Single(result);
            Assert.Equal("Mensual", result.First().Tipo);
        }

        
        [Fact]
        public async Task GetByIdAsync_ReturnsReporte()
        {
            var rep = new ReporteApiModel { Id = 10, Tipo = "General" };

            _mockApi.Setup(api =>
                api.GetAsync<ReporteApiModel>("Reporte/10"))
                .ReturnsAsync(ApiResponse<ReporteApiModel>.Ok(rep));

            var result = await _service.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
        }

       
        [Fact]
        public async Task GetByIdAsync_WhenNull_ReturnsNull()
        {
            _mockApi.Setup(api =>
                api.GetAsync<ReporteApiModel>("Reporte/999"))
                .ReturnsAsync(ApiResponse<ReporteApiModel>.Ok(null));

            var result = await _service.GetByIdAsync(999);

            Assert.Null(result);
        }

       
        [Fact]
        public async Task CreateAsync_CallsApiCorrectly()
        {
            var model = new ReporteApiCreateModel
            {
                Tipo = "General",
                Contenido = "contenido generado"
            };

            _mockApi.Setup(api =>
                api.PostAsync<ReporteApiModel>("Reporte/generar", model))
                .ReturnsAsync(ApiResponse<ReporteApiModel>.Ok(new ReporteApiModel()));

            var result = await _service.CreateAsync(model);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PostAsync<ReporteApiModel>("Reporte/generar", model), Times.Once);
        }

      
        [Fact]
        public async Task UpdateAsync_CallsApiCorrectly()
        {
            var model = new ReporteApiUpdateModel
            {
                Id = 1,
                Tipo = "devoluciones",
                Contenido = "nuevo contenido"
            };

            _mockApi.Setup(api =>
                api.PutAsync<object>("Reporte/actualizar", model))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.UpdateAsync(model);

            Assert.True(result.Success);

            _mockApi.Verify(api =>
                api.PutAsync<object>("Reporte/actualizar", model), Times.Once);
        }

       
        [Fact]
        public async Task ExportarAsync_ReturnsFilePath()
        {
            _mockApi.Setup(api =>
                api.GetAsync<string>("Reporte/exportar/5?formato=pdf"))
                .ReturnsAsync(ApiResponse<string>.Ok("C:/Reportes/reporte5.pdf"));

            var result = await _service.ExportarAsync(5, "pdf");

            Assert.True(result.Success);
            Assert.Contains("reporte5.pdf", result.Data!);
        }
    }
}
