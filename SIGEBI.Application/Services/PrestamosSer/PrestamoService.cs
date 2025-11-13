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

              
                var validation = PrestamoValidator.Validar(entidad);
                if (!validation.Success)
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = validation.Message
                    };

              
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

              
                if (prestamo.Estado == "Cancelado" || prestamo.Estado == "Devuelto")
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "No se puede renovar un préstamo cancelado o devuelto."
                    };

                if (dto.FechaVencimiento != default)
                    prestamo.FechaVencimiento = dto.FechaVencimiento;


                if (dto.FechaDevolucion.HasValue)
                    prestamo.FechaDevolucion = dto.FechaDevolucion;

                if (dto.Penalizacion.HasValue)
                    prestamo.Penalizacion = dto.Penalizacion;

           
                prestamo.Estado = "Activo";

                var updateResult = await _prestamoRepository.UpdateAsync(prestamo);

                return new OperationResult<T>
                {
                    Success = updateResult.Success,
                    Message = updateResult.Message
                };
            });



     
        public Task<ServiceResult<T>> RegistrarDevolucionAsync<T>(int prestamoId, DateTime fechaDevolucion) =>
            ExecuteAsync(async () =>
            {
                var prestamoResult = await _prestamoRepository.GetByIdAsync(prestamoId);

                if (!prestamoResult.Success || prestamoResult.Data == null)
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "Préstamo no encontrado."
                    };

                var prestamo = prestamoResult.Data;

       
                if (prestamo.Estado == "Devuelto")
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "El préstamo ya fue devuelto previamente."
                    };

               
                prestamo.FechaDevolucion = fechaDevolucion;
                prestamo.Estado = "Devuelto";

                var updateResult = await _prestamoRepository.UpdateAsync(prestamo);

                return new OperationResult<T>
                {
                    Success = updateResult.Success,
                    Message = updateResult.Message
                };
            });








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

        public Task<ServiceResult<T>> RemoveAsync<T>(int id) =>
    ExecuteAsync(async () =>
    {
        var result = await _prestamoRepository.RemoveAsync(id);

        return new OperationResult<T>
        {
            Success = result.Success,
            Message = result.Message
        };
    });

    }
}