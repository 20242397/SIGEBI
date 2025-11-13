using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar;
using SIGEBI.Application.Interfaces;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin", "Docente")]
    public class EjemplarAdmController : Controller
    {
        private readonly IEjemplarService _ejemplarService;

        public EjemplarAdmController(IEjemplarService ejemplarService)
        {
            _ejemplarService = ejemplarService;
        }

       
        public async Task<ActionResult> Index(string? search)
        {
            var result = await _ejemplarService.ObtenerTodosAsync<IEnumerable<EjemplarGetDto>>();
            if (!result.Success)
                return View("Error", result);

            var ejemplares = result.Data;


            if (!string.IsNullOrWhiteSpace(search))
            {
                if (int.TryParse(search, out int libroIdBuscado))
                {
                    ejemplares = ejemplares.Where(e => e.LibroId == libroIdBuscado);
                }
                else
                {
                   
                    ejemplares = ejemplares.Where(e =>
                        e.CodigoBarras.Contains(search, StringComparison.OrdinalIgnoreCase));
                }
            }


            
            ViewBag.SearchTerm = search;

            
            return View(ejemplares);
        }


        
        public async Task<ActionResult> Details(int id)
        {
            var result = await _ejemplarService.ObtenerPorIdAsync<EjemplarGetDto>(id);

            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = result.Message ?? "Ejemplar no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        public ActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EjemplarCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique los datos ingresados.";
                return View(dto);
            }

            var result = await _ejemplarService.RegistrarEjemplarAsync<EjemplarGetDto>(dto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(dto);
            }

            TempData["Ok"] = "Ejemplar registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

       
        public async Task<ActionResult> Edit(int id)
        {
            var result = await _ejemplarService.ObtenerPorIdAsync<EjemplarGetDto>(id);

            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = result.Message ?? "Ejemplar no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var updateDto = new EjemplarUpdateDto
            {
                Id = result.Data.Id,
                CodigoBarras = result.Data.CodigoBarras,
                Estado = result.Data.Estado,
                LibroId = result.Data.LibroId
            };

            return View(updateDto);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EjemplarUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifique los campos ingresados.";
                return View(dto);
            }

            var result = await _ejemplarService.ActualizarEjemplarAsync(dto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(dto);
            }

            TempData["Ok"] = "Ejemplar actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

     
        [HttpPost]
        public async Task<ActionResult> MarcarComoPerdido(int id)
        {
            var result = await _ejemplarService.MarcarComoPerdidoAsync<bool>(id);

            if (!result.Success)
                TempData["Error"] = result.Message ?? "No se pudo marcar el ejemplar como perdido.";
            else
                TempData["Ok"] = "Ejemplar marcado como perdido correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}
