using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.LibroApi;
using System.Text;
using System.Text.Json;

namespace SIGEBI.Web.Controllers
{
    public class LibroWebController : Controller
    {
        private const string BaseUrl = "http://localhost:5286/api/";

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);

            var response = await client.GetAsync("Libro/todos");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error obteniendo la lista de libros.";
                return View(new List<LibroApiModel>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<LibroApiModel>>>(json, _jsonOptions);

            return View(result?.Data ?? new List<LibroApiModel>());
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{BaseUrl}Libro/{id}");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<LibroApiModel>>(json, _jsonOptions);

            if (result == null || result.Data == null)
                return NotFound();

            return View(result.Data);
        }


        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(LibroApiCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{BaseUrl}Libro/registrar", content);
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);

            if (result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al registrar el libro.";
                return View(model);
            }

            TempData["Ok"] = "Libro registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync($"{BaseUrl}Libro/{id}");
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<LibroApiUpdateModel>>(json, _jsonOptions);

            if (result == null || result.Data == null)
                return NotFound();

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LibroApiUpdateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{BaseUrl}Libro/modificar", content);
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);

            if (result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al actualizar el libro.";
                return View(model);
            }

            TempData["Ok"] = "Libro actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{BaseUrl}Libro/{id}");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<LibroApiModel>>(json, _jsonOptions);

            if (result == null || result.Data == null)
                return NotFound();

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmado(int id)
        {
            using var client = new HttpClient();

            var response = await client.DeleteAsync($"{BaseUrl}Libro/{id}");
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);

            if (result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al eliminar el libro.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Ok"] = result.Message ?? "Libro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> BuscarPorTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "Debe escribir un título.";
                return RedirectToAction(nameof(Index));
            }

            using var client = new HttpClient();

            var url = $"{BaseUrl}Libro/titulo/{Uri.EscapeDataString(titulo)}";
            var response = await client.GetAsync(url);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<LibroApiModel>>>(json, _jsonOptions);

            return View("Index", result?.Data ?? new List<LibroApiModel>());
        }


        [HttpPost]
        public async Task<IActionResult> BuscarPorAutor(string autor)
        {
            if (string.IsNullOrWhiteSpace(autor))
            {
                TempData["Error"] = "Debe escribir un autor.";
                return RedirectToAction(nameof(Index));
            }

            using var client = new HttpClient();

            var url = $"{BaseUrl}Libro/autor/{Uri.EscapeDataString(autor)}";
            var response = await client.GetAsync(url);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<LibroApiModel>>>(json, _jsonOptions);

            return View("Index", result?.Data ?? new List<LibroApiModel>());
        }



        [HttpPost]
        public async Task<IActionResult> Filtrar(
            string? titulo,
            string? autor,
            string? categoria,
            int? anio,
            string? estado)
        {
            using var client = new HttpClient();

            var queryParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(titulo))
                queryParts.Add($"titulo={Uri.EscapeDataString(titulo)}");

            if (!string.IsNullOrWhiteSpace(autor))
                queryParts.Add($"autor={Uri.EscapeDataString(autor)}");

            if (!string.IsNullOrWhiteSpace(categoria))
                queryParts.Add($"categoria={Uri.EscapeDataString(categoria)}");

            if (anio.HasValue)
                queryParts.Add($"anio={anio.Value}");

            if (!string.IsNullOrWhiteSpace(estado))
                queryParts.Add($"estado={Uri.EscapeDataString(estado)}");

            var url = $"{BaseUrl}Libro/filtrar";
            if (queryParts.Any())
                url += "?" + string.Join("&", queryParts);

            var response = await client.GetAsync(url);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<LibroApiModel>>>(json, _jsonOptions);

            return View("Index", result?.Data ?? new List<LibroApiModel>());
        }


        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(estado))
            {
                TempData["Error"] = "Datos inválidos para cambiar estado.";
                return RedirectToAction(nameof(Index));
            }

            using var client = new HttpClient();

            var jsonBody = JsonSerializer.Serialize(estado);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{BaseUrl}Libro/estado/{id}", content);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);

            if (result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al cambiar el estado.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Ok"] = result.Message ?? "Estado actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}