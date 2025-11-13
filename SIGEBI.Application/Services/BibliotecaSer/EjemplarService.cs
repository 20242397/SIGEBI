using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Mappers;
using SIGEBI.Application.Repositories.Configuration.IBiblioteca;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;

namespace SIGEBI.Application.Services.BibliotecaSer
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
                    Data = (T)(object)result.Data!.ToDto()
                };
            });

        
        public Task<ServiceResult<Ejemplar>> ActualizarEjemplarAsync(EjemplarUpdateDto dto) =>
      ExecuteAsync<Ejemplar>(async () =>
      {
          var ejemplarResult = await _ejemplarRepository.GetByIdAsync(dto.Id);
          if (!ejemplarResult.Success || ejemplarResult.Data == null)
              return new OperationResult<Ejemplar> { Success = false, Message = "Ejemplar no encontrado." };

          var ejemplar = ejemplarResult.Data;
          ejemplar.Estado = Enum.Parse<EstadoEjemplar>(dto.Estado, true);
          _logger.LogInformation("Ejemplar (ID: {Id}) actualizado con estado {Estado}", dto.Id, dto.Estado);


          var validation = EjemplarValidator.Validar(ejemplar);
          if (!validation.Success)
              return new OperationResult<Ejemplar> { Success = false, Message = validation.Message };

          var updateResult = await _ejemplarRepository.UpdateAsync(ejemplar);
          _logger.LogInformation($"Ejemplar actualizado (ID: {dto.Id})");

       


          return updateResult;
      });


        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
    ExecuteAsync<T>(async () =>
    {
        var result = await _ejemplarRepository.GetAllAsync();

        if (!result.Success)
            return new OperationResult<T>
            {
                Success = false,
                Message = result.Message
            };

        var listaDto = result.Data
            .Select(e => e.ToDto())
            .ToList();

        return new OperationResult<T>
        {
            Success = true,
            Data = (T)(object)listaDto
        };
    });

        public Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id) =>
    ExecuteAsync<T>(async () =>
    {
        var result = await _ejemplarRepository.GetByIdAsync(id);

        if (!result.Success || result.Data == null)
            return new OperationResult<T>
            {
                Success = false,
                Message = result.Message
            };

        var dto = result.Data.ToDto();

        return new OperationResult<T>
        {
            Success = true,
            Data = (T)(object)dto
        };
    });




      
        public Task<ServiceResult<T>> ObtenerPorLibroAsync<T>(int libroId) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _ejemplarRepository.ObtenerPorLibroAsync(libroId);
                _logger.LogInformation("Consulta de ejemplares del libro ID {LibroId}: {Count}", libroId, result?.Count() ?? 0);

                var listaDto = result.Select(e => e.ToDto()).ToList();
                return new OperationResult<T> { Success = true, Data = (T)(object)listaDto };

               
            });

      
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
    }
}