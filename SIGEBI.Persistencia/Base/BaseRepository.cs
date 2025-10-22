using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Repository;
using SIGEBI.Infrastructure.Logging;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Base
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly SIGEBIContext _context;
        protected readonly DbSet<T> _entities;
        protected readonly ILoggerService<T> _logger;
       
        protected BaseRepository(SIGEBIContext context, ILoggerService<T> logger)
        {
            _context = context;
            _entities = _context.Set<T>();
            _logger = logger;
        }

        

        /// <summary>
        /// Ejecuta una acción asíncrona con manejo de errores centralizado.
        /// </summary>
        protected async Task<OperationResult<TResult>> ExecuteAsync<TResult>(
            Func<Task<OperationResult<TResult>>> action,
            string errorMessage)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                return new OperationResult<TResult>
                {
                    Success = false,
                    Message = errorMessage
                };
            }
        }

        // ✅ Agregar entidad
        public virtual async Task<OperationResult<T>> AddAsync(T entity)
        {
            return await ExecuteAsync(async () =>
            {
                await _entities.AddAsync(entity);
                await _context.SaveChangesAsync();
                return new OperationResult<T> { Success = true, Data = entity };
            }, "Error al agregar entidad.");
        }

        // ✅ Obtener todas las entidades
        public virtual async Task<OperationResult<IEnumerable<T>>> GetAllAsync()
        {
            return await ExecuteAsync(async () =>
            {
                var list = await _entities.ToListAsync();
                return new OperationResult<IEnumerable<T>> { Success = true, Data = list };
            }, "Error al obtener entidades.");
        }

        // ✅ Buscar por ID
        public virtual async Task<OperationResult<T>> GetByIdAsync(int id)
        {
            return await ExecuteAsync(async () =>
            {
                var entity = await _entities.FindAsync(id);
                return entity == null
                    ? new OperationResult<T> { Success = false, Message = "No se encontró la entidad." }
                    : new OperationResult<T> { Success = true, Data = entity };
            }, "Error al buscar entidad.");
        }

        // ✅ Actualizar
        public virtual async Task<OperationResult<T>> UpdateAsync(T entity)
        {
            return await ExecuteAsync(async () =>
            {
                _entities.Update(entity);
                await _context.SaveChangesAsync();
                return new OperationResult<T> { Success = true, Data = entity };
            }, "Error al actualizar entidad.");
        }

        // ✅ Eliminar
        public virtual async Task<OperationResult<bool>> RemoveAsync(int id)
        {
            return await ExecuteAsync(async () =>
            {
                var entity = await _entities.FindAsync(id);
                if (entity == null)
                    return new OperationResult<bool> { Success = false, Message = "No se encontró la entidad." };

                _entities.Remove(entity);
                await _context.SaveChangesAsync();
                return new OperationResult<bool> { Success = true, Data = true };
            }, "Error al eliminar entidad.");
        }

        // Implementación explícita para IBaseRepository
        Task<OperationResult<T>> IBaseRepository<T>.RemoveAsync(int id)
        {
            return RemoveAsync(id).ContinueWith(task =>
            {
                var result = task.Result;
                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = default
                };
            });
        }
    }
}
