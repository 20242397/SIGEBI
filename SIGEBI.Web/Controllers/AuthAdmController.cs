using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Auth;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRequestDto());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = result.Message;
                return View(dto);
            }

            // ====== GUARDAR SESIÓN ======
            HttpContext.Session.SetInt32("UserId", result.Data.Id);
            HttpContext.Session.SetString("UserName", result.Data.NombreCompleto);
            HttpContext.Session.SetString("UserRole", result.Data.Role);
            HttpContext.Session.SetString("UserEmail", result.Data.Email);

            // ====== REDIRECCIÓN POR ROL ======
            switch (result.Data.Role)
            {
                case "Admin":
                    return RedirectToAction("Index", "DashboardAdm");

                case "Docente":
                    return RedirectToAction("Index", "DashboardDoc");

                case "Estudiante":
                    return RedirectToAction("Index", "DashboardEst");

                default:
                    // Por si acaso viene algún rol raro
                    return RedirectToAction("Index", "DashboardAdm");
            }
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

