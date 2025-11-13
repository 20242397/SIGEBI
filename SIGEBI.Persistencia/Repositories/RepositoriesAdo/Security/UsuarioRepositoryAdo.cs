using Microsoft.Extensions.Logging;
using SIGEBI.Application.Repositories.Configuration.ISecurity;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using SIGEBI.Persistence;
using SIGEBI.Persistence.Repositories.RepositoriesAdo;

public sealed class UsuarioRepositoryAdo : IUsuarioRepository
{
    private readonly DbHelper _dbHelper;
    private readonly ILogger<UsuarioRepositoryAdo> _logger;

    public UsuarioRepositoryAdo(DbHelper dbHelper, ILogger<UsuarioRepositoryAdo> logger)
    {
        _dbHelper = dbHelper;
        _logger = logger;
    }



    public async Task<OperationResult<Usuario>> AddAsync(Usuario entity)
    {
        try
        {
            string query = @"
            INSERT INTO Usuario 
            (Nombre, Apellido, Email, Password, PhoneNumber, Role, Estado, Activo)
            OUTPUT INSERTED.Id
            VALUES (@Nombre, @Apellido, @Email, @Password, @PhoneNumber, @Role, @Estado, @Activo)";

            var parameters = new Dictionary<string, object>
        {
            {"@Nombre", entity.Nombre},
            {"@Apellido", entity.Apellido},
            {"@Email", entity.Email},
            {"@Password", entity.Password},  // ✔ ARREGLADO
            {"@PhoneNumber", entity.PhoneNumber ?? (object)DBNull.Value},
            {"@Role", entity.Role},
            {"@Estado", entity.Estado},
            {"@Activo", entity.Activo}
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




    public async Task<OperationResult<IEnumerable<Usuario>>> GetAllAsync()
    {
        try
        {
            var query = @"SELECT * FROM Usuario";   

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
            _logger.LogError(ex, "Error al obtener usuarios");
            return new OperationResult<IEnumerable<Usuario>>
            {
                Success = false,
                Message = $"Error al obtener usuarios: {ex.Message}"
            };
        }
    }


    public async Task<OperationResult<Usuario>> GetByIdAsync(int id)
    {
        try
        {
            var query = @"SELECT TOP 1 * FROM Usuario WHERE Id = @Id";
            var parameters = new Dictionary<string, object> { { "@Id", id } };

            var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);
            if (!rows.Any())
                return new OperationResult<Usuario> { Success = false, Message = "Usuario no encontrado." };

            return new OperationResult<Usuario>
            {
                Success = true,
                Data = EntityToModelMapper.ToUsuario(rows.First())
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario por ID");
            return new OperationResult<Usuario> { Success = false, Message = ex.Message };
        }
    }

   
   
    public async Task<OperationResult<Usuario>> GetByEmailAsync(string email)
    {
        try
        {
            var query = @"SELECT TOP 1 * FROM Usuario WHERE Email = @Email";
            var parameters = new Dictionary<string, object> { { "@Email", email } };

            var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);

            if (!rows.Any())
                return new OperationResult<Usuario> { Success = false, Message = "Usuario no encontrado." };

            return new OperationResult<Usuario>
            {
                Success = true,
                Data = EntityToModelMapper.ToUsuario(rows.First())
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario por email");
            return new OperationResult<Usuario> { Success = false, Message = ex.Message };
        }
    }

   
  
    public async Task<OperationResult<Usuario>> UpdateAsync(Usuario entity)
    {
        try
        {
            var query = @"
                UPDATE Usuario SET
                    Nombre=@Nombre,
                    Apellido=@Apellido,
                    Email=@Email,
                    Password=@Password,
                    PhoneNumber=@PhoneNumber,
                    Role=@Role,
                    Estado=@Estado,
                    Activo=@Activo
                WHERE Id=@Id";

            var parameters = new Dictionary<string, object>
            {
                {"@Nombre", entity.Nombre},
                {"@Apellido", entity.Apellido},
                {"@Email", entity.Email},
                {"@Password", entity.Password},
                {"@PhoneNumber", entity.PhoneNumber ?? (object)DBNull.Value},
                {"@Role", entity.Role},
                {"@Estado", entity.Estado},
                {"@Activo", entity.Activo},
                {"@Id", entity.Id}
            };

            var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);

            if (rows > 0)
            {
                var refreshed = await GetByIdAsync(entity.Id);
                return new OperationResult<Usuario>
                {
                    Success = true,
                    Message = "Usuario actualizado correctamente.",
                    Data = refreshed.Data
                };
            }

            return new OperationResult<Usuario> { Success = false, Message = "No se actualizó." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario");
            return new OperationResult<Usuario> { Success = false, Message = ex.Message };
        }
    }

   

    public async Task<OperationResult<Usuario>> RemoveAsync(int id)
    {
        try
        {
            var query = @"UPDATE Usuario SET Estado='Inactivo', Activo=0 WHERE Id=@Id";

            var parameters = new Dictionary<string, object> { { "@Id", id } };

            var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);

            return new OperationResult<Usuario>
            {
                Success = rows > 0,
                Message = rows > 0 ? "Usuario inactivado." : "Usuario no encontrado."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario");
            return new OperationResult<Usuario> { Success = false, Message = ex.Message };
        }
    }

    public async Task<OperationResult<bool>> CambiarEstadoAsync(int id, bool activo)
    {
        if (id <= 0)
            return new OperationResult<bool>
            {
                Success = false,
                Message = "El Id proporcionado no es válido.",
                Data = false
            };

        try
        {
            const string query = @"
            UPDATE Usuario
            SET Activo = @Activo,
                Estado = @Estado
            WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
        {
            { "@Activo", activo },
            { "@Estado", activo ? "Activo" : "Inactivo" },
            { "@Id", id }
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
            _logger.LogError(ex, "Error al cambiar estado del usuario {Id}", id);
            return new OperationResult<bool>
            {
                Success = false,
                Message = $"Error al cambiar estado: {ex.Message}",
                Data = false
            };
        }
    }

    public async Task<OperationResult<bool>> AsignarRolAsync(int id, string rol)
    {
        if (id <= 0)
            return new OperationResult<bool>
            {
                Success = false,
                Message = "El Id proporcionado no es válido.",
                Data = false
            };

        if (string.IsNullOrWhiteSpace(rol))
            return new OperationResult<bool>
            {
                Success = false,
                Message = "El rol no puede estar vacío.",
                Data = false
            };

        try
        {
            const string query = @"
            UPDATE Usuario
            SET Role = @Role
            WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
        {
            { "@Role", rol },
            { "@Id", id }
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
            _logger.LogError(ex, "Error al asignar rol '{Rol}' al usuario {Id}", rol, id);
            return new OperationResult<bool>
            {
                Success = false,
                Message = $"Error al asignar rol: {ex.Message}",
                Data = false
            };
        }
    }

    public async Task<OperationResult<Usuario>> ObtenerPorEmailYPasswordAsync(string email, string password)
    {
        try
        {
            const string query = @"
                SELECT TOP 1 *
                FROM Usuario
                WHERE Email = @Email AND Activo = 1";

            var parameters = new Dictionary<string, object>
            {
                {"@Email", email}
            };

            var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);

            if (!rows.Any())
            {
                return new OperationResult<Usuario>
                {
                    Success = false,
                    Message = "Correo no encontrado."
                };
            }

            var usuario = EntityToModelMapper.ToUsuario(rows.First());

            // ✔ LOGIN TEXTO PLANO
            if (usuario.Password != password)
            {
                return new OperationResult<Usuario>
                {
                    Success = false,
                    Message = "Contraseña incorrecta."
                };
            }

            return new OperationResult<Usuario>
            {
                Success = true,
                Data = usuario
            };
        }
        catch (Exception ex)
        {
            return new OperationResult<Usuario>
            {
                Success = false,
                Message = $"Error al validar credenciales: {ex.Message}"
            };
        }
    }
}






