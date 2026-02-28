创建的文件
1. .dockerignore
排除 bin/、obj/、.vs/、docs/ 等构建产物，减小 Docker 上下文大小。
2. Dockerfile — 多阶段构建
阶段	基础镜像	作用
build	dotnet/sdk:8.0	先单独 COPY *.csproj → dotnet restore（利用层缓存），再 COPY 全部源码 → dotnet publish
runtime	dotnet/aspnet:8.0	仅包含运行时，创建 /app/uploads + /app/logs 目录，以非 root 用户 appuser 运行
关键配置：
•	ASPNETCORE_ENVIRONMENT=Docker → 自动加载 appsettings.Docker.json
•	EXPOSE 8080，ASPNETCORE_URLS=http://+:8080
3. docker-compose.yml
┌──────────────┐       ┌──────────────┐
│  sqlserver    │◄──────│     app      │
│  (1433:1433) │ TCP   │  (8080:8080) │
│  SA密码安全   │       │  depends_on  │
│  健康检查     │       │  health条件  │
└──────────────┘       └──────────────┘
       │                      │
  sqlserver-data         app-uploads
  (持久化卷)            app-logs

  服务	镜像	端口	说明
sqlserver	mssql/server:2022-latest	1433	SA 密码 CloudDrive@2024!，healthcheck 确保就绪
app	本地 Dockerfile 构建	8080	depends_on: sqlserver (service_healthy)，环境变量覆盖连接串
持久化卷：sqlserver-data（数据库）、app-uploads（文件）、app-logs（日志）
4. appsettings.Docker.json
Docker 环境专用配置，连接字符串指向 sqlserver 容器名，存储路径指向 /app/uploads。
---
使用方式

# 构建并启动（首次会自动 build 镜像）
docker-compose up -d --build

# 查看日志
docker-compose logs -f app

# 访问 API
curl http://localhost:8080/health
curl http://localhost:8080/swagger

# 停止
docker-compose down

# 停止并清理数据卷
docker-compose down -v