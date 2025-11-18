using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Mappers;

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
            var result = await _libroService.RegistrarLibroAsync<LibroGetDto>(dto);

            return Ok(new
            {
                success = result.Success,
                data = result.Data,
                message = result.Message
            });
        }

       
        [HttpPut("modificar")]
        public async Task<IActionResult> Modificar([FromBody] LibroUpdateDto dto)
        {
            var result = await _libroService.ModificarLibroAsync<LibroGetDto>(dto);

            return Ok(new
            {
                success = result.Success,
                data = result.Data,
                message = result.Message
            });
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _libroService.RemoveAsync(id);

            return Ok(new
            {
                success = result.Success,
                data = (object?)null,
                message = result.Message
            });
        }

        
        [HttpGet("titulo/{titulo}")]
        public async Task<IActionResult> BuscarPorTitulo(string titulo)
        {
            var result = await _libroService.BuscarPorTituloAsync<IEnumerable<LibroGetDto>>(titulo);

            return Ok(new
            {
                success = result.Success,
                data = result.Data,
                message = result.Message
            });
        }

       
        [HttpGet("autor/{autor}")]
        public async Task<IActionResult> BuscarPorAutor(string autor)
        {
            var result = await _libroService.BuscarPorAutorAsync<IEnumerable<LibroGetDto>>(autor);

            return Ok(new
            {
                success = result.Success,
                data = result.Data,
                message = result.Message
            });
        }

       
        [HttpGet("categoria/{categoria}")]
        public async Task<IActionResult> BuscarPorCategoria(string categoria)
        {
            var result = await _libroService.BuscarPorCategoriaAsync<IEnumerable<LibroGetDto>>(categoria);

            return Ok(new
            {
                success = result.Success,
                data = result.Data,
                message = result.Message
            });
        }

       
        [HttpGet("isbn/{isbn}")]
        public async Task<IActionResult> BuscarPorISBN(string isbn)
        {
            var result = await _libroService.BuscarPorISBNAsync(isbn);

            return Ok(new
            {
                success = result.Success,
                data = result.Data?.ToDto(),
                message = result.Message
            });
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _libroService.ObtenerPorIdAsync<LibroGetDto>(id);

            return Ok(new
            {
                success = result.Success,
                data = result.Data,
                message = result.Message
            });
        }

       
        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _libroService.ObtenerTodosAsync<IEnumerable<LibroGetDto>>();

            return Ok(new
            {
                success = result.Success,
                data = result.Data,
                message = result.Message
            });
        }

       
        [HttpPut("estado/{id:int}")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            if (string.IsNullOrWhiteSpace(nuevoEstado))
            {
                return BadRequest(new
                {
                    success = false,
                    data = (object?)null,
                    message = "El estado no puede estar vacío."
                });
            }

            var result = await _libroService.CambiarEstadoAsync(id, nuevoEstado);

            return Ok(new
            {
                success = result.Success,
                data = result.Data?.ToDto(),
                message = result.Message
            });
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

            return Ok(new
            {
                success = result.Success,
                data = result.Data,
                message = result.Message
            });
        }
    }
}
