using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;
using SIGEBI.Application.Interfaces;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers.Integracion
{
    [AuthFilter]
    [RoleFilter("Admin", "Docente")]
    public class PrestamoAdmController : Controller
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamoAdmController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        public async Task<ActionResult> Index()
        {
            var result = await _prestamoService.ObtenerTodosAsync<IEnumerable<PrestamoGetDto>>();

            if (!result.Success)
                return View("Error", result);

            return View(result.Data);
        }


        public async Task<ActionResult> Details(int id)
        {
            var result = await _prestamoService.ObtenerTodosAsync<IEnumerable<PrestamoGetDto>>();
            var prestamo = result.Data?.FirstOrDefault(p => p.Id == id);

            if (prestamo == null)
                return NotFound();

            return View(prestamo);
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PrestamoCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique los datos ingresados.";
                return View(dto);
            }

           
            var restriccion = await _prestamoService.RestringirPrestamoSiPenalizadoAsync<bool>(dto.UsuarioId);

            if (!restriccion.Success)
            {
                TempData["Error"] = restriccion.Message;
                return RedirectToAction(nameof(Index));
            }

            if (restriccion.Data) 
            {
                TempData["Error"] = "El usuario tiene penalizaciones pendientes y no puede realizar nuevos préstamos.";
                return RedirectToAction(nameof(Index));
            }

           
            var result = await _prestamoService.RegistrarPrestamoAsync<PrestamoGetDto>(dto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Ok"] = "Préstamo registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> Edit(int id)
        {
            var result = await _prestamoService.ObtenerTodosAsync<IEnumerable<PrestamoGetDto>>();
            var prestamo = result.Data?.FirstOrDefault(p => p.Id == id);

            if (prestamo == null)
                return NotFound();

            var updateDto = new PrestamoUpdateDto
            {
                Id = prestamo.Id,
                FechaVencimiento = prestamo.FechaVencimiento
            };

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PrestamoUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique los campos ingresados.";
                return View(dto);
            }

            var result = await _prestamoService.ExtenderPrestamoAsync<PrestamoGetDto>(dto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(dto);
            }

            TempData["Ok"] = "Préstamo actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> RegistrarDevolucion(int id)
        {
            var result = await _prestamoService.RegistrarDevolucionAsync<PrestamoGetDto>(id, DateTime.Now);

            TempData[result.Success ? "Ok" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> CalcularPenalizacion(int id)
        {
            var result = await _prestamoService.CalcularPenalizacionAsync<PrestamoGetDto>(id);

            TempData[result.Success ? "Ok" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> Historial(int usuarioId)
        {
            var result = await _prestamoService.ObtenerHistorialUsuarioAsync<IEnumerable<PrestamoGetDto>>(usuarioId);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UsuarioId = usuarioId;
            return View(result.Data);
        }


        public async Task<ActionResult> Delete(int id)
        {
            var result = await _prestamoService.ObtenerTodosAsync<IEnumerable<PrestamoGetDto>>();
            var prestamo = result.Data?.FirstOrDefault(p => p.Id == id);

            if (prestamo == null)
                return NotFound();

            return View(prestamo);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var result = await _prestamoService.RemoveAsync<object>(id);

            TempData[result.Success ? "Ok" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

    }
}
