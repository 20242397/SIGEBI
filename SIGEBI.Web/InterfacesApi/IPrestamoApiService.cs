using SIGEBI.Web.Models;
using SIGEBI.Web.Models.PrestamoApi;

namespace SIGEBI.Web.InterfacesApi
{
    public interface IPrestamoApiService
    {
        Task<IEnumerable<PrestamoApiModel>> GetAllAsync();
        Task<PrestamoApiModel?> GetByIdAsync(int id);
        Task<ApiResponse<PrestamoApiModel>> CreateAsync(PrestamoApiCreateModel model);
        Task<ApiResponse<PrestamoApiModel>> UpdateAsync(PrestamoApiUpdateModel model);
        Task<ApiResponse<object>> RegistrarDevolucionAsync(int id);
        Task<ApiResponse<object>> CalcularPenalizacionAsync(int id);
        Task<IEnumerable<PrestamoApiModel>> HistorialAsync(int usuarioId);
        Task<ApiResponse<object>> DeleteAsync(int id);
        Task<ApiResponse<bool>> VerificarRestriccionesAsync(int usuarioId);
    }

}
