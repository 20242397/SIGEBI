using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;

namespace SIGEBI.Domain.Repository
{
    public interface IUsuarioRepository
    {
        Task<OperationResult<IEnumerable<Usuario>>> GetAllAsync();
        Task<OperationResult<bool>> AddAsync(Usuario usuario);
    }
}