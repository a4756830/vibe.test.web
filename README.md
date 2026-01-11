"# vibe.test.web" 

````````

# Vibe.Test.Web

.NET 10 Web API 專案，使用 Entity Framework Core 連接 PostgreSQL 資料庫。

## 專案架構

```
Vibe.Test.Web/          # Web API 層 (Controllers, Program.cs)
Vibe.Test.Servcie/      # 服務層 (Business Logic)
Vibe.Test.Model/        # 資料層 (DbContext, Entities)
Vibe.Test.Libary/       # 共用函式庫
```

### 專案參考關係

```
Vibe.Test.Web
  └─> Vibe.Test.Servcie
        └─> Vibe.Test.Model (DbContext, Entities)
```

## 環境需求

- .NET 10 SDK
- Docker Desktop
- PostgreSQL 16 (透過 Docker Compose)

## 快速開始

### 1. 啟動資料庫

```sh
docker-compose up -d
```

### 2. 執行應用程式

使用 Visual Studio 或 CLI：

```sh
cd Vibe.Test.Web
dotnet run
```

### 3. 開啟 API 文件

- **Scalar UI**: https://localhost:7099/scalar/v1
- **OpenAPI JSON**: https://localhost:7099/openapi/v1.json

## API 端點

| Method | Route | 說明 |
|--------|-------|------|
| GET | /api/users | 取得所有使用者 |
| GET | /api/users/{id} | 取得單一使用者 |
| POST | /api/users | 新增使用者 |
| PUT | /api/users/{id} | 更新使用者 |
| DELETE | /api/users/{id} | 刪除使用者 |

## 資料庫設定

### 連線字串

| 環境 | 設定檔 | Host |
|------|--------|------|
| 本機開發 | `appsettings.json` | `localhost` |
| Docker | `appsettings.Docker.json` | `postgres` |

### 資料庫初始化

初始化 SQL 腳本位於 `docker/init/init.sql`，會在 PostgreSQL 容器首次啟動時自動執行。

## EF Core 指令

### Database First (從資料庫產生 Entity)

```sh
# 套件管理器主控台
Scaffold-DbContext "Host=localhost;Port=5432;Database=vibedb;Username=postgres;Password=postgres123" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Entities -ContextDir Data -Context AppDbContext -Project Vibe.Test.Model -StartupProject Vibe.Test.Web

# .NET CLI
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=vibedb;Username=postgres;Password=postgres123" Npgsql.EntityFrameworkCore.PostgreSQL --output-dir Entities --context-dir Data --context AppDbContext --project Vibe.Test.Model --startup-project Vibe.Test.Web
```

### Migration (Code First)

```sh
# 新增 Migration
dotnet ef migrations add InitialCreate --project Vibe.Test.Model --startup-project Vibe.Test.Web

# 更新資料庫
dotnet ef database update --project Vibe.Test.Model --startup-project Vibe.Test.Web
```

## Docker

### 啟動服務

```sh
docker-compose up -d
```

### 查看日誌

```sh
docker-compose logs -f
```

### 停止服務

```sh
docker-compose down

# 停止並刪除資料 (包含資料庫)
docker-compose down -v
```

## 套件版本

| 套件 | 版本 | 專案 |
|------|------|------|
| Microsoft.EntityFrameworkCore | 10.0.x | Vibe.Test.Model |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.x | Vibe.Test.Model |
| Microsoft.EntityFrameworkCore.Design | 10.0.x | Vibe.Test.Web |
| Scalar.AspNetCore | 2.0.x | Vibe.Test.Web |

## 注意事項

- 本機執行時，`appsettings.json` 的 `Host` 需為 `localhost`
- Docker 環境會自動載入 `appsettings.Docker.json`
- `init.sql` 只會在 PostgreSQL 容器首次建立時執行，重新執行需先 `docker-compose down -v`
