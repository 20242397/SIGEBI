using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Libro;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Models;

namespace SIGEBI.Application.Services
{
    public class LibroService : BaseService, ILibroService
    {
        private readonly ILibroRepository _libroRepository;
        private readonly ILogger<LibroService> _logger;

        public LibroService(ILibroRepository libroRepository, ILogger<LibroService> logger)
        {
            _libroRepository = libroRepository;
            _logger = logger;
        }

        public Task<ServiceResult<T>> BuscarPorAutorAsync<T>(string autor) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _libroRepository.GetByAuthorAsync(autor);
                _logger.LogInformation("Búsqueda de libros por autor: {Autor}. Resultados encontrados: {Count}",
                    autor, (result.Data as IEnumerable<object>)?.Count() ?? 0);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToList()
                };
            });
       
        public Task<ServiceResult<T>> BuscarPorCategoriaAsync<T>(string categoria) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _libroRepository.GetByCategoryAsync(categoria);
                _logger.LogInformation("Búsqueda de libros por categoría: {Categoria}. Resultados encontrados: {Count}",
                    categoria, (result.Data as IEnumerable<object>)?.Count() ?? 0);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToList()
                };
            });


        public Task<ServiceResult<T>> BuscarPorTituloAsync<T>(string titulo) =>
           ExecuteAsync<T>(async () =>
           {
               var result = await _libroRepository.SearchByTitleAsync(titulo);
               _logger.LogInformation("Búsqueda de libros por título: {Titulo}. Resultados encontrados: {Count}",
                    titulo, (result.Data as IEnumerable<object>)?.Count() ?? 0);
               return new OperationResult<T>
               {
                   Success = result.Success,
                   Message = result.Message,
                   Data = (T)(object)result.Data!.ToList()
               };
           });

        

        public Task<ServiceResult<T>> EliminarLibroAsync<T>(int id) =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _libroRepository.RemoveAsync(id);
                if (result.Success)
                    _logger.LogInformation("Libro eliminado con éxito: {Id}", id);
                else
                    _logger.LogWarning("Error al eliminar libro: {Id}", id);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });


        public Task<ServiceResult<T>> ModificarLibroAsync<T>(LibroGetModel.LibroUpdateDto dto)=>
            ExecuteAsync<T>(async () =>
            {
                var libro = dto.ToEntity();
                var result = await _libroRepository.UpdateAsync(libro);
                if (result.Success)
                    _logger.LogInformation("Libro modificado con éxito: {Id}", libro.Id);
                else
                    _logger.LogWarning("Error al modificar libro: {Id}", libro.Id);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToModel()
                };
            });


        public Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id)=>
                        ExecuteAsync<T>(async () =>
            {
                var result = await _libroRepository.GetByIdAsync(id);
                if (result.Success)
                    _logger.LogInformation("Libro encontrado por ID: {Id}", id);
                else
                    _logger.LogWarning("Libro no encontrado con ID: {Id}", id);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data != null ? (T)(object)result.Data.ToModel() : default!
                };
            });

        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
            ExecuteAsync<T>(async () =>
            {
                var result = await _libroRepository.GetAllAsync();
                _logger.LogInformation("Obtención de todos los libros. Total encontrados: {Count}",
                    (result.Data as IEnumerable<object>)?.Count() ?? 0);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToList()
                };
            });

        public Task<ServiceResult<T>> RegistrarLibroAsync<T>(LibroGetModel.LibroCreateDto dto)=>
            ExecuteAsync<T>(async () =>
            {
                var libro = dto.ToEntity(); 
                var result = await _libroRepository.AddAsync(libro);
                if (result.Success)
                    _logger.LogInformation("Libro registrado con éxito: {Id}", libro.Id);
                else
                    _logger.LogWarning("Error al registrar libro: {Id}", libro.Id);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!.ToModel()
                };
            });
    }
}
