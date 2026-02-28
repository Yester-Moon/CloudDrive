# ============================================================
# Stage 1: Build — 还原 + 编译 + 发布
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) 先复制所有 csproj / slnx，利用 Docker 层缓存加速 restore
COPY CloudDrive.slnx ./
COPY CloudDrive.Domain/CloudDrive.Domain.csproj              CloudDrive.Domain/
COPY CloudDrive.Common/CloudDrive.Common.csproj              CloudDrive.Common/
COPY CloudDrive.Application/CloudDrive.Application.csproj    CloudDrive.Application/
COPY CloudDrive.Infrastructure/CloudDrive.Infrastructure.csproj CloudDrive.Infrastructure/
COPY CloudDrive.WebApi/CloudDrive.WebApi.csproj              CloudDrive.WebApi/

RUN dotnet restore CloudDrive.WebApi/CloudDrive.WebApi.csproj

# 2) 复制全部源码并发布
COPY . .
RUN dotnet publish CloudDrive.WebApi/CloudDrive.WebApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ============================================================
# Stage 2: Runtime — 仅包含运行时，镜像最小化
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 创建上传目录与日志目录
RUN mkdir -p /app/uploads /app/logs

# 非 root 用户运行（安全最佳实践）
RUN adduser --disabled-password --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser

COPY --from=build /app/publish .

# 默认端口
EXPOSE 8080

# ASP.NET Core 监听配置
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "CloudDrive.WebApi.dll"]
