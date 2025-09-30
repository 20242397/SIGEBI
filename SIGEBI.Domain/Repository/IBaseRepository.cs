
using SIGEBI.Domain.Base;


namespace SIGEBI.Domain.Repository
{
    public interface IBaseRepository<T> where T : Base.BaseEntity
    {
        Task<OperationResult<T>> GetByIdAsync(int id);
        Task<OperationResult<IEnumerable<T>>> GetAllAsync();
        Task<OperationResult<T>> AddAsync(T entity);
        Task<OperationResult<T>> UpdateAsync(T entity);
        Task<OperationResult<bool>> RemoveAsync(int id);
    }
}
