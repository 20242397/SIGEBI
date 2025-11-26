using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.EjemplarApi;
using SIGEBI.Web.Refactory;

namespace SIGEBI.Web.ServiciosApi
{


    public class EjemplarApiService : IEjemplarApiService
    {
        private readonly IApiClient _api;

        public EjemplarApiService(IApiClient api)
        {
            _api = api;
        }

        public async Task<IEnumerable<EjemplarApiModel>> GetAllAsync()
        {
            var r = await _api.GetAsync<IEnumerable<EjemplarApiModel>>("Ejemplar/todos");
            return r.Data ?? new List<EjemplarApiModel>();
        }

        public async Task<EjemplarApiModel?> GetByIdAsync(int id)
        {
            var r = await _api.GetAsync<EjemplarApiModel>($"Ejemplar/{id}");
            return r.Data;
        }




        public Task<ApiResponse<object>> CreateAsync(EjemplarApiCreateModel model)
        {
            return _api.PostAsync<object>("Ejemplar/registrar", model);
        }

        public Task<ApiResponse<object>> UpdateAsync(EjemplarApiUpdateModel model)
        {
            return _api.PutAsync<object>("Ejemplar/actualizar", model);
        }

        public Task<ApiResponse<object>> MarcarComoPerdidoAsync(int id)
        {
            return _api.PutAsync<object>($"Ejemplar/perdido/{id}", new { });
        }
    }


}
