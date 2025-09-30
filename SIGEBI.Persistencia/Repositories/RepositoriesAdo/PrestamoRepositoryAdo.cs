using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Helpers;
using SIGEBI.Persistence.Models;                // <-- importar mapper
using SIGEBI.Persistence.Models.Configuration;  // <-- para PrestamoGetModel
using Microsoft.Extensions.Logging;

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
            if (entity.UsuarioId <= 0 || entity.EjemplarId <= 0)
                return new OperationResult<Prestamo> { Success = false, Message = "Usuario y Ejemplar son obligatorios" };

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
                var query = "SELECT Id, UsuarioId, EjemplarId, FechaPrestamo, FechaVencimiento, FechaDevolucion, Penalizacion FROM Prestamos";
                var rows = await _dbHelper.ExecuteQueryAsync(query);

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

        public Task<OperationResult<Prestamo>> GetByIdAsync(int id)
        {
            throw new NotImplementedException("Este método no es requerido en los requisitos actuales.");
        }

        public Task<OperationResult<Prestamo>> UpdateAsync(Prestamo entity)
        {
            throw new NotImplementedException("Este método no es requerido en los requisitos actuales.");
        }

        public Task<OperationResult<bool>> RemoveAsync(int id)
        {
            throw new NotImplementedException("Este método no es requerido en los requisitos actuales.");
        }
        #endregion

        #region IPrestamoRepository
        public async Task<OperationResult<bool>> RegistrarPrestamoAsync(Prestamo prestamo)
        {
            var result = await AddAsync(prestamo);
            return new OperationResult<bool>
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Success
            };
        }

        public async Task<OperationResult<bool>> RegistrarDevolucionAsync(int prestamoId, DateTime fechaDevolucion, decimal? penalizacion)
        {
            if (prestamoId <= 0)
                return new OperationResult<bool> { Success = false, Message = "El ID de préstamo es inválido" };

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
                return new OperationResult<bool>
                {
                    Success = rows > 0,
                    Data = rows > 0,
                    Message = rows > 0 ? null : "No se actualizó el registro"
                };
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
                var query = "SELECT Id, UsuarioId, EjemplarId, FechaPrestamo, FechaVencimiento, FechaDevolucion, Penalizacion FROM Prestamos WHERE FechaDevolucion IS NULL";
                var rows = await _dbHelper.ExecuteQueryAsync(query);

                return new OperationResult<IEnumerable<Prestamo>>
                {
                    Success = true,
                    Data = rows.Select(EntityToModelMapper.ToPrestamo)
                };
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
                var query = "SELECT Id, UsuarioId, EjemplarId, FechaPrestamo, FechaVencimiento, FechaDevolucion, Penalizacion FROM Prestamos WHERE Penalizacion IS NOT NULL";
                var rows = await _dbHelper.ExecuteQueryAsync(query);

                return new OperationResult<IEnumerable<Prestamo>>
                {
                    Success = true,
                    Data = rows.Select(EntityToModelMapper.ToPrestamo)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener préstamos con penalización");
                return new OperationResult<IEnumerable<Prestamo>> { Success = false, Message = "Error al obtener préstamos con penalización" };
            }
        }
        #endregion

        #region Métodos DTO
        public async Task<OperationResult<IEnumerable<PrestamoGetModel>>> GetAllModelsAsync()
        {
            try
            {
                var query = "SELECT Id, UsuarioId, EjemplarId, FechaPrestamo, FechaVencimiento, FechaDevolucion, Penalizacion FROM Prestamos";
                var rows = await _dbHelper.ExecuteQueryAsync(query);
                return new OperationResult<IEnumerable<PrestamoGetModel>>
                {
                    Success = true,
                    Data = rows.Select(EntityToModelMapper.ToPrestamoGetModel)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener modelos de préstamos");
                return new OperationResult<IEnumerable<PrestamoGetModel>> { Success = false, Message = "Error al obtener modelos de préstamos" };
            }
        }

        Task<OperationResult<bool>> IPrestamoRepository.AddAsync(Prestamo prestamo)
        {
            
            return RegistrarPrestamoAsync(prestamo);
        }
        #endregion
    }
}

