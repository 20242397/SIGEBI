using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models.NotificacionApi;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin", "Docente")]
    public class NotificacionApiController : Controller
    {
        private readonly INotificacionApiService _service;

        public NotificacionApiController(INotificacionApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int? usuarioId, string? tipo, string? estado, bool? noLeidas)
        {
            var notifs = await _service.FiltrarAsync(usuarioId, tipo, estado, noLeidas);

            ViewBag.UsuarioId = usuarioId;
            ViewBag.Tipo = tipo;
            ViewBag.Estado = estado;
            ViewBag.NoLeidas = noLeidas;

            return View(notifs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var n = await _service.GetByIdAsync(id);
            return n == null ? RedirectToAction(nameof(Index)) : View(n);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(NotificacionApiCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos inválidos.";
                return View(model);
            }

            var r = await _service.CreateAsync(model);

            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> GenerarAutomaticas()
        {
            var r = await _service.GenerarAutomaticasAsync();

            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarcarTodasEnviadas(int usuarioId)
        {
            var r = await _service.MarcarTodasEnviadasAsync(usuarioId);

            TempData[r.Success ? "Ok" : "Error"] =
                r.Success ? $"Se marcaron {r.Data} notificaciones como enviadas." : r.Message!;

            return RedirectToAction(nameof(Index), new { usuarioId });
        }
    }
}

