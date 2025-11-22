namespace SIGEBI.Web.Refactory
{
    using SIGEBI.Web.Models;
    using System.Net.Http.Json;
    using System.Text.Json;

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClient(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("SIGEBIApi");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            HttpResponseMessage response;

            try
            {
                response = await _http.GetAsync(endpoint);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Error("Error de conexión: " + ex.Message);
            }

            return await DeserializeResponse<T>(response);
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            HttpResponseMessage response;

            try
            {
                response = await _http.PostAsJsonAsync(endpoint, data, _jsonOptions);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Error("Error enviando datos: " + ex.Message);
            }

            return await DeserializeResponse<T>(response);
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
        {
            HttpResponseMessage response;

            try
            {
                response = await _http.PutAsJsonAsync(endpoint, data, _jsonOptions);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Error("Error actualizando datos: " + ex.Message);
            }

            return await DeserializeResponse<T>(response);
        }

        public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
        {
            HttpResponseMessage response;

            try
            {
                response = await _http.DeleteAsync(endpoint);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Error("Error eliminando recurso: " + ex.Message);
            }

            return await DeserializeResponse<T>(response);
        }

        private async Task<ApiResponse<T>> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

           
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var apiError = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
                    return ApiResponse<T>.Error(apiError?.Message ?? "Error al procesar la solicitud.");
                }
                catch
                {
                    return ApiResponse<T>.Error("Error HTTP: " + response.StatusCode);
                }
            }

           
            try
            {
                var wrapped = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
                if (wrapped != null && (wrapped.Data != null || wrapped.Success))
                    return wrapped;
            }
            catch { }

           
            try
            {
                var direct = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                if (direct != null)
                    return ApiResponse<T>.Ok(direct);
            }
            catch { }

          
            return ApiResponse<T>.Error("La API devolvió una respuesta inválida.");
        }

    }
}
