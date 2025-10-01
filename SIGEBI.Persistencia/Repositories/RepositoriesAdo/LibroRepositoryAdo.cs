using Microsoft.Extensions.Logging;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Helpers;
using SIGEBI.Persistence.Models;
using SIGEBI.Persistence.Models.Configuration.Libro;
using SIGEBI.Persistence.Validators;  // Importante

namespace SIGEBI.Persistence.Repositories.Ado
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

        #region IBaseRepository<Libro>
        public async Task<OperationResult<Libro>> AddAsync(Libro entity)
        {
            // Validación centralizada
            var validation = LibroValidator.Validar(entity);
            if (!validation.Success) return validation;

            try
            {
                var query = @"INSERT INTO Libros (Titulo, Autor, ISBN, Editorial, AñoPublicacion, Categoria)
                              OUTPUT INSERTED.Id
                              VALUES (@Titulo, @Autor, @ISBN, @Editorial, @AñoPublicacion, @Categoria)";
                var parameters = new Dictionary<string, object>
                {
                    {"@Titulo", entity.Titulo},
                    {"@Autor", entity.Autor},
                    {"@ISBN", entity.ISBN},
                    {"@Editorial", entity.Editorial},
                    {"@AñoPublicacion", entity.AñoPublicacion},
                    {"@Categoria", entity.Categoria ?? (object)DBNull.Value}
                };

                var id = await _dbHelper.ExecuteScalarAsync(query, parameters);
                entity.Id = (int)id;

                return new OperationResult<Libro> { Success = true, Data = entity };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar libro");
                return new OperationResult<Libro> { Success = false, Message = "Error al agregar libro" };
            }
        }

        public async Task<OperationResult<IEnumerable<Libro>>> GetAllAsync()
        {
            try
            {
                var rows = await _dbHelper.ExecuteQueryAsync(
                    "SELECT Id, Titulo, Autor, ISBN, Editorial, AñoPublicacion, Categoria FROM Libros");

                return new OperationResult<IEnumerable<Libro>>
                {
                    Success = true,
                    Data = rows.Select(EntityToModelMapper.ToLibro)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener libros");
                return new OperationResult<IEnumerable<Libro>> { Success = false, Message = "Error al obtener libros" };
            }
        }

        public async Task<OperationResult<Libro>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return new OperationResult<Libro> { Success = false, Message = "El ID debe ser mayor que 0" };

            try
            {
                var rows = await _dbHelper.ExecuteQueryAsync(
                    "SELECT TOP 1 Id, Titulo, Autor, ISBN, Editorial, AñoPublicacion, Categoria FROM Libros WHERE Id=@Id",
                    new() { { "@Id", id } });

                if (!rows.Any())
                    return new OperationResult<Libro> { Success = false, Message = "Libro no encontrado" };

                return new OperationResult<Libro> { Success = true, Data = EntityToModelMapper.ToLibro(rows.First()) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener libro por Id");
                return new OperationResult<Libro> { Success = false, Message = "Error al obtener libro por Id" };
            }
        }

        public async Task<OperationResult<Libro>> UpdateAsync(Libro entity)
        {
            if (entity.Id <= 0)
                return new OperationResult<Libro> { Success = false, Message = "El ID es inválido" };

            // Validación centralizada
            var validation = LibroValidator.Validar(entity);
            if (!validation.Success) return validation;

            try
            {
                var query = @"UPDATE Libros 
                              SET Titulo=@Titulo, Autor=@Autor, ISBN=@ISBN, Editorial=@Editorial, 
                                  AñoPublicacion=@AñoPublicacion, Categoria=@Categoria
                              WHERE Id=@Id";
                var parameters = new Dictionary<string, object>
                {
                    {"@Titulo", entity.Titulo},
                    {"@Autor", entity.Autor},
                    {"@ISBN", entity.ISBN},
                    {"@Editorial", entity.Editorial},
                    {"@AñoPublicacion", entity.AñoPublicacion},
                    {"@Categoria", entity.Categoria ?? (object)DBNull.Value},
                    {"@Id", entity.Id}
                };

                var rows = await _dbHelper.ExecuteCommandAsync(query, parameters);
                return new OperationResult<Libro>
                {
                    Success = rows > 0,
                    Data = entity,
                    Message = rows > 0 ? null : "No se actualizó el registro"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar libro");
                return new OperationResult<Libro> { Success = false, Message = "Error al actualizar libro" };
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(int id)
        {
            if (id <= 0)
                return new OperationResult<bool> { Success = false, Message = "El ID debe ser mayor que 0" };

            try
            {
                var rows = await _dbHelper.ExecuteCommandAsync("DELETE FROM Libros WHERE Id=@Id", new() { { "@Id", id } });
                return new OperationResult<bool>
                {
                    Success = rows > 0,
                    Data = rows > 0,
                    Message = rows > 0 ? null : "No se eliminó el registro"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar libro");
                return new OperationResult<bool> { Success = false, Message = "Error al eliminar libro" };
            }
        }
        #endregion

        #region ILibroRepository (específicos)
        public Task<OperationResult<IEnumerable<Libro>>> BuscarPorTituloAsync(string titulo)
            => BuscarAsync("Titulo", titulo);

        public Task<OperationResult<IEnumerable<Libro>>> BuscarPorAutorAsync(string autor)
            => BuscarAsync("Autor", autor);

        public Task<OperationResult<IEnumerable<Libro>>> BuscarPorCategoriaAsync(string categoria)
            => BuscarAsync("Categoria", categoria);

        public async Task<OperationResult<IEnumerable<Libro>>> GetLibrosMasPrestadosAsync(int topN)
        {
            if (topN <= 0)
                return new OperationResult<IEnumerable<Libro>> { Success = false, Message = "El número debe ser mayor que 0" };

            try
            {
                var query = @"SELECT TOP (@TopN) l.Id, l.Titulo, l.Autor, l.ISBN, l.Editorial, l.AñoPublicacion, l.Categoria, COUNT(p.Id) AS TotalPrestamos
                              FROM Libros l
                              INNER JOIN Ejemplares e ON l.Id = e.LibroId
                              INNER JOIN Prestamos p ON e.Id = p.EjemplarId
                              GROUP BY l.Id, l.Titulo, l.Autor, l.ISBN, l.Editorial, l.AñoPublicacion, l.Categoria
                              ORDER BY TotalPrestamos DESC";

                var rows = await _dbHelper.ExecuteQueryAsync(query, new() { { "@TopN", topN } });
                return new OperationResult<IEnumerable<Libro>> { Success = true, Data = rows.Select(EntityToModelMapper.ToLibro) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener libros más prestados");
                return new OperationResult<IEnumerable<Libro>> { Success = false, Message = "Error al obtener libros más prestados" };
            }
        }

        async Task<OperationResult<bool>> ILibroRepository.AddAsync(Libro libro)
        {
            var result = await AddAsync(libro);
            return new OperationResult<bool> { Success = result.Success, Message = result.Message, Data = result.Success };
        }
        #endregion

        #region Helpers
        private async Task<OperationResult<IEnumerable<Libro>>> BuscarAsync(string campo, string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return new OperationResult<IEnumerable<Libro>> { Success = false, Message = $"El valor de búsqueda para {campo} no puede estar vacío" };

            try
            {
                var rows = await _dbHelper.ExecuteQueryAsync(
                    $"SELECT Id, Titulo, Autor, ISBN, Editorial, AñoPublicacion, Categoria FROM Libros WHERE {campo} LIKE @Valor",
                    new() { { "@Valor", $"%{valor}%" } });

                return new OperationResult<IEnumerable<Libro>> { Success = true, Data = rows.Select(EntityToModelMapper.ToLibro) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar libros por {Campo}", campo);
                return new OperationResult<IEnumerable<Libro>> { Success = false, Message = $"Error al buscar libros por {campo}" };
            }
        }
        #endregion
    }
}

