-- UnifiedApiPlatform 数据库初始化脚本
-- 此脚本会在 PostgreSQL 容器首次启动时自动执行

-- 创建扩展
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm"; -- 支持模糊搜索

-- 设置时区
SET timezone = 'UTC';

-- 显示数据库信息
\echo '======================================'
\echo 'UnifiedApiPlatform Database Initialized'
\echo '======================================'
\echo 'Database: unifiedapi_db'
\echo 'User: postgres'
\echo 'Extensions: uuid-ossp, pg_trgm'
\echo 'Timezone: UTC'
\echo '======================================'
