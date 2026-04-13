namespace Dike.Identity.Core.Common
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string? InternalCode { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }


        public static BaseResponse<T> Ok(T data, string message = "Success")
            => new() { Success = true, Data = data, Message = message };

        public static BaseResponse<T> Failure(string internalCode, string message, IDictionary<string, string[]>? errors = null)
            => new() { Success = false, InternalCode = internalCode, Message = message, Errors = errors };

    }
}
