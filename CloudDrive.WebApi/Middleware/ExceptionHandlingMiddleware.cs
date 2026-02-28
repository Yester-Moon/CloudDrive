using CloudDrive.Common.Exceptions;
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
            int businessErrorCode = 0;

            var (statusCode, message) = exception switch
            {
                BusinessException bex => (MapBusinessErrorCode(bex.ErrorCode, out businessErrorCode), bex.Message),
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

            var responseCode = businessErrorCode > 0 ? businessErrorCode : (int)statusCode;
            var response = ApiResponse.Fail(message, responseCode);
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        /// <summary>
        /// 将业务错误码映射到 HTTP 状态码
        /// </summary>
        private static HttpStatusCode MapBusinessErrorCode(int errorCode, out int businessCode)
        {
            businessCode = errorCode;
            return errorCode switch
            {
                >= 40100 and < 40200 => HttpStatusCode.Unauthorized,
                >= 40300 and < 40400 => HttpStatusCode.Forbidden,
                >= 40400 and < 40500 => HttpStatusCode.NotFound,
                >= 40000 and < 40100 or >= 41000 and < 43000 => HttpStatusCode.BadRequest,
                >= 50000 => HttpStatusCode.InternalServerError,
                _ => HttpStatusCode.BadRequest
            };
        }
    }
}
