using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration.IBiblioteca
{
    public interface ILibroRepository : IBaseRepository<Libro>
    {
        // 🔹 Agregar un nuevo libro
        Task<OperationResult<Libro>> AddAsync(Libro libro);

        // 🔹 Obtener todos los libros
        Task<OperationResult<IEnumerable<Libro>>> GetAllAsync();

        // 🔹 Buscar libro por ID
        Task<OperationResult<Libro>> GetByIdAsync(int id);

        // 🔹 Buscar libros por autor
        Task<OperationResult<IEnumerable<Libro>>> GetByAuthorAsync(string autor);

        // 🔹 Buscar libros por categoría
        Task<OperationResult<IEnumerable<Libro>>> GetByCategoryAsync(string categoria);

        // 🔹 Buscar libros por título
        Task<OperationResult<IEnumerable<Libro>>> SearchByTitleAsync(string titulo);

        // 🔹 Actualizar un libro
        Task<OperationResult<Libro>> UpdateAsync(Libro libro);

        // 🔹 Eliminar un libro
        Task<OperationResult<bool>> RemoveAsync(int id);
        Task<OperationResult<bool>> GetByISBNAsync(string isbn);
    }
}
