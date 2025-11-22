using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;
using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models.LibroApi;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin", "Docente")]
    public class LibroApiController : Controller
    {
        private readonly ILibroApiService _service;

        public LibroApiController(ILibroApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var libros = await _service.GetAllAsync();
            return View(libros);
        }

        public async Task<IActionResult> Details(int id)
        {
            var libro = await _service.GetByIdAsync(id);
            return libro == null ? NotFound() : View(libro);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(LibroApiCreateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var r = await _service.CreateAsync(model);

            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var libro = await _service.GetByIdAsync(id);

            if (libro == null)
                return NotFound();

            var model = new LibroApiUpdateModel
            {
                Id = libro.Id,
                Titulo = libro.Titulo,
                Autor = libro.Autor,
                ISBN = libro.ISBN,
                Editorial = libro.Editorial,
                Categoria = libro.Categoria,
                AñoPublicacion = libro.AñoPublicacion,
                Estado = libro.Estado
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(LibroApiUpdateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var r = await _service.UpdateAsync(model);

            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _service.GetByIdAsync(id);
            return libro == null ? NotFound() : View(libro);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmado(int id)
        {
            var r = await _service.DeleteAsync(id);

            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> BuscarPorTitulo(string titulo)
        {
            var libros = await _service.BuscarPorTituloAsync(titulo);
            return View("Index", libros);
        }

        [HttpPost]
        public async Task<IActionResult> BuscarPorAutor(string autor)
        {
            var libros = await _service.BuscarPorAutorAsync(autor);
            return View("Index", libros);
        }

        [HttpPost]
        public async Task<IActionResult> Filtrar(string? titulo, string? autor, string? categoria, int? anio, string? estado)
        {
            var libros = await _service.FiltrarAsync(titulo, autor, categoria, anio, estado);
            return View("Index", libros);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            var r = await _service.CambiarEstadoAsync(id, estado);

            TempData[r.Success ? "Ok" : "Error"] = r.Message!;
            return RedirectToAction(nameof(Index));
        }
    }
}


