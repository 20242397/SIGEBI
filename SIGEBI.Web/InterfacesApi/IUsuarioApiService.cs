using SIGEBI.Web.Models;
using SIGEBI.Web.Models.UsuarioApi;

namespace SIGEBI.Web.InterfacesApi
{
    public interface IUsuarioApiService
    {
        Task<IEnumerable<UsuarioApiModel>> GetAllAsync();
        Task<UsuarioApiModel?> GetByIdAsync(int id);
        Task<ApiResponse<object>> CreateAsync(UsuarioApiCreateModel model);
        Task<ApiResponse<object>> UpdateAsync(UsuarioApiUpdateModel model);
        Task<UsuarioApiModel?> GetByEmailAsync(string email);
        Task<ApiResponse<object>> CambiarEstadoAsync(int id, bool activo);
        Task<ApiResponse<object>> AsignarRolAsync(int id, string rol);
        Task<ApiResponse<object>> DeleteAsync(int id);
    }
}
