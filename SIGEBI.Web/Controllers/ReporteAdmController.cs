using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Reporte;
using SIGEBI.Application.Interfaces;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin")]
    public class ReporteAdmController : Controller
    {
        private readonly IReporteService _reporteService;
        private readonly ILogger<ReporteAdmController> _logger;

        public ReporteAdmController(IReporteService reporteService, ILogger<ReporteAdmController> logger)
        {
            _reporteService = reporteService;
            _logger = logger;
        }

     
        [HttpGet]
        public async Task<IActionResult> Index(string tipo = "", DateTime? inicio = null, DateTime? fin = null)
        {
            try
            {
                var result = string.IsNullOrWhiteSpace(tipo)
                    ? await _reporteService.ObtenerTodosAsync<IEnumerable<ReporteGetDto>>()
                    : await _reporteService.ObtenerPorTipoAsync<IEnumerable<ReporteGetDto>>(tipo);

                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    return View(new List<ReporteGetDto>());
                }

                var lista = result.Data ?? new List<ReporteGetDto>();

             
                if (inicio.HasValue && fin.HasValue)
                    lista = lista.Where(r => r.FechaGeneracion >= inicio && r.FechaGeneracion <= fin).ToList();

                ViewBag.Tipo = tipo;
                ViewBag.Inicio = inicio?.ToString("yyyy-MM-dd");
                ViewBag.Fin = fin?.ToString("yyyy-MM-dd");

                return View(lista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los reportes.");
                TempData["Error"] = "Error al cargar los reportes.";
                return View(new List<ReporteGetDto>());
            }
        }

      
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _reporteService.ObtenerPorIdAsync<ReporteGetDto>(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = result.Message ?? "Reporte no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new ReporteCreateDto());
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReporteCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos inválidos.";
                return View(dto);
            }

            var result = await _reporteService.GenerarReporteAsync<ReporteGetDto>(dto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(dto);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

       
        [HttpGet]
        public async Task<IActionResult> Exportar(int id, string formato)
        {
            var result = await _reporteService.ExportarReporteAsync<string>(id, formato);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            string rutaArchivo = result.Data!;
            var fileName = Path.GetFileName(rutaArchivo);
            var contentType = formato.ToLower() switch
            {
                "pdf" => "application/pdf",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "text/plain"
            };

            var fileBytes = await System.IO.File.ReadAllBytesAsync(rutaArchivo);
            return File(fileBytes, contentType, fileName);
        }

       
        [HttpPost]
        public async Task<IActionResult> FiltrarPorFechas(DateTime inicio, DateTime fin)
        {
            var result = await _reporteService.ObtenerPorFechaAsync<IEnumerable<ReporteGetDto>>(inicio, fin);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Inicio = inicio.ToString("yyyy-MM-dd");
            ViewBag.Fin = fin.ToString("yyyy-MM-dd");
            ViewBag.FiltroActivo = true;

            return View("Index", result.Data);
        }

       
        [HttpPost]
        public async Task<IActionResult> MarcarResuelto(int id, string contenido)
        {
            var dto = new ReporteUpdateDto
            {
                Id = id,
                Contenido = contenido,
                MarcarComoResuelto = true
            };

            var result = await _reporteService.ActualizarReporteAsync<ReporteGetDto>(dto);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}

