using Microsoft.Extensions.Logging;
using SIGEBI.Application.Repositories.Configuration.IBiblioteca;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Models;
using SIGEBI.Application.Validators;

namespace SIGEBI.Persistence.Repositories.RepositoriesAdo.Biblioteca
{
    public sealed class LibroRepositoryAdo : ILibroRepository
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<LibroRepositoryAdo> _logger;

        public LibroRepositoryAdo(DbHelper dbHelper, ILogger<LibroRepositoryAdo> logger)
        {
            _dbHelper = dbHelper;
            _logger = logger;
        }

        #region  Add
        public async Task<OperationResult<Libro>> AddAsync(Libro entity)
        {
            var validacion = LibroValidator.Validar(entity);
            if (!validacion.Success) return validacion;

            try
            {
                string query = @"
                    INSERT INTO Libro (Titulo, Autor, ISBN, Editorial, AñoPublicacion, Categoria, Estado)
                    OUTPUT INSERTED.Id
                    VALUES (@Titulo, @Autor, @ISBN, @Editorial, @AñoPublicacion, @Categoria, @Estado)";

                var parameters = new Dictionary<string, object>
                {
                    {"@Titulo", entity.Titulo},
                    {"@Autor", entity.Autor},
                    {"@ISBN", entity.ISBN},
                    {"@Editorial", entity.Editorial},
                    {"@AñoPublicacion", entity.AñoPublicacion},
                    {"@Categoria", entity.Categoria ?? (object)DBNull.Value},
                    {"@Estado", entity.Estado ?? "Disponible"}
                };

                var id = await _dbHelper.ExecuteScalarAsync(query, parameters);
                entity.Id = Convert.ToInt32(id);

                return new OperationResult<Libro>
                {
                    Success = true,
                    Message = "Libro registrado correctamente.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar libro");
                return new OperationResult<Libro>
                {
                    Success = false,
                    Message = $"Error al registrar libro: {ex.Message}"
                };
            }
        }
        #endregion

