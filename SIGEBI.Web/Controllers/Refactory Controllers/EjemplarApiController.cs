using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models.EjemplarApi;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin", "Docente")]
    public class EjemplarApiController : Controller
    {
        private readonly IEjemplarApiService _service;

        public EjemplarApiController(IEjemplarApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var ejemplares = (await _service.GetAllAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                ViewBag.SearchTerm = search;

                if (int.TryParse(search, out int numero))
                {
                    ejemplares = ejemplares
                        .Where(e => e.Id == numero || e.LibroId == numero)
                        .ToList();
                }
                else
                {
                    ejemplares = ejemplares
                        .Where(e => !string.IsNullOrEmpty(e.CodigoBarras) &&
                                    e.CodigoBarras.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            return View(ejemplares);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ejemplar = await _service.GetByIdAsync(id);
            return ejemplar == null ? NotFound() : View(ejemplar);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(EjemplarApiCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _service.CreateAsync(model);

            if (!result.Success)
            {
                TempData["Error"] = result.Message!;
                return View(model);
            }

            TempData["Ok"] = "Ejemplar registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ejemplar = await _service.GetByIdAsync(id);
            if (ejemplar == null)
                return NotFound();

            return View(new EjemplarApiUpdateModel
            {
                Id = ejemplar.Id,
                Estado = ejemplar.Estado
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EjemplarApiUpdateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _service.UpdateAsync(model);

            TempData[result.Success ? "Ok" : "Error"] = result.Message!;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoPerdido(int id)
        {
            var result = await _service.MarcarComoPerdidoAsync(id);

            TempData[result.Success ? "Ok" : "Error"] =
                result.Message ?? "No se pudo marcar como perdido.";

            return RedirectToAction(nameof(Index));
        }
    }
}
