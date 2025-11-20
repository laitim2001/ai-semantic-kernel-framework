# Backend API (FastAPI + Semantic Kernel)

IPA Platform 的後端服務，基於 FastAPI 和 Semantic Kernel 構建。

## 📁 項目結構

```
backend/
├── src/
│   ├── workflow/          # 工作流管理模塊
│   │   ├── __init__.py
│   │   ├── models.py      # 工作流數據模型
│   │   ├── service.py     # 工作流業務邏輯
│   │   ├── router.py      # 工作流 API 路由
│   │   └── schemas.py     # Pydantic schemas
│   ├── execution/         # 執行引擎模塊
│   │   ├── __init__.py
│   │   ├── engine.py      # Semantic Kernel 執行引擎
│   │   ├── service.py     # 執行業務邏輯
│   │   └── router.py      # 執行 API 路由
│   ├── agent/             # Agent 管理模塊
│   │   ├── __init__.py
│   │   ├── models.py      # Agent 數據模型
│   │   ├── service.py     # Agent 業務邏輯
│   │   └── router.py      # Agent API 路由
│   ├── auth/              # 身份驗證模塊
│   │   ├── __init__.py
│   │   ├── azure_ad.py    # Azure AD OAuth
│   │   └── dependencies.py # FastAPI 依賴項
│   ├── core/              # 核心配置
│   │   ├── __init__.py
│   │   ├── config.py      # 配置管理
│   │   ├── database.py    # 數據庫連接
│   │   └── cache.py       # Redis 緩存
│   └── main.py            # FastAPI 應用入口
├── tests/
│   ├── unit/              # 單元測試
│   ├── integration/       # 集成測試
│   └── conftest.py        # pytest 配置
├── alembic/               # 數據庫遷移
│   └── versions/
├── Dockerfile             # Docker 構建文件
├── requirements.txt       # Python 依賴
├── pyproject.toml         # 項目配置
└── README.md
```

## 🚀 快速開始

### 本地開發（Docker Compose）

```bash
# 啟動所有服務
docker-compose up -d

# 查看日誌
docker-compose logs -f backend

# 停止服務
docker-compose down
```

### 本地開發（Python 虛擬環境）

```bash
cd backend

# 創建虛擬環境
python -m venv venv

# 激活虛擬環境
# Windows
.\venv\Scripts\activate
# Linux/Mac
source venv/bin/activate

# 安裝依賴
pip install -r requirements.txt

# 運行數據庫遷移
alembic upgrade head

# 啟動開發服務器
uvicorn main:app --reload --host 0.0.0.0 --port 8000
```

### API 文檔

啟動服務後，訪問：
- **Swagger UI**: http://localhost:8000/docs
- **ReDoc**: http://localhost:8000/redoc

## 🧪 測試

```bash
# 運行所有測試
pytest

# 運行特定測試文件
pytest tests/unit/test_workflow_service.py

# 查看測試覆蓋率
pytest --cov=src --cov-report=html

# 打開覆蓋率報告
start htmlcov/index.html  # Windows
open htmlcov/index.html   # Mac
```

## 📦 依賴管理

```bash
# 添加新依賴
pip install package-name
pip freeze > requirements.txt

# 或使用 pip-tools
pip-compile requirements.in
```

## 🔐 環境變量

參見根目錄的 `.env.example` 文件。

## 📚 技術棧

- **Web 框架**: FastAPI 0.104+
- **AI 框架**: Semantic Kernel 1.0+
- **數據庫**: PostgreSQL + SQLAlchemy
- **緩存**: Redis + redis-py
- **消息隊列**: Azure Service Bus
- **監控**: Application Insights
- **測試**: pytest + httpx
