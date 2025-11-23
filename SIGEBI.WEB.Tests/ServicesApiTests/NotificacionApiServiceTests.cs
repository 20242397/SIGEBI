using Moq;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.NotificacionApi;
using SIGEBI.Web.Refactory;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Test.ServicesApiTests
{
    public class NotificacionApiServiceTests
    {
        private readonly Mock<IApiClient> _mockApi;
        private readonly NotificacionApiService _service;

        public NotificacionApiServiceTests()
        {
            _mockApi = new Mock<IApiClient>();
            _service = new NotificacionApiService(_mockApi.Object);
        }

        
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var items = new List<NotificacionApiModel>
            {
                new NotificacionApiModel { Id = 1, UsuarioId = 10, Tipo = "Alerta" },
                new NotificacionApiModel { Id = 2, UsuarioId = 20, Tipo = "Recordatorio" }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<NotificacionApiModel>>("Notificacion/todas"))
                .ReturnsAsync(new ApiResponse<IEnumerable<NotificacionApiModel>>
                {
                    Success = true,
                    Data = items
                });

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

       
        [Fact]
        public async Task GetByIdAsync_ReturnsNotification()
        {
            var items = new List<NotificacionApiModel>
            {
                new NotificacionApiModel { Id = 1, UsuarioId = 10 },
                new NotificacionApiModel { Id = 5, UsuarioId = 99 }
            };

            _mockApi.Setup(api =>
                api.GetAsync<ApiResponse<IEnumerable<NotificacionApiModel>>>("Notificacion/todas"))
                .ReturnsAsync(new ApiResponse<ApiResponse<IEnumerable<NotificacionApiModel>>>
                {
                    Success = true,
                    Data = new ApiResponse<IEnumerable<NotificacionApiModel>>
                    {
                        Success = true,
                        Data = items
                    }
                });

            var result = await _service.GetByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal(5, result.Id);
        }

       
        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            var items = new List<NotificacionApiModel>
            {
                new NotificacionApiModel { Id = 1 }
            };

            _mockApi.Setup(api =>
                api.GetAsync<ApiResponse<IEnumerable<NotificacionApiModel>>>("Notificacion/todas"))
                .ReturnsAsync(new ApiResponse<ApiResponse<IEnumerable<NotificacionApiModel>>>
                {
                    Success = true,
                    Data = new ApiResponse<IEnumerable<NotificacionApiModel>>
                    {
                        Success = true,
                        Data = items
                    }
                });

            var result = await _service.GetByIdAsync(99);

            Assert.Null(result);
        }

        
        [Fact]
        public async Task CreateAsync_CallsApiCorrectly()
        {
            var model = new NotificacionApiCreateModel
            {
                UsuarioId = 1,
                Mensaje = "Hola",
                Tipo = "Alerta"
            };

            _mockApi.Setup(api =>
                api.PostAsync<object>("Notificacion/enviar", model))
                .ReturnsAsync(ApiResponse<object>.Ok(null));

            var result = await _service.CreateAsync(model);

            Assert.True(result.Success);
            _mockApi.Verify(api =>
                api.PostAsync<object>("Notificacion/enviar", model), Times.Once);
        }

        
        [Fact]
        public async Task GenerarAutomaticas_CallsApiCorrectly()
        {
            _mockApi.Setup(api =>
                api.PostAsync<bool>("Notificacion/generar-automaticas", It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<bool>.Ok(true));

            var result = await _service.GenerarAutomaticasAsync();

            Assert.True(result.Success);
        }

        
        [Fact]
        public async Task MarcarTodasEnviadasAsync_CallsApiCorrectly()
        {
            _mockApi.Setup(api =>
                api.PutAsync<int>("Notificacion/usuario/10/marcar-enviadas", It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<int>.Ok(3));

            var result = await _service.MarcarTodasEnviadasAsync(10);

            Assert.True(result.Success);
            Assert.Equal(3, result.Data);

            _mockApi.Verify(api =>
                api.PutAsync<int>("Notificacion/usuario/10/marcar-enviadas", It.IsAny<object>()),
                Times.Once);
        }

      
        [Fact]
        public async Task FiltrarAsync_FiltersCorrectly()
        {
            var items = new List<NotificacionApiModel>
            {
                new NotificacionApiModel { Id = 1, UsuarioId = 10, Tipo = "Alerta", Enviado = false },
                new NotificacionApiModel { Id = 2, UsuarioId = 10, Tipo = "Recordatorio", Enviado = true },
                new NotificacionApiModel { Id = 3, UsuarioId = 20, Tipo = "Alerta", Enviado = false }
            };

            _mockApi.Setup(api =>
                api.GetAsync<IEnumerable<NotificacionApiModel>>("Notificacion/todas"))
                .ReturnsAsync(ApiResponse<IEnumerable<NotificacionApiModel>>.Ok(items));

            var result = await _service.FiltrarAsync(
                usuarioId: 10,
                tipo: "Alerta",
                estado: "NoEnviada",
                noLeidas: true
            );

            var list = result.ToList();

            Assert.Single(list);
            Assert.Equal(1, list.First().Id);
        }
    }
}
