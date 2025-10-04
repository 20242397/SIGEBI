using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using SIGEBI.Persistence.Models;

namespace SIGEBI.Domain.Repository
{
    public interface IUsuarioRepository
    {
        Task<OperationResult<IEnumerable<Usuario>>> GetAllAsync();

        Task<OperationResult<bool>> AddAsync(Usuario usuario);
        Task<OperationResult<bool>> UpdateAsync(Usuario usuario);
        Task<OperationResult<object>> GetByIdAsync(int id);
        Task<OperationResult<object>> GetByEmailAsync(string email);


    }
}