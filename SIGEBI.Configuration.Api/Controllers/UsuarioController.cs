using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Dtos.Models.Configuration.Usuario;

namespace SIGEBI.Configuration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] UsuarioGetModel.UsuarioCreateDto dto)
        {
            var result = await _usuarioService.RegistrarUsuarioAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("editar")]
        public async Task<IActionResult> Editar([FromBody] UsuarioGetModel.UsuarioUpdateDto dto)
        {
            var result = await _usuarioService.EditarUsuarioAsync<object>(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _usuarioService.ObtenerTodosAsync<object>();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> ObtenerPorEmail(string email)
        {
            var result = await _usuarioService.ObtenerPorEmailAsync<object>(email);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("estado/{id}")]
        public async Task<IActionResult> CambiarEstado(int id, [FromQuery] bool activo)
        {
            var result = await _usuarioService.CambiarEstadoAsync<object>(id, activo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("rol/{id}")]
        public async Task<IActionResult> AsignarRol(int id, [FromQuery] string rol)
        {
            var result = await _usuarioService.AsignarRolAsync<object>(id, rol);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
