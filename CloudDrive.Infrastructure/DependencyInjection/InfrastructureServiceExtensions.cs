using CloudDrive.Application.Interfaces;
using CloudDrive.Application.Services;
using CloudDrive.Application.Validators;
using CloudDrive.Common.JWT;
using FluentValidation;
using CloudDrive.Domain.Entities;
using CloudDrive.Domain.Interfaces;
using CloudDrive.Domain.RepositoryInterfaces;
using CloudDrive.Infrastructure.EventHandlers;
using CloudDrive.Infrastructure.Repositories;
using CloudDrive.Infrastructure.Services;
using CloudDrive.Infrastructure.Storage;
using CloudDrive.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudDrive.Infrastructure.DependencyInjection
{
    /// <summary>
    /// 基础设施层DI注册扩展
    /// </summary>
    public static class InfrastructureServiceExtensions
    {
        /// <summary>
        /// 注册基础设施层服务
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 数据库
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("未配置数据库连接字符串 'DefaultConnection'");

            services.AddDbContext<CloudDriveDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Identity
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<CloudDriveDbContext>()
            .AddDefaultTokenProviders();

            // 存储配置
            services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

            // 仓储
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IShareLinkRepository, ShareLinkRepository>();
            services.AddScoped<IChunkUploadRepository, ChunkUploadRepository>();

            // 工作单元
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            // 存储提供者（根据配置选择）
            var storageProvider = configuration.GetSection(StorageOptions.SectionName)
                .GetValue<string>("Provider") ?? "Local";

            if (storageProvider.Equals("Oss", StringComparison.OrdinalIgnoreCase))
            {
                services.AddScoped<IStorageProvider, OssStorageProvider>();
            }
            else
            {
                services.AddScoped<IStorageProvider, LocalStorageProvider>();
            }

            // 外部服务
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<INotificationService, NotificationService>();

            // MediatR（扫描 Infrastructure 程序集以注册事件处理器）
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(InfrastructureServiceExtensions).Assembly);
            });

            // JWT
            var jwtSection = configuration.GetSection("JWT");
            var jwtOptions = jwtSection.Get<JWTOptions>()
                ?? throw new InvalidOperationException("未配置 JWT 选项");

            services.AddSingleton(jwtOptions);
            services.AddSingleton<ITokenService, TokenService>();
            services.AddJWTAuthentication(jwtOptions);

            return services;
        }

        /// <summary>
        /// 注册应用层服务
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<FileDeduplicationService>();
            services.AddScoped<QuotaService>();
            services.AddScoped<ShareLinkAccessService>();
            services.AddScoped<FileUploadValidator>();

            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IShareService, ShareService>();

            // FluentValidation — 自动注册 Application 层所有 AbstractValidator<T>
            services.AddValidatorsFromAssembly(typeof(UploadFileCommandValidator).Assembly);

            return services;
        }
    }
}
