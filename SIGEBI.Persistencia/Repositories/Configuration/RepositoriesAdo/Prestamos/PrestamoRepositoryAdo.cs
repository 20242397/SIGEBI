using Microsoft.Extensions.Logging;
using SIGEBI.Application.Repositories.Configuration.IPrestamo;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Models;

namespace SIGEBI.Persistence.Repositories.Configuration.RepositoriesAdo.Prestamos
{
    public sealed class PrestamoRepositoryAdo : IPrestamoRepository
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<PrestamoRepositoryAdo> _logger;

        public PrestamoRepositoryAdo(DbHelper dbHelper, ILogger<PrestamoRepositoryAdo> logger)
        {
            _dbHelper = dbHelper;
            _logger = logger;
        }

        #region CRUD BASE
        public async Task<OperationResult<Prestamo>> AddAsync(Prestamo entity)
        {
            var validation = PrestamoValidator.Validar(entity);
            if (!validation.Success) return validation;

            try
            {
                string query = @"
                    INSERT INTO Prestamos (UsuarioId, LibroId, FechaPrestamo, FechaVencimiento, FechaDevolucion, Penalizacion)
                    OUTPUT INSERTED.Id
                    VALUES (@UsuarioId, @LibroId, @FechaPrestamo, @FechaVencimiento, @FechaDevolucion, @Penalizacion)";

                var parameters = new Dictionary<string, object>
                {
                    {"@UsuarioId", entity.UsuarioId},
                    {"@LibroId", entity.LibroId},
                    {"@FechaPrestamo", entity.FechaPrestamo},
                    {"@FechaVencimiento", entity.FechaVencimiento},
                    {"@FechaDevolucion", entity.FechaDevolucion ?? (object)DBNull.Value},
                    {"@Penalizacion", entity.Penalizacion ?? (object)DBNull.Value}
                };

                var id = await _dbHelper.ExecuteScalarAsync(query, parameters);
                entity.Id = Convert.ToInt32(id);

                return new OperationResult<Prestamo>
                {
                    Success = true,
                    Message = "Préstamo registrado correctamente.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar préstamo");
                return new OperationResult<Prestamo>
                {
                    Success = false,
                    Message = $"Error al registrar préstamo: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult<IEnumerable<Prestamo>>> GetAllAsync()
        {
            try
            {
                string query = "SELECT Id, UsuarioId, LibroId, FechaPrestamo, FechaVencimiento, FechaDevolucion, Penalizacion FROM Prestamos";
                var rows = await _dbHelper.ExecuteQueryAsync(query);

                var prestamos = rows.Select(EntityToModelMapper.ToPrestamo).ToList();

                return new OperationResult<IEnumerable<Prestamo>> { Success = true, Data = prestamos };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener préstamos");
                return new OperationResult<IEnumerable<Prestamo>>
                {
                    Success = false,
                    Message = $"Error al obtener préstamos: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult<Prestamo>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return new OperationResult<Prestamo> { Success = false, Message = "El ID debe ser mayor que 0." };

            try
            {
                string query = "SELECT TOP 1 * FROM Prestamos WHERE Id=@Id";
                var parameters = new Dictionary<string, object> { { "@Id", id } };

                var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);
                if (!rows.Any())
                    return new OperationResult<Prestamo> { Success = false, Message = "Préstamo no encontrado." };

                var prestamo = EntityToModelMapper.ToPrestamo(rows.First());
                return new OperationResult<Prestamo> { Success = true, Data = prestamo };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener préstamo por Id");
                return new OperationResult<Prestamo>
                {
                    Success = false,
                    Message = $"Error al obtener préstamo por Id: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult<Prestamo>> UpdateAsync(Prestamo entity)
        {
            if (entity.Id <= 0)
                return new OperationResult<Prestamo> { Success = false, Message = "El ID es inválido." };

            try
            {
                string query = @"
                    UPDATE Prestamos 
                    SET UsuarioId=@UsuarioId, LibroId=@LibroId, FechaPrestamo=@FechaPrestamo,
                        FechaVencimiento=@FechaVencimiento, FechaDevolucion=@FechaDevolucion, Penalizacion=@Penalizacion
                    WHERE Id=@Id";

                var parameters = new Dictionary<string, object>
                {
                    {"@UsuarioId", entity.UsuarioId},
                    {"@LibroId", entity.LibroId},
                    {"@FechaPrestamo", entity.FechaPrestamo},
                    {"@FechaVencimiento", entity.FechaVencimiento},
                    {"@FechaDevolucion", entity.FechaDevolucion ?? (object)DBNull.Value},
                    {"@Penalizacion", entity.Penalizacion ?? (object)DBNull.Value},
                    {"@Id", entity.Id}
                };

                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);
                return new OperationResult<Prestamo>
                {
                    Success = rows > 0,
                    Message = rows > 0 ? "Préstamo actualizado correctamente." : "No se actualizó el registro.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar préstamo");
                return new OperationResult<Prestamo>
                {
                    Success = false,
                    Message = $"Error al actualizar préstamo: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(int id)
        {
            if (id <= 0)
                return new OperationResult<bool> { Success = false, Message = "El ID debe ser mayor que 0." };

            try
            {
                string query = "DELETE FROM Prestamos WHERE Id=@Id";
                var rows = await _dbHelper.ExecuteCommandAsync(query, new() { { "@Id", id } });

                return new OperationResult<bool>
                {
                    Success = rows > 0,
                    Data = rows > 0,
                    Message = rows > 0 ? "Préstamo eliminado correctamente." : "No se encontró el préstamo."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar préstamo");
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = $"Error al eliminar préstamo: {ex.Message}"
                };
            }
        }
        #endregion

        #region MÉTODOS ESPECÍFICOS

        public async Task<OperationResult<Prestamo>> RegistrarPrestamoAsync(Prestamo prestamo)
        {
            return await AddAsync(prestamo);
        }

        public async Task<OperationResult<Prestamo>> RegistrarDevolucionAsync(int prestamoId, DateTime fechaDevolucion, decimal? penalizacion)
        {
            var prestamoResult = await GetByIdAsync(prestamoId);
            if (!prestamoResult.Success || prestamoResult.Data == null)
                return new OperationResult<Prestamo> { Success = false, Message = "Préstamo no encontrado." };

            var prestamo = prestamoResult.Data;
            prestamo.FechaDevolucion = fechaDevolucion;
            prestamo.Penalizacion = penalizacion;

            return await UpdateAsync(prestamo);
        }

        public async Task<OperationResult<IEnumerable<Prestamo>>> GetPrestamosActivosAsync()
        {
            try
            {
                string query = "SELECT * FROM Prestamos WHERE FechaDevolucion IS NULL";
                var rows = await _dbHelper.ExecuteQueryAsync(query);
                var prestamos = rows.Select(EntityToModelMapper.ToPrestamo).ToList();

                return new OperationResult<IEnumerable<Prestamo>> { Success = true, Data = prestamos };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener préstamos activos");
                return new OperationResult<IEnumerable<Prestamo>>
                {
                    Success = false,
                    Message = $"Error al obtener préstamos activos: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult<IEnumerable<Prestamo>>> GetPrestamosConPenalizacionAsync()
        {
            try
            {
                string query = "SELECT * FROM Prestamos WHERE Penalizacion IS NOT NULL AND Penalizacion > 0";
                var rows = await _dbHelper.ExecuteQueryAsync(query);
                var prestamos = rows.Select(EntityToModelMapper.ToPrestamo).ToList();

                return new OperationResult<IEnumerable<Prestamo>> { Success = true, Data = prestamos };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener préstamos con penalización");
                return new OperationResult<IEnumerable<Prestamo>>
                {
                    Success = false,
                    Message = $"Error al obtener préstamos con penalización: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult<bool>> CalcularPenalizacionAsync(int prestamoId)
        {
            var prestamoResult = await GetByIdAsync(prestamoId);
            if (!prestamoResult.Success || prestamoResult.Data == null)
                return new OperationResult<bool> { Success = false, Message = "Préstamo no encontrado." };

            var prestamo = prestamoResult.Data;
            if (prestamo.FechaDevolucion != null)
                return new OperationResult<bool> { Success = false, Message = "El préstamo ya fue devuelto." };

            int diasAtraso = (DateTime.Now - prestamo.FechaVencimiento).Days;
            decimal penalizacion = diasAtraso > 0 ? diasAtraso * 1.00m : 0;

            var devolucion = await RegistrarDevolucionAsync(prestamoId, DateTime.Now, penalizacion);
            return new OperationResult<bool>
            {
                Success = devolucion.Success,
                Data = devolucion.Success,
                Message = devolucion.Message
            };
        }

        public async Task<OperationResult<IEnumerable<Prestamo>>> GetHistorialPorUsuarioAsync(int usuarioId)
        {
            if (usuarioId <= 0)
                return new OperationResult<IEnumerable<Prestamo>> { Success = false, Message = "ID de usuario inválido." };

            try
            {
                string query = "SELECT * FROM Prestamos WHERE UsuarioId=@UsuarioId";
                var rows = await _dbHelper.ExecuteQueryAsync(query, new() { { "@UsuarioId", usuarioId } });
                var prestamos = rows.Select(EntityToModelMapper.ToPrestamo).ToList();

                return new OperationResult<IEnumerable<Prestamo>> { Success = true, Data = prestamos };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de préstamos");
                return new OperationResult<IEnumerable<Prestamo>>
                {
                    Success = false,
                    Message = $"Error al obtener historial: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult<bool>> RestringirPrestamoSiPenalizadoAsync(int usuarioId)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) AS Penalizados 
                    FROM Prestamos 
                    WHERE UsuarioId=@UsuarioId AND Penalizacion IS NOT NULL AND Penalizacion > 0";

                var rows = await _dbHelper.ExecuteQueryAsync(query, new() { { "@UsuarioId", usuarioId } });
                int penalizados = Convert.ToInt32(rows.First()["Penalizados"]);
                bool restringido = penalizados > 0;

                return new OperationResult<bool>
                {
                    Success = true,
                    Data = restringido,
                    Message = restringido
                        ? "El usuario tiene préstamos con penalización y no puede realizar nuevos préstamos."
                        : "El usuario no tiene restricciones."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar restricción de préstamo");
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = $"Error al verificar restricción: {ex.Message}"
                };
            }
        }

        Task<OperationResult<Prestamo>> IBaseRepository<Prestamo>.RemoveAsync(int id)
        {
            // Call RemoveAsync and convert the result to OperationResult<Prestamo>
            return RemovePrestamoAsync(id);
        }

        private async Task<OperationResult<Prestamo>> RemovePrestamoAsync(int id)
        {
            var result = await RemoveAsync(id);
            return new OperationResult<Prestamo>
            {
                Success = result.Success,
                Message = result.Message,
                Data = null // No Prestamo data to return after removal
            };
        }

        #endregion
    }
}

