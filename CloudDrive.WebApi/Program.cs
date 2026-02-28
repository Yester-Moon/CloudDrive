using CloudDrive.Infrastructure.DependencyInjection;
using CloudDrive.WebApi.Filters;
using CloudDrive.WebApi.Middleware;
using CloudDrive.WebApi.Validators;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

// === Bootstrap Serilog（在 Host 构建之前就能捕获启动异常） ===
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{

var builder = WebApplication.CreateBuilder(args);

// 用 Serilog 完全替换默认日志
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "logs/clouddrive-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}{NewLine}  {Message:lj}{NewLine}{Exception}");

    // Seq（可选，仅配置了 ServerUrl 时启用）
    var seqUrl = context.Configuration["Serilog:Seq:ServerUrl"];
    if (!string.IsNullOrWhiteSpace(seqUrl))
    {
        configuration.WriteTo.Seq(seqUrl);
    }

    // Elasticsearch（可选，仅配置了 NodeUris 时启用）
    var esUri = context.Configuration["Serilog:Elasticsearch:NodeUris"];
    if (!string.IsNullOrWhiteSpace(esUri))
    {
        configuration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esUri))
        {
            AutoRegisterTemplate = true,
            IndexFormat = "clouddrive-logs-{0:yyyy.MM.dd}",
            MinimumLogEventLevel = LogEventLevel.Information
        });
    }
});

// === Services Registration ===

builder.Services.AddControllers(options =>
    {
        // 全局验证过滤器 — 自动对 [FromBody] 参数执行 FluentValidation
        options.Filters.Add<ValidationActionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// FluentValidation — 注册 WebApi 层请求模型验证器
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CloudDrive API",
        Version = "v1",
        Description = "CloudDrive 云盘系统 API 文档"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "请输入JWT Token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Infrastructure services (DbContext, Identity, repos, storage, JWT, MediatR, etc.)
builder.Services.AddInfrastructureServices(builder.Configuration);

// 数据库健康检查（SqlServer HealthChecks 在 WebApi 层注册以避免与 EF Core 冲突）
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString, name: "dbHealthCheck", tags: ["db", "sql"]);

// Application services (FileService, UserService, ShareService, etc.)
builder.Services.AddApplicationServices();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// === Middleware Pipeline ===

// Global exception handler (first in pipeline)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Serilog 结构化请求日志（替代 RequestLoggingMiddleware 或与之共存）
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    options.GetLevel = (httpContext, elapsed, ex) =>
        ex != null ? LogEventLevel.Error
        : httpContext.Response.StatusCode >= 500 ? LogEventLevel.Error
        : elapsed > 3000 ? LogEventLevel.Warning
        : LogEventLevel.Information;
});

// Request logging
app.UseMiddleware<RequestLoggingMiddleware>();

// Rate limiting
app.UseMiddleware<RateLimitingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CloudDrive API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 健康检查端点
app.MapHealthChecks("/health");

app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "应用程序启动失败");
}
finally
{
    Log.CloseAndFlush();
}
