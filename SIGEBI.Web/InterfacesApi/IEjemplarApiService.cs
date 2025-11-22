using SIGEBI.Web.Models;
using SIGEBI.Web.Models.EjemplarApi;

namespace SIGEBI.Web.InterfacesApi
{
    public interface IEjemplarApiService
    {
        Task<IEnumerable<EjemplarApiModel>> GetAllAsync();
        Task<EjemplarApiModel?> GetByIdAsync(int id);
        Task<ApiResponse<object>> CreateAsync(EjemplarApiCreateModel model);
        Task<ApiResponse<object>> UpdateAsync(EjemplarApiUpdateModel model);
        Task<ApiResponse<object>> MarcarComoPerdidoAsync(int id);
    }

}
