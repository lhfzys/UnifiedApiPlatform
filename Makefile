.PHONY: help up down logs clean build migrate seed ps restart shell-api shell-db

# 默认目标：显示帮助
help:
	@echo "UnifiedApiPlatform - Makefile 命令帮助"
	@echo ""
	@echo "使用方法: make [命令]"
	@echo ""
	@echo "可用命令："
	@echo "  up              - 启动所有服务"
	@echo "  down            - 停止所有服务"
	@echo "  logs            - 查看所有服务日志"
	@echo "  logs-api        - 查看 API 服务日志"
	@echo "  logs-postgres   - 查看 PostgreSQL 日志"
	@echo "  logs-redis      - 查看 Redis 日志"
	@echo "  ps              - 查看服务运行状态"
	@echo "  restart         - 重启所有服务"
	@echo "  restart-api     - 重启 API 服务"
	@echo "  clean           - 清理容器和卷"
	@echo "  build           - 构建镜像"
	@echo "  migrate         - 执行数据库迁移"
	@echo "  seed            - 执行种子数据"
	@echo "  shell-api       - 进入 API 容器"
	@echo "  shell-db        - 进入数据库容器"
	@echo ""

# 启动所有服务
up:
	docker-compose up -d
	@echo "✅ 所有服务已启动"
	@echo "📊 查看状态: make ps"
	@echo "📝 查看日志: make logs"

# 停止所有服务
down:
	docker-compose down
	@echo "✅ 所有服务已停止"

# 查看所有服务日志
logs:
	docker-compose logs -f

# 查看 API 服务日志
logs-api:
	docker-compose logs -f api

# 查看 PostgreSQL 日志
logs-postgres:
	docker-compose logs -f postgres

# 查看 Redis 日志
logs-redis:
	docker-compose logs -f redis

# 查看服务运行状态
ps:
	docker-compose ps

# 重启所有服务
restart:
	docker-compose restart
	@echo "✅ 所有服务已重启"

# 重启 API 服务
restart-api:
	docker-compose restart api
	@echo "✅ API 服务已重启"

# 清理容器和卷
clean:
	@echo "⚠️  警告：此操作将删除所有容器和数据卷"
	@read -p "确认继续? [y/N] " -n 1 -r; \
	echo; \
	if [[ $$REPLY =~ ^[Yy]$$ ]]; then \
		docker-compose down -v; \
		docker system prune -f; \
		echo "✅ 清理完成"; \
	else \
		echo "❌ 取消清理"; \
	fi

# 构建镜像
build:
	docker-compose build
	@echo "✅ 镜像构建完成"

# 数据库迁移（需要 API 容器运行）
migrate:
	docker-compose exec api dotnet ef database update \
		--project /src/UnifiedApiPlatform.Infrastructure \
		--startup-project /src/UnifiedApiPlatform.Api
	@echo "✅ 数据库迁移完成"

# 添加迁移
add-migration:
	@read -p "请输入迁移名称: " name; \
	cd src/UnifiedApiPlatform.Infrastructure && \
	dotnet ef migrations add $$name --startup-project ../UnifiedApiPlatform.Api
	@echo "✅ 迁移已创建"

# 执行种子数据
seed:
	docker-compose exec api dotnet run --seed
	@echo "✅ 种子数据已导入"

# 进入 API 容器
shell-api:
	docker-compose exec api sh

# 进入数据库容器
shell-db:
	docker-compose exec postgres psql -U postgres -d unifiedapi_db

# 查看数据库表
db-tables:
	docker-compose exec postgres psql -U postgres -d unifiedapi_db -c "\dt"

# 重启 PostgreSQL
restart-postgres:
	docker-compose restart postgres
	@echo "✅ PostgreSQL 已重启"

# 重启 Redis
restart-redis:
	docker-compose restart redis
	@echo "✅ Redis 已重启"

# 重启 RabbitMQ
restart-rabbitmq:
	docker-compose restart rabbitmq
	@echo "✅ RabbitMQ 已重启"

# 查看 Docker 容器资源使用
stats:
	docker stats --format "table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}"

# 查看所有容器日志（最近100行）
logs-tail:
	docker-compose logs --tail=100
