using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers.Integracion
{
    [AuthFilter]
    [RoleFilter("Admin", "Docente","Estudiante")]
    public class LibroAdmController : Controller
    {
        private readonly ILibroService _libroService;

        public LibroAdmController(ILibroService libroService)
        {
            _libroService = libroService;
        }


        // LISTAR TODOS LOS LIBROS
        public async Task<ActionResult> Index()
        {
          
            var result = await _libroService.ObtenerTodosAsync<IEnumerable<LibroGetDto>>();

            if (!result.Success)
                return View("Error", result);

            return View(result.Data);
        }


        // DETALLES
        public async Task<ActionResult> Details(int id)
        {
            var result = await _libroService.ObtenerPorIdAsync<LibroGetDto>(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return View(result.Data);
        }

        // CREAR (GET)
        public ActionResult Create() => View();

        //  CREAR (POST)

        [HttpPost]
        public async Task<ActionResult> Create(LibroCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _libroService.RegistrarLibroAsync<LibroGetDto>(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message ?? "Error al registrar el libro.");
                return View(dto);
            }

            TempData["Ok"] = "Libro registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        //  EDITAR (GET)


        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var result = await _libroService.ObtenerPorIdAsync<LibroUpdateDto>(id);
                if (!result.Success || result.Data == null)
                {
                    TempData["Error"] = "Libro no encontrado.";
                    return RedirectToAction(nameof(Index));
                }
                return View(result.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la vista de edición: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }



        //  EDITAR (POST)


        [HttpPost]
        public async Task<ActionResult> Edit(LibroUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _libroService.ModificarLibroAsync<LibroGetDto>(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message ?? "Error al actualizar el libro.");
                return View(dto);
            }

            TempData["Ok"] = "Libro actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        
        // ELIMINAR (GET)
       
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _libroService.ObtenerPorIdAsync<LibroGetDto>(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return View(result.Data);
        }

        // ELIMINAR (POST)

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmado(int id)
        {
            var result = await _libroService.RemoveAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Message ?? "Error al eliminar el libro.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Ok"] = "Libro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> CambiarEstado(int id, string estado)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(estado))
                return BadRequest("Datos inválidos.");

            var result = await _libroService.CambiarEstadoAsync(id, estado);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Ok"] = $"Estado del libro actualizado a '{estado}'.";
            return RedirectToAction(nameof(Index));
        }




        // BÚSQUEDA POR TÍTULO

        [HttpPost]
        public async Task<ActionResult> BuscarPorTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "Debe escribir un título.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _libroService.BuscarPorTituloAsync<IEnumerable<LibroGetDto>>(titulo);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View("Index", result.Data);
        }

        // BÚSQUEDA POR AUTOR

        [HttpPost]
        public async Task<ActionResult> BuscarPorAutor(string autor)
        {
            var result = await _libroService.BuscarPorAutorAsync<IEnumerable<LibroGetDto>>(autor);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View("Index", result.Data);
        }

        // FILTRO AVANZADO

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Filtrar(
            string? titulo, string? autor, string? categoria, int? anio, string? estado)
        {
            var result = await _libroService.FiltrarAsync<IEnumerable<LibroGetDto>>(
                titulo, autor, categoria, anio, estado);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            
            return View("Index", result.Data);
        }
    }
}

