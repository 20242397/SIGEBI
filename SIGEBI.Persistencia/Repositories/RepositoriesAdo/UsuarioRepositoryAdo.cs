using Microsoft.Extensions.Logging;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Helpers;
using SIGEBI.Persistence.Models.Configuration.Usuario;
using SIGEBI.Persistence.Validators;
using SIGEBI.Persistence.Models;

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
                        ? ((IEnumerable<UsuarioGetModel>)data).Select(m => new Usuario
                        {
                            Id = m.Id,
                            Nombre = m.Nombre,
                            Apellido = m.Apellido,
                            Email = m.Email,
                            PhoneNumber = m.PhoneNumber,
                            Role = m.Role,
                            Estado = m.Estado
                        })
                        : Enumerable.Empty<Usuario>()
                };
            });
        }
        #endregion
    }
}
