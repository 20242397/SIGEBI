using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Usuario;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;

namespace SIGEBI.Application.Interfaces
{
    public interface IUsuarioService
    {
       
        Task<ServiceResult<T>> RegistrarUsuarioAsync<T>(UsuarioCreateDto dto);
        Task<ServiceResult<T>> EditarUsuarioAsync<T>(UsuarioUpdateDto dto);
        Task<ServiceResult<T>> AsignarRolAsync<T>(int id, string rol);
        Task<ServiceResult<T>> CambiarEstadoAsync<T>(int id, bool activo);
        Task<ServiceResult<T>> ObtenerPorEmailAsync<T>(string email);
        Task<ServiceResult<T>> ObtenerTodosAsync<T>();
        Task<OperationResult<Usuario>> RemoveAsync(int id);
        Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id);

    }
}

