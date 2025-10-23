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
            var result = await _ejemplarService.RegistrarEjemplarAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPut("actualizar")]
        public async Task<IActionResult> ActualizarEjemplar([FromBody] EjemplarUpdateDto dto)
        {
            var result = await _ejemplarService.ActualizarEjemplarAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }




        [HttpGet("por-libro/{libroId}")]
        public async Task<IActionResult> ObtenerPorLibro(int libroId)
        {
            var result = await _ejemplarService.ObtenerPorLibroAsync<object>(libroId);
            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpGet("disponibles/{libroId}")]
        public async Task<IActionResult> ObtenerDisponibles(int libroId)
        {
            var result = await _ejemplarService.ObtenerDisponiblesPorLibroAsync<object>(libroId);
            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpGet("prestados")]
        public async Task<IActionResult> ObtenerPrestados()
        {
            var result = await _ejemplarService.ObtenerPrestadosAsync<object>();
            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpGet("reservados")]
        public async Task<IActionResult> ObtenerReservados()
        {
            var result = await _ejemplarService.ObtenerReservadosAsync<object>();
            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpPut("perdido/{id}")]
        public async Task<IActionResult> MarcarComoPerdido(int id)
        {
            var result = await _ejemplarService.MarcarComoPerdidoAsync<object>(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
