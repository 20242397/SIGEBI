using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.PrestamoApi;
using System.Text;
using System.Text.Json;

namespace SIGEBI.Web.Controllers.Integracion
{
    [AuthFilter]
    [RoleFilter("Admin", "Docente", "Estudiante")]
    public class PrestamoApiController : Controller
    {
        private readonly string _baseUrl = "http://localhost:5286/api/Prestamo/";

     
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(_baseUrl + "todos");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al obtener los préstamos.";
                return View(new List<PrestamoApiModel>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<PrestamoApiModel>>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(result?.Data ?? new List<PrestamoApiModel>());
        }



      
        public async Task<IActionResult> Details(int id)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(_baseUrl + "todos");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<PrestamoApiModel>>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var prestamo = result?.Data?.FirstOrDefault(p => p.Id == id);

            if (prestamo == null)
                return NotFound();

            return View(prestamo);
        }



       
        public IActionResult Create() => View();



       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrestamoApiCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique los datos ingresados.";
                return View(model);
            }

            using var client = new HttpClient();

            
            var restriccion = await client.GetAsync(_baseUrl + $"restringir/{model.UsuarioId}");

            var restriccionJson = await restriccion.Content.ReadAsStringAsync();
            var restriccionResult = JsonSerializer.Deserialize<ApiResponse<bool>>(restriccionJson);

            if (restriccionResult?.Data == true)
            {
                TempData["Error"] = "El usuario tiene penalizaciones pendientes.";
                return RedirectToAction(nameof(Index));
            }

          
            var body = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(_baseUrl + "registrar", body);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<PrestamoApiModel>>(json);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = result?.Message ?? "Error en el registro.";
                return View(model);
            }

            TempData["Ok"] = "Préstamo registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }



       
        public async Task<IActionResult> Edit(int id)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(_baseUrl + "todos");

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<PrestamoApiModel>>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var prestamo = result?.Data?.FirstOrDefault(p => p.Id == id);

            if (prestamo == null)
                return NotFound();

            var update = new PrestamoApiUpdateModel
            {
                Id = prestamo.Id,
                FechaVencimiento = prestamo.FechaVencimiento
            };

            return View(update);
        }



      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PrestamoApiUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique los campos ingresados.";
                return View(model);
            }

            using var client = new HttpClient();

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PutAsync(_baseUrl + "extender", content);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<PrestamoApiModel>>(json);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = result?.Message ?? "Error al extender préstamo.";
                return View(model);
            }

            TempData["Ok"] = "Préstamo actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }



       
        public async Task<IActionResult> RegistrarDevolucion(int id)
        {
            using var client = new HttpClient();

            var response = await client.PutAsync(
                _baseUrl + $"devolucion/{id}?fechaDevolucion={DateTime.Now:yyyy-MM-dd}",
                null
            );

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json);

            TempData[response.IsSuccessStatusCode ? "Ok" : "Error"] =
                result?.Message ?? "Error al registrar devolución.";

            return RedirectToAction(nameof(Index));
        }

        



        public async Task<IActionResult> CalcularPenalizacion(int id)
        {
            using var client = new HttpClient();

            var response = await client.PutAsync(_baseUrl + $"penalizacion/{id}", null);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json);

            if (response.IsSuccessStatusCode)
            {
                
                TempData["Ok"] = string.IsNullOrWhiteSpace(result?.Message)
                    ? "Penalización calculada correctamente."
                    : result.Message;
            }
            else
            {
               
                TempData["Error"] = string.IsNullOrWhiteSpace(result?.Message)
                    ? "Error al calcular la penalización."
                    : result.Message;
            }


            return RedirectToAction(nameof(Index));
        }



       
        public async Task<IActionResult> Historial(int usuarioId)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(_baseUrl + $"historial/{usuarioId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo obtener el historial.";
                return RedirectToAction(nameof(Index));
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<PrestamoApiModel>>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.UsuarioId = usuarioId;

            return View(result?.Data ?? new List<PrestamoApiModel>());
        }



       
        public async Task<IActionResult> Delete(int id)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(_baseUrl + "todos");

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<PrestamoApiModel>>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var prestamo = result?.Data?.FirstOrDefault(p => p.Id == id);

            if (prestamo == null)
                return NotFound();

            return View(prestamo);
        }



       
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var client = new HttpClient();

            var response = await client.DeleteAsync(_baseUrl + $"{id}");

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ApiResponse<object>>(json);

            TempData[response.IsSuccessStatusCode ? "Ok" : "Error"] =
                result?.Message ?? "Error al eliminar.";

            return RedirectToAction(nameof(Index));
        }
    }
}
