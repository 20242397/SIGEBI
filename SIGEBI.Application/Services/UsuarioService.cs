using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using SIGEBI.Domain.Repository;
using static SIGEBI.Application.Dtos.Models.Configuration.Usuario.UsuarioGetModel;

namespace SIGEBI.Application.Services
{
    public class UsuarioService : BaseService, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public Task<ServiceResult<T>> RegistrarUsuarioAsync<T>(UsuarioCreateDto dto) =>
            ExecuteAsync<T>(async () =>
            {
                var usuario = dto.ToEntity();
                var result = await _usuarioRepository.AddAsync(usuario);

                if (result.Success)
                    _logger.LogInformation("Usuario registrado con éxito: {Email}", usuario.Email);
                else
                    _logger.LogWarning("Error al registrar usuario: {Email}", usuario.Email);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToModel()
                };
            });

        public Task<ServiceResult<T>> EditarUsuarioAsync<T>(UsuarioUpdateDto dto) =>
            ExecuteAsync<T>(async () =>
            {
                var usuario = dto.ToEntity();
                var result = await _usuarioRepository.UpdateAsync(usuario);

                if (result.Success)
                    _logger.LogInformation("Usuario actualizado: {Id}", usuario.Id);
                else
                    _logger.LogWarning("Error al actualizar usuario: {Id}", usuario.Id);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToModel()
                };
            });

        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _usuarioRepository.GetAllAsync();

                _logger.LogInformation("Se consultaron todos los usuarios. Total: {Count}",
                    (result.Data as IEnumerable<object>)?.Count() ?? 0
 );


                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToList()
                };
            });

        public Task<ServiceResult<T>> ObtenerPorEmailAsync<T>(string email) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _usuarioRepository.GetByEmailAsync(email);

                if (result.Success)
                    _logger.LogInformation("Usuario encontrado por email: {Email}", email);
                else
                    _logger.LogWarning("Usuario no encontrado con email: {Email}", email);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data != null ? (T)(object)result.Data : default!
                };
            });

        public Task<ServiceResult<T>> CambiarEstadoAsync<T>(int id, bool activo) =>
            ExecuteAsync<T>(async () =>
            {
                var usuarioResult = await _usuarioRepository.GetByIdAsync(id);
                if (!usuarioResult.Success || usuarioResult.Data == null)
                {
                    _logger.LogWarning("Usuario no encontrado para cambiar estado. Id: {Id}", id);
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "Usuario no encontrado"
                    };
                }

                var usuario = (Usuario)usuarioResult.Data;
                usuario.Estado = activo ? "Activo" : "Inactivo";
                var result = await _usuarioRepository.UpdateAsync(usuario);

                _logger.LogInformation("Estado del usuario {Id} cambiado a {Estado}", id, usuario.Estado);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToModel()
                };
            });

        public Task<ServiceResult<T>> AsignarRolAsync<T>(int id, string rol) =>
            ExecuteAsync<T>(async () =>
            {
                var usuarioResult = await _usuarioRepository.GetByIdAsync(id);
                if (!usuarioResult.Success || usuarioResult.Data == null)
                {
                    _logger.LogWarning("Usuario no encontrado para asignar rol. Id: {Id}", id);
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "Usuario no encontrado"
                    };
                }

                var usuario = (Usuario)usuarioResult.Data;
                usuario.Role = rol;
                var result = await _usuarioRepository.UpdateAsync(usuario);

                _logger.LogInformation("Rol {Rol} asignado al usuario {Id}", rol, id);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToModel()
                };
            });
    }
}
