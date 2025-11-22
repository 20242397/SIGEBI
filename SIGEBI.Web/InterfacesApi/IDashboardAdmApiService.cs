using SIGEBI.Web.Models.EjemplarApi;
using SIGEBI.Web.Models.LibroApi;
using SIGEBI.Web.Models.NotificacionApi;
using SIGEBI.Web.Models.PrestamoApi;
using SIGEBI.Web.Models.ReporteApi;
using SIGEBI.Web.Models.UsuarioApi;

namespace SIGEBI.Web.InterfacesApi
{

   public interface IDashboardApiService
   {
        Task<IEnumerable<UsuarioApiModel>> GetUsuariosAsync();
        Task<IEnumerable<LibroApiModel>> GetLibrosAsync();
        Task<IEnumerable<EjemplarApiModel>> GetEjemplaresAsync();
        Task<IEnumerable<NotificacionApiModel>> GetNotificacionesAsync();
        Task<IEnumerable<PrestamoApiModel>> GetPrestamosAsync();
        Task<IEnumerable<ReporteApiModel>> GetReportesAsync();
   }

}

