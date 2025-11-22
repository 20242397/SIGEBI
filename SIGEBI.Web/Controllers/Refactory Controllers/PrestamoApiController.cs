using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models.PrestamoApi;

namespace SIGEBI.Web.Controllers.Integracion
{
    public class PrestamoApiController : Controller
    {
        private readonly IPrestamoApiService _service;

        public PrestamoApiController(IPrestamoApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var prestamos = await _service.GetAllAsync();
            return View(prestamos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var p = await _service.GetByIdAsync(id);
            return p == null ? RedirectToAction(nameof(Index)) : View(p);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(PrestamoApiCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var restricciones = await _service.VerificarRestriccionesAsync(model.UsuarioId);

            if (restricciones.Data)
            {
                TempData["Error"] = "El usuario tiene penalizaciones pendientes.";
                return RedirectToAction(nameof(Index));
            }

            var r = await _service.CreateAsync(model);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _service.GetByIdAsync(id);
            if (p == null) return NotFound();

            return View(new PrestamoApiUpdateModel
            {
                Id = p.Id,
                FechaVencimiento = p.FechaVencimiento
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PrestamoApiUpdateModel model)
        {
            var r = await _service.UpdateAsync(model);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RegistrarDevolucion(int id)
        {
            var r = await _service.RegistrarDevolucionAsync(id);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CalcularPenalizacion(int id)
        {
            var r = await _service.CalcularPenalizacionAsync(id);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Historial(int usuarioId)
        {
            var prestamos = await _service.HistorialAsync(usuarioId);
            ViewBag.UsuarioId = usuarioId;
            return View(prestamos);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var p = await _service.GetByIdAsync(id);
            return p == null ? NotFound() : View(p);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var r = await _service.DeleteAsync(id);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }
    }

}
