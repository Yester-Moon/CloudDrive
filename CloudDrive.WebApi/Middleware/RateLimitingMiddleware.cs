using CloudDrive.WebApi.Models;
using System.Collections.Concurrent;
using System.Text.Json;

namespace CloudDrive.WebApi.Middleware
{
    /// <summary>
    /// 简单限流中间件（基于IP的滑动窗口）
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly int _maxRequests;
        private readonly TimeSpan _window;
        private static readonly ConcurrentDictionary<string, SlidingWindow> _clients = new();

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger,
            int maxRequests = 100,
            int windowSeconds = 60)
        {
            _next = next;
            _logger = logger;
            _maxRequests = maxRequests;
            _window = TimeSpan.FromSeconds(windowSeconds);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var window = _clients.GetOrAdd(clientIp, _ => new SlidingWindow(_window));

            if (!window.TryAcquire(_maxRequests))
            {
                _logger.LogWarning("IP {ClientIp} 请求频率超限", clientIp);

                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers.RetryAfter = "60";

                var response = ApiResponse.Fail("请求过于频繁，请稍后再试", 429);
                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                await context.Response.WriteAsync(json);
                return;
            }

            await _next(context);
        }

        private class SlidingWindow
        {
            private readonly TimeSpan _window;
            private readonly Queue<DateTime> _timestamps = new();
            private readonly object _lock = new();

            public SlidingWindow(TimeSpan window)
            {
                _window = window;
            }

            public bool TryAcquire(int maxRequests)
            {
                lock (_lock)
                {
                    var now = DateTime.UtcNow;
                    var threshold = now - _window;

                    // 清除过期记录
                    while (_timestamps.Count > 0 && _timestamps.Peek() < threshold)
                    {
                        _timestamps.Dequeue();
                    }

                    if (_timestamps.Count >= maxRequests)
                        return false;

                    _timestamps.Enqueue(now);
                    return true;
                }
            }
        }
    }
}
