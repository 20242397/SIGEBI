using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Models;
using System.Text;
using System.Text.Json;
using SIGEBI.Web.Models.NotificacionApi;

namespace SIGEBI.Web.Controllers
{
    public class NotificacionApiController : Controller
    {
        private const string BaseUrl = "http://localhost:5286/api/Notificacion/";

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };


        public async Task<IActionResult> Index(int? usuarioId, string? tipo, string? estado, bool? noLeidas)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync($"{BaseUrl}todas");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudieron obtener notificaciones.";
                return View(new List<NotificacionApiModel>());
            }

            var json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json) || !json.Trim().StartsWith("{"))
            {
                TempData["Error"] = "La API devolvió datos inválidos.";
                return View(new List<NotificacionApiModel>());
            }

            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<NotificacionApiModel>>>(json, _jsonOptions);

            if (result == null || result.Data == null)
            {
                TempData["Error"] = "Error procesando los datos.";
                return View(new List<NotificacionApiModel>());
            }

            var notificaciones = result.Data.AsQueryable();

            if (usuarioId.HasValue)
                notificaciones = notificaciones.Where(n => n.UsuarioId == usuarioId.Value);

            if (!string.IsNullOrWhiteSpace(tipo))
                notificaciones = notificaciones.Where(n => n.Tipo.Contains(tipo, StringComparison.OrdinalIgnoreCase));

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

        public async Task<IActionResult> Details(int id)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync($"{BaseUrl}todas");
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<NotificacionApiModel>>>(json, _jsonOptions);

            var notificacion = result?.Data?.FirstOrDefault(n => n.Id == id);

            if (notificacion == null)
            {
                TempData["Error"] = "Notificación no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(notificacion);
        }

        public IActionResult Create() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NotificacionApiCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos inválidos.";
                return View(model);
            }

            using var client = new HttpClient();

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{BaseUrl}enviar", content);
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);

            if (result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al crear la notificación.";
                return View(model);
            }

            TempData["Ok"] = "Notificación creada correctamente.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> GenerarAutomaticas()
        {
            using var client = new HttpClient();

            var response = await client.PostAsync($"{BaseUrl}generar-automaticas", null);
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);

            if (result == null || !result.Success)
                TempData["Error"] = result?.Message ?? "Error generando notificaciones.";
            else
                TempData["Ok"] = result.Message ?? "Notificaciones automáticas generadas.";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> MarcarTodasEnviadas(int usuarioId)
        {
            using var client = new HttpClient();

            var response = await client.PutAsync($"{BaseUrl}usuario/{usuarioId}/marcar-enviadas", null);

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<int>>(json, _jsonOptions);

            if (result == null || !result.Success)
                TempData["Error"] = result?.Message ?? "Error al marcar notificaciones.";
            else
                TempData["Ok"] = $"Se marcaron {result.Data} notificaciones como enviadas.";

            return RedirectToAction(nameof(Index), new { usuarioId });
        }
    }
}

