using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.UsuarioApi;
using System.Text.Json;
using System.Text;
using System.Net.Http;

namespace SIGEBI.Web.Controllers.ConsumoApi
{
    [AuthFilter]
    [RoleFilter("Admin")]
    public class UsuarioApiController : Controller
    {
        private readonly string _baseUrl = "http://localhost:5286/api/";

        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync("Usuario/todos");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al obtener usuarios.";
                return View(new List<UsuarioApiModel>());
            }

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<UsuarioApiModel>>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(result?.Data ?? new List<UsuarioApiModel>());
        }

       
        public async Task<IActionResult> Details(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync($"Usuario/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<UsuarioApiModel>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(result?.Data);
        }

       
        public IActionResult Create() => View();

       
        [HttpPost]
        public async Task<IActionResult> Create(UsuarioApiCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Usuario/registrar", content);

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al crear el usuario.";
                return View(model);
            }

            TempData["Ok"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

      
        public async Task<IActionResult> Edit(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync($"Usuario/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<UsuarioApiUpdateModel>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(result?.Data);
        }

      
        [HttpPost]
        public async Task<IActionResult> Edit(UsuarioApiUpdateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("Usuario/editar", content);
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Error al editar el usuario.";
                return View(model);
            }

            TempData["Ok"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

     
        [HttpPost]
        public async Task<IActionResult> BuscarPorEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Debe introducir un correo válido.";
                return RedirectToAction(nameof(Index));
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync($"Usuario/email/{email}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se encontró un usuario con ese email.";
                return RedirectToAction(nameof(Index));
            }

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<UsuarioApiModel>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View("Index", new List<UsuarioApiModel> { result.Data });
        }

      
        public async Task<IActionResult> CambiarEstado(int id, bool activo)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.PutAsync(
                $"Usuario/{id}/estado?activo={activo}",
                null
            );

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
        public async Task<IActionResult> AsignarRol(int id, string nuevoRol)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.PutAsync(
                $"Usuario/{id}/rol?rol={nuevoRol}",
                null
            );

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            TempData[result?.Success == true ? "Ok" : "Error"] =
                result?.Message ?? "Error al asignar rol.";

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.GetAsync($"Usuario/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<UsuarioApiModel>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return View(result?.Data);
        }

        
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmado(int id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            var response = await client.DeleteAsync($"Usuario/{id}");

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            TempData[result?.Success == true ? "Ok" : "Error"] =
                result?.Message ?? "Error al eliminar usuario.";

            return RedirectToAction(nameof(Index));
        }
    }
}
