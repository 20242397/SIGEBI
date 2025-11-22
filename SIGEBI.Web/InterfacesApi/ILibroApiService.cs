using SIGEBI.Web.Models;
using SIGEBI.Web.Models.LibroApi;

namespace SIGEBI.Web.InterfacesApi
{
    public interface ILibroApiService
    {
            Task<IEnumerable<LibroApiModel>> GetAllAsync();
            Task<LibroApiModel?> GetByIdAsync(int id);
            Task<ApiResponse<object>> CreateAsync(LibroApiCreateModel model);
            Task<ApiResponse<object>> UpdateAsync(LibroApiUpdateModel model);
            Task<ApiResponse<object>> DeleteAsync(int id);

            Task<IEnumerable<LibroApiModel>> BuscarPorTituloAsync(string titulo);
            Task<IEnumerable<LibroApiModel>> BuscarPorAutorAsync(string autor);
            Task<IEnumerable<LibroApiModel>> FiltrarAsync(string? titulo, string? autor, string? categoria, int? anio, string? estado);

            Task<ApiResponse<object>> CambiarEstadoAsync(int id, string estado);
       
    }

}
