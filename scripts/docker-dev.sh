#!/bin/bash

# 颜色定义
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${GREEN}=== UnifiedAPI Platform Docker Manager ===${NC}"

# 检查 Docker 是否运行
if ! docker info > /dev/null 2>&1; then
    echo -e "${RED}Docker 未运行，请先启动 Docker${NC}"
    exit 1
fi

case "$1" in
    start)
        echo -e "${YELLOW}启动所有服务...${NC}"
        docker-compose up -d
        echo -e "${GREEN}所有服务已启动！${NC}"
        echo ""
        echo "服务访问地址："
        echo "  - PostgreSQL: localhost:5432"
        echo "  - Redis: localhost:6379"
        echo "  - RabbitMQ: http://localhost:15672"
        echo "  - Seq: http://localhost:5341"
        echo "  - pgAdmin: http://localhost:5050"
        echo "  - Redis Commander: http://localhost:8081"
        ;;

    stop)
        echo -e "${YELLOW}停止所有服务...${NC}"
        docker-compose down
        echo -e "${GREEN}所有服务已停止！${NC}"
        ;;

    restart)
        echo -e "${YELLOW}重启所有服务...${NC}"
        docker-compose restart
        echo -e "${GREEN}所有服务已重启！${NC}"
        ;;

    logs)
        if [ -z "$2" ]; then
            docker-compose logs -f
        else
            docker-compose logs -f $2
        fi
        ;;

    status)
        echo -e "${YELLOW}服务状态：${NC}"
        docker-compose ps
        ;;

    clean)
        echo -e "${RED}警告：这将删除所有数据卷！${NC}"
        read -p "确定要继续吗？(y/N) " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            docker-compose down -v
            echo -e "${GREEN}所有服务和数据已清理！${NC}"
        else
            echo "操作已取消"
        fi
        ;;

    backup)
        echo -e "${YELLOW}备份 PostgreSQL 数据库...${NC}"
        BACKUP_FILE="backup_$(date +%Y%m%d_%H%M%S).sql"
        docker exec unifiedapi-postgres pg_dump -U postgres UnifiedApiPlatform > $BACKUP_FILE
        echo -e "${GREEN}数据库已备份到: $BACKUP_FILE${NC}"
        ;;

    restore)
        if [ -z "$2" ]; then
            echo -e "${RED}请指定备份文件！${NC}"
            echo "用法: $0 restore backup_file.sql"
            exit 1
        fi
        echo -e "${YELLOW}恢复数据库从: $2${NC}"
        cat $2 | docker exec -i unifiedapi-postgres psql -U postgres UnifiedApiPlatform
        echo -e "${GREEN}数据库已恢复！${NC}"
        ;;

    *)
        echo "用法: $0 {start|stop|restart|logs|status|clean|backup|restore}"
        echo ""
        echo "命令说明："
        echo "  start   - 启动所有服务"
        echo "  stop    - 停止所有服务"
        echo "  restart - 重启所有服务"
        echo "  logs    - 查看日志 (可选：指定服务名)"
        echo "  status  - 查看服务状态"
        echo "  clean   - 清理所有服务和数据卷"
        echo "  backup  - 备份 PostgreSQL 数据库"
        echo "  restore - 恢复 PostgreSQL 数据库"
        exit 1
        ;;
esac
