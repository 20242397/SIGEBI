using Microsoft.Extensions.Logging;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Helpers;
using SIGEBI.Persistence.Validators;
using SIGEBI.Persistence.Models;
using SIGEBI.Application.Dtos.Models.Configuration.Usuario;
using SIGEBI.Application.Dtos.Models.Configuration.Libro;      // For LibroGetModel
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;   // For PrestamoGetModel

namespace SIGEBI.Persistence.Repositories.Ado
{
    public sealed class UsuarioRepositoryAdo : IUsuarioRepository
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<UsuarioRepositoryAdo> _logger;

        public UsuarioRepositoryAdo(DbHelper dbHelper, ILogger<UsuarioRepositoryAdo> logger)
        {
            _dbHelper = dbHelper;
            _logger = logger;
        }

        #region IBaseRepository
        public async Task<OperationResult<Usuario>> AddAsync(Usuario entity)
        {
            // Validación encapsulada
            var validacion = UsuarioValidator.Validar(entity);
            if (!validacion.Success) return validacion;

            try
            {
                var query = @"INSERT INTO Usuarios 
                                (Nombre, Apellido, Email, PasswordHash, PhoneNumber, Role, Estado)
                              OUTPUT INSERTED.Id
                              VALUES (@Nombre, @Apellido, @Email, @PasswordHash, @PhoneNumber, @Role, @Estado)";

                var parameters = new Dictionary<string, object>
                {
                    {"@Nombre", entity.Nombre},
                    {"@Apellido", entity.Apellido},
                    {"@Email", entity.Email},
                    {"@PasswordHash", entity.PasswordHash},
                    {"@PhoneNumber", entity.PhoneNumber ?? (object)DBNull.Value},
                    {"@Role", entity.Role ?? "User"},
                    {"@Estado", entity.Estado}
                };

                var id = await _dbHelper.ExecuteScalarAsync(query, parameters);
                entity.Id = (int)id;

                return new OperationResult<Usuario>
                {
                    Success = true,
                    Data = entity,
                    Message = "Usuario agregado correctamente."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar usuario");
                return new OperationResult<Usuario>
                {
                    Success = false,
                    Message = "Error al agregar usuario"
                };
            }
        }

        public async Task<OperationResult<IEnumerable<UsuarioGetModel>>> GetAllAsync()
        {
            try
            {
                var query = "SELECT Id, Nombre, Apellido, Email, PhoneNumber, Role, Estado FROM Usuarios";
                var rows = await _dbHelper.ExecuteQueryAsync(query);

                // Mapeo con EntityToModelMapper
                var usuarios = rows.Select(EntityToModelMapper.ToUsuario)
                                   .Select(u => u.ToModel());

                return new OperationResult<IEnumerable<UsuarioGetModel>>
                {
                    Success = true,
                    Data = usuarios
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return new OperationResult<IEnumerable<UsuarioGetModel>>
                {
                    Success = false,
                    Message = "Error al obtener usuarios"
                };
            }
        }
        #endregion

        #region IUsuarioRepository
        public async Task<OperationResult<UsuarioGetModel>> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new OperationResult<UsuarioGetModel>
                {
                    Success = false,
                    Message = "Email requerido"
                };

            try
            {
                var query = "SELECT TOP 1 Id, Nombre, Apellido, Email, PhoneNumber, Role, Estado FROM Usuarios WHERE Email=@Email";
                var parameters = new Dictionary<string, object> { { "@Email", email } };

                var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);

                if (!rows.Any())
                    return new OperationResult<UsuarioGetModel>
                    {
                        Success = false,
                        Message = "No encontrado"
                    };

                // Mapeo con EntityToModelMapper
                var usuario = EntityToModelMapper.ToUsuario(rows.First());

                return new OperationResult<UsuarioGetModel>
                {
                    Success = true,
                    Data = usuario.ToModel()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar usuario por email");
                return new OperationResult<UsuarioGetModel>
                {
                    Success = false,
                    Message = "Error al buscar usuario"
                };
            }
        }

       

        Task<OperationResult<bool>> IUsuarioRepository.AddAsync(Usuario usuario)
        {
            return AddAsync(usuario).ContinueWith(task =>
            {
                var result = task.Result;
                return new OperationResult<bool>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success
                };
            });
        }

        Task<OperationResult<IEnumerable<Usuario>>> IUsuarioRepository.GetAllAsync()
        {
            return GetAllAsync().ContinueWith(task =>
            {
                OperationResult<IEnumerable<UsuarioGetModel>> result = task.Result;
                var data = result.Data;

                return new OperationResult<IEnumerable<Usuario>>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success && data != null
                        ? Enumerable.Empty<Usuario>() // Se cambio el mapeo a Usuario, ya que UsuarioGetModel no tiene las propiedades requeridas.
                        : Enumerable.Empty<Usuario>()
                };
            });
        }

        public async Task<OperationResult<Usuario>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return new OperationResult<Usuario>
                {
                    Success = false,
                    Message = "El ID debe ser mayor que 0"
                };

            try
            {
                var query = @"SELECT TOP 1 Id, Nombre, Apellido, Email, PasswordHash, PhoneNumber, Role, Estado 
                      FROM Usuarios WHERE Id=@Id";
                var parameters = new Dictionary<string, object> { { "@Id", id } };

                var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);

                if (!rows.Any())
                    return new OperationResult<Usuario>
                    {
                        Success = false,
                        Message = "Usuario no encontrado"
                    };

                var usuario = EntityToModelMapper.ToUsuario(rows.First());

                return new OperationResult<Usuario>
                {
                    Success = true,
                    Data = usuario
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por Id");
                return new OperationResult<Usuario>
                {
                    Success = false,
                    Message = "Error al obtener usuario"
                };
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(Usuario usuario)
        {
            if (usuario.Id <= 0)
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = "El ID es inválido"
                };

            try
            {
                var query = @"UPDATE Usuarios 
                      SET Nombre=@Nombre, Apellido=@Apellido, Email=@Email, 
                          PasswordHash=@PasswordHash, PhoneNumber=@PhoneNumber, 
                          Role=@Role, Estado=@Estado
                      WHERE Id=@Id";

                var parameters = new Dictionary<string, object>
        {
            {"@Nombre", usuario.Nombre},
            {"@Apellido", usuario.Apellido},
            {"@Email", usuario.Email},
            {"@PasswordHash", usuario.PasswordHash},
            {"@PhoneNumber", usuario.PhoneNumber ?? (object)DBNull.Value},
            {"@Role", usuario.Role ?? "User"},
            {"@Estado", usuario.Estado},
            {"@Id", usuario.Id}
        };

                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);

                return new OperationResult<bool>
                {
                    Success = rows > 0,
                    Data = rows > 0,
                    Message = rows > 0 ? "Usuario actualizado correctamente" : "No se actualizó el usuario"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = "Error al actualizar usuario"
                };
            }
        }

        // Implementación explícita para GetByEmailAsync
        async Task<OperationResult<object>> IUsuarioRepository.GetByEmailAsync(string email)
        {
            var result = await GetByEmailAsync(email);

            return new OperationResult<object>
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Data // devuelve el UsuarioGetModel como object
            };
        }

        Task<OperationResult<object>> IUsuarioRepository.GetByIdAsync(int id)
        {
            return GetByIdAsync(id).ContinueWith(task =>
            {
                var result = task.Result;
                return new OperationResult<object>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data // Usuario como object
                };
            });
        }



        #endregion
    }
}
