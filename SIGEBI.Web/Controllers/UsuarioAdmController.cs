using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Usuario;
using SIGEBI.Application.Interfaces;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin")]
    public class UsuarioAdmController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioAdmController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // LISTAR
        public async Task<ActionResult> Index()
        {
            var result = await _usuarioService.ObtenerTodosAsync<IEnumerable<UsuarioGetDto>>();

            if (!result.Success)
                return View("Error", result);

            return View(result.Data);
        }

        // BUSCAR POR EMAIL
        [HttpPost]
        public async Task<ActionResult> BuscarPorEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Debe introducir un correo válido.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _usuarioService.ObtenerPorEmailAsync<UsuarioGetDto>(email);

            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = result.Message ?? "No se encontró un usuario con ese email.";
                return RedirectToAction(nameof(Index));
            }

            return View("Index", new List<UsuarioGetDto> { result.Data });
        }

        // CREAR (GET)
        public ActionResult Create() => View();

        // CREAR (POST)
        [HttpPost]
        public async Task<ActionResult> Create(UsuarioCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _usuarioService.RegistrarUsuarioAsync<UsuarioGetDto>(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message ?? "Error al registrar el usuario.");
                return View(dto);
            }

            TempData["Ok"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // EDITAR (GET)
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var result = await _usuarioService.ObtenerPorIdAsync<UsuarioUpdateDto>(id);

            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = result.Message ?? "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UsuarioUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _usuarioService.EditarUsuarioAsync<UsuarioGetDto>(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message ?? "Error al editar el usuario.");
                return View(dto);
            }

            TempData["Ok"] = "Usuario editado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // CAMBIAR ESTADO
        public async Task<ActionResult> CambiarEstado(int id, bool activo)
        {
            var result = await _usuarioService.CambiarEstadoAsync<UsuarioGetDto>(id, activo);

            TempData[result.Success ? "Ok" : "Error"] = result.Message ??
                (result.Success ? "Estado actualizado." : "Error al cambiar estado.");

            return RedirectToAction(nameof(Index));
        }

        // ASIGNAR ROL
        public async Task<ActionResult> AsignarRol(int id, string nuevoRol)
        {
            var result = await _usuarioService.AsignarRolAsync<UsuarioGetDto>(id, nuevoRol);

            TempData[result.Success ? "Ok" : "Error"] = result.Message ??
                (result.Success ? "Rol actualizado." : "Error al asignar rol.");

            return RedirectToAction(nameof(Index));
        }

        // DETAILS
        public async Task<ActionResult> Details(int id)
        {
            var result = await _usuarioService.ObtenerPorIdAsync<UsuarioGetDto>(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return View(result.Data);
        }

        // DELETE
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _usuarioService.ObtenerPorIdAsync<UsuarioGetDto>(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return View(result.Data);
        }

        // DELETE CONFIRMADO
        [HttpPost]
        public async Task<ActionResult> DeleteConfirmado(int id)
        {
            var result = await _usuarioService.RemoveAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Message ?? "Error al eliminar el usuario.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Ok"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
