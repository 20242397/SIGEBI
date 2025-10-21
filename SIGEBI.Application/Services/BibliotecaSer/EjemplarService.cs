using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.IBiblioteca;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;

namespace SIGEBI.Application.Services
{
    public sealed class EjemplarService : BaseService, IEjemplarService
    {
        private readonly IEjemplarRepository _ejemplarRepository;
        private readonly ILogger<EjemplarService> _logger;

        public EjemplarService(IEjemplarRepository ejemplarRepository, ILogger<EjemplarService> logger)
        {
            _ejemplarRepository = ejemplarRepository;
            _logger = logger;
        }

        // ✅ Registrar nuevo ejemplar
        public Task<ServiceResult<T>> RegistrarEjemplarAsync<T>(EjemplarCreateDto dto) =>
            ExecuteAsync<T>(async () =>
            {
                var entity = new Ejemplar
                {
                    LibroId = dto.LibroId,
                    CodigoBarras = dto.CodigoBarras,
                    Estado = EstadoEjemplar.Disponible
                };

                var validation = EjemplarValidator.Validar(entity);
                if (!validation.Success)
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = validation.Message
                    };

                var result = await _ejemplarRepository.AddAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Ejemplar agregado correctamente para el libro ID {LibroId}", dto.LibroId);
                else
                    _logger.LogWarning("Error al registrar ejemplar para el libro ID {LibroId}", dto.LibroId);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        // ✅ Actualizar ejemplar
        public Task<ServiceResult<T>> ActualizarEjemplarAsync<T>(EjemplarUpdateDto dto) =>
            ExecuteAsync<T>(async () =>
            {
                var ejemplarResult = await _ejemplarRepository.GetByIdAsync(dto.Id);
                if (!ejemplarResult.Success || ejemplarResult.Data == null)
                    return new OperationResult<T> { Success = false, Message = "Ejemplar no encontrado" };

                var ejemplar = ejemplarResult.Data;
                ejemplar.Estado = dto.Estado ?? ejemplar.Estado;

                var validation = EjemplarValidator.Validar(ejemplar);
                if (!validation.Success)
                    return new OperationResult<T> { Success = false, Message = validation.Message };

                var updateResult = await _ejemplarRepository.UpdateAsync(ejemplar);
                _logger.LogInformation("Ejemplar actualizado (ID {Id})", dto.Id);

                return new OperationResult<T>
                {
                    Success = updateResult.Success,
                    Message = updateResult.Message,
                    Data = (T)(object)updateResult.Data!
                };
            });

        // ✅ Obtener ejemplares por libro
        public Task<ServiceResult<T>> ObtenerPorLibroAsync<T>(int libroId) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _ejemplarRepository.ObtenerPorLibroAsync(libroId);
                _logger.LogInformation("Consulta de ejemplares del libro ID {LibroId}: {Count}", libroId, result?.Count() ?? 0);

                return new OperationResult<T>
                {
                    Success = true,
                    Data = (T)(object)result!
                };
            });

        // ✅ Obtener disponibles por libro
        public Task<ServiceResult<T>> ObtenerDisponiblesPorLibroAsync<T>(int libroId) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _ejemplarRepository.ObtenerDisponiblesPorLibroAsync(libroId);
                _logger.LogInformation("Ejemplares disponibles para el libro ID {LibroId}: {Count}", libroId, result?.Count() ?? 0);

                return new OperationResult<T>
                {
                    Success = true,
                    Data = (T)(object)result!
                };
            });

        // ✅ Listar prestados
        public Task<ServiceResult<T>> ObtenerPrestadosAsync<T>() =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _ejemplarRepository.ObtenerPrestadosAsync();
                _logger.LogInformation("Consulta de ejemplares prestados completada: {Count}", result?.Count() ?? 0);

                return new OperationResult<T>
                {
                    Success = true,
                    Data = (T)(object)result!
                };
            });

        // ✅ Listar reservados
        public Task<ServiceResult<T>> ObtenerReservadosAsync<T>() =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _ejemplarRepository.ObtenerReservadosAsync();
                _logger.LogInformation("Consulta de ejemplares reservados completada: {Count}", result?.Count() ?? 0);

                return new OperationResult<T>
                {
                    Success = true,
                    Data = (T)(object)result!
                };
            });

        // ✅ Marcar como perdido
        public Task<ServiceResult<T>> MarcarComoPerdidoAsync<T>(int ejemplarId) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _ejemplarRepository.MarcarComoPerdidoAsync(ejemplarId);
                _logger.LogInformation("Ejemplar marcado como perdido: {Id}", ejemplarId);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = default!
                };
            });

        // ✅ Marcar como dañado
        public Task<ServiceResult<T>> MarcarComoDañadoAsync<T>(int ejemplarId) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _ejemplarRepository.MarcarComoDañadoAsync(ejemplarId);
                _logger.LogInformation("Ejemplar marcado como dañado: {Id}", ejemplarId);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = default!
                };
            });
    }
}
