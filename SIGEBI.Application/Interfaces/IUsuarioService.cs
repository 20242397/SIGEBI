using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Usuario;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using System.Threading.Tasks;

namespace SIGEBI.Application.Interfaces
{
    public interface IUsuarioService
    {
        // RF2.1 - Registrar usuario
        Task<ServiceResult<T>> RegistrarUsuarioAsync<T>(UsuarioCreateDto dto);

        // RF2.3 - Editar información
        Task<ServiceResult<T>> EditarUsuarioAsync<T>(UsuarioUpdateDto dto);

        // RF2.2 - Asignar rol
        Task<ServiceResult<T>> AsignarRolAsync<T>(int id, string rol);

        // RF2.4 - Activar / desactivar usuario
        Task<ServiceResult<T>> CambiarEstadoAsync<T>(int id, bool activo);

        // RF2.5 - Consultas
        Task<ServiceResult<T>> ObtenerPorEmailAsync<T>(string email);
        Task<ServiceResult<T>> ObtenerTodosAsync<T>();

        Task<OperationResult<Usuario>> RemoveAsync(int id);
    }
}

