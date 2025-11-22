using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models.AuthApi;

namespace SIGEBI.Web.Controllers
{
    public class AuthApiController : Controller
    {
        private readonly IAuthApiService _authService;

        public AuthApiController(IAuthApiService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete todos los campos.";
                return View(model);
            }

            var user = await _authService.LoginAsync(model);

            if (user == null)
            {
                TempData["Error"] = "Credenciales inválidas o error del servidor.";
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

        public IActionResult NoAutorizado() => View();
    }
}

