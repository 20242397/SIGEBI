using SIGEBI.Application.Dtos.Auth;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.ISecurity;
using SIGEBI.Domain.Base;

namespace SIGEBI.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<OperationResult<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            var userResult = await _usuarioRepository.GetByEmailAsync(dto.Email);

            if (!userResult.Success || userResult.Data == null)
            {
                return new OperationResult<LoginResponseDto>
                {
                    Success = false,
                    Message = "Usuario no encontrado."
                };
            }

            var usuario = userResult.Data;

           
            if (usuario.Password != dto.Password)
            {
                return new OperationResult<LoginResponseDto>
                {
                    Success = false,
                    Message = "Credenciales incorrectas."
                };
            }

            
            if (!usuario.Activo)
            {
                return new OperationResult<LoginResponseDto>
                {
                    Success = false,
                    Message = "Su usuario está inactivo. Contacte al administrador."
                };
            }

            return new OperationResult<LoginResponseDto>
            {
                Success = true,
                Data = new LoginResponseDto
                {
                    Id = usuario.Id,
                    NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                    Email = usuario.Email,
                    Role = usuario.Role,
                    Activo = usuario.Activo
                }
            };
        }
    }
}
