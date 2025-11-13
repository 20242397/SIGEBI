using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace SIGEBI.Persistence.Repositories.RepositoriesAdo
{
    public class DbHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DbHelper> _logger;

        public DbHelper(IConfiguration configuration, ILogger<DbHelper> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<int> ExecuteCommandAsync(string query, Dictionary<string, object> parameters)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("SIGEBIConnString"));
                using var command = new SqlCommand(query, connection);

                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar comando SQL");
                throw;
            }
        }

        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("SIGEBIConnString"));
                using var command = new SqlCommand(query, connection);

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }
                    result.Add(row);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar consulta SQL");
                throw;
            }

            return result;
        }

        public async Task<object> ExecuteScalarAsync(string query, Dictionary<string, object> parameters)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("SIGEBIConnString"));
                using var command = new SqlCommand(query, connection);

                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                await connection.OpenAsync();
                return await command.ExecuteScalarAsync() ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar scalar SQL");
                throw;
            }
        }
    }
}