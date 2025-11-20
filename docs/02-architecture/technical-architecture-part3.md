# Technical Architecture Design (Part 3)
# 安全架構、監控日誌、部署架構

> ℹ️ **MVP 調整說明**: 本文檔為原始完整架構設計（包含 Kubernetes、ELK Stack）。
> MVP 階段已調整為 Azure App Service + Application Insights。
> 請參閱 [technical-architecture.md](./technical-architecture.md) Section 1.2 了解 MVP 部署策略。
> 本文檔保留作為**生產環境擴展時的參考**。

**接續文檔**: [technical-architecture-part2.md](./technical-architecture-part2.md)

---

## 7. 安全架構

### 7.1 認證與授權

#### OAuth 2.0 + JWT 流程

```
┌──────────┐                                           ┌──────────────┐
│  Client  │                                           │ Auth Service │
└────┬─────┘                                           └──────┬───────┘
     │                                                         │
     │ 1. POST /auth/login                                    │
     │    {username, password}                                │
     ├────────────────────────────────────────────────────────>│
     │                                                         │
     │ 2. Verify Credentials                                  │
     │    + Generate JWT Token                                │
     │<────────────────────────────────────────────────────────┤
     │ 3. Return {accessToken, refreshToken}                  │
     │                                                         │
     │                                                         │
     │ 4. GET /api/v1/workflows                               │
     │    Headers: Authorization: Bearer {accessToken}        │
     ├────────────────────────────────────────────────────────>│
     │                                                         │
     │ 5. Verify JWT Signature                                │
     │    + Check Expiration                                  │
     │    + Extract User Claims                               │
     │<────────────────────────────────────────────────────────┤
     │ 6. Return Workflows                                    │
     │                                                         │
```

#### JWT Token 結構

```typescript
// Token Payload
interface JwtPayload {
  sub: string;           // User ID
  email: string;         // User Email
  name: string;          // User Name
  roles: string[];       // User Roles ['admin', 'user', 'viewer']
  permissions: string[]; // Fine-grained permissions
  iat: number;           // Issued At (timestamp)
  exp: number;           // Expiration (timestamp)
  jti: string;           // JWT ID (unique identifier)
}

// Token Generation
function generateAccessToken(user: User): string {
  const payload: JwtPayload = {
    sub: user.id,
    email: user.email,
    name: user.name,
    roles: user.roles,
    permissions: user.permissions,
    iat: Math.floor(Date.now() / 1000),
    exp: Math.floor(Date.now() / 1000) + (15 * 60), // 15 minutes
    jti: crypto.randomUUID()
  };
  
  return jwt.sign(payload, process.env.JWT_SECRET, {
    algorithm: 'HS256'
  });
}

// Token Verification Middleware
async function verifyToken(req: Request, res: Response, next: NextFunction) {
  const token = req.headers.authorization?.split(' ')[1];
  
  if (!token) {
    return res.status(401).json({ error: 'No token provided' });
  }
  
  try {
    const decoded = jwt.verify(token, process.env.JWT_SECRET) as JwtPayload;
    
    // Check if token is blacklisted (logout)
    const isBlacklisted = await redis.get(`blacklist:${decoded.jti}`);
    if (isBlacklisted) {
      return res.status(401).json({ error: 'Token has been revoked' });
    }
    
    req.user = decoded;
    next();
  } catch (error) {
    return res.status(401).json({ error: 'Invalid token' });
  }
}
```

#### RBAC (Role-Based Access Control)

