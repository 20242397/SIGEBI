using SIGEBI.Web.Models;
using SIGEBI.Web.Models.NotificacionApi;

namespace SIGEBI.Web.InterfacesApi
{
    public interface INotificacionApiService
    {
            Task<IEnumerable<NotificacionApiModel>> GetAllAsync();
            Task<NotificacionApiModel?> GetByIdAsync(int id);

            Task<ApiResponse<object>> CreateAsync(NotificacionApiCreateModel model);
            Task<ApiResponse<bool>> GenerarAutomaticasAsync();
            Task<ApiResponse<int>> MarcarTodasEnviadasAsync(int usuarioId);

            Task<IEnumerable<NotificacionApiModel>> FiltrarAsync(
                int? usuarioId, string? tipo, string? estado, bool? noLeidas);
    }
   

}
