namespace SIGEBI.Web.Refactory
{
    using SIGEBI.Web.Models;
    using System.Net.Http.Json;
    using System.Text.Json;

    public class ApiClient : IApiClient
    {
        private readonly IHttpClientFactory _factory;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClient(IHttpClientFactory factory)
        {
            _factory = factory;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                using var client = _factory.CreateClient("SIGEBIApi");

                var response = await client.GetAsync(endpoint);
                return await DeserializeResponse<T>(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Error("Error de conexión: " + ex.Message);
            }
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                using var client = _factory.CreateClient("SIGEBIApi");

                var response = await client.PostAsJsonAsync(endpoint, data, _jsonOptions);
                return await DeserializeResponse<T>(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Error("Error enviando datos: " + ex.Message);
            }
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                using var client = _factory.CreateClient("SIGEBIApi");

                var response = await client.PutAsJsonAsync(endpoint, data, _jsonOptions);
                return await DeserializeResponse<T>(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Error("Error actualizando datos: " + ex.Message);
            }
        }

        public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
        {
            try
            {
                using var client = _factory.CreateClient("SIGEBIApi");

                var response = await client.DeleteAsync(endpoint);
                return await DeserializeResponse<T>(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Error("Error eliminando recurso: " + ex.Message);
            }
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
