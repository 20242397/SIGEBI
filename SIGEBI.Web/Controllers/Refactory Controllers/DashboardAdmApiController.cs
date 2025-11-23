using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.InterfacesApi;
namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin")]
    public class DashboardAdmApiController : Controller
    {
        private readonly IDashboardApiService _dashboard;

        public DashboardAdmApiController(IDashboardApiService dashboard)
        {
            _dashboard = dashboard;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _dashboard.GetUsuariosAsync();
            var libros = await _dashboard.GetLibrosAsync();
            var ejemplares = await _dashboard.GetEjemplaresAsync();
            var notificaciones = await _dashboard.GetNotificacionesAsync();
            var prestamos = await _dashboard.GetPrestamosAsync();
            var reportes = await _dashboard.GetReportesAsync();

            ViewBag.TotalUsuarios = usuarios.Count();
            ViewBag.TotalLibros = libros.Count();
            ViewBag.TotalEjemplares = ejemplares.Count();
            ViewBag.TotalNotificaciones = notificaciones.Count();
            ViewBag.NotificacionesEnviadas = notificaciones.Count(n => n.Enviado == true);

            ViewBag.TotalPrestamos = prestamos.Count();
            ViewBag.PrestamosActivos = prestamos.Count(p => p.Estado == "Activo");
            ViewBag.PrestamosDevueltos = prestamos.Count(p => p.Estado == "Devuelto");
            ViewBag.PrestamosConPenalizacion = prestamos.Count(p => p.Penalizacion > 0);

            ViewBag.TotalReportes = reportes.Count();

            return View();
        }
    }
}
