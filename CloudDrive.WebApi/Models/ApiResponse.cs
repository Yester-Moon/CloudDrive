namespace CloudDrive.WebApi.Models
{
    /// <summary>
    /// 统一API响应模型
    /// </summary>
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public int Code { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public static ApiResponse Ok(object? data = null, string? message = null)
            => new() { Success = true, Code = 200, Data = data, Message = message ?? "操作成功" };

        public static ApiResponse Fail(string message, int code = 400)
            => new() { Success = false, Code = code, Message = message };

        public static ApiResponse Error(string message, int code = 500)
            => new() { Success = false, Code = code, Message = message };
    }

    /// <summary>
    /// 泛型统一API响应模型
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public int Code { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public static ApiResponse<T> Ok(T data, string? message = null)
            => new() { Success = true, Code = 200, Data = data, Message = message ?? "操作成功" };

        public static ApiResponse<T> Fail(string message, int code = 400)
            => new() { Success = false, Code = code, Message = message };
    }
}
