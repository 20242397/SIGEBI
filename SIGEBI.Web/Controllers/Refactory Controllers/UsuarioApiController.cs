using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models.LibroApi;
using SIGEBI.Web.Models.UsuarioApi;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin")]
    public class UsuarioApiController : Controller
    {
        private readonly IUsuarioApiService _service;

        public UsuarioApiController(IUsuarioApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _service.GetAllAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Details(int id)
        {
            var u = await _service.GetByIdAsync(id);
            return u == null ? RedirectToAction(nameof(Index)) : View(u);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioApiCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var r = await _service.CreateAsync(model);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _service.GetByIdAsync(id);

            if (usuario == null)
                return NotFound();

            var model = new UsuarioApiUpdateModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                PhoneNumber = usuario.PhoneNumber,
                Estado = usuario.Estado,
                Role = usuario.Role,
                Activo = usuario.Activo
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(UsuarioApiUpdateModel model)
        {
            var r = await _service.UpdateAsync(model);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
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

            var user = await _service.GetByEmailAsync(email);

            if (user == null)
            {
                TempData["Error"] = "No se encontró el usuario.";
                return RedirectToAction(nameof(Index));
            }

            return View("Index", new List<UsuarioApiModel> { user });
        }

        public async Task<IActionResult> CambiarEstado(int id, bool activo)
        {
            var r = await _service.CambiarEstadoAsync(id, activo);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AsignarRol(int id, string nuevoRol)
        {
            var r = await _service.AsignarRolAsync(id, nuevoRol);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var u = await _service.GetByIdAsync(id);
            return u == null ? NotFound() : View(u);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmado(int id)
        {
            var r = await _service.DeleteAsync(id);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }
    }
}

