using SIGEBI.Web.InterfacesApi;
using SIGEBI.Web.Models;
using SIGEBI.Web.Models.LibroApi;
using SIGEBI.Web.Refactory;

namespace SIGEBI.Web.ServiciosApi
{
    public class LibroApiService : ILibroApiService
    {
        private readonly IApiClient _api;

        public LibroApiService(IApiClient api)
        {
            _api = api;
        }

        public async Task<IEnumerable<LibroApiModel>> GetAllAsync()
        {
            var r = await _api.GetAsync<IEnumerable<LibroApiModel>>("Libro/todos");
            return r.Data ?? new List<LibroApiModel>();
        }

        public async Task<LibroApiModel?> GetByIdAsync(int id)
        {
            var r = await _api.GetAsync<ApiResponse<LibroApiModel>>($"Libro/{id}");
            return r.Data?.Data;
        }

        public Task<ApiResponse<object>> CreateAsync(LibroApiCreateModel model)
            => _api.PostAsync<object>("Libro/registrar", model);

        public Task<ApiResponse<object>> UpdateAsync(LibroApiUpdateModel model)
            => _api.PutAsync<object>("Libro/modificar", model);

        public Task<ApiResponse<object>> DeleteAsync(int id)
            => _api.DeleteAsync<object>($"Libro/{id}");

        public async Task<IEnumerable<LibroApiModel>> BuscarPorTituloAsync(string titulo)
        {
            var r = await _api.GetAsync<IEnumerable<LibroApiModel>>($"Libro/titulo/{titulo}");
            return r.Data ?? new List<LibroApiModel>();
        }

        public async Task<IEnumerable<LibroApiModel>> BuscarPorAutorAsync(string autor)
        {
            var r = await _api.GetAsync<IEnumerable<LibroApiModel>>($"Libro/autor/{autor}");
            return r.Data ?? new List<LibroApiModel>();
        }

        public async Task<IEnumerable<LibroApiModel>> FiltrarAsync(string? titulo, string? autor, string? categoria, int? anio, string? estado)
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(titulo)) query.Add($"titulo={titulo}");
            if (!string.IsNullOrWhiteSpace(autor)) query.Add($"autor={autor}");
            if (!string.IsNullOrWhiteSpace(categoria)) query.Add($"categoria={categoria}");
            if (anio.HasValue) query.Add($"anio={anio}");
            if (!string.IsNullOrWhiteSpace(estado)) query.Add($"estado={estado}");

            var url = "Libro/filtrar";

            if (query.Any()) url += "?" + string.Join("&", query);

            var r = await _api.GetAsync<IEnumerable<LibroApiModel>>(url);
            return r.Data ?? new List<LibroApiModel>();
        }

        public Task<ApiResponse<object>> CambiarEstadoAsync(int id, string estado)
            => _api.PutAsync<object>($"Libro/estado/{id}", estado);
    }
}
