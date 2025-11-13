using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Models.Configuration.Notificacion;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Configuracion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

       

        [HttpPost("enviar")]
        public async Task<IActionResult> Enviar([FromBody] NotificacionCreateDto dto)
        {
            var result = await _notificacionService.EnviarNotificacionAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
        {
            var result = await _notificacionService.ObtenerPorUsuarioAsync<object>(usuarioId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("pendientes")]
        public async Task<IActionResult> ObtenerPendientes()
        {
            var result = await _notificacionService.ObtenerPendientesAsync<object>();
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("MarcarTodasComoEnviadas/{usuarioId}")]
        public async Task<IActionResult> MarcarTodasComoEnviadas(int usuarioId)
        {
            var result = await _notificacionService.MarcarTodasComoEnviadasPorUsuarioAsync<object>(usuarioId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("todas")]
        public async Task<IActionResult> ObtenerTodas()
        {
            var result = await _notificacionService.ObtenerTodosAsync<IEnumerable<NotificacionGetDto>>();

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("usuario/{usuarioId}/no-leidas")]
        public async Task<IActionResult> ObtenerNoLeidas(int usuarioId)
        {
            var result = await _notificacionService.ObtenerNoLeidasAsync<IEnumerable<NotificacionGetDto>>(usuarioId);

            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpGet("tipo/{tipo}")]
        public async Task<IActionResult> ObtenerPorTipo(string tipo)
        {
            var result = await _notificacionService.ObtenerPorTipoAsync<IEnumerable<NotificacionGetDto>>(tipo);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("generar-automaticas")]
        public async Task<IActionResult> GenerarAutomaticas()
        {
            var result = await _notificacionService.GenerarNotificacionesAutomaticasAsync();

            return result.Success ? Ok(result) : BadRequest(result);
        }



    }
}