# Phase 8: 部署架構 (Deployment Architecture)

## 階段概述

本階段定義 AI Workflow Platform 的完整部署架構,包括 Docker 容器化、Kubernetes 編排、CI/CD 流程和多環境部署策略。

## 目標

- ✅ 實現容器化部署
- ✅ 建立 Kubernetes 集群架構
- ✅ 配置 CI/CD 自動化流程
- ✅ 實現多環境部署 (Dev, Staging, Production)
- ✅ 確保高可用性和可擴展性

## 文檔結構

### 1. Docker 容器化
**文件**: `docker-containerization.md`

**內容**:
- Dockerfile 最佳實踐
- 多階段構建
- Docker Compose 配置
- 容器網絡和存儲
- 鏡像優化策略
- 容器安全配置

**技術重點**:
- Docker 24.0
- Docker Compose v2
- Multi-stage builds
- .dockerignore 優化

### 2. Kubernetes 部署
**文件**: `kubernetes-deployment.md`

**內容**:
- Kubernetes 集群架構
- Deployment/StatefulSet 配置
- Service/Ingress 配置
- ConfigMap/Secret 管理
- Persistent Volumes
- HorizontalPodAutoscaler
- Helm Charts

**技術重點**:
- Kubernetes 1.28
- Helm 3.13
- NGINX Ingress Controller
- cert-manager (HTTPS)

### 3. CI/CD 流程
**文件**: `cicd-pipeline.md`

**內容**:
- GitHub Actions 工作流程
- Azure DevOps Pipeline
- 構建流程自動化
- 測試自動化集成
- 容器鏡像構建和推送
- 自動部署策略
- 回滾機制

**技術重點**:
- GitHub Actions
- Azure Container Registry
- GitOps (ArgoCD)

### 4. 多環境配置
**文件**: `multi-environment-configuration.md`

**內容**:
- 環境劃分策略
- 配置管理 (appsettings, .env)
- 密鑰管理 (Azure Key Vault, Kubernetes Secrets)
- 環境特定資源配置
- 數據庫遷移策略
- 環境隔離

**技術重點**:
- Azure Key Vault
- Kubernetes Namespaces
- Environment Variables

## 架構圖

### 高層架構

```
┌─────────────────────────────────────────────────────────┐
│                     Internet                             │
└─────────────────────┬───────────────────────────────────┘
                      │
              ┌───────┴───────┐
              │  Load Balancer │
              │   (Azure LB)   │
              └───────┬───────┘
                      │
      ┌───────────────┼───────────────┐
      │               │               │
┌─────┴─────┐  ┌─────┴─────┐  ┌─────┴─────┐
│ AKS Node 1│  │ AKS Node 2│  │ AKS Node 3│
└───────────┘  └───────────┘  └───────────┘
      │               │               │
      └───────────────┼───────────────┘
                      │
         ┌────────────┴────────────┐
         │   Kubernetes Cluster     │
         │                          │
         │  ┌────────────────────┐ │
         │  │   Ingress NGINX    │ │
         │  └─────────┬──────────┘ │
         │            │             │
         │  ┌─────────┴──────────┐ │
         │  │  Frontend (React)  │ │
         │  │  Frontend (Vue)    │ │
         │  └─────────┬──────────┘ │
         │            │             │
         │  ┌─────────┴──────────┐ │
         │  │   Backend API      │ │
         │  │   (ASP.NET Core)   │ │
         │  └─────────┬──────────┘ │
         │            │             │
         │  ┌─────────┴──────────┐ │
         │  │   PostgreSQL 16    │ │
         │  │   Redis 7          │ │
         │  │   MongoDB 7        │ │
         │  └────────────────────┘ │
         └──────────────────────────┘
```

### CI/CD 流程

```
┌────────────┐
│  Git Push  │
└─────┬──────┘
      │
┌─────▼──────────────────────────────────────────┐
│           GitHub Actions                        │
│                                                 │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐    │
│  │  Build   │─→│   Test   │─→│  Package │    │
│  └──────────┘  └──────────┘  └──────────┘    │
└─────┬───────────────────────────────────────┘
      │
┌─────▼────────────────────────┐
│  Azure Container Registry     │
│  (Docker Images)              │
└─────┬────────────────────────┘
      │
┌─────▼────────────────────────┐
│  ArgoCD (GitOps)             │
└─────┬────────────────────────┘
      │
┌─────▼────────────────────────┐
│  Kubernetes (AKS)            │
│  - Development               │
│  - Staging                   │
│  - Production                │
└──────────────────────────────┘
```

## Docker 配置

### Backend Dockerfile

```dockerfile
# AIWorkflow.API/Dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製 csproj 文件並還原依賴
COPY ["AIWorkflow.API/AIWorkflow.API.csproj", "AIWorkflow.API/"]
COPY ["AIWorkflow.Application/AIWorkflow.Application.csproj", "AIWorkflow.Application/"]
COPY ["AIWorkflow.Domain/AIWorkflow.Domain.csproj", "AIWorkflow.Domain/"]
COPY ["AIWorkflow.Infrastructure/AIWorkflow.Infrastructure.csproj", "AIWorkflow.Infrastructure/"]
RUN dotnet restore "AIWorkflow.API/AIWorkflow.API.csproj"

# 複製所有源代碼並構建
COPY . .
WORKDIR "/src/AIWorkflow.API"
RUN dotnet build "AIWorkflow.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "AIWorkflow.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 創建非 root 用戶
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser

# 複製構建結果
COPY --from=publish /app/publish .

# 切換到非 root 用戶
USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "AIWorkflow.API.dll"]
```

### Docker Compose

```yaml
# docker-compose.yml
version: '3.8'

services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_DB: aiworkflow
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  mongodb:
    image: mongo:7
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: ${MONGO_PASSWORD}
    ports:
      - "27017:27017"
    volumes:
      - mongo_data:/data/db

  backend:
    build:
      context: .
      dockerfile: AIWorkflow.API/Dockerfile
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "Host=postgres;Database=aiworkflow;Username=postgres;Password=${POSTGRES_PASSWORD}"
      Redis__Configuration: "redis:6379"
      MongoDB__ConnectionString: "mongodb://admin:${MONGO_PASSWORD}@mongodb:27017"
    ports:
      - "8080:8080"
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
      mongodb:
        condition: service_started

  frontend-react:
    build:
      context: ./frontend/react-app
      dockerfile: Dockerfile
    ports:
      - "3000:80"
    environment:
      VITE_API_BASE_URL: http://backend:8080

  frontend-vue:
    build:
      context: ./frontend/vue-workflow-editor
      dockerfile: Dockerfile
    ports:
      - "3001:80"
    environment:
      VITE_API_BASE_URL: http://backend:8080
      VITE_WS_URL: ws://backend:8080/ws

volumes:
  postgres_data:
  redis_data:
  mongo_data:
```

## Kubernetes 配置

### Backend Deployment

```yaml
# k8s/backend-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: backend-api
  namespace: aiworkflow
  labels:
    app: backend-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: backend-api
  template:
    metadata:
      labels:
        app: backend-api
    spec:
      containers:
      - name: api
        image: aiworkflow.azurecr.io/backend-api:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: database-secrets
              key: connection-string
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: backend-api-service
  namespace: aiworkflow
spec:
  selector:
    app: backend-api
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8080
  type: ClusterIP
---
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: backend-api-hpa
  namespace: aiworkflow
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: backend-api
  minReplicas: 3
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

### Ingress Configuration

```yaml
# k8s/ingress.yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: aiworkflow-ingress
  namespace: aiworkflow
  annotations:
    cert-manager.io/cluster-issuer: "letsencrypt-prod"
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
    nginx.ingress.kubernetes.io/proxy-body-size: "10m"
