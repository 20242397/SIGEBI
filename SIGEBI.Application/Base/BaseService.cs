using SIGEBI.Application.Base;
using SIGEBI.Domain.Base;

namespace SIGEBI.Application.Services
{
    public abstract class BaseService
    {
        protected ServiceResult<T> FromOperationResult<T>(OperationResult<T> operationResult)
        {
            return new ServiceResult<T>
            {
                Success = operationResult.Success,
                Message = operationResult.Message,
                Data = operationResult.Data
            };
        }

        protected async Task<ServiceResult<T>> ExecuteAsync<T>(Func<Task<OperationResult<T>>> action)
        {
            try
            {
                if (action == null)
                    return ServiceResult<T>.Fail("Error interno: la acción no puede ser nula.");

                var result = await action();

                if (result == null)
                    return ServiceResult<T>.Fail("Error interno: el resultado fue nulo.");

                if (result.Success)
                    return ServiceResult<T>.Ok(result.Data!, result.Message);

                return ServiceResult<T>.Fail(result.Message ?? "Ocurrió un error durante la operación.");
            }
            catch (NullReferenceException)
            {
                return ServiceResult<T>.Fail("Excepción: Referencia a objeto no establecida.");
            }
            catch (ArgumentNullException ex)
            {
                return ServiceResult<T>.Fail($"Excepción: el parámetro '{ex.ParamName}' no puede ser nulo.");
            }
            catch (InvalidOperationException ex)
            {
                return ServiceResult<T>.Fail($"Excepción: operación no válida. Detalles: {ex.Message}");
            }
            catch (Exception ex)
            {
               
                return ServiceResult<T>.Fail($"Excepción inesperada: {ex.Message}");
            }
        }

        protected ServiceResult<T> SafeReturn<T>(OperationResult<T>? result)
        {
            if (result == null)
                return ServiceResult<T>.Fail("Error interno: el resultado no puede ser nulo.");

            return FromOperationResult(result);
        }

        
        protected OperationResult<T> Ok<T>(T? data = default, string? message = null)
        {
            return new OperationResult<T>
            {
                Success = true,
                Data = data,
                Message = message ?? "Operación exitosa."
            };
        }

        protected OperationResult<T> Fail<T>(string message)
        {
            return new OperationResult<T>
            {
                Success = false,
                Message = message
            };
        }

    }
}