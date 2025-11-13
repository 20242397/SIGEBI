using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Notificacion;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Web.Controllers
{
    public class NotificacionAdmController : Controller
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionAdmController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        public async Task<ActionResult> Index(int? usuarioId, string? tipo, string? estado, bool? noLeidas)
        {
            var result = await _notificacionService.ObtenerTodosAsync<IEnumerable<NotificacionGetDto>>();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View("Error", result);
            }

            var notificaciones = result.Data.AsQueryable();

           
            if (usuarioId.HasValue)
                notificaciones = notificaciones.Where(n => n.UsuarioId == usuarioId.Value);

            if (!string.IsNullOrWhiteSpace(tipo))
                notificaciones = notificaciones.Where(n =>
                    n.Tipo.Contains(tipo, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(estado))
            {
                bool enviado = estado.Equals("Enviada", StringComparison.OrdinalIgnoreCase);
                notificaciones = notificaciones.Where(n => n.Enviado == enviado);
            }

            if (noLeidas == true)
                notificaciones = notificaciones.Where(n => !n.Enviado);

          
            ViewBag.UsuarioId = usuarioId;
            ViewBag.Tipo = tipo;
            ViewBag.Estado = estado;
            ViewBag.NoLeidas = noLeidas;

            return View(notificaciones.ToList());
        }

       
        public async Task<ActionResult> Details(int id)
        {
            var result = await _notificacionService.ObtenerTodosAsync<IEnumerable<NotificacionGetDto>>();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var notificacion = result.Data.FirstOrDefault(n => n.Id == id);
            if (notificacion == null)
            {
                TempData["Error"] = "Notificación no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(notificacion);
        }

      
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NotificacionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique los campos ingresados.";
                return View(dto);
            }

            var result = await _notificacionService.EnviarNotificacionAsync<NotificacionGetDto>(dto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(dto);
            }

            TempData["Ok"] = "Notificación creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> GenerarAutomaticas()
        {
            var result = await _notificacionService.GenerarNotificacionesAutomaticasAsync();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Ok"] = result.Message ?? "Notificaciones automáticas generadas correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

       
        [HttpPost]
        public async Task<ActionResult> MarcarTodasEnviadas(int usuarioId)
        {
            var result = await _notificacionService.MarcarTodasComoEnviadasPorUsuarioAsync<int>(usuarioId);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Ok"] = $"Se marcaron {result.Data} notificaciones como enviadas.";

            return RedirectToAction(nameof(Index), new { usuarioId });
        }
    }
}
