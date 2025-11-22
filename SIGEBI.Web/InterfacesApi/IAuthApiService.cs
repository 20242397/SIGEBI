using SIGEBI.Web.Models.AuthApi;

namespace SIGEBI.Web.InterfacesApi
{
    public interface IAuthApiService
    {
        Task<LoginResponseModel?> LoginAsync(LoginModel model);
    }

}