```typescript
// Permission Matrix
const permissions = {
  admin: [
    'workflows:read',
    'workflows:create',
    'workflows:update',
    'workflows:delete',
    'executions:read',
    'executions:retry',
    'executions:cancel',
    'agents:read',
    'agents:create',
    'agents:update',
    'agents:delete',
    'users:manage',
    'settings:manage'
  ],
  user: [
    'workflows:read',
    'workflows:create',
    'workflows:update',
    'executions:read',
    'executions:retry',
    'agents:read'
  ],
  viewer: [
    'workflows:read',
    'executions:read',
    'agents:read'
  ]
};

// Authorization Middleware
function requirePermission(permission: string) {
  return (req: Request, res: Response, next: NextFunction) => {
    if (!req.user.permissions.includes(permission)) {
      return res.status(403).json({
        error: 'Forbidden',
        message: `Required permission: ${permission}`
      });
    }
    next();
  };
}

// Usage
app.post('/api/v1/workflows',
  verifyToken,
  requirePermission('workflows:create'),
  createWorkflow
);
```

---

### 7.2 審計追蹤

#### Append-Only Audit Log

```csharp
public class AuditService
{
    private readonly IDbConnection _db;
    private readonly ILogger<AuditService> _logger;
    
    public async Task LogAction(AuditEntry entry)
    {
        // 計算簽名確保數據完整性
        entry.Signature = CalculateSignature(entry);
        
        const string sql = @"
            INSERT INTO audit_logs 
                (timestamp, user_id, action, resource_type, resource_id, 
                 details, ip_address, user_agent, signature)
            VALUES 
                (@Timestamp, @UserId, @Action, @ResourceType, @ResourceId,
                 @Details, @IpAddress, @UserAgent, @Signature)";
        
        await _db.ExecuteAsync(sql, entry);
        
        _logger.LogInformation(
            "Audit log created: {Action} on {ResourceType}/{ResourceId} by {UserId}",
            entry.Action, entry.ResourceType, entry.ResourceId, entry.UserId
        );
    }
    
    private string CalculateSignature(AuditEntry entry)
    {
        var data = $"{entry.Timestamp:O}|{entry.UserId}|{entry.Action}|" +
                   $"{entry.ResourceType}|{entry.ResourceId}|" +
                   $"{JsonSerializer.Serialize(entry.Details)}";
        
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash);
    }
    
    public async Task<bool> VerifyIntegrity(Guid auditLogId)
    {
        const string sql = "SELECT * FROM audit_logs WHERE id = @Id";
        var entry = await _db.QuerySingleAsync<AuditEntry>(sql, new { Id = auditLogId });
        
        var expectedSignature = CalculateSignature(entry);
        return entry.Signature == expectedSignature;
    }
}

// Usage: Audit Middleware
public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAuditService _auditService;
    
    public async Task InvokeAsync(HttpContext context)
    {
        // 記錄請求前狀態
        var startTime = DateTime.UtcNow;
        var originalBodyStream = context.Response.Body;
        
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        await _next(context);
        
        // 記錄請求後狀態
        var duration = DateTime.UtcNow - startTime;
        
        // 只審計狀態變更操作
        if (context.Request.Method != "GET" && context.Response.StatusCode < 400)
        {
            await _auditService.LogAction(new AuditEntry
            {
                Timestamp = startTime,
                UserId = context.User.FindFirst("sub")?.Value,
                Action = $"{context.Request.Method} {context.Request.Path}",
                ResourceType = ExtractResourceType(context.Request.Path),
                ResourceId = ExtractResourceId(context.Request.Path),
                Details = new
                {
                    StatusCode = context.Response.StatusCode,
                    DurationMs = duration.TotalMilliseconds,
                    RequestBody = await ReadRequestBody(context.Request)
                },
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers["User-Agent"].ToString()
            });
        }
        
        // 恢復響應流
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }
}
```

---

### 7.3 數據加密

#### 傳輸加密 (TLS 1.3)

```yaml
# Nginx TLS Configuration
server {
    listen 443 ssl http2;
    server_name ipa.example.com;
    
    # TLS Configuration
    ssl_certificate /etc/nginx/ssl/cert.pem;
    ssl_certificate_key /etc/nginx/ssl/key.pem;
    ssl_protocols TLSv1.3 TLSv1.2;
    ssl_ciphers 'ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384';
    ssl_prefer_server_ciphers on;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;
    
    # HSTS Header
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    
    # Security Headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Content-Security-Policy "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';" always;
    
    location / {
        proxy_pass http://api-gateway:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

#### 靜態加密 (Database)

```csharp
// 敏感字段加密
public class EncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;
    
    public EncryptionService(IConfiguration config)
    {
        _key = Convert.FromBase64String(config["Encryption:Key"]);
        _iv = Convert.FromBase64String(config["Encryption:IV"]);
    }
    
    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        
        sw.Write(plainText);
        sw.Close();
        
        return Convert.ToBase64String(ms.ToArray());
    }
    
    public string Decrypt(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        
        return sr.ReadToEnd();
    }
}

// Entity Framework Encryption
public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        // 自動加密/解密敏感字段
        builder.Property(w => w.TriggerConfig)
            .HasConversion(
                v => _encryptionService.Encrypt(JsonSerializer.Serialize(v)),
                v => JsonSerializer.Deserialize<TriggerConfig>(_encryptionService.Decrypt(v))
            );
    }
}
```

---

## 8. 監控與日誌

### 8.1 指標收集 (Prometheus)

#### 自定義指標

```csharp
// Prometheus Metrics
public class MetricsService
{
    // Counters
    private static readonly Counter ExecutionStarted = Metrics.CreateCounter(
        "ipa_execution_started_total",
        "Total number of executions started",
        new CounterConfiguration
        {
            LabelNames = new[] { "workflow_id", "trigger_type" }
        }
    );
    
    private static readonly Counter ExecutionCompleted = Metrics.CreateCounter(
        "ipa_execution_completed_total",
        "Total number of executions completed",
        new CounterConfiguration
        {
            LabelNames = new[] { "workflow_id", "status" }
        }
    );
    
    // Histograms
    private static readonly Histogram ExecutionDuration = Metrics.CreateHistogram(
        "ipa_execution_duration_seconds",
        "Execution duration in seconds",
        new HistogramConfiguration
        {
            LabelNames = new[] { "workflow_id" },
            Buckets = Histogram.ExponentialBuckets(0.1, 2, 10) // 0.1s, 0.2s, 0.4s, ...
        }
    );
    
    private static readonly Histogram AgentTokenUsage = Metrics.CreateHistogram(
        "ipa_agent_token_usage",
        "Token usage per agent execution",
        new HistogramConfiguration
        {
            LabelNames = new[] { "agent_id", "agent_type" },
            Buckets = new[] { 100, 500, 1000, 5000, 10000, 50000 }
        }
    );
    
    // Gauges
    private static readonly Gauge ActiveExecutions = Metrics.CreateGauge(
        "ipa_active_executions",
        "Number of currently active executions"
    );
    
    private static readonly Gauge QueueDepth = Metrics.CreateGauge(
        "ipa_queue_depth",
        "Number of messages in execution queue",
        new GaugeConfiguration
        {
            LabelNames = new[] { "queue_name" }
        }
    );
    
    // Usage
    public async Task<ExecutionResult> ExecuteWorkflow(Guid workflowId, ...)
    {
        ExecutionStarted.WithLabels(workflowId.ToString(), "n8n_webhook").Inc();
        ActiveExecutions.Inc();
        
        using (ExecutionDuration.WithLabels(workflowId.ToString()).NewTimer())
        {
            try
            {
                var result = await _orchestrator.ExecuteWorkflow(...);
                
                ExecutionCompleted
                    .WithLabels(workflowId.ToString(), result.Success ? "success" : "failed")
                    .Inc();
                
                return result;
            }
            finally
            {
                ActiveExecutions.Dec();
            }
        }
    }
}
```

#### Grafana Dashboard 配置

```json
{
  "dashboard": {
    "title": "IPA Platform - Execution Monitoring",
    "panels": [
      {
        "title": "Execution Rate",
        "targets": [
          {
            "expr": "rate(ipa_execution_started_total[5m])",
            "legendFormat": "{{workflow_id}}"
          }
        ],
        "type": "graph"
      },
      {
        "title": "Success Rate",
        "targets": [
          {
            "expr": "sum(rate(ipa_execution_completed_total{status=\"success\"}[5m])) / sum(rate(ipa_execution_completed_total[5m]))",
            "legendFormat": "Success Rate"
          }
        ],
        "type": "gauge"
      },
      {
        "title": "P95 Execution Duration",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(ipa_execution_duration_seconds_bucket[5m]))",
            "legendFormat": "{{workflow_id}}"
          }
        ],
        "type": "graph"
      },
      {
        "title": "Active Executions",
        "targets": [
          {
            "expr": "ipa_active_executions",
            "legendFormat": "Active"
          }
        ],
        "type": "stat"
      }
    ]
  }
}
```

---

### 8.2 分布式追蹤 (OpenTelemetry)

```csharp
// OpenTelemetry Configuration
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services.AddOpenTelemetry()
                .WithTracing(builder => builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddSource("IPA.ExecutionService")
                    .AddJaegerExporter(options =>
                    {
                        options.AgentHost = "jaeger";
                        options.AgentPort = 6831;
                    })
                );
        });

// Custom Span
public async Task<ExecutionResult> ExecuteWorkflow(Guid executionId, ...)
{
    using var activity = _activitySource.StartActivity("ExecuteWorkflow");
    activity?.SetTag("execution.id", executionId);
    activity?.SetTag("workflow.id", workflowId);
    
    foreach (var agentConfig in workflow.AgentChain)
    {
        using var agentActivity = _activitySource.StartActivity(
            "ExecuteAgent",
            ActivityKind.Internal,
            activity.Context
        );
        
        agentActivity?.SetTag("agent.id", agentConfig.AgentId);
        agentActivity?.SetTag("agent.type", agentConfig.Type);
        
        try
        {
            var result = await ExecuteAgent(agentConfig, ...);
            agentActivity?.SetTag("agent.success", result.Success);
            agentActivity?.SetTag("agent.tokens", result.TokensUsed);
        }
        catch (Exception ex)
        {
            agentActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
    
    return new ExecutionResult { ... };
}
```

---

### 8.3 結構化日誌 (Serilog)

```csharp
// Serilog Configuration
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "IPA.ExecutionService")
            .WriteTo.Console(new JsonFormatter())
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200"))
            {
                IndexFormat = "ipa-logs-{0:yyyy.MM.dd}",
                AutoRegisterTemplate = true,
                NumberOfShards = 2,
                NumberOfReplicas = 1
            })
        );

// Structured Logging
_logger.LogInformation(
    "Execution started: {ExecutionId} for workflow {WorkflowId} triggered by {TriggerType}",
    executionId,
    workflowId,
    triggerType
);

_logger.LogError(
    exception,
    "Agent execution failed: {AgentId} in execution {ExecutionId}. Retry count: {RetryCount}",
    agentId,
    executionId,
    retryCount
);

// Log Context
using (LogContext.PushProperty("ExecutionId", executionId))
using (LogContext.PushProperty("WorkflowId", workflowId))
{
    _logger.LogInformation("Starting agent chain execution");
    // All logs in this scope will include ExecutionId and WorkflowId
}
```

---

## 9. 部署架構

### 9.1 Kubernetes 部署

#### Deployment 配置

```yaml
# execution-service-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: execution-service
  namespace: ipa-platform
spec:
  replicas: 3
  selector:
    matchLabels:
      app: execution-service
  template:
    metadata:
      labels:
        app: execution-service
        version: v1.0.0
    spec:
      containers:
      - name: execution-service
        image: ipa/execution-service:1.0.0
        ports:
        - containerPort: 5000
          name: http
        - containerPort: 9090
          name: metrics
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__PostgreSQL
          valueFrom:
            secretKeyRef:
              name: db-credentials
              key: connection-string
        - name: Redis__Host
          value: "redis-service"
        - name: RabbitMQ__Host
          value: "rabbitmq-service"
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "1000m"
        livenessProbe:
          httpGet:
            path: /health/live
            port: 5000
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 5000
          initialDelaySeconds: 10
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: execution-service
  namespace: ipa-platform
