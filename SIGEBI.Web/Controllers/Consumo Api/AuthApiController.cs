using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Auth;

namespace SIGEBI.Web.Controllers.Integracion
{
    public class AuthApiController : Controller
    {
        private readonly HttpClient _http;

        public AuthApiController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("SIGEBIApi");
        }

     
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRequestDto());
        }

        
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
          
            var response = await _http.PostAsJsonAsync("Auth/login", dto);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                TempData["Error"] = "Credenciales incorrectas.";
                return View(dto);
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al contactar el servidor.";
                return View(dto);
            }

          
            var user = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            if (user == null)
            {
                TempData["Error"] = "Error inesperado en la respuesta del servidor.";
                return View(dto);
            }

           
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.NombreCompleto);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserEmail", user.Email);

            
            return user.Role switch
            {
                "Admin" => RedirectToAction("Index", "DashboardAdm"),
                "Docente" => RedirectToAction("Index", "DashboardDoc"),
                "Estudiante" => RedirectToAction("Index", "DashboardEst"),
                _ => RedirectToAction("Index", "DashboardAdm")
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
