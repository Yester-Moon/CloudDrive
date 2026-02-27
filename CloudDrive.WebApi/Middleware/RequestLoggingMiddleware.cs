using System.Diagnostics;

namespace CloudDrive.WebApi.Middleware
{
    /// <summary>
    /// 请求日志中间件
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestPath = context.Request.Path;
            var method = context.Request.Method;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var statusCode = context.Response.StatusCode;
                var elapsed = stopwatch.ElapsedMilliseconds;

                if (elapsed > 3000)
                {
                    _logger.LogWarning(
                        "慢请求 {Method} {Path} 响应 {StatusCode} 耗时 {ElapsedMs}ms",
                        method, requestPath, statusCode, elapsed);
                }
                else
                {
                    _logger.LogInformation(
                        "{Method} {Path} 响应 {StatusCode} 耗时 {ElapsedMs}ms",
                        method, requestPath, statusCode, elapsed);
                }
            }
        }
    }
}
