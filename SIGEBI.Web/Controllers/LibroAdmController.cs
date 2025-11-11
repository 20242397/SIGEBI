using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;

namespace SIGEBI.Web.Controllers
{
    public class LibroAdmController : Controller
    {
        private readonly ILibroService _libroService;

        public LibroAdmController(ILibroService libroService)
        {
            _libroService = libroService;
        }

        // ✅ LISTAR LIBROS
        public async Task<ActionResult> Index()
        {
            var result = await _libroService.ObtenerTodosAsync<IEnumerable<Libro>>();

            if (!result.Success)
                return View("Error", result);

            var dto = result.Data.Select(l => new LibroGetDto
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Autor = l.Autor,
                ISBN = l.ISBN,
                Editorial = l.Editorial!,
                AñoPublicacion = l.AñoPublicacion ?? 0,
                Categoria = l.Categoria,
                Estado = l.Estado
            }).ToList();

            return View(dto);
        }

        // ✅ BUSCAR POR AUTOR
        [HttpPost]
        public async Task<ActionResult> BuscarPorAutor(string autor)
        {
            if (string.IsNullOrWhiteSpace(autor))
            {
                TempData["Error"] = "Debe ingresar un autor.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _libroService.BuscarPorAutorAsync<IEnumerable<Libro>>(autor);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var dto = result.Data.Select(l => new LibroGetDto
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Autor = l.Autor,
                ISBN = l.ISBN,
                Editorial = l.Editorial!,
                AñoPublicacion = l.AñoPublicacion ?? 0,
                Categoria = l.Categoria,
                Estado = l.Estado
            }).ToList();

            return View("Index", dto);
        }

        // ✅ BUSCAR POR TÍTULO
        [HttpPost]
        public async Task<ActionResult> BuscarPorTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "Debe ingresar un título.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _libroService.BuscarPorTituloAsync<IEnumerable<Libro>>(titulo);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var dto = result.Data.Select(l => new LibroGetDto
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Autor = l.Autor,
                ISBN = l.ISBN,
                Editorial = l.Editorial!,
                AñoPublicacion = l.AñoPublicacion ?? 0,
                Categoria = l.Categoria,
                Estado = l.Estado
            }).ToList();

            return View("Index", dto);
        }

        // ✅ CREATE (GET)
        public ActionResult Create() => View();

        // ✅ CREATE (POST)
        [HttpPost]
        public async Task<ActionResult> Create(LibroCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _libroService.RegistrarLibroAsync<Libro>(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ DETAILS
        public async Task<ActionResult> Details(int id)
        {
            var result = await _libroService.ObtenerPorIdAsync<Libro>(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            var dto = new LibroGetDto
            {
                Id = result.Data.Id,
                Titulo = result.Data.Titulo,
                Autor = result.Data.Autor,
                ISBN = result.Data.ISBN,
                Editorial = result.Data.Editorial!,
                AñoPublicacion = result.Data.AñoPublicacion ?? 0,
                Categoria = result.Data.Categoria,
                Estado = result.Data.Estado
            };

            return View(dto);
        }

        // ✅ EDIT (GET)
        public async Task<ActionResult> Edit(int id)
        {
            var result = await _libroService.ObtenerPorIdAsync<Libro>(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            var dto = new LibroUpdateDto
            {
                Id = result.Data.Id,
                Titulo = result.Data.Titulo,
                Autor = result.Data.Autor,
                ISBN = result.Data.ISBN,
                Editorial = result.Data.Editorial!,
                AñoPublicacion = result.Data.AñoPublicacion,
                Categoria = result.Data.Categoria,
                Estado = result.Data.Estado
            };

            return View(dto);
        }

        // ✅ EDIT (POST)
        [HttpPost]
        public async Task<ActionResult> Edit(LibroUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _libroService.ModificarLibroAsync<Libro>(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // CAMBIAR ESTADO DEL LIBRO
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
        {
            // Obtener libro por ID
            var result = await _libroService.ObtenerPorIdAsync<Libro>(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            var libro = result.Data;
            libro.Estado = nuevoEstado;

            var dto = new LibroUpdateDto
            {
                Id = libro.Id,
                Titulo = libro.Titulo,
                Autor = libro.Autor,
                ISBN = libro.ISBN,
                Editorial = libro.Editorial,
                AñoPublicacion = libro.AñoPublicacion,
                Categoria = libro.Categoria,
                Estado = libro.Estado
            };

            var updateResult = await _libroService.ModificarLibroAsync<Libro>(dto);

            TempData[updateResult.Success ? "Ok" : "Error"] = updateResult.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Filtrar(string? titulo, string? autor, string? categoria, int? año, string? estado)
        {
            var result = await _libroService.FiltrarAsync<IEnumerable<Libro>>(titulo, autor, categoria, año, estado);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            // Convertimos a DTO
            var dto = result.Data.Select(l => new LibroGetDto
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Autor = l.Autor,
                ISBN = l.ISBN,
                Editorial = l.Editorial!,
                AñoPublicacion = l.AñoPublicacion ?? 0,
                Categoria = l.Categoria,
                Estado = l.Estado
            }).ToList();

            return View("Index", dto);
        }


        // DELETE (GET)

        public async Task<ActionResult> Delete(int id)
        {
            var result = await _libroService.ObtenerTodosAsync<IEnumerable<Libro>>();
            var libro = result.Data.FirstOrDefault(l => l.Id == id);

            if (libro == null)
                return NotFound();

            var dto = new LibroGetDto
            {
                Id = libro.Id,
                Titulo = libro.Titulo,
                Autor = libro.Autor,
                ISBN = libro.ISBN,
                Editorial = libro.Editorial!,
                AñoPublicacion = libro.AñoPublicacion ?? 0,
                Categoria = libro.Categoria,
                Estado = libro.Estado
            };

            return View(dto);
        }



        // DELETE (POST)
        [HttpPost]
        public async Task<ActionResult> DeleteConfirmado(int id)
        {
            var result = await _libroService.RemoveAsync(id);

            return result.Success
                ? RedirectToAction(nameof(Index))
                : View("Error", result);
        }
    }
}