spec:
  ingressClassName: nginx
  tls:
  - hosts:
    - app.aiworkflow.com
    - api.aiworkflow.com
    secretName: aiworkflow-tls
  rules:
  - host: app.aiworkflow.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: frontend-react-service
            port:
              number: 80
  - host: api.aiworkflow.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: backend-api-service
            port:
              number: 80
```

## GitHub Actions CI/CD

```yaml
# .github/workflows/deploy.yml
name: Build and Deploy

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

env:
  REGISTRY: aiworkflow.azurecr.io
  IMAGE_NAME: backend-api

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --configuration Release --no-restore

      - name: Test
        run: dotnet test --no-build --verbosity normal

  build-and-push-docker:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main' || github.ref == 'refs/heads/develop'

    steps:
      - uses: actions/checkout@v4

      - name: Log in to Azure Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
          tags: |
            type=ref,event=branch
            type=sha,prefix={{branch}}-

      - name: Build and push
        uses: docker/build-push-action@v5
        with:
          context: .
          file: ./AIWorkflow.API/Dockerfile
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}

  deploy-to-aks:
    needs: build-and-push-docker
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'

    steps:
      - uses: actions/checkout@v4

      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Set AKS context
        uses: azure/aks-set-context@v3
        with:
          resource-group: aiworkflow-rg
          cluster-name: aiworkflow-aks

      - name: Deploy to AKS
        run: |
          kubectl apply -f k8s/ --namespace aiworkflow
          kubectl rollout status deployment/backend-api --namespace aiworkflow
```

## 環境配置

### Development

```yaml
# k8s/overlays/development/kustomization.yaml
apiVersion: kustomize.config.k8s.io/v1beta1
kind: Kustomization

namespace: aiworkflow-dev

resources:
- ../../base

replicas:
- name: backend-api
  count: 1

images:
- name: backend-api
  newTag: develop-latest
```

### Production

```yaml
# k8s/overlays/production/kustomization.yaml
apiVersion: kustomize.config.k8s.io/v1beta1
kind: Kustomization

namespace: aiworkflow-prod

resources:
- ../../base

replicas:
- name: backend-api
  count: 3

images:
- name: backend-api
  newTag: main-stable
```

## 檢查清單

### 容器化
- [ ] 創建優化的 Dockerfile
- [ ] 配置 Docker Compose
- [ ] 實現多階段構建
- [ ] 配置 .dockerignore
- [ ] 測試容器本地運行

### Kubernetes
- [ ] 創建 Deployment 配置
- [ ] 配置 Service 和 Ingress
- [ ] 設置 ConfigMap 和 Secrets
- [ ] 配置 HPA 自動擴展
- [ ] 實現健康檢查

### CI/CD
- [ ] 配置 GitHub Actions
- [ ] 實現自動化測試
- [ ] 配置容器鏡像構建
- [ ] 實現自動部署
- [ ] 配置回滾機制

### 安全性
- [ ] 使用非 root 用戶運行容器
- [ ] 配置 HTTPS/TLS
- [ ] 管理密鑰和配置
- [ ] 實現網絡策略
- [ ] 掃描容器漏洞

## 相關文檔

- [Phase 6: 數據庫標準](../6-database-standards/README.md)
- [Phase 7: 測試策略](../7-testing-strategy/README.md)
- [Phase 9: 安全標準](../9-security-standards/README.md)
- [Phase 10: 監控與運維](../10-monitoring-operations/README.md)

## 參考資源

### 官方文檔
- [Docker Documentation](https://docs.docker.com/)
- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Azure Kubernetes Service (AKS)](https://learn.microsoft.com/en-us/azure/aks/)
- [GitHub Actions](https://docs.github.com/en/actions)

### 最佳實踐
- [Dockerfile Best Practices](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/)
- [Kubernetes Best Practices](https://kubernetes.io/docs/concepts/configuration/overview/)
- [12-Factor App](https://12factor.net/)

---

**文檔維護**: DevOps Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
