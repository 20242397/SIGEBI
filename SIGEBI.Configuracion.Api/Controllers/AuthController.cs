using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Dtos.Auth;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Configuracion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Petición inválida.");

            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(result.Data);
        }
    }
}
