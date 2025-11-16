using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Models.AuthApi;
using System.Text;
using System.Text.Json;

namespace SIGEBI.Web.Controllers
{
    public class AuthApiController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5286/api/");

           
            var jsonBody = JsonSerializer.Serialize(model);

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

          
            var response = await client.PostAsync("Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Credenciales incorrectas o error del servidor.";
                return View(model);
            }

            var body = await response.Content.ReadAsStringAsync();

            var user = JsonSerializer.Deserialize<LoginResponseModel>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (user == null)
            {
                TempData["Error"] = "Error al procesar la respuesta del servidor.";
                return View(model);
            }

           
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.NombreCompleto);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserEmail", user.Email);

            return user.Role switch
            {
                "Admin" => RedirectToAction("Index", "DashboardAdmApi"),
                "Docente" => RedirectToAction("Index", "DashboardDocApi"),
                "Estudiante" => RedirectToAction("Index", "DashboardEstApi"),
                _ => RedirectToAction("Login")
            };
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult NoAutorizado()
        {
            return View();
        }
    }
}
