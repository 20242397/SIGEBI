using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Usuario;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.ISecurity;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;

namespace SIGEBI.Application.Services.SecuritySer
{
    public sealed class UsuarioService : BaseService, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        // Registrar usuario
        public Task<ServiceResult<T>> RegistrarUsuarioAsync<T>(UsuarioCreateDto dto) =>
            ExecuteAsync(async () =>
            {
                var entity = new Usuario
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Email = dto.Email,
                    PasswordHash = dto.PasswordHash,
                    PhoneNumber = dto.PhoneNumber,
                    Role = "Estudiante",
                    Estado = "Activo",
                    Activo = true
                };

                var validation = UsuarioValidator.Validar(entity);
                if (!validation.Success)
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = validation.Message
                    };

                // Verificar si el correo ya está registrado
                var existingUser = await _usuarioRepository.GetByEmailAsync(dto.Email);
                if (existingUser.Success && existingUser.Data != null)
                {
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "Ya existe un usuario registrado con ese correo electrónico."
                    };
                }


                var result = await _usuarioRepository.AddAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Usuario registrado correctamente: {Email}", entity.Email);
                else
                    _logger.LogWarning("Error al registrar usuario: {Email}", entity.Email);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        // Editar usuario
        public Task<ServiceResult<T>> EditarUsuarioAsync<T>(UsuarioUpdateDto dto) =>
            ExecuteAsync(async () =>
            {
                var usuarioResult = await _usuarioRepository.GetByIdAsync(dto.Id);
                if (!usuarioResult.Success || usuarioResult.Data == null)
                {
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "Usuario no encontrado"
                    };
                }

                var usuario = usuarioResult.Data;
                usuario.Nombre = dto.Nombre;
                usuario.Apellido = dto.Apellido;
                usuario.Email = dto.Email;
                usuario.PhoneNumber = dto.PhoneNumber;
                usuario.Role = dto.Role;
                usuario.Estado = dto.Activo ? "Activo" : "Inactivo";
                usuario.Activo = dto.Activo;

                var validation = UsuarioValidator.Validar(usuario);
                if (!validation.Success)
                    return new OperationResult<T> { Success = false, Message = validation.Message };

                var updateResult = await _usuarioRepository.UpdateAsync(usuario);

                _logger.LogInformation("Usuario actualizado: {Email}", (object)usuario.Email);

                return new OperationResult<T>
                {
                    Success = updateResult.Success,
                    Message = updateResult.Message,
                    Data = (T)(object)updateResult.Data!
                };
            });

        //  Asignar rol
        public Task<ServiceResult<T>> AsignarRolAsync<T>(int id, string rol) =>
            ExecuteAsync(async () =>
            {
                var usuarioResult = await _usuarioRepository.GetByIdAsync(id);
                if (!usuarioResult.Success || usuarioResult.Data == null)
                    return new OperationResult<T> { Success = false, Message = "Usuario no encontrado" };

                var usuario = usuarioResult.Data;
                usuario.Role = rol;

                var rolesValidos = new[] { "Admin", "Docente", "Estudiante" };
                if (!rolesValidos.Contains(rol))
                {
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "El rol especificado no es válido."
                    };
                }


                var result = await _usuarioRepository.UpdateAsync(usuario);
                _logger.LogInformation("Rol asignado al usuario {Id}: {Rol}", id, rol);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        // Activar o desactivar usuario
        public Task<ServiceResult<T>> CambiarEstadoAsync<T>(int id, bool activo) =>
            ExecuteAsync(async () =>
            {
                var usuarioResult = await _usuarioRepository.GetByIdAsync(id);
                if (!usuarioResult.Success || usuarioResult.Data == null)
                    return new OperationResult<T> { Success = false, Message = "Usuario no encontrado" };

                var usuario = usuarioResult.Data;

                if (usuario.Estado == (activo ? "Activo" : "Inactivo"))
                {
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = $"El usuario ya está {(activo ? "activo" : "inactivo")}."
                    };
                }

                // Solo si pasa la validación, actualiza
                usuario.Activo = activo;
                usuario.Estado = activo ? "Activo" : "Inactivo";


                var result = await _usuarioRepository.UpdateAsync(usuario);
                _logger.LogInformation("Estado actualizado para el usuario {Id}: {Estado}", id, (object)usuario.Estado);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        //  Obtener usuario por email
        public Task<ServiceResult<T>> ObtenerPorEmailAsync<T>(string email) =>
            ExecuteAsync(async () =>
            {
                var result = await _usuarioRepository.GetByEmailAsync(email);
                if (!result.Success)
                    _logger.LogWarning("Usuario no encontrado con email: {Email}", email);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data != null ? (T)(object)result.Data! : default!
                };
            });

        // Obtener todos los usuarios
        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
            ExecuteAsync(async () =>
            {
                var result = await _usuarioRepository.GetAllAsync();

                _logger.LogInformation("Consulta de usuarios completada. Total: {Count}",
                    (result.Data as IEnumerable<object>)?.Count() ?? 0);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        public async Task<OperationResult<Usuario>> RemoveAsync(int id)
        {
            return await _usuarioRepository.RemoveAsync(id);
        }
    }
}