spec:
  selector:
    app: execution-service
  ports:
  - name: http
    port: 80
    targetPort: 5000
  - name: metrics
    port: 9090
    targetPort: 9090
  type: ClusterIP
```

#### Horizontal Pod Autoscaler

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: execution-service-hpa
  namespace: ipa-platform
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: execution-service
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
  - type: Pods
    pods:
      metric:
        name: ipa_active_executions
      target:
        type: AverageValue
        averageValue: "50"
  behavior:
    scaleUp:
      stabilizationWindowSeconds: 60
      policies:
      - type: Percent
        value: 50
        periodSeconds: 60
    scaleDown:
      stabilizationWindowSeconds: 300
      policies:
      - type: Pods
        value: 1
        periodSeconds: 60
```

---

### 9.2 CI/CD Pipeline

#### GitHub Actions Workflow

```yaml
name: Build and Deploy

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
    
    - name: Upload coverage
      uses: codecov/codecov-action@v3

  build-and-push:
    needs: test
    runs-on: ubuntu-latest
    if: github.event_name == 'push'
    steps:
    - uses: actions/checkout@v3
    
    - name: Log in to Container Registry
      uses: docker/login-action@v2
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Extract metadata
      id: meta
      uses: docker/metadata-action@v4
      with:
        images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
    
    - name: Build and push Docker image
      uses: docker/build-push-action@v4
      with:
        context: .
        push: true
        tags: ${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}
    
    - name: Security Scan
      uses: aquasecurity/trivy-action@master
      with:
        image-ref: ${{ steps.meta.outputs.tags }}
        format: 'sarif'
        output: 'trivy-results.sarif'
    
    - name: Upload Trivy results to GitHub Security
      uses: github/codeql-action/upload-sarif@v2
      with:
        sarif_file: 'trivy-results.sarif'

  deploy:
    needs: build-and-push
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup kubectl
      uses: azure/setup-kubectl@v3
    
    - name: Set Kubernetes context
      uses: azure/k8s-set-context@v3
      with:
        kubeconfig: ${{ secrets.KUBE_CONFIG }}
    
    - name: Deploy to Kubernetes
      run: |
        kubectl apply -f k8s/namespace.yaml
        kubectl apply -f k8s/configmap.yaml
        kubectl apply -f k8s/secrets.yaml
        kubectl apply -f k8s/deployment.yaml
        kubectl rollout status deployment/execution-service -n ipa-platform
    
    - name: Run smoke tests
      run: |
        kubectl run smoke-test --image=curlimages/curl --rm -i --restart=Never -- \
          curl -f http://execution-service/health || exit 1
```

---

### 9.3 災難恢復

#### 備份策略

```bash
#!/bin/bash
# backup.sh - Database Backup Script

DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups/$DATE"
S3_BUCKET="s3://ipa-backups"

mkdir -p $BACKUP_DIR

# 1. PostgreSQL Backup
echo "Starting PostgreSQL backup..."
pg_dump -h $POSTGRES_HOST -U $POSTGRES_USER -d ipa_platform \
  -F c -f $BACKUP_DIR/postgresql_$DATE.dump

# 2. Redis Backup
echo "Starting Redis backup..."
redis-cli --rdb $BACKUP_DIR/redis_$DATE.rdb

# 3. Upload to S3
echo "Uploading to S3..."
aws s3 sync $BACKUP_DIR $S3_BUCKET/$DATE

# 4. Verify Backup Integrity
echo "Verifying backup..."
pg_restore --list $BACKUP_DIR/postgresql_$DATE.dump > /dev/null
if [ $? -eq 0 ]; then
  echo "Backup verification successful"
else
  echo "Backup verification failed!"
  exit 1
fi

# 5. Cleanup old backups (keep last 30 days)
find /backups -type d -mtime +30 -exec rm -rf {} \;

echo "Backup completed: $BACKUP_DIR"
```

