using SIGEBI.Web.Models;
using SIGEBI.Web.Models.ReporteApi;

namespace SIGEBI.Web.InterfacesApi
{
    public interface IReporteApiService
    {
            Task<IEnumerable<ReporteApiModel>> GetAllAsync();
            Task<IEnumerable<ReporteApiModel>> GetByTipoAsync(string tipo);
            Task<ReporteApiModel?> GetByIdAsync(int id);
            Task<ApiResponse<ReporteApiModel>> CreateAsync(ReporteApiCreateModel model);
            Task<ApiResponse<object>> UpdateAsync(ReporteApiUpdateModel model);
            Task<ApiResponse<string>> ExportarAsync(int id, string formato);
        
    }

}
