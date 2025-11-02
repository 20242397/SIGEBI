

namespace SIGEBI.Domain.Base
{
    public  class OperationResult<T>
    {
        public OperationResult() { 
        
            this.Success = true;
        }
        public string Message { get; set; } 
        public bool Success { get; set; }
        public T? Data { get; set; }


    }
}
