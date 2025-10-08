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
                var result = await action();

                if (result.Success)
                    return ServiceResult<T>.Ok(result.Data!, result.Message);

                return ServiceResult<T>.Fail(result.Message ?? "Ocurrió un error");
            }
            catch (Exception ex)
            {
                return ServiceResult<T>.Fail($"Excepción: {ex.Message}");
            }
        }

        protected ServiceResult<T> SafeReturn<T>(OperationResult<T>? result)
        {
            if (result == null)
                return ServiceResult<T>.Fail("Error interno: resultado nulo.");

            return FromOperationResult(result);
        }
    }
}