        #region  GetAll
        public async Task<OperationResult<IEnumerable<Libro>>> GetAllAsync()
        {
            try
            {
                var query = "SELECT Id, Titulo, Autor, ISBN, Editorial, AñoPublicacion, Categoria, Estado FROM Libro";
                var rows = await _dbHelper.ExecuteQueryAsync(query);

                var libros = rows.Select(EntityToModelMapper.ToLibro).ToList();

                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = true,
                    Data = libros
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener libros");
                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = false,
                    Message = $"Error al obtener libros: {ex.Message}"
                };
            }
        }
        #endregion

        #region  GetById
        public async Task<OperationResult<Libro>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return new OperationResult<Libro> { Success = false, Message = "El ID debe ser mayor que 0." };

            try
            {
                var query = "SELECT TOP 1 * FROM Libro WHERE Id=@Id";
                var parameters = new Dictionary<string, object> { { "@Id", id } };

                var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);
                if (!rows.Any())
                    return new OperationResult<Libro> { Success = false, Message = "Libro no encontrado." };

                var libro = EntityToModelMapper.ToLibro(rows.First());
                return new OperationResult<Libro> { Success = true, Data = libro };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener libro por Id");
                return new OperationResult<Libro>
                {
                    Success = false,
                    Message = $"Error al obtener libro por Id: {ex.Message}"
                };
            }
        }
        #endregion

        #region  GetByAuthor
        public async Task<OperationResult<IEnumerable<Libro>>> GetByAuthorAsync(string autor)
        {
            if (string.IsNullOrWhiteSpace(autor))
                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = false,
                    Message = "Debe proporcionar un autor."
                };

            try
            {
                var query = "SELECT * FROM Libro WHERE Autor LIKE @Autor";
                var parameters = new Dictionary<string, object> { { "@Autor", $"%{autor}%" } };

                var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);
                var libros = rows.Select(EntityToModelMapper.ToLibro).ToList();

                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = true,
                    Data = libros
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar libros por autor");
                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = false,
                    Message = $"Error al buscar libros por autor: {ex.Message}"
                };
            }
        }
        #endregion

        #region  GetByCategory
        public async Task<OperationResult<IEnumerable<Libro>>> GetByCategoryAsync(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria))
                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = false,
                    Message = "Debe proporcionar una categoría."
                };

            try
            {
                var query = "SELECT * FROM Libro WHERE Categoria LIKE @Categoria";
                var parameters = new Dictionary<string, object> { { "@Categoria", $"%{categoria}%" } };

                var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);
                var libros = rows.Select(EntityToModelMapper.ToLibro).ToList();

                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = true,
                    Data = libros
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar libros por categoría");
                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = false,
                    Message = $"Error al buscar libros por categoría: {ex.Message}"
                };
            }
        }
        #endregion

        #region  SearchByTitle
        public async Task<OperationResult<IEnumerable<Libro>>> SearchByTitleAsync(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = false,
                    Message = "Debe proporcionar un título."
                };

            try
            {
                var query = "SELECT * FROM Libro WHERE Titulo LIKE @Titulo";
                var parameters = new Dictionary<string, object> { { "@Titulo", $"%{titulo}%" } };

                var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);
                var libros = rows.Select(EntityToModelMapper.ToLibro).ToList();

                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = true,
                    Data = libros
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar libros por título");
                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = false,
                    Message = $"Error al buscar libros por título: {ex.Message}"
                };
            }
        }
        #endregion

        #region  Update

        public async Task<OperationResult<Libro>> UpdateAsync(Libro entity)
        {
            if (entity.Id <= 0)
                return new OperationResult<Libro>
                {
                    Success = false,
                    Message = "El ID es inválido."
                };

            try
            {
                var query = @"
                   UPDATE Libro
                   SET 
                   Titulo = @Titulo,
                   Autor = @Autor,
                   ISBN = @ISBN,
                   Editorial = @Editorial,
                   AñoPublicacion = @AñoPublicacion,
                   Categoria = @Categoria,
                   Estado = @Estado
                   WHERE Id = @Id";

                var parameters = new Dictionary<string, object>
                {
                    { "@Titulo", entity.Titulo},
                    {"@Autor", entity.Autor},
                    {"@ISBN", entity.ISBN},
                    {"@Editorial", entity.Editorial},
                    {"@AñoPublicacion", entity.AñoPublicacion},
                    {"@Categoria", entity.Categoria ?? (object)DBNull.Value},
                    {"@Estado", entity.Estado ?? "Disponible"},
                    {"@Id", entity.Id}
                };

                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);

                if (rows > 0)
                {
                    //  Refrescamos el registro desde la base para devolver datos actualizados
                    var refreshed = await GetByIdAsync(entity.Id);
                    return new OperationResult<Libro>
                    {
                        Success = true,
                        Message = "Libro actualizado correctamente.",
                        Data = refreshed.Data
                    };
                }

                return new OperationResult<Libro>
                {
                    Success = false,
                    Message = "No se actualizó el registro.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar libro");
                return new OperationResult<Libro>
                {
                    Success = false,
                    Message = $"Error al actualizar libro: {ex.Message}"
                };
            }
        }



        #endregion

        #region  Remove

        #region  Remove (Eliminación Lógica)
        public async Task<OperationResult<bool>> RemoveAsync(int id)
        {
            if (id <= 0)
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = "El ID del libro no es válido."
                };

            try
            {
                // Cambiamos el estado a 'Inactivo' en lugar de eliminar físicamente
                var query = "UPDATE Libro SET Estado = 'Inactivo' WHERE Id = @Id";
                var parameters = new Dictionary<string, object> { { "@Id", id } };

                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);

                if (rows == 0)
                    return new OperationResult<bool>
                    {
                        Success = false,
                        Message = "No se encontró el libro o ya está inactivo.",
                        Data = false
                    };

                return new OperationResult<bool>
                {
                    Success = true,
                    Message = "Libro desactivado correctamente.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar libro");
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = $"Error al desactivar libro: {ex.Message}"
                };
            }
        }
        #endregion


        #region  GetByISBN
        public async Task<OperationResult<Libro>> GetByISBNAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return new OperationResult<Libro>
                {
                    Success = false,
                    Message = "Debe proporcionar un ISBN válido."
                };

            try
            {
                string query = "SELECT TOP 1 * FROM Libro WHERE ISBN = @ISBN";
                var parameters = new Dictionary<string, object> { { "@ISBN", isbn } };

                var rows = await _dbHelper.ExecuteQueryAsync(query, parameters);
                if (!rows.Any())
                    return new OperationResult<Libro>
                    {
                        Success = false,
                        Message = "No se encontró un libro con ese ISBN."
                    };

                var libro = EntityToModelMapper.ToLibro(rows.First());
                return new OperationResult<Libro>
                {
                    Success = true,
                    Data = libro
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener libro por ISBN");
                return new OperationResult<Libro>
                {
                    Success = false,
                    Message = $"Error al obtener libro por ISBN: {ex.Message}"
                };
            }
        }
        #endregion

        Task<OperationResult<Libro>> IBaseRepository<Libro>.RemoveAsync(int id)
        {
            // Call RemoveAsync and convert the result to OperationResult<Libro>
            return RemoveAsync(id).ContinueWith(task =>
            {
                var boolResult = task.Result;
                return new OperationResult<Libro>
                {
                    Success = boolResult.Success,
                    Message = boolResult.Message,
                    Data = null // No Libro data to return on delete
                };
            });
        }

        

    }

    #endregion

}

