using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.UsuarioApi;
using SIGEBI.Web.Refactory;

namespace SIGEBI.Web.ServiciosApi
{
    public class UsuarioApiService : IUsuarioApiService
    {
        private readonly IApiClient _api;

        public UsuarioApiService(IApiClient api)
        {
            _api = api;
        }

        public async Task<IEnumerable<UsuarioApiModel>> GetAllAsync()
        {
            var r = await _api.GetAsync<IEnumerable<UsuarioApiModel>>("Usuario/todos");
            return r.Data ?? new List<UsuarioApiModel>();
        }

        public async Task<UsuarioApiModel?> GetByIdAsync(int id)
        {
            var r = await _api.GetAsync<UsuarioApiModel>($"Usuario/{id}");
            return r.Data;
        }

        public Task<ApiResponse<object>> CreateAsync(UsuarioApiCreateModel model)
            => _api.PostAsync<object>("Usuario/registrar", model);

        public Task<ApiResponse<object>> UpdateAsync(UsuarioApiUpdateModel model)
            => _api.PutAsync<object>("Usuario/editar", model);

        public async Task<UsuarioApiModel?> GetByEmailAsync(string email)
        {
            var r = await _api.GetAsync<UsuarioApiModel>($"Usuario/email/{email}");
            return r.Data;
        }

        public Task<ApiResponse<object>> CambiarEstadoAsync(int id, bool activo)
            => _api.PutAsync<object>($"Usuario/{id}/estado?activo={activo}", new { });

        public Task<ApiResponse<object>> AsignarRolAsync(int id, string rol)
            => _api.PutAsync<object>($"Usuario/{id}/rol?rol={rol}", new { });

        public Task<ApiResponse<object>> DeleteAsync(int id)
            => _api.DeleteAsync<object>($"Usuario/{id}");
    }
}
