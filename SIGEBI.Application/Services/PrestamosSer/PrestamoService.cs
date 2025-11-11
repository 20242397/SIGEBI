using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Mappers;
using SIGEBI.Application.Repositories.Configuration.IPrestamo;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;

namespace SIGEBI.Application.Services.PrestamosSer
{
    public sealed class PrestamoService : BaseService, IPrestamoService
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly ILogger<PrestamoService> _logger;

        public PrestamoService(
            IPrestamoRepository prestamoRepository,
            ILogger<PrestamoService> logger)
        {
            _prestamoRepository = prestamoRepository;
            _logger = logger;
        }

        //  Obtener todos (Index)
        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
            ExecuteAsync(async () =>
            {
                var result = await _prestamoRepository.GetAllAsync();

                if (!result.Success)
                {
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = result.Message
                    };
                }

                var listaDto = result.Data
                    .Select(p => p.ToDto())   
                    .ToList();

                return new OperationResult<T>
                {
                    Success = true,
                    Data = (T)(object)listaDto
                };
            });



        // Registrar préstamo
        public Task<ServiceResult<T>> RegistrarPrestamoAsync<T>(PrestamoCreateDto dto) =>
            ExecuteAsync(async () =>
            {
                var entidad = new Prestamo
                {
                    UsuarioId = dto.UsuarioId,
                    EjemplarId = dto.EjemplarId,
                    LibroId = dto.LibroId,
                    FechaPrestamo = DateTime.Now,
                    FechaVencimiento = dto.FechaVencimiento,
                    Penalizacion = 0,
                    Estado = "Activo"
                };

                // Validación de dominio
                var validation = PrestamoValidator.Validar(entidad);
                if (!validation.Success)
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = validation.Message
                    };

                // Validación de penalización
                var restriccion = await _prestamoRepository.RestringirPrestamoSiPenalizadoAsync(dto.UsuarioId);
                if (restriccion.Success && restriccion.Data)
                {
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "El usuario tiene penalización activa y no puede realizar préstamos."
                    };
                }

                var result = await _prestamoRepository.RegistrarPrestamoAsync(entidad);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message
                };
            });



        // ✅ Extender préstamo
        public Task<ServiceResult<T>> ExtenderPrestamoAsync<T>(PrestamoUpdateDto dto) =>
            ExecuteAsync(async () =>
            {
                var prestamoResult = await _prestamoRepository.GetByIdAsync(dto.Id);

                if (!prestamoResult.Success || prestamoResult.Data == null)
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "Préstamo no encontrado."
                    };

                var prestamo = prestamoResult.Data;
                prestamo.FechaVencimiento = dto.NuevaFechaVencimiento;

                var updateResult = await _prestamoRepository.UpdateAsync(prestamo);

                return new OperationResult<T>
                {
                    Success = updateResult.Success,
                    Message = updateResult.Message
                };
            });



        // ✅ Registrar devolución
        public Task<ServiceResult<T>> RegistrarDevolucionAsync<T>(int prestamoId, DateTime fechaDevolucion) =>
            ExecuteAsync(async () =>
            {
                var result = await _prestamoRepository.RegistrarDevolucionAsync(prestamoId, fechaDevolucion, null);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message
                };
            });



        // ✅ Calcular penalización
        public Task<ServiceResult<T>> CalcularPenalizacionAsync<T>(int prestamoId) =>
            ExecuteAsync(async () =>
            {
                var result = await _prestamoRepository.CalcularPenalizacionAsync(prestamoId);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message
                };
            });



        // ✅ Restringir préstamo si penalizado
        public Task<ServiceResult<T>> RestringirPrestamoSiPenalizadoAsync<T>(int usuarioId) =>
            ExecuteAsync(async () =>
            {
                var result = await _prestamoRepository.RestringirPrestamoSiPenalizadoAsync(usuarioId);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message
                };
            });



        // ✅ Historial del usuario
        public Task<ServiceResult<T>> ObtenerHistorialUsuarioAsync<T>(int usuarioId) =>
            ExecuteAsync(async () =>
            {
                var result = await _prestamoRepository.GetHistorialPorUsuarioAsync(usuarioId);

                if (!result.Success)
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = result.Message
                    };

                var listaDto = result.Data
                    .Select(p => p.ToDto())
                    .ToList();

                return new OperationResult<T>
                {
                    Success = true,
                    Data = (T)(object)listaDto
                };
            });
    }
}
