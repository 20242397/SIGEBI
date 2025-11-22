using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.NotificacionApi;
using SIGEBI.Web.Refactory;

namespace SIGEBI.Web.ServiciosApi
{
    public class NotificacionApiService : INotificacionApiService
    {
        private readonly IApiClient _api;

        public NotificacionApiService(IApiClient api)
        {
            _api = api;
        }

        public async Task<IEnumerable<NotificacionApiModel>> GetAllAsync()
        {
            var r = await _api.GetAsync<IEnumerable<NotificacionApiModel>>("Notificacion/todas");
            return r.Data ?? new List<NotificacionApiModel>();
        }

        public async Task<NotificacionApiModel?> GetByIdAsync(int id)
        {
           
            var r = await _api.GetAsync<ApiResponse<IEnumerable<NotificacionApiModel>>>("Notificacion/todas");

           
            if (r == null || r.Data?.Data == null)
                return null;

          
            return r.Data.Data.FirstOrDefault(n => n.Id == id);
        }


        public Task<ApiResponse<object>> CreateAsync(NotificacionApiCreateModel model)
            => _api.PostAsync<object>("Notificacion/enviar", model);

        public Task<ApiResponse<bool>> GenerarAutomaticasAsync()
            => _api.PostAsync<bool>("Notificacion/generar-automaticas", new { });

        public Task<ApiResponse<int>> MarcarTodasEnviadasAsync(int usuarioId)
            => _api.PutAsync<int>($"Notificacion/usuario/{usuarioId}/marcar-enviadas", new { });

        public async Task<IEnumerable<NotificacionApiModel>> FiltrarAsync(
            int? usuarioId, string? tipo, string? estado, bool? noLeidas)
        {
            var all = await GetAllAsync();
            var q = all.AsQueryable();

            if (usuarioId.HasValue)
                q = q.Where(n => n.UsuarioId == usuarioId.Value);

            if (!string.IsNullOrWhiteSpace(tipo))
                q = q.Where(n => n.Tipo.Contains(tipo, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(estado))
            {
                bool enviado = estado.Equals("Enviada", StringComparison.OrdinalIgnoreCase);
                q = q.Where(n => n.Enviado == enviado);
            }

            if (noLeidas == true)
                q = q.Where(n => !n.Enviado);

            return q.ToList();
        }
    }
}
