# UnifiedApiPlatform

基于 .NET 9.0 的多租户 RBAC 企业级 API 平台

## 🚀 技术栈

- **.NET 9.0** - 核心框架
- **FastEndpoints** - 高性能 API 框架
- **PostgreSQL** - 主数据库
- **Entity Framework Core** - ORM
- **MediatR** - CQRS 实现
- **FluentValidation** - 数据验证
- **Redis** - 分布式缓存
- **RabbitMQ** - 消息队列
- **Hangfire** - 后台任务
- **SignalR** - 实时通信
- **Serilog + Seq** - 结构化日志
- **OpenTelemetry** - 可观测性
- **Docker** - 容器化部署

## 📋 功能模块

- ✅ 多租户管理（数据隔离）
- ✅ 用户管理（CRUD + 数据权限）
- ✅ 角色管理（RBAC）
- ✅ 权限管理（操作级权限）
- ✅ 菜单管理（3级菜单树 + 按钮权限）
- ✅ 组织架构（5级树形结构）
- ✅ 字典管理（系统字典 + 租户自定义）
- ✅ 系统配置（系统级 + 租户级）
- ✅ 文件管理（上传/下载 + 附件关联）
- ✅ 通知系统（SignalR 实时推送 + 邮件）
- ✅ 公告系统（系统公告 + 租户公告）
- ✅ 审计日志（数据变更追踪）
- ✅ 操作日志（API 调用记录）
- ✅ 登录日志（安全审计）
- ✅ 导入任务（Excel 批量导入 + 进度追踪）
- ✅ 定时任务（Hangfire 配置管理）

## 🛠️ 开发环境要求

- .NET 9.0 SDK
- Docker Desktop
- JetBrains Rider / Visual Studio 2022
- Git

## 🚀 快速开始

### 1. 克隆项目

```bash
git clone <repository-url>
cd UnifiedApiPlatform
```

### 2. 配置环境变量

```bash
# 复制环境变量模板
cp .env.example .env

# 编辑 .env 文件，修改必要的配置
# 特别注意修改：
# - JWT_SECRET_KEY（生产环境必须使用强密钥）
# - 数据库密码
# - SMTP 配置（如需邮件功能）
```

### 3. 启动基础服务

```bash
# 启动所有 Docker 服务
make up

# 或使用 docker-compose
docker-compose up -d

# 查看服务状态
make ps
```

### 4. 运行项目（开发环境）

```bash
# 使用 Rider 打开解决方案
# 或使用命令行
cd src/UnifiedApiPlatform.Api
dotnet run
```

### 5. 访问应用

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Seq 日志**: http://localhost:5341
- **RabbitMQ 管理**: http://localhost:15672 (guest/guest)
- **MinIO 控制台**: http://localhost:9001 (minioadmin/minioadmin)

## 📦 项目结构

```
UnifiedApiPlatform/
├── src/
│   ├── UnifiedApiPlatform.Api/           # API 层（FastEndpoints）
│   ├── UnifiedApiPlatform.Application/   # 应用层（CQRS、DTOs）
│   ├── UnifiedApiPlatform.Domain/        # 领域层（实体、值对象）
│   ├── UnifiedApiPlatform.Infrastructure/# 基础设施层（EF Core、外部服务）
│   └── UnifiedApiPlatform.Shared/        # 共享层（常量、扩展）
├── seed-data/                            # 种子数据（JSON）
├── logs/                                 # 日志输出
├── uploads/                              # 文件上传
├── docker-compose.yml                    # Docker 编排
├── Makefile                              # 便捷命令
└── README.md                             # 项目说明
```

## 🎯 Makefile 命令

```bash
make help          # 显示所有可用命令
make up            # 启动所有服务
make down          # 停止所有服务
make logs          # 查看所有服务日志
make ps            # 查看服务状态
make restart       # 重启所有服务
make clean         # 清理容器和卷（危险操作）
make migrate       # 执行数据库迁移
make seed          # 导入种子数据
make shell-api     # 进入 API 容器
make shell-db      # 进入数据库容器
```

## 🗄️ 数据库迁移

### 创建迁移

```bash
# 方法 1: 使用 Makefile
make add-migration

# 方法 2: 手动执行
cd src/UnifiedApiPlatform.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../UnifiedApiPlatform.Api
```

### 应用迁移

```bash
# 开发环境（本地）
cd src/UnifiedApiPlatform.Infrastructure
dotnet ef database update --startup-project ../UnifiedApiPlatform.Api

# Docker 环境
make migrate
```

### 回滚迁移

```bash
cd src/UnifiedApiPlatform.Infrastructure
dotnet ef database update PreviousMigrationName --startup-project ../UnifiedApiPlatform.Api
```

## 🔐 默认账户

### 超级管理员

- **邮箱**: admin@example.com
- **密码**: Admin@123

⚠️ **重要**：生产环境部署前务必修改默认密码！

## 📖 API 文档

启动项目后访问 Swagger UI：

- 开发环境: http://localhost:5000/swagger
- 生产环境: 默认禁用（可通过配置启用）

## 🧪 测试

```bash
# 运行所有测试
dotnet test

# 运行特定测试项目
dotnet test tests/UnifiedApiPlatform.UnitTests
```

## 📊 日志查看

### Seq（推荐）

访问 http://localhost:5341 查看结构化日志

### 文件日志

日志文件位于 `logs/` 目录：

- `logs/log-YYYYMMDD.txt` - 所有日志
- `logs/errors/error-YYYYMMDD.txt` - 仅错误日志

### Docker 日志

```bash
# 所有服务
make logs

# 特定服务
make logs-api
make logs-postgres
make logs-redis
```

## 🐳 Docker 部署

### 构建镜像

```bash
make build
```

### 生产环境部署

```bash
# 1. 修改 .env 文件为生产配置
# 2. 启动服务
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## 🔧 故障排查

### 数据库连接失败

```bash
# 检查 PostgreSQL 容器状态
make ps

# 查看 PostgreSQL 日志
make logs-postgres

# 重启 PostgreSQL
make restart-postgres
```

### Redis 连接失败

```bash
# 检查 Redis 容器状态
docker-compose ps redis

# 测试 Redis 连接
docker-compose exec redis redis-cli ping
# 应返回: PONG
```

### 清理并重新开始

```bash
# 警告：会删除所有数据
make clean
make up
```

## 📝 开发规范

### Git 提交规范

```
feat: 新功能
fix: 修复 bug
docs: 文档更新
style: 代码格式调整
refactor: 重构
perf: 性能优化
test: 测试相关
chore: 构建/工具链相关
```

### 代码风格

项目使用 `.editorconfig` 统一代码风格，请确保 IDE 已启用 EditorConfig 支持。

## 🤝 贡献指南

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'feat: Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 提交 Pull Request

## 📄 许可证

[MIT License](LICENSE)

## 📞 联系方式

- 项目维护者: [Your Name]
- Email: support@yourcompany.com
- 项目地址: [GitHub Repository]

## 🙏 致谢

感谢所有开源项目的贡献者！

---

**开始构建您的企业级 API 平台！** 🚀
