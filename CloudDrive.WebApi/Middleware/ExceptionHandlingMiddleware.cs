using CloudDrive.WebApi.Models;
using System.Net;
using System.Text.Json;

namespace CloudDrive.WebApi.Middleware
{
    /// <summary>
    /// 全局异常处理中间件
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                UnauthorizedAccessException => (HttpStatusCode.Forbidden, exception.Message),
                InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
                FileNotFoundException => (HttpStatusCode.NotFound, "资源未找到"),
                KeyNotFoundException => (HttpStatusCode.NotFound, "资源未找到"),
                _ => (HttpStatusCode.InternalServerError, "服务器内部错误")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "未处理的异常：{Message}", exception.Message);
            }
            else
            {
                _logger.LogWarning("业务异常：{StatusCode} - {Message}", (int)statusCode, message);
            }

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse.Fail(message, (int)statusCode);
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
