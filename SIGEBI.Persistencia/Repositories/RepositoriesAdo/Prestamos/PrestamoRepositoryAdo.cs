using Microsoft.Extensions.Logging;
using SIGEBI.Application.Repositories.Configuration.IPrestamo;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Models;

namespace SIGEBI.Persistence.Repositories.RepositoriesAdo.Prestamos
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
                    INSERT INTO Prestamo (UsuarioId, LibroId, EjemplarId, FechaPrestamo, FechaVencimiento, FechaDevolucion, Penalizacion)
                    OUTPUT INSERTED.Id
                    VALUES (@UsuarioId, @LibroId, @EjemplarId, @FechaPrestamo, @FechaVencimiento, @FechaDevolucion, @Penalizacion)";

                var parameters = new Dictionary<string, object>
                {
                    {"@UsuarioId", entity.UsuarioId},
                    {"@LibroId", entity.LibroId},
                    {"@EjemplarId", entity.EjemplarId },
                    {"@FechaPrestamo", entity.FechaPrestamo},
                    {"@FechaVencimiento", entity.FechaVencimiento},
                    {"@FechaDevolucion", entity.FechaDevolucion ?? (object)DBNull.Value},
                    {"@Penalizacion", entity.Penalizacion ?? 0}
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
                string query = "SELECT Id, UsuarioId, LibroId, FechaPrestamo, FechaVencimiento, FechaDevolucion, Penalizacion FROM Prestamo";
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
                string query = "SELECT TOP 1 * FROM Prestamo WHERE Id=@Id";
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
            // Validar ID
            if (entity.Id <= 0)
                return new OperationResult<Prestamo>
                {
                    Success = false,
                    Message = "El ID del préstamo es inválido."
                };

            try
            {
                // ✅ Consulta SQL: solo actualiza los campos que pueden cambiar
                string query = @"
                  UPDATE Prestamo
                  SET 
                   FechaVencimiento = @FechaVencimiento,
                   FechaDevolucion = @FechaDevolucion,
                   Penalizacion = @Penalizacion
                   WHERE Id = @Id";

                // ✅ Diccionario de parámetros (solo los campos necesarios)
                var parameters = new Dictionary<string, object>
                {
                 { "@FechaVencimiento", entity.FechaVencimiento },
                 { "@FechaDevolucion", entity.FechaDevolucion ?? (object)DBNull.Value },
                 { "@Penalizacion", entity.Penalizacion ?? (object)DBNull.Value },
                 { "@Id", entity.Id }
                };

                // ✅ Ejecutar la actualización
                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);

                // ✅ Retornar resultado
                return new OperationResult<Prestamo>
                {
                    Success = rows > 0,
                    Message = rows > 0
                        ? "Préstamo actualizado correctamente."
                        : "No se encontró el préstamo o no se realizaron cambios.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                // ✅ Manejo de errores y logging
                _logger.LogError(ex, "Error al actualizar préstamo (ID: {Id})", entity.Id);

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
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = "El ID del préstamo no es válido."
                };

            try
            {
                // Solo permite cancelarlo si aún no se ha devuelto
                string query = @"
            UPDATE Prestamo
            SET Estado = 'Cancelado'
            WHERE Id = @Id AND FechaDevolucion IS NULL";

                var parameters = new Dictionary<string, object> { { "@Id", id } };
                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);

                if (rows == 0)
                    return new OperationResult<bool>
                    {
                        Success = false,
                        Message = "El préstamo no se puede eliminar (ya devuelto o no existe).",
                        Data = false
                    };

                return new OperationResult<bool>
                {
                    Success = true,
                    Message = "Préstamo cancelado correctamente.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar préstamo");
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = $"Error al cancelar préstamo: {ex.Message}"
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
                string query = "SELECT * FROM Prestamo WHERE Penalizacion IS NOT NULL AND Penalizacion > 0";
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

            // ✅ Ahora permite calcular penalización incluso si ya fue devuelto
            DateTime fechaComparacion = prestamo.FechaDevolucion ?? DateTime.Now;
            int diasAtraso = (fechaComparacion - prestamo.FechaVencimiento).Days;

            if (diasAtraso <= 0)
                return new OperationResult<bool> { Success = false, Message = "No hay atraso, no se aplica penalización." };

            decimal penalizacion = diasAtraso * 1.00m;
            prestamo.Penalizacion = penalizacion;

            // ✅ Si no tiene fecha de devolución, la establece a hoy
            if (prestamo.FechaDevolucion == null)
                prestamo.FechaDevolucion = DateTime.Now;

            var update = await UpdateAsync(prestamo);
            return new OperationResult<bool>
            {
                Success = update.Success,
                Message = update.Success
                    ? $"Penalización aplicada correctamente: {penalizacion:C}"
                    : "No se pudo aplicar la penalización.",
                Data = update.Success
            };
        }


        public async Task<OperationResult<IEnumerable<Prestamo>>> GetHistorialPorUsuarioAsync(int usuarioId)
        {
            if (usuarioId <= 0)
                return new OperationResult<IEnumerable<Prestamo>> { Success = false, Message = "ID de usuario inválido." };

            try
            {
                string query = "SELECT * FROM Prestamo WHERE UsuarioId=@UsuarioId";
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
                    FROM Prestamo
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