#### 恢復流程

```bash
#!/bin/bash
# restore.sh - Database Restore Script

BACKUP_DATE=$1
BACKUP_DIR="/backups/$BACKUP_DATE"

if [ -z "$BACKUP_DATE" ]; then
  echo "Usage: ./restore.sh <backup_date>"
  exit 1
fi

# 1. Download from S3
echo "Downloading backup from S3..."
aws s3 sync s3://ipa-backups/$BACKUP_DATE $BACKUP_DIR

# 2. Stop services
echo "Stopping services..."
kubectl scale deployment --all --replicas=0 -n ipa-platform

# 3. Restore PostgreSQL
echo "Restoring PostgreSQL..."
dropdb -h $POSTGRES_HOST -U $POSTGRES_USER ipa_platform
createdb -h $POSTGRES_HOST -U $POSTGRES_USER ipa_platform
pg_restore -h $POSTGRES_HOST -U $POSTGRES_USER -d ipa_platform \
  $BACKUP_DIR/postgresql_$BACKUP_DATE.dump

# 4. Restore Redis
echo "Restoring Redis..."
redis-cli FLUSHALL
cat $BACKUP_DIR/redis_$BACKUP_DATE.rdb | redis-cli --pipe

# 5. Restart services
echo "Restarting services..."
kubectl scale deployment --all --replicas=3 -n ipa-platform

echo "Restore completed from $BACKUP_DATE"
```

---

## 10. 性能優化策略

### 10.1 數據庫優化

```sql
-- 分區表 (Executions by date)
CREATE TABLE executions (
  id UUID NOT NULL,
  workflow_id UUID NOT NULL,
  status VARCHAR(50) NOT NULL,
  triggered_at TIMESTAMPTZ NOT NULL,
  ...
) PARTITION BY RANGE (triggered_at);

CREATE TABLE executions_2025_11 PARTITION OF executions
  FOR VALUES FROM ('2025-11-01') TO ('2025-12-01');

CREATE TABLE executions_2025_12 PARTITION OF executions
  FOR VALUES FROM ('2025-12-01') TO ('2026-01-01');

-- 連接池配置
max_connections = 200
shared_buffers = 2GB
effective_cache_size = 6GB
maintenance_work_mem = 512MB
work_mem = 16MB
```

### 10.2 緩存策略

```typescript
// Multi-layer Caching
class CacheService {
  async getWorkflow(workflowId: string): Promise<Workflow> {
    // L1: In-memory cache (fastest)
    let workflow = this.memoryCache.get(workflowId);
    if (workflow) return workflow;
    
    // L2: Redis cache
    workflow = await this.redis.get(`workflow:${workflowId}`);
    if (workflow) {
      this.memoryCache.set(workflowId, workflow);
      return workflow;
    }
    
    // L3: Database
    workflow = await this.db.query('SELECT * FROM workflows WHERE id = $1', [workflowId]);
    
    // Populate caches
    await this.redis.setex(`workflow:${workflowId}`, 3600, JSON.stringify(workflow));
    this.memoryCache.set(workflowId, workflow);
    
    return workflow;
  }
}
```

---

**文檔狀態**: 技術架構設計完成 ✅

**總覽**:
- Part 1: 架構概覽、設計原則、技術棧選擇、系統架構
- Part 2: 核心模塊設計(API Gateway, Workflow, Execution, Agent Service)、集成架構
- Part 3: 安全架構、監控日誌、部署架構、性能優化、災難恢復

**總行數**: ~2,500+ 行

下一階段: 實現階段 (Implementation)
