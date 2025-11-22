using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models.AuthApi;
using SIGEBI.Web.Refactory;

namespace SIGEBI.Web.ServiciosApi
{
    

    public class AuthApiService : IAuthApiService
    {
        private readonly IApiClient _api;

        public AuthApiService(IApiClient api)
        {
            _api = api;
        }

        public async Task<LoginResponseModel?> LoginAsync(LoginModel model)
        {
            var result = await _api.PostAsync<LoginResponseModel>("Auth/login", model);

            if (!result.Success)
                return null;

            return result.Data;
        }
    }

}
