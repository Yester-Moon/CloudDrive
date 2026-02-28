using CloudDrive.WebApi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CloudDrive.WebApi.Filters
{
    /// <summary>
    /// 全局验证过滤器 — 自动对 Action 参数执行 FluentValidation 验证
    /// 如果 DI 中注册了对应 IValidator&lt;T&gt;，则在 Action 执行前自动校验，
    /// 校验失败返回 400 + 结构化错误信息。
    /// </summary>
    public class ValidationActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationActionFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null)
                    continue;

                var argumentType = argument.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator is null)
                    continue;

                var validationContext = new ValidationContext<object>(argument);
                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray());

                    var response = ApiResponse.Fail("请求参数验证失败", 400);

                    context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(
                        new
                        {
                            response.Code,
                            response.Message,
                            response.Success,
                            Errors = errors
                        });
                    return;
                }
            }

            await next();
        }
    }
}
