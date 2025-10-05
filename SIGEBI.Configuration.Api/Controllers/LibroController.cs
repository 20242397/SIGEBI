using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Dtos.Models.Configuration.Libro;

namespace SIGEBI.Configuration.API.Controllers
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
        public async Task<IActionResult> Registrar([FromBody] LibroGetModel.LibroCreateDto dto)
        {
            var result = await _libroService.RegistrarLibroAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("modificar")]
        public async Task<IActionResult> Modificar([FromBody] LibroGetModel.LibroUpdateDto dto)
        {
            var result = await _libroService.ModificarLibroAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _libroService.EliminarLibroAsync<object>(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _libroService.ObtenerTodosAsync<object>();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _libroService.ObtenerPorIdAsync<object>(id);
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

        [HttpGet("titulo/{titulo}")]
        public async Task<IActionResult> BuscarPorTitulo(string titulo)
        {
            var result = await _libroService.BuscarPorTituloAsync<object>(titulo);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
