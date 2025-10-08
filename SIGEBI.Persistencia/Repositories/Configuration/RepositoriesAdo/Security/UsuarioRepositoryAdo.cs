using Microsoft.Extensions.Logging;
using SIGEBI.Application.Repositories.Configuration.ISecurity;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using SIGEBI.Persistence.Models;
namespace SIGEBI.Persistence.Repositories.Configuration.RepositoriesAdo.Security
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

        #region ✅ Add Usuario
        public async Task<OperationResult<Usuario>> AddAsync(Usuario entity)
        {
            var validacion = UsuarioValidator.Validar(entity);
            if (!validacion.Success) return validacion;

            try
            {
                string query = @"
                    INSERT INTO Usuarios (Nombre, Apellido, Email, PasswordHash, PhoneNumber, Role, Estado)
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
                    {"@Estado", entity.Estado ?? "Activo"}
                };

                var id = await _dbHelper.ExecuteScalarAsync(query, parameters);
                entity.Id = Convert.ToInt32(id);

                return new OperationResult<Usuario>
                {
                    Success = true,
                    Message = "Usuario agregado correctamente.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar usuario");
                return new OperationResult<Usuario>
                {
                    Success = false,
                    Message = $"Error al agregar usuario: {ex.Message}"
                };
            }
        }
        #endregion

        #region ✅ GetAll Usuarios
        public async Task<OperationResult<IEnumerable<Usuario>>> GetAllAsync()
        {
            try
            {
                var query = "SELECT Id, Nombre, Apellido, Email, PhoneNumber, Role, Estado FROM Usuarios";
                var rows = await _dbHelper.ExecuteQueryAsync(query);

                var usuarios = rows.Select(EntityToModelMapper.ToUsuario).ToList();

                return new OperationResult<IEnumerable<Usuario>>
                {
                    Success = true,
                    Data = usuarios
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                return new OperationResult<IEnumerable<Usuario>>
                {
                    Success = false,
                    Message = $"Error al obtener usuarios: {ex.Message}"
                };
            }
        }
        #endregion

        #region ✅ GetById Usuario
        public async Task<OperationResult<Usuario>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return new OperationResult<Usuario> { Success = false, Message = "El ID debe ser mayor que 0." };

            try
            {
                var query = @"SELECT TOP 1 * FROM Usuarios WHERE Id=@Id";
                var parameters = new Dictionary<string, object> { { "@Id", id } };

                var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);
                if (!rows.Any())
                    return new OperationResult<Usuario> { Success = false, Message = "Usuario no encontrado." };

                var usuario = EntityToModelMapper.ToUsuario(rows.First());
                return new OperationResult<Usuario> { Success = true, Data = usuario };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por Id");
                return new OperationResult<Usuario>
                {
                    Success = false,
                    Message = $"Error al obtener usuario por Id: {ex.Message}"
                };
            }
        }
        #endregion

        #region ✅ GetByEmail Usuario
        public async Task<OperationResult<Usuario>> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new OperationResult<Usuario> { Success = false, Message = "El email es obligatorio." };

            try
            {
                var query = @"SELECT TOP 1 * FROM Usuarios WHERE Email=@Email";
                var parameters = new Dictionary<string, object> { { "@Email", email } };

                var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);
                if (!rows.Any())
                    return new OperationResult<Usuario> { Success = false, Message = "Usuario no encontrado." };

                var usuario = EntityToModelMapper.ToUsuario(rows.First());
                return new OperationResult<Usuario> { Success = true, Data = usuario };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar usuario por email");
                return new OperationResult<Usuario>
                {
                    Success = false,
                    Message = $"Error al buscar usuario: {ex.Message}"
                };
            }
        }
        #endregion

        #region ✅ Update Usuario
        public async Task<OperationResult<Usuario>> UpdateAsync(Usuario entity)
        {
            if (entity.Id <= 0)
                return new OperationResult<Usuario> { Success = false, Message = "El ID es inválido." };

            try
            {
                var query = @"
                    UPDATE Usuarios
                    SET Nombre=@Nombre, Apellido=@Apellido, Email=@Email, PasswordHash=@PasswordHash,
                        PhoneNumber=@PhoneNumber, Role=@Role, Estado=@Estado
                    WHERE Id=@Id";

                var parameters = new Dictionary<string, object>
                {
                    {"@Nombre", entity.Nombre},
                    {"@Apellido", entity.Apellido},
                    {"@Email", entity.Email},
                    {"@PasswordHash", entity.PasswordHash},
                    {"@PhoneNumber", entity.PhoneNumber ?? (object)DBNull.Value},
                    {"@Role", entity.Role ?? "User"},
                    {"@Estado", entity.Estado ?? "Activo"},
                    {"@Id", entity.Id}
                };

                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);
                return new OperationResult<Usuario>
                {
                    Success = rows > 0,
                    Message = rows > 0 ? "Usuario actualizado correctamente." : "No se actualizó el usuario.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return new OperationResult<Usuario>
                {
                    Success = false,
                    Message = $"Error al actualizar usuario: {ex.Message}"
                };
            }
        }
        #endregion

        #region ✅ Cambiar Estado
        public async Task<OperationResult<bool>> CambiarEstadoAsync(int id, bool activo)
        {
            try
            {
                var query = "UPDATE Usuarios SET Estado=@Estado WHERE Id=@Id";
                var parameters = new Dictionary<string, object>
                {
                    {"@Estado", activo ? "Activo" : "Inactivo"},
                    {"@Id", id}
                };

                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);
                return new OperationResult<bool>
                {
                    Success = rows > 0,
                    Data = rows > 0,
                    Message = rows > 0 ? "Estado actualizado correctamente." : "Usuario no encontrado."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del usuario");
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = $"Error al cambiar estado: {ex.Message}"
                };
            }
        }
        #endregion

        #region ✅ Asignar Rol
        public async Task<OperationResult<bool>> AsignarRolAsync(int id, string rol)
        {
            try
            {
                var query = "UPDATE Usuarios SET Role=@Role WHERE Id=@Id";
                var parameters = new Dictionary<string, object>
                {
                    {"@Role", rol},
                    {"@Id", id}
                };

                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);
                return new OperationResult<bool>
                {
                    Success = rows > 0,
                    Data = rows > 0,
                    Message = rows > 0 ? $"Rol '{rol}' asignado correctamente." : "Usuario no encontrado."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar rol");
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = $"Error al asignar rol: {ex.Message}"
                };
            }
        }

        public Task<OperationResult<Usuario>> RemoveAsync(int id)
        {
           return Task.FromResult(new OperationResult<Usuario>
            {
                Success = false,
                Message = "Eliminación de usuarios no permitida."
            });
        }
        #endregion
    }
}

