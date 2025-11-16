using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.EjemplarApi;
using System.Text;
using System.Text.Json;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin", "Docente")]
    public class EjemplarApiController : Controller
    {
        private readonly string _baseUrl = "http://localhost:5286/api/";


        public async Task<IActionResult> Index(string? search)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync("Ejemplar/todos"); 

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al obtener ejemplares.";
                return View(new List<EjemplarApiModel>());
            }

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<EjemplarApiModel>>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var ejemplares = result?.Data?.ToList() ?? new List<EjemplarApiModel>();

           
            if (!string.IsNullOrWhiteSpace(search))
            {
                ViewBag.SearchTerm = search;

                if (int.TryParse(search, out int numero))
                {
                    
                    ejemplares = ejemplares
                        .Where(e => e.Id == numero || e.LibroId == numero)
                        .ToList();
                }
                else
                {
                   
                    ejemplares = ejemplares
                        .Where(e => !string.IsNullOrEmpty(e.CodigoBarras) &&
                                    e.CodigoBarras.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            return View(ejemplares);
        }



        public async Task<IActionResult> Details(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync($"Ejemplar/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<EjemplarApiModel>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (result == null || !result.Success || result.Data == null)
                return NotFound();

            return View(result.Data);
        }

        
        public IActionResult Create() => View();

        
        [HttpPost]
        public async Task<IActionResult> Create(EjemplarApiCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

           
            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Ejemplar/registrar", content);

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<object>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al crear el ejemplar.";
                return View(model);
            }

            TempData["Ok"] = "Ejemplar registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync($"Ejemplar/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<EjemplarApiModel>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (result == null || !result.Success || result.Data == null)
                return NotFound();

           
            var updateModel = new EjemplarApiUpdateModel
            {
                Id = result.Data.Id,
                Estado = result.Data.Estado
            };

            return View(updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EjemplarApiUpdateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("Ejemplar/actualizar", content);

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al editar el ejemplar.";
                return View(model);
            }

            TempData["Ok"] = "Ejemplar actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }




        [HttpPost]
        public async Task<IActionResult> MarcarComoPerdido(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

           
            var response = await client.PutAsync($"Ejemplar/perdido/{id}", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo marcar como perdido.";
                return RedirectToAction(nameof(Index));
            }

            var body = await response.Content.ReadAsStringAsync();

            ApiResponse<object>? result = null;
            if (!string.IsNullOrWhiteSpace(body))
            {
                result = JsonSerializer.Deserialize<ApiResponse<object>>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "No se pudo marcar como perdido.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Ok"] = "Ejemplar marcado como perdido.";
            return RedirectToAction(nameof(Index));
        }

    }
}
