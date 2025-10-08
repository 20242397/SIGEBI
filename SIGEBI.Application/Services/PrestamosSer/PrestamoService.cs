using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.IPrestamo;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;

namespace SIGEBI.Application.Services.Prestamos
{
    public sealed class PrestamoService : BaseService, IPrestamoService
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly ILogger<PrestamoService> _logger;

        public PrestamoService(IPrestamoRepository prestamoRepository, ILogger<PrestamoService> logger)
        {
            _prestamoRepository = prestamoRepository;
            _logger = logger;
        }

        // ✅ RF3.1 - Registrar préstamo
        public Task<ServiceResult<T>> RegistrarPrestamoAsync<T>(PrestamoCreateDto dto) =>
            ExecuteAsync(async () =>
            {
                var entity = new Prestamo
                {
                    UsuarioId = dto.UsuarioId,
                    EjemplarId = dto.EjemplarId,
                    FechaPrestamo = DateTime.Now,
                    FechaVencimiento = dto.FechaVencimiento,
                    Penalizacion = 0,
                    Estado = "Activo"
                };

                var validation = PrestamoValidator.Validar(entity);
                if (!validation.Success)
                    return new OperationResult<T> { Success = false, Message = validation.Message };

                var result = await _prestamoRepository.RegistrarPrestamoAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Préstamo registrado correctamente para usuario {UsuarioId}, ejemplar {EjemplarId}", dto.UsuarioId, dto.EjemplarId);
                else
                    _logger.LogWarning("Error al registrar préstamo: {Mensaje}", result.Message);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = default!
                };
            });

        // ✅ RF3.2 - Extender préstamo
        public Task<ServiceResult<T>> ExtenderPrestamoAsync<T>(PrestamoUpdateDto dto) =>
            ExecuteAsync(async () =>
            {
                var prestamoResult = await _prestamoRepository.GetByIdAsync(dto.Id);
                if (!prestamoResult.Success || prestamoResult.Data == null)
                    return new OperationResult<T> { Success = false, Message = "Préstamo no encontrado." };

                var prestamo = prestamoResult.Data;
                prestamo.FechaVencimiento = dto.NuevaFechaVencimiento;

                var updateResult = await _prestamoRepository.UpdateAsync(prestamo);

                _logger.LogInformation("Préstamo {Id} extendido hasta {Fecha}", dto.Id, dto.NuevaFechaVencimiento);

                return new OperationResult<T>
                {
                    Success = updateResult.Success,
                    Message = updateResult.Message,
                    Data = (T)(object)updateResult.Data!
                };
            });

        // ✅ RF3.3 - Registrar devolución
        public Task<ServiceResult<T>> RegistrarDevolucionAsync<T>(int prestamoId, DateTime fechaDevolucion) =>
            ExecuteAsync(async () =>
            {
                var result = await _prestamoRepository.RegistrarDevolucionAsync(prestamoId, fechaDevolucion, null);

                if (result.Success)
                    _logger.LogInformation("Devolución registrada para préstamo {Id}", prestamoId);
                else
                    _logger.LogWarning("Error al registrar devolución para préstamo {Id}: {Mensaje}", prestamoId, result.Message);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = default!
                };
            });

        // ✅ RF3.4 - Calcular penalización
        public Task<ServiceResult<T>> CalcularPenalizacionAsync<T>(int prestamoId) =>
            ExecuteAsync(async () =>
            {
                var result = await _prestamoRepository.CalcularPenalizacionAsync(prestamoId);

                if (result.Success)
                    _logger.LogInformation("Penalización calculada correctamente para préstamo {Id}", prestamoId);
                else
                    _logger.LogWarning("Error al calcular penalización para préstamo {Id}: {Mensaje}", prestamoId, result.Message);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = default!
                };
            });

        // ✅ RF3.5 - Restringir préstamo si penalizado
        public Task<ServiceResult<T>> RestringirPrestamoSiPenalizadoAsync<T>(int usuarioId) =>
            ExecuteAsync(async () =>
            {
                var result = await _prestamoRepository.RestringirPrestamoSiPenalizadoAsync(usuarioId);

                if (result.Success)
                    _logger.LogInformation("Verificación de restricción completada para usuario {UsuarioId}", usuarioId);
                else
                    _logger.LogWarning("Error al verificar restricción de préstamo para usuario {UsuarioId}", usuarioId);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = default!
                };
            });

        // ✅ RF2.5 - Obtener historial del usuario
        public Task<ServiceResult<T>> ObtenerHistorialUsuarioAsync<T>(int usuarioId) =>
            ExecuteAsync(async () =>
            {
                var result = await _prestamoRepository.GetHistorialPorUsuarioAsync(usuarioId);

                if (result.Success)
                    _logger.LogInformation("Historial de préstamos obtenido para usuario {UsuarioId}", usuarioId);
                else
                    _logger.LogWarning("Error al obtener historial para usuario {UsuarioId}: {Mensaje}", usuarioId, result.Message);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });
    }
}
