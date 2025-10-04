using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Services
{
    public class PrestamoService : BaseService, IPrestamoService
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly ILogger<PrestamoService> _logger;

        public PrestamoService(IPrestamoRepository prestamoRepository, ILogger<PrestamoService> logger)
        {
            _prestamoRepository = prestamoRepository;
            _logger = logger;
        }

        public Task<ServiceResult<T>> CalcularPenalizacionAsync<T>(int prestamoId) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _prestamoRepository.CalcularPenalizacionAsync(prestamoId);
                _logger.LogInformation($"Cálculo de penalización para el préstamo ID: {prestamoId}. Resultado: {result.Data}");
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        public Task<ServiceResult<T>> ObtenerHistorialUsuarioAsync<T>(int usuarioId) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _prestamoRepository.GetHistorialPorUsuarioAsync(usuarioId);
                var count = (result.Data as IEnumerable<object>)?.Count() ?? 0;
                _logger.LogInformation($"Historial de préstamos obtenido para usuario ID: {usuarioId}. Total registros: {count}");
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        public Task<ServiceResult<T>> RegistrarDevolucionAsync<T>(int prestamoId, DateTime fechaDevolucion) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _prestamoRepository.RegistrarDevolucionAsync(prestamoId, fechaDevolucion, null);
                _logger.LogInformation($"Devolución registrada para el préstamo ID: {prestamoId} en fecha {fechaDevolucion}. Resultado: {result.Data}");
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        public Task<ServiceResult<T>> RegistrarPrestamoAsync<T>(PrestamoGetModel.PrestamoCreateDto dto) =>
            ExecuteAsync<T>(async () =>
            {
                var prestamo = new Domain.Entitines.Configuration.Prestamos.Prestamo
                {
                    UsuarioId = dto.UsuarioId,
                    LibroId = dto.LibroId,
                    FechaPrestamo = dto.FechaPrestamo,
                    FechaVencimiento = dto.FechaVencimiento,
                    Penalizacion = 0m
                };

                var result = await _prestamoRepository.RegistrarPrestamoAsync(prestamo);
                _logger.LogInformation($"Nuevo préstamo registrado. Usuario ID: {dto.UsuarioId}, Libro ID: {dto.LibroId}, Resultado: {result.Data}");
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        public Task<ServiceResult<T>> RestringirPrestamoSiPenalizadoAsync<T>(int usuarioId) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _prestamoRepository.RestringirPrestamoSiPenalizadoAsync(usuarioId);
                _logger.LogInformation($"Verificación de restricciones de préstamo para usuario ID: {usuarioId}. Resultado: {result.Data}");
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });
    }
}
