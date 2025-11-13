using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Repositories.Configuration.ISecurity
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {

        Task<OperationResult<Usuario>> AddAsync(Usuario usuario);


        Task<OperationResult<IEnumerable<Usuario>>> GetAllAsync();


        Task<OperationResult<Usuario>> GetByIdAsync(int id);


        Task<OperationResult<Usuario>> GetByEmailAsync(string email);


        Task<OperationResult<Usuario>> UpdateAsync(Usuario usuario);


        Task<OperationResult<bool>> CambiarEstadoAsync(int id, bool activo);


        Task<OperationResult<bool>> AsignarRolAsync(int id, string rol);

        Task<OperationResult<Usuario>> ObtenerPorEmailYPasswordAsync(string email, string password);

    }
}