
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.Entitines.Configuration.Security;

namespace SIGEBI.Application.Repositories
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {

        Task<Usuario?> GetByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetUsuariosActivosAsync();
        Task<IEnumerable<Usuario>> GetUsuariosInactivosAsync();
        Task<bool> CambiarRolAsync(int usuarioId, string nuevoRol);
    }
}
