using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models.EjemplarApi;
using SIGEBI.Web.Models.LibroApi;
using SIGEBI.Web.Models.NotificacionApi;
using SIGEBI.Web.Models.PrestamoApi;
using SIGEBI.Web.Models.ReporteApi;
using SIGEBI.Web.Models.UsuarioApi;
using SIGEBI.Web.Refactory;

namespace SIGEBI.Web.ServiciosApi
{
   

    public class DashboardApiService : IDashboardApiService
    {
        private readonly IApiClient _api;

        public DashboardApiService(IApiClient api)
        {
            _api = api;
        }

        public async Task<IEnumerable<UsuarioApiModel>> GetUsuariosAsync()
        {
            var r = await _api.GetAsync<IEnumerable<UsuarioApiModel>>("Usuario/todos");
            return r.Data ?? new List<UsuarioApiModel>();
        }

        public async Task<IEnumerable<LibroApiModel>> GetLibrosAsync()
        {
            var r = await _api.GetAsync<IEnumerable<LibroApiModel>>("Libro/todos");
            return r.Data ?? new List<LibroApiModel>();
        }

        public async Task<IEnumerable<EjemplarApiModel>> GetEjemplaresAsync()
        {
            var r = await _api.GetAsync<IEnumerable<EjemplarApiModel>>("Ejemplar/todos");
            return r.Data ?? new List<EjemplarApiModel>();
        }

        public async Task<IEnumerable<NotificacionApiModel>> GetNotificacionesAsync()
        {
            var r = await _api.GetAsync<IEnumerable<NotificacionApiModel>>("Notificacion/todas");
            return r.Data ?? new List<NotificacionApiModel>();
        }

        public async Task<IEnumerable<PrestamoApiModel>> GetPrestamosAsync()
        {
            var r = await _api.GetAsync<IEnumerable<PrestamoApiModel>>("Prestamo/todos");
            return r.Data ?? new List<PrestamoApiModel>();
        }

        public async Task<IEnumerable<ReporteApiModel>> GetReportesAsync()
        {
            var r = await _api.GetAsync<IEnumerable<ReporteApiModel>>("Reporte/todos");
            return r.Data ?? new List<ReporteApiModel>();
        }
    }

}
