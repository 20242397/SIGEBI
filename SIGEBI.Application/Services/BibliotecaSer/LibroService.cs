using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.IBiblioteca;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;

namespace SIGEBI.Application.Services.BibliotecaSer
{
    public sealed class LibroService : BaseService, ILibroService
    {
        private readonly ILibroRepository _libroRepository;
        private readonly ILogger<LibroService> _logger;

        public LibroService(ILibroRepository libroRepository, ILogger<LibroService> logger)
        {
            _libroRepository = libroRepository;
            _logger = logger;
        }

        // Registrar libro
        public Task<ServiceResult<T>> RegistrarLibroAsync<T>(LibroCreateDto dto) =>
            ExecuteAsync(async () =>
            {
                var entity = new Libro
                {
                    Titulo = dto.Titulo,
                    Autor = dto.Autor,
                    ISBN = dto.ISBN,
                    Editorial = dto.Editorial,
                    AñoPublicacion = dto.AñoPublicacion,
                    Categoria = dto.Categoria
                };

                var validation = LibroValidator.Validar(entity);
                if (!validation.Success)
                    return new OperationResult<T> { Success = false, Message = validation.Message };

                // Verificar ISBN único
                var existing = await _libroRepository.GetByISBNAsync(dto.ISBN);
                if (existing.Success && existing.Data is not null)
                {
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "Ya existe un libro registrado con este ISBN."
                    };
                }


                var result = await _libroRepository.AddAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Libro registrado correctamente: {Titulo}", dto.Titulo);
                else
                    _logger.LogWarning("Error al registrar libro: {Titulo}", dto.Titulo);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        // Modificar datos
        public Task<ServiceResult<T>> ModificarLibroAsync<T>(LibroUpdateDto dto) =>
            ExecuteAsync(async () =>
            {
                var libroResult = await _libroRepository.GetByIdAsync(dto.Id);
                if (!libroResult.Success || libroResult.Data == null)
                    return new OperationResult<T> { Success = false, Message = "Libro no encontrado." };

                var libro = libroResult.Data;
                libro.Titulo = dto.Titulo;
                libro.Autor = dto.Autor;
                libro.ISBN = dto.ISBN;
                libro.Editorial = dto.Editorial;
                libro.AñoPublicacion = dto.AñoPublicacion;
                libro.Categoria = dto.Categoria;
                libro.Estado = dto.Estado;

                var validation = LibroValidator.Validar(libro);
                if (!validation.Success)
                    return new OperationResult<T> { Success = false, Message = validation.Message };

                if (libro.Estado == "Inactivo")
                {
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "No se puede modificar un libro inactivo."
                    };
                }

                var updateResult = await _libroRepository.UpdateAsync(libro);
                _logger.LogInformation("Libro actualizado correctamente: {Titulo}", (object)libro.Titulo);

                return new OperationResult<T>
                {
                    Success = updateResult.Success,
                    Message = updateResult.Message,
                    Data = (T)(object)updateResult.Data!
                };
            });

        // Eliminar lógicamente un libro
        public async Task<OperationResult<bool>> RemoveAsync(int id)
        {
            return await _libroRepository.RemoveAsync(id);
        }

        // Buscar por título
        public Task<ServiceResult<T>> BuscarPorTituloAsync<T>(string titulo) =>
            ExecuteAsync(async () =>
            {
                var result = await _libroRepository.SearchByTitleAsync(titulo);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        // Buscar por autor
        public Task<ServiceResult<T>> BuscarPorAutorAsync<T>(string autor) =>
            ExecuteAsync(async () =>
            {
                var result = await _libroRepository.GetByAuthorAsync(autor);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        // Buscar por categoría
        public Task<ServiceResult<T>> BuscarPorCategoriaAsync<T>(string categoria) =>
            ExecuteAsync(async () =>
            {
                var result = await _libroRepository.GetByCategoryAsync(categoria);
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });

        // Buscar por ISBN
        public async Task<OperationResult<Libro>> BuscarPorISBNAsync(string isbn)
        {
            return await _libroRepository.GetByISBNAsync(isbn);
        }

        // Obtener libro por ID
        public Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id) =>
            ExecuteAsync(async () =>
            {
                var result = await _libroRepository.GetByIdAsync(id);

                if (result.Success)
                    _logger.LogInformation("Libro obtenido correctamente por ID:{Id}", id);

                else
                    _logger.LogWarning("Error al obtener libro por ID: {Id}", id);


                    return new OperationResult<T>
                    {
                        Success = result.Success,
                        Message = result.Message,
                        Data = (T)(object)result.Data!
                    };
            });

        // Obtener todos los libros
        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
            ExecuteAsync(async () =>
            {
                var result = await _libroRepository.GetAllAsync();

                _logger.LogInformation("Consulta de libros completada. Total: {Count}",
                    (result.Data as IEnumerable<object>)?.Count() ?? 0);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object)result.Data!
                };
            });
    }
}
