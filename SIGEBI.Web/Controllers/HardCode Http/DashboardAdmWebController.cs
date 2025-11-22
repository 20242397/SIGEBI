using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.EjemplarApi;
using SIGEBI.Web.Models.LibroApi;
using SIGEBI.Web.Models.NotificacionApi;
using SIGEBI.Web.Models.PrestamoApi;
using SIGEBI.Web.Models.UsuarioApi;
using SIGEBI.Web.Models.ReporteApi;
using System.Text.Json;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin")]
    public class DashboardAdmWebController : Controller
    {
        private readonly string _baseUrl = "http://localhost:5286/api/";

        public async Task<IActionResult> Index()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            using var client = new HttpClient();


            var usersResponse = await client.GetAsync(_baseUrl + "Usuario/todos");
            var usersJson = await usersResponse.Content.ReadAsStringAsync();
            var usuariosData = JsonSerializer.Deserialize<ApiResponse<IEnumerable<UsuarioApiModel>>>(usersJson, options);
            ViewBag.TotalUsuarios = usuariosData?.Data?.Count() ?? 0;


            var librosResponse = await client.GetAsync(_baseUrl + "Libro/todos");
            var librosJson = await librosResponse.Content.ReadAsStringAsync();
            var librosData = JsonSerializer.Deserialize<ApiResponse<IEnumerable<LibroApiModel>>>(librosJson, options);
            ViewBag.TotalLibros = librosData?.Data?.Count() ?? 0;


            var ejemplaresResponse = await client.GetAsync(_baseUrl + "Ejemplar/todos");
            var ejemplaresJson = await ejemplaresResponse.Content.ReadAsStringAsync();
            var ejemplaresData = JsonSerializer.Deserialize<ApiResponse<IEnumerable<EjemplarApiModel>>>(ejemplaresJson, options);
            ViewBag.TotalEjemplares = ejemplaresData?.Data?.Count() ?? 0;


            var notiResponse = await client.GetAsync(_baseUrl + "Notificacion/todas");
            var notiJson = await notiResponse.Content.ReadAsStringAsync();
            var notiData = JsonSerializer.Deserialize<ApiResponse<IEnumerable<NotificacionApiModel>>>(notiJson, options);

            var notificaciones = notiData?.Data?.ToList() ?? new List<NotificacionApiModel>();
            ViewBag.TotalNotificaciones = notificaciones.Count;
            ViewBag.NotificacionesEnviadas = notificaciones.Count(n => n.Enviado == true);


            var prestamoResponse = await client.GetAsync(_baseUrl + "Prestamo/todos");
            var prestamoJson = await prestamoResponse.Content.ReadAsStringAsync();
            var prestamosData = JsonSerializer.Deserialize<ApiResponse<IEnumerable<PrestamoApiModel>>>(prestamoJson, options);

            var prestamos = prestamosData?.Data?.ToList() ?? new List<PrestamoApiModel>();

            ViewBag.TotalPrestamos = prestamos.Count;
            ViewBag.PrestamosActivos = prestamos.Count(p => p.Estado == "Activo");
            ViewBag.PrestamosDevueltos = prestamos.Count(p => p.Estado == "Devuelto");
            ViewBag.PrestamosConPenalizacion = prestamos.Count(p => p.Penalizacion > 0);


            var reportesResponse = await client.GetAsync(_baseUrl + "Reporte/todos");
            var reportesJson = await reportesResponse.Content.ReadAsStringAsync();
            var reportesData = JsonSerializer.Deserialize<ApiResponse<IEnumerable<ReporteApiModel>>>(reportesJson, options);

            ViewBag.TotalReportes = reportesData?.Data?.Count() ?? 0;

            return View();
        }
    }
}