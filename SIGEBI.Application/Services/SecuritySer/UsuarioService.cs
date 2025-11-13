using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Usuario;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Mappers;
using SIGEBI.Application.Repositories.Configuration.ISecurity;
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


        public Task<ServiceResult<T>> RegistrarUsuarioAsync<T>(UsuarioCreateDto dto) =>
      ExecuteAsync(async () =>
      {
         
          var entity = dto.ToEntity();


          if (string.IsNullOrWhiteSpace(dto.Password))
          {
              return new OperationResult<T>
              {
                  Success = false,
                  Message = "La contraseña es obligatoria."
              };
          }


          entity.Password = dto.Password;

         
          entity.Estado = "Activo";
          entity.Activo = true;

        
          var validation = UsuarioValidator.Validar(entity);
          if (!validation.Success)
              return new OperationResult<T>
              {
                  Success = false,
                  Message = validation.Message
              };

          
          var existingUser = await _usuarioRepository.GetByEmailAsync(dto.Email);
          if (existingUser.Success && existingUser.Data != null)
          {
              return new OperationResult<T>
              {
                  Success = false,
                  Message = "El correo ya está registrado."
              };
          }

          // 7️⃣ Insertar usuario
          var result = await _usuarioRepository.AddAsync(entity);

          if (result.Success)
              _logger.LogInformation("Usuario registrado correctamente: {Email}", entity.Email);

          // 8️⃣ Retornar DTO correcto
          return new OperationResult<T>
          {
              Success = result.Success,
              Message = result.Message,
              Data = (T)(object)result.Data!.ToDto()
          };
      });



        public Task<ServiceResult<T>> EditarUsuarioAsync<T>(UsuarioUpdateDto dto) =>
     ExecuteAsync(async () =>
     {
         var usuarioResult = await _usuarioRepository.GetByIdAsync(dto.Id);
         if (!usuarioResult.Success || usuarioResult.Data == null)
             return new OperationResult<T> { Success = false, Message = "Usuario no encontrado." };

         var usuario = usuarioResult.Data;

         usuario.Nombre = dto.Nombre;
         usuario.Apellido = dto.Apellido;
         usuario.Email = dto.Email;
         usuario.PhoneNumber = dto.PhoneNumber;
         usuario.Role = dto.Role;
         usuario.Activo = dto.Activo;
         usuario.Estado = usuario.Activo ? "Activo" : "Inactivo";

         if (!string.IsNullOrWhiteSpace(dto.Password))
         {
             usuario.Password = dto.Password;
         }

         var validation = UsuarioValidator.Validar(usuario);
         if (!validation.Success)
             return new OperationResult<T> { Success = false, Message = validation.Message };

         var updateResult = await _usuarioRepository.UpdateAsync(usuario);

         return new OperationResult<T>
         {
             Success = updateResult.Success,
             Message = updateResult.Message,
             Data = (T)(object)updateResult.Data!.ToDto()
         };
     });


      
        public Task<ServiceResult<T>> AsignarRolAsync<T>(int id, string rol) =>
            ExecuteAsync(async () =>
            {
                var usuarioResult = await _usuarioRepository.GetByIdAsync(id);
                if (!usuarioResult.Success || usuarioResult.Data == null)
                    return new OperationResult<T> { Success = false, Message = "Usuario no encontrado." };

                var usuario = usuarioResult.Data;

                var rolesValidos = new[] { "Admin", "Docente", "Estudiante" };
                if (!rolesValidos.Contains(rol))
                    return new OperationResult<T> { Success = false, Message = "Rol no válido." };

                usuario.Role = rol;

                var result = await _usuarioRepository.UpdateAsync(usuario);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToDto()
                };
            });


      
        public Task<ServiceResult<T>> CambiarEstadoAsync<T>(int id, bool activo) =>
            ExecuteAsync(async () =>
            {
                var usuarioResult = await _usuarioRepository.GetByIdAsync(id);
                if (!usuarioResult.Success || usuarioResult.Data == null)
                    return new OperationResult<T> { Success = false, Message = "Usuario no encontrado." };

                var usuario = usuarioResult.Data;

                usuario.Activo = activo;
                usuario.Estado = activo ? "Activo" : "Inactivo";

                var result = await _usuarioRepository.UpdateAsync(usuario);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToDto()
                };
            });

       
        public Task<ServiceResult<T>> ObtenerPorEmailAsync<T>(string email) =>
            ExecuteAsync(async () =>
            {
                var result = await _usuarioRepository.GetByEmailAsync(email);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data != null ? (T)(object)result.Data!.ToDto() : default!
                };
            });

       
        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
            ExecuteAsync(async () =>
            {
                var result = await _usuarioRepository.GetAllAsync();

                var listaDto = result.Data.Select(u => u.ToDto()).ToList();

                return new OperationResult<T>
                {
                    Success = true,
                    Data = (T)(object)listaDto
                };
            });

       
        public Task<OperationResult<Usuario>> RemoveAsync(int id) =>
            _usuarioRepository.RemoveAsync(id);



        public Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id) =>
            ExecuteAsync(async () =>
            {
                var result = await _usuarioRepository.GetByIdAsync(id);

                if (!result.Success || result.Data == null)
                {
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "Usuario no encontrado."
                    };
                }

                object dto;

               
                if (typeof(T) == typeof(UsuarioUpdateDto))
                {
                    dto = result.Data.ToUpdateDto();
                }
                else if (typeof(T) == typeof(UsuarioGetDto))
                {
                    dto = result.Data.ToDto();
                }
                else
                {
                  
                    dto = result.Data.ToDto();
                }

                return new OperationResult<T>
                {
                    Success = true,
                    Data = (T)dto
                };
            });

    }
}