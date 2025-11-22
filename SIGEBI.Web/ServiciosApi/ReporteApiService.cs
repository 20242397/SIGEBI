using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.ReporteApi;
using SIGEBI.Web.Refactory;

namespace SIGEBI.Web.ServiciosApi
{
    public class ReporteApiService : IReporteApiService
    {
        private readonly IApiClient _api;

            public ReporteApiService(IApiClient api)
            {
                _api = api;
            }

            public async Task<IEnumerable<ReporteApiModel>> GetAllAsync()
            {
                var r = await _api.GetAsync<IEnumerable<ReporteApiModel>>("Reporte/todos");
                return r.Data ?? new List<ReporteApiModel>();
            }

            public async Task<IEnumerable<ReporteApiModel>> GetByTipoAsync(string tipo)
            {
                var r = await _api.GetAsync<IEnumerable<ReporteApiModel>>($"Reporte/tipo/{tipo}");
                return r.Data ?? new List<ReporteApiModel>();
            }

            public async Task<ReporteApiModel?> GetByIdAsync(int id)
            {
                var r = await _api.GetAsync<ReporteApiModel>($"Reporte/{id}");
                return r.Data;
            }

            public Task<ApiResponse<ReporteApiModel>> CreateAsync(ReporteApiCreateModel model)
                => _api.PostAsync<ReporteApiModel>("Reporte/generar", model);

            public Task<ApiResponse<object>> UpdateAsync(ReporteApiUpdateModel model)
                => _api.PutAsync<object>("Reporte/actualizar", model);

            public Task<ApiResponse<string>> ExportarAsync(int id, string formato)
                => _api.GetAsync<string>($"Reporte/exportar/{id}?formato={formato}");
    }
    

}
