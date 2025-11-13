using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Configuracion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PrestamoController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamoController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] PrestamoCreateDto dto)
        {
            var result = await _prestamoService.RegistrarPrestamoAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPut("extender")]
        public async Task<IActionResult> Extender([FromBody] PrestamoUpdateDto dto)
        {
            var result = await _prestamoService.ExtenderPrestamoAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPut("devolucion/{prestamoId}")]
        public async Task<IActionResult> RegistrarDevolucion(int prestamoId, [FromQuery] DateTime fechaDevolucion)
        {
            var result = await _prestamoService.RegistrarDevolucionAsync<object>(prestamoId, fechaDevolucion);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _prestamoService.ObtenerTodosAsync<IEnumerable<PrestamoGetDto>>();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _prestamoService.RemoveAsync<object>(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPut("penalizacion/{prestamoId}")]
        public async Task<IActionResult> CalcularPenalizacion(int prestamoId)
        {
            var result = await _prestamoService.CalcularPenalizacionAsync<object>(prestamoId);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("restringir/{usuarioId}")]
        public async Task<IActionResult> RestringirPrestamo(int usuarioId)
        {
            var result = await _prestamoService.RestringirPrestamoSiPenalizadoAsync<object>(usuarioId);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("historial/{usuarioId}")]
        public async Task<IActionResult> ObtenerHistorialUsuario(int usuarioId)
        {
            var result = await _prestamoService.ObtenerHistorialUsuarioAsync<object>(usuarioId);
            return result.Success ? Ok(result) : NotFound(result);
        }


    }
}