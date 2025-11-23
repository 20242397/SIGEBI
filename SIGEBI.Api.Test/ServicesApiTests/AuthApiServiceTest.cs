using Moq;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.AuthApi;
using SIGEBI.Web.Refactory;
using SIGEBI.Web.ServiciosApi;

namespace SIGEBI.Web.Test.ServicesApiTests
{
    public class AuthApiServiceTests
    {
        private readonly Mock<IApiClient> _mockApi;
        private readonly AuthApiService _service;

        public AuthApiServiceTests()
        {
            _mockApi = new Mock<IApiClient>();
            _service = new AuthApiService(_mockApi.Object);
        }

        
        [Fact]
        public async Task LoginAsync_ReturnsUser_WhenCredentialsAreCorrect()
        {
            var model = new LoginModel
            {
                Email = "pedro@test.com",
                Password = "123"
            };

            var user = new LoginResponseModel
            {
                Id = 1,
                NombreCompleto = "Test User",
                Role = "Admin",
                Email = "pedro@test.com"
            };

            _mockApi.Setup(api =>
                api.PostAsync<LoginResponseModel>("Auth/login", model))
                .ReturnsAsync(ApiResponse<LoginResponseModel>.Ok(user));

            var result = await _service.LoginAsync(model);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Admin", result.Role);
        }

       
        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenApiFails()
        {
            var model = new LoginModel
            {
                Email = "genesis@test.com",
                Password = "incorrect"
            };

            _mockApi.Setup(api =>
                api.PostAsync<LoginResponseModel>("Auth/login", model))
                .ReturnsAsync(ApiResponse<LoginResponseModel>.Error("Credenciales inválidas"));

            var result = await _service.LoginAsync(model);

            Assert.Null(result);
        }

      
        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenDataIsNull()
        {
            var model = new LoginModel
            {
                Email = "sayi@test.com",
                Password = "123"
            };

            _mockApi.Setup(api =>
                api.PostAsync<LoginResponseModel>("Auth/login", model))
                .ReturnsAsync(new ApiResponse<LoginResponseModel>
                {
                    Success = true,
                    Data = null
                });

            var result = await _service.LoginAsync(model);

            Assert.Null(result);
        }
    }
}
