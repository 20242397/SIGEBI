using Microsoft.Extensions.Logging;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Helpers;
using SIGEBI.Persistence.Models;
using SIGEBI.Persistence.Validators;

namespace SIGEBI.Persistence.Repositories.Ado
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

        #region IBaseRepository<Prestamo>
        public async Task<OperationResult<Prestamo>> AddAsync(Prestamo entity)
        {
            // Validación centralizada
            var validation = PrestamoValidator.Validar(entity);
            if (!validation.Success) return validation;

            try
            {
                var query = @"INSERT INTO Prestamos (UsuarioId, EjemplarId, FechaPrestamo, FechaVencimiento, FechaDevolucion, Penalizacion)
                              OUTPUT INSERTED.Id
                              VALUES (@UsuarioId, @EjemplarId, @FechaPrestamo, @FechaVencimiento, @FechaDevolucion, @Penalizacion)";

                var parameters = new Dictionary<string, object>
                {
                    {"@UsuarioId", entity.UsuarioId},
                    {"@EjemplarId", entity.EjemplarId},
                    {"@FechaPrestamo", entity.FechaPrestamo},
                    {"@FechaVencimiento", entity.FechaVencimiento},
                    {"@FechaDevolucion", entity.FechaDevolucion ?? (object)DBNull.Value},
                    {"@Penalizacion", entity.Penalizacion ?? (object)DBNull.Value}
                };

                var id = await _dbHelper.ExecuteScalarAsync(query, parameters);
                entity.Id = (int)id;

                return new OperationResult<Prestamo> { Success = true, Data = entity };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar préstamo");
                return new OperationResult<Prestamo> { Success = false, Message = "Error al registrar préstamo" };
            }
        }

        public async Task<OperationResult<IEnumerable<Prestamo>>> GetAllAsync()
        {
            try
            {
                var rows = await _dbHelper.ExecuteQueryAsync(
                    "SELECT Id, UsuarioId, EjemplarId, FechaPrestamo, FechaVencimiento, FechaDevolucion, Penalizacion FROM Prestamos");

                return new OperationResult<IEnumerable<Prestamo>>
                {
                    Success = true,
                    Data = rows.Select(EntityToModelMapper.ToPrestamo)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener préstamos");
                return new OperationResult<IEnumerable<Prestamo>> { Success = false, Message = "Error al obtener préstamos" };
            }
        }

        public async Task<OperationResult<Prestamo>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return new OperationResult<Prestamo> { Success = false, Message = "El ID debe ser mayor que 0" };

            try
            {
                var rows = await _dbHelper.ExecuteQueryAsync(
                    "SELECT TOP 1 * FROM Prestamos WHERE Id=@Id",
                    new() { { "@Id", id } });

                if (!rows.Any())
                    return new OperationResult<Prestamo> { Success = false, Message = "Préstamo no encontrado" };

                return new OperationResult<Prestamo> { Success = true, Data = EntityToModelMapper.ToPrestamo(rows.First()) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener préstamo por Id");
                return new OperationResult<Prestamo> { Success = false, Message = "Error al obtener préstamo por Id" };
            }
        }

        public async Task<OperationResult<Prestamo>> UpdateAsync(Prestamo entity)
        {
            if (entity.Id <= 0)
                return new OperationResult<Prestamo> { Success = false, Message = "El ID es inválido" };

            // Validación centralizada
            var validation = PrestamoValidator.Validar(entity);
            if (!validation.Success) return validation;

            try
            {
                var query = @"UPDATE Prestamos
                              SET UsuarioId=@UsuarioId, EjemplarId=@EjemplarId, FechaPrestamo=@FechaPrestamo, 
                                  FechaVencimiento=@FechaVencimiento, FechaDevolucion=@FechaDevolucion, Penalizacion=@Penalizacion
                              WHERE Id=@Id";

                var parameters = new Dictionary<string, object>
                {
                    {"@UsuarioId", entity.UsuarioId},
                    {"@EjemplarId", entity.EjemplarId},
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
                    Data = entity,
                    Message = rows > 0 ? null : "No se actualizó el registro"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar préstamo");
                return new OperationResult<Prestamo> { Success = false, Message = "Error al actualizar préstamo" };
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(int id)
        {
            if (id <= 0)
                return new OperationResult<bool> { Success = false, Message = "El ID debe ser mayor que 0" };

            try
            {
                var rows = await _dbHelper.ExecuteCommandAsync("DELETE FROM Prestamos WHERE Id=@Id", new() { { "@Id", id } });
                return new OperationResult<bool>
                {
                    Success = rows > 0,
                    Data = rows > 0,
                    Message = rows > 0 ? null : "No se eliminó el registro"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar préstamo");
                return new OperationResult<bool> { Success = false, Message = "Error al eliminar préstamo" };
            }
        }
        #endregion

        #region IPrestamoRepository (métodos específicos)
        public async Task<OperationResult<bool>> RegistrarPrestamoAsync(Prestamo prestamo)
            => (await AddAsync(prestamo)).Success
                ? new OperationResult<bool> { Success = true, Data = true }
                : new OperationResult<bool> { Success = false, Message = "Error al registrar préstamo" };

        public async Task<OperationResult<bool>> RegistrarDevolucionAsync(int prestamoId, DateTime fechaDevolucion, decimal? penalizacion)
        {
            if (prestamoId <= 0)
                return new OperationResult<bool> { Success = false, Message = "El ID es inválido" };

            try
            {
                var query = @"UPDATE Prestamos 
                              SET FechaDevolucion=@FechaDevolucion, Penalizacion=@Penalizacion
                              WHERE Id=@Id";

                var parameters = new Dictionary<string, object>
                {
                    {"@FechaDevolucion", fechaDevolucion},
                    {"@Penalizacion", penalizacion ?? (object)DBNull.Value},
                    {"@Id", prestamoId}
                };

                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);
                return new OperationResult<bool> { Success = rows > 0, Data = rows > 0 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar devolución");
                return new OperationResult<bool> { Success = false, Message = "Error al registrar devolución" };
            }
        }

        public async Task<OperationResult<IEnumerable<Prestamo>>> GetPrestamosActivosAsync()
        {
            try
            {
                var rows = await _dbHelper.ExecuteQueryAsync(
                    "SELECT * FROM Prestamos WHERE FechaDevolucion IS NULL");

                return new OperationResult<IEnumerable<Prestamo>> { Success = true, Data = rows.Select(EntityToModelMapper.ToPrestamo) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener préstamos activos");
                return new OperationResult<IEnumerable<Prestamo>> { Success = false, Message = "Error al obtener préstamos activos" };
            }
        }

        public async Task<OperationResult<IEnumerable<Prestamo>>> GetPrestamosConPenalizacionAsync()
        {
            try
            {
                var rows = await _dbHelper.ExecuteQueryAsync(
                    "SELECT * FROM Prestamos WHERE Penalizacion IS NOT NULL AND Penalizacion > 0");

                return new OperationResult<IEnumerable<Prestamo>> { Success = true, Data = rows.Select(EntityToModelMapper.ToPrestamo) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener préstamos con penalización");
                return new OperationResult<IEnumerable<Prestamo>> { Success = false, Message = "Error al obtener préstamos con penalización" };
            }
        }

        Task<OperationResult<bool>> IPrestamoRepository.AddAsync(Prestamo prestamo)
        {
            // Use the existing AddAsync(Prestamo) method and map its result to OperationResult<bool>
            return AddAsync(prestamo).ContinueWith(task =>
            {
                var result = task.Result;
                return new OperationResult<bool>
                {
                    Success = result.Success,
                    Data = result.Success,
                    Message = result.Message
                };
            });
        }
        #endregion
    }
}


