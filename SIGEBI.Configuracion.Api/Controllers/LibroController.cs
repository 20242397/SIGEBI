using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Configuracion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LibroController : ControllerBase
    {
        private readonly ILibroService _libroService;

        public LibroController(ILibroService libroService)
        {
            _libroService = libroService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] LibroCreateDto dto)
        {
            var result = await _libroService.RegistrarLibroAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("modificar")]
        public async Task<IActionResult> Modificar([FromBody] LibroUpdateDto dto)
        {
            var result = await _libroService.ModificarLibroAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _libroService.RemoveAsync(id);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpGet("titulo/{titulo}")]
        public async Task<IActionResult> BuscarPorTitulo(string titulo)
        {
            var result = await _libroService.BuscarPorTituloAsync<object>(titulo);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("autor/{autor}")]
        public async Task<IActionResult> BuscarPorAutor(string autor)
        {
            var result = await _libroService.BuscarPorAutorAsync<object>(autor);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("categoria/{categoria}")]
        public async Task<IActionResult> BuscarPorCategoria(string categoria)
        {
            var result = await _libroService.BuscarPorCategoriaAsync<object>(categoria);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<IActionResult> BuscarPorISBN(string isbn)
        {
            var result = await _libroService.BuscarPorISBNAsync(isbn);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _libroService.ObtenerPorIdAsync<object>(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _libroService.ObtenerTodosAsync<object>();
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPut("estado/{id:int}")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            if (string.IsNullOrWhiteSpace(nuevoEstado))
                return BadRequest("El estado no puede estar vacío.");

            var result = await _libroService.CambiarEstadoAsync(id, nuevoEstado);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("filtrar")]
        public async Task<IActionResult> Filtrar(
    string? titulo,
    string? autor,
    string? categoria,
    int? anio,
    string? estado)
        {
            var result = await _libroService.FiltrarAsync<IEnumerable<LibroGetDto>>(
                titulo, autor, categoria, anio, estado
            );

            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}


    