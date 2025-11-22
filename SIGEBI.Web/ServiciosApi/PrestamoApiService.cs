using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.PrestamoApi;
using SIGEBI.Web.Refactory;

namespace SIGEBI.Web.ServiciosApi
{
    public class PrestamoApiService : IPrestamoApiService
    {
        private readonly IApiClient _api;

        public PrestamoApiService(IApiClient api)
        {
            _api = api;
        }

        public async Task<IEnumerable<PrestamoApiModel>> GetAllAsync()
        {
            var r = await _api.GetAsync<IEnumerable<PrestamoApiModel>>("Prestamo/todos");
            return r.Data ?? new List<PrestamoApiModel>();
        }

        public async Task<PrestamoApiModel?> GetByIdAsync(int id)
        {
            var r = await _api.GetAsync<IEnumerable<PrestamoApiModel>>("Prestamo/todos");
            return r.Data?.FirstOrDefault(p => p.Id == id);
        }

        public Task<ApiResponse<bool>> VerificarRestriccionesAsync(int usuarioId)
            => _api.GetAsync<bool>($"Prestamo/restringir/{usuarioId}");

        public Task<ApiResponse<PrestamoApiModel>> CreateAsync(PrestamoApiCreateModel model)
            => _api.PostAsync<PrestamoApiModel>("Prestamo/registrar", model);

        public Task<ApiResponse<PrestamoApiModel>> UpdateAsync(PrestamoApiUpdateModel model)
            => _api.PutAsync<PrestamoApiModel>("Prestamo/extender", model);

        public Task<ApiResponse<object>> RegistrarDevolucionAsync(int id)
        {
            var fecha = DateTime.Now.ToString("yyyy-MM-dd");
            return _api.PutAsync<object>($"Prestamo/devolucion/{id}?fechaDevolucion={fecha}", new { });
        }

        public Task<ApiResponse<object>> CalcularPenalizacionAsync(int id)
            => _api.PutAsync<object>($"Prestamo/penalizacion/{id}", new { });

        public async Task<IEnumerable<PrestamoApiModel>> HistorialAsync(int usuarioId)
        {
            var r = await _api.GetAsync<IEnumerable<PrestamoApiModel>>($"Prestamo/historial/{usuarioId}");
            return r.Data ?? new List<PrestamoApiModel>();
        }

        public Task<ApiResponse<object>> DeleteAsync(int id)
            => _api.DeleteAsync<object>($"Prestamo/{id}");
    }

}
