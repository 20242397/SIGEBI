using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Mappers;
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

        public async Task<OperationResult<Libro>> CambiarEstadoAsync(int id, string nuevoEstado)
        {
            try
            {
                var libroResult = await _libroRepository.GetByIdAsync(id);

                if (!libroResult.Success || libroResult.Data == null)
                    return new OperationResult<Libro> { Success = false, Message = "Libro no encontrado." };

                var libro = libroResult.Data;
                libro.Estado = nuevoEstado;

                var updateResult = await _libroRepository.UpdateAsync(libro);

                if (!updateResult.Success)
                    return new OperationResult<Libro> { Success = false, Message = "No se pudo actualizar el estado." };

                _logger.LogInformation("Estado del libro {Titulo} actualizado a {Estado}", libro.Titulo, nuevoEstado);

                return new OperationResult<Libro>
                {
                    Success = true,
                    Data = updateResult.Data,
                    Message = "Estado actualizado correctamente."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del libro");
                return new OperationResult<Libro>
                {
                    Success = false,
                    Message = $"Error al cambiar el estado: {ex.Message}"
                };
            }
        }


       
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

          object data;

         
          if (typeof(T) == typeof(LibroGetDto))
              data = ((Libro)updateResult.Data!).ToDto();
          else if (typeof(T) == typeof(LibroUpdateDto))
              data = ((Libro)updateResult.Data!).ToUpdateDto();
          else
              data = updateResult.Data!;

          return new OperationResult<T>
          {
              Success = updateResult.Success,
              Message = updateResult.Message,
              Data = (T)data
          };
      });


       
        public async Task<OperationResult<bool>> RemoveAsync(int id)
        {
            return await _libroRepository.RemoveAsync(id);
        }

        
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

       
        public async Task<OperationResult<Libro>> BuscarPorISBNAsync(string isbn)
        {
            return await _libroRepository.GetByISBNAsync(isbn);
        }

        public Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id) =>
      ExecuteAsync(async () =>
      {
          var result = await _libroRepository.GetByIdAsync(id);

          if (!result.Success || result.Data == null)
          {
              return new OperationResult<T>
              {
                  Success = false,
                  Message = "Libro no encontrado."
              };
          }

          var libro = result.Data;

          object data;

          
          if (typeof(T) == typeof(LibroGetDto))
          {
              data = ((Libro)libro).ToDto();
          }
          else if (typeof(T) == typeof(LibroUpdateDto))
          {
              data = ((Libro)libro).ToUpdateDto();
          }
          else if (typeof(T) == typeof(Libro))
          {
              data = libro;
          }
          else
          {
              throw new InvalidOperationException(
                  $"Tipo de retorno no soportado: {typeof(T).Name}");
          }

          return new OperationResult<T>
          {
              Success = true,
              Message = result.Message,
              Data = (T)data
          };
      });



       
        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
     ExecuteAsync(async () =>
     {
         var result = await _libroRepository.GetAllAsync();

         if (!result.Success)
         {
             return new OperationResult<T>
             {
                 Success = false,
                 Message = result.Message
             };
         }

        
         object data;

         if (typeof(T) == typeof(IEnumerable<LibroGetDto>))
         {
             data = result.Data
                 .Select(l => ((Libro)l).ToDto())   
                 .ToList();
         }
         else
         {
            
             data = result.Data!;
         }

         return new OperationResult<T>
         {
             Success = true,
             Message = result.Message,
             Data = (T)data
         };
     });


        public Task<ServiceResult<T>> FiltrarAsync<T>(
      string? titulo, string? autor, string? categoria, int? anio, string? estado)
        {
            return ExecuteAsync(async () =>
            {
                
                var result = await _libroRepository.FiltrarAsync(titulo, autor, categoria, anio, estado);

               
                if (!result.Success || result.Data == null)
                {
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = result.Message ?? "No se pudieron obtener los libros filtrados."
                    };
                }

                object data;

               
                if (typeof(T) == typeof(IEnumerable<LibroGetDto>))
                {
                    data = result.Data
                        .Select(l => l.ToDto())
                        .ToList();
                }
                else if (typeof(T) == typeof(IEnumerable<LibroUpdateDto>))
                {
                    data = result.Data
                        .Select(l => l.ToUpdateDto())
                        .ToList();
                }
                else
                {
                    data = result.Data!;
                }

              
                return new OperationResult<T>
                {
                    Success = true,
                    Data = (T)data,
                    Message = result.Message ?? "Filtrado realizado correctamente."
                };
            });
        }


    }
}