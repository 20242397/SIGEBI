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
    }
}
