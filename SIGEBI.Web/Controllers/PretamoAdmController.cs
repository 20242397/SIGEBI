using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Web.Controllers
{
    public class PrestamoAdmController : Controller
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamoAdmController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

       
        public async Task<IActionResult> Index()
        {
            var result = await _prestamoService.ObtenerTodosAsync<IEnumerable<PrestamoGetDto>>();

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(new List<PrestamoGetDto>());
            }

            var prestamos = result.Data ?? new List<PrestamoGetDto>();

            // Estadísticas
            ViewBag.Total = prestamos.Count();
            ViewBag.Activos = prestamos.Count(p => p.FechaDevolucion == null);
            ViewBag.Devueltos = prestamos.Count(p => p.FechaDevolucion != null);
            ViewBag.Vencidos = prestamos.Count(p => p.FechaVencimiento < DateTime.Now && p.FechaDevolucion == null);

            return View(prestamos);
        }

       
        // CREAR PRÉSTAMO
      
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PrestamoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _prestamoService.RegistrarPrestamoAsync<object>(dto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(dto);
            }

            TempData["Ok"] = "Préstamo registrado correctamente.";
            return RedirectToAction("Index");
        }

      
        // ✅ EXTENDER PRÉSTAMO
      
        public async Task<IActionResult> Extender(int id)
        {
            // Cargar datos del préstamo
            var result = await _prestamoService.ObtenerHistorialUsuarioAsync<IEnumerable<PrestamoGetDto>>(id);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Extender(PrestamoUpdateDto dto)
        {
            var result = await _prestamoService.ExtenderPrestamoAsync<object>(dto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(dto);
            }

            TempData["Ok"] = "Fecha de vencimiento actualizada.";
            return RedirectToAction("Index");
        }

      
        // REGISTRAR DEVOLUCIÓN
       
        public async Task<IActionResult> Devolver(int id)
        {
            var fecha = DateTime.Now;

            var result = await _prestamoService.RegistrarDevolucionAsync<object>(id, fecha);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Ok"] = "Devolución registrada correctamente.";

            return RedirectToAction("Index");
        }

       
        // CALCULAR PENALIZACIÓN
      
        public async Task<IActionResult> Penalizar(int id)
        {
            var result = await _prestamoService.CalcularPenalizacionAsync<object>(id);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Ok"] = "Penalización calculada correctamente.";

            return RedirectToAction("Index");
        }

        
        //HISTORIAL POR USUARIO
       
        public async Task<IActionResult> Historial(int usuarioId)
        {
            var result = await _prestamoService.ObtenerHistorialUsuarioAsync<IEnumerable<PrestamoGetDto>>(usuarioId);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }

            ViewBag.UsuarioId = usuarioId;
            return View(result.Data);
        }
    }
}
