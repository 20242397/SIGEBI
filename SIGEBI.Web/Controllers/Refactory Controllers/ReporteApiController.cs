using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models.ReporteApi;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin")]
    public class ReporteApiController : Controller
    {
        private readonly IReporteApiService _service;

        public ReporteApiController(IReporteApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string tipo = "", DateTime? inicio = null, DateTime? fin = null)
        {
            IEnumerable<ReporteApiModel> lista;

            if (string.IsNullOrWhiteSpace(tipo))
                lista = await _service.GetAllAsync();
            else
                lista = await _service.GetByTipoAsync(tipo);

            if (inicio.HasValue && fin.HasValue)
                lista = lista.Where(r => r.FechaGeneracion >= inicio && r.FechaGeneracion <= fin).ToList();

            ViewBag.Tipo = tipo;
            ViewBag.Inicio = inicio?.ToString("yyyy-MM-dd");
            ViewBag.Fin = fin?.ToString("yyyy-MM-dd");

            return View(lista);
        }

        public async Task<IActionResult> Details(int id)
        {
            var rep = await _service.GetByIdAsync(id);
            return rep == null ? RedirectToAction(nameof(Index)) : View(rep);
        }

        [HttpGet]
        public IActionResult Create() => View(new ReporteApiCreateModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReporteApiCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var r = await _service.CreateAsync(model);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar(ReporteApiUpdateModel model)
        {
            var r = await _service.UpdateAsync(model);
            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Exportar(int id, string formato)
        {
            var result = await _service.ExportarAsync(id, formato);

            if (!result.Success)
            {
                TempData["Error"] = result.Message!;
                return RedirectToAction(nameof(Index));
            }

            var ruta = result.Data!;
            var bytes = await System.IO.File.ReadAllBytesAsync(ruta);
            var fileName = Path.GetFileName(ruta);

            string contentType = formato.ToLower() switch
            {
                "pdf" => "application/pdf",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "text/plain"
            };

            return File(bytes, contentType, fileName);
        }
    }
}
