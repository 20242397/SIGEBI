using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.ReporteApi;
using System.Text;
using System.Text.Json;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin")]
    public class ReporteApiController : Controller
    {
        private readonly string _baseUrl = "http://localhost:5286/api/Reporte/";
        private readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

       
        public async Task<IActionResult> Index(string tipo = "", DateTime? inicio = null, DateTime? fin = null)
        {
            using var client = new HttpClient();

            HttpResponseMessage response;

            if (string.IsNullOrWhiteSpace(tipo))
                response = await client.GetAsync(_baseUrl + "todos");
            else
                response = await client.GetAsync(_baseUrl + $"tipo/{tipo}");

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<ApiResponse<IEnumerable<ReporteApiModel>>>(json, _jsonOptions);

            if (data == null || data.Data == null)
                return View(new List<ReporteApiModel>());

            var lista = data.Data.ToList();


            if (inicio.HasValue && fin.HasValue)
            {
                lista = lista.Where(r =>
                    r.FechaGeneracion >= inicio.Value &&
                    r.FechaGeneracion <= fin.Value
                ).ToList();
            }

            ViewBag.Tipo = tipo;
            ViewBag.Inicio = inicio?.ToString("yyyy-MM-dd");
            ViewBag.Fin = fin?.ToString("yyyy-MM-dd");

            return View(lista);
        }

      
        public async Task<IActionResult> Details(int id)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(_baseUrl + id);

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<ApiResponse<ReporteApiModel>>(json, _jsonOptions);

            if (data == null || data.Data == null)
            {
                TempData["Error"] = "Reporte no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(data.Data);
        }

     
        [HttpGet]
        public IActionResult Create() => View(new ReporteApiCreateModel());


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReporteApiCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_baseUrl + "generar", content);
            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<ApiResponse<ReporteApiModel>>(json, _jsonOptions);

            if (data == null || !data.Success)
            {
                TempData["Error"] = data?.Message ?? "Error al generar el reporte.";
                return View(model);
            }

            TempData["Ok"] = data.Message;
            return RedirectToAction(nameof(Index));
        }

       
        [HttpPost]
        public async Task<IActionResult> Actualizar(ReporteApiUpdateModel model)
        {
            using var client = new HttpClient();

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(_baseUrl + "actualizar", content);
            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);

            if (data == null || !data.Success)
                TempData["Error"] = data?.Message;
            else
                TempData["Ok"] = data.Message;

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

      
        [HttpGet]
        public async Task<IActionResult> Exportar(int id, string formato)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(_baseUrl + $"exportar/{id}?formato={formato}");
            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<ApiResponse<string>>(json, _jsonOptions);

            if (data == null || !data.Success)
            {
                TempData["Error"] = data?.Message;
                return RedirectToAction(nameof(Index));
            }

            var ruta = data.Data!;
            var bytes = await System.IO.File.ReadAllBytesAsync(ruta);
            var fileName = Path.GetFileName(ruta);

            string contentType = formato.ToLower() switch
            {
                "pdf" => "application/pdf",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "text/plain"
            };

            return File(bytes, contentType, fileName);
        }

        

    }
}
