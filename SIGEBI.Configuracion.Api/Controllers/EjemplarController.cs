using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Configuracion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EjemplarController : ControllerBase
    {
        private readonly IEjemplarService _ejemplarService;

        public EjemplarController(IEjemplarService ejemplarService)
        {
            _ejemplarService = ejemplarService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] EjemplarCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _ejemplarService.RegistrarEjemplarAsync<EjemplarGetDto>(dto);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("actualizar")]
        public async Task<IActionResult> Actualizar([FromBody] EjemplarUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _ejemplarService.ActualizarEjemplarAsync(dto);

            return result.Success ? Ok(result) : BadRequest(result);
        }

      
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _ejemplarService.ObtenerPorIdAsync<EjemplarGetDto>(id);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("por-libro/{libroId:int}")]
        public async Task<IActionResult> ObtenerPorLibro(int libroId)
        {
            var result = await _ejemplarService.ObtenerPorLibroAsync<IEnumerable<EjemplarGetDto>>(libroId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("disponibles/{libroId:int}")]
        public async Task<IActionResult> Disponibles(int libroId)
        {
            var result = await _ejemplarService.ObtenerDisponiblesPorLibroAsync<IEnumerable<EjemplarGetDto>>(libroId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("prestados")]
        public async Task<IActionResult> Prestados()
        {
            var result = await _ejemplarService.ObtenerPrestadosAsync<IEnumerable<EjemplarGetDto>>();

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("reservados")]
        public async Task<IActionResult> Reservados()
        {
            var result = await _ejemplarService.ObtenerReservadosAsync<IEnumerable<EjemplarGetDto>>();

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("perdido/{id:int}")]
        public async Task<IActionResult> MarcarPerdido(int id)
        {
            var result = await _ejemplarService.MarcarComoPerdidoAsync<object>(id);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
