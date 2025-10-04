
using SIGEBI.Application.Base;
using static SIGEBI.Application.Dtos.Models.Configuration.Usuario.UsuarioGetModel;

namespace SIGEBI.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<ServiceResult<T>> RegistrarUsuarioAsync<T>(UsuarioCreateDto dto);
        Task<ServiceResult<T>> EditarUsuarioAsync<T>(UsuarioUpdateDto dto);
        Task<ServiceResult<T>> CambiarEstadoAsync<T>(int id, bool activo);
        Task<ServiceResult<T>> AsignarRolAsync<T>(int id, string rol);
        Task<ServiceResult<T>> ObtenerPorEmailAsync<T>(string email);
        Task<ServiceResult<T>> ObtenerTodosAsync<T>();
    }
}
