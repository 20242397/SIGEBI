using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Reporte;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Configuracion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

      
        [HttpPost("generar")]
        public async Task<IActionResult> Generar([FromBody] ReporteCreateDto dto)
        {
            var result = await _reporteService.GenerarReporteAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

       
        [HttpGet("libros-mas-prestados")]
        public async Task<IActionResult> GenerarLibrosMasPrestados([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
        {
            var result = await _reporteService.GenerarReporteAsync<object>(new ReporteCreateDto
            {
                Tipo = "libros mas prestados",
                FechaInicio = inicio,
                FechaFin = fin,
                UsuarioId = 1 
            });

            return result.Success ? Ok(result) : BadRequest(result);
        }

       
        [HttpPut("actualizar")]
        public async Task<IActionResult> Actualizar([FromBody] ReporteUpdateDto dto)
        {
            var result = await _reporteService.ActualizarReporteAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

       
        [HttpGet("exportar/{id}")]
        public async Task<IActionResult> Exportar(int id, [FromQuery] string formato)
        {
            var result = await _reporteService.ExportarReporteAsync<object>(id, formato);
            return result.Success ? Ok(result) : BadRequest(result);
        }

      
        [HttpGet("fecha")]
        public async Task<IActionResult> ObtenerPorFecha([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
        {
            var result = await _reporteService.ObtenerPorFechaAsync<object>(inicio, fin);
            return result.Success ? Ok(result) : NotFound(result);
        }

      
        [HttpGet("tipo/{tipo}")]
        public async Task<IActionResult> ObtenerPorTipo(string tipo)
        {
            var result = await _reporteService.ObtenerPorTipoAsync<IEnumerable<object>>(tipo);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _reporteService.ObtenerPorIdAsync<object>(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

       
        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _reporteService.ObtenerTodosAsync<object>();
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}

