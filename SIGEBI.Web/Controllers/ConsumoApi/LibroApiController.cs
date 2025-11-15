using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.Models;
using System.Text.Json;
using System.Text;
using SIGEBI.Web.Models.LibroApi;

namespace SIGEBI.Web.Controllers.ConsumoApi
{
    [AuthFilter]
    [RoleFilter("Admin", "Docente", "Estudiante")]
    public class LibroApiController : Controller
    {
        private readonly string _baseUrl = "http://localhost:5286/api/";

      
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync("Libro/todos");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al obtener libros.";
                return View(new List<LibroApiModel>());
            }

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<LibroApiModel>>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(result?.Data ?? new List<LibroApiModel>());
        }

       
        public async Task<IActionResult> Details(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync($"Libro/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<LibroApiModel>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(result?.Data);
        }

        
        public IActionResult Create() => View();

      
        [HttpPost]
        public async Task<IActionResult> Create(LibroApiCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Libro/registrar", content);
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al registrar el libro.";
                return View(model);
            }

            TempData["Ok"] = "Libro registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync($"Libro/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<LibroApiUpdateModel>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(result?.Data);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LibroApiUpdateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("Libro/modificar", content);
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al editar el libro.";
                return View(model);
            }

            TempData["Ok"] = "Libro actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.DeleteAsync($"Libro/{id}");
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            TempData[result?.Success == true ? "Ok" : "Error"] =
                result?.Message ?? "Error al eliminar el libro.";

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var content = new StringContent($"\"{estado}\"", Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"Libro/estado/{id}", content);


            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            TempData[result?.Success == true ? "Ok" : "Error"] =
                result?.Message ?? "Error al cambiar el estado.";

            return RedirectToAction(nameof(Index));
        }

       
        [HttpPost]
        public async Task<IActionResult> BuscarPorTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "Debe introducir un título.";
                return RedirectToAction(nameof(Index));
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync($"Libro/titulo/{titulo}");


            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se encontraron libros.";
                return RedirectToAction(nameof(Index));
            }

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<LibroApiModel>>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View("Index", result?.Data);
        }

      
        [HttpPost]
        public async Task<IActionResult> BuscarPorAutor(string autor)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync($"Libro/autor/{autor}");


            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se encontraron libros.";
                return RedirectToAction(nameof(Index));
            }

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<LibroApiModel>>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View("Index", result?.Data);
        }

       
        [HttpPost]
        public async Task<IActionResult> Filtrar(string? titulo, string? autor, string? categoria, int? anio, string? estado)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var url = $"Libro/filtrar?titulo={titulo}&autor={autor}&categoria={categoria}&anio={anio}&estado={estado}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se encontraron libros con los filtros aplicados.";
                return RedirectToAction(nameof(Index));
            }

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<LibroApiModel>>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View("Index", result?.Data);
        }
    }
}
