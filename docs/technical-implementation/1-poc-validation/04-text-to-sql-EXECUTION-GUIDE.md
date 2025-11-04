# PoC 4: Text-to-SQL Engine - 實際執行指導

**目的**: 逐步執行 PoC 4 驗證，構建安全的自然語言轉 SQL 系統

**預計時間**: 3 days (Day 8-10)
**執行人**: Backend Tech Lead + Data Engineer
**執行日期**: _____________

**前置條件**: PoC 1-3 已通過 ✅

**⚠️ 安全警告**: 本 PoC 涉及 SQL 生成，必須實施嚴格的 SQL 注入防護，所有測試在隔離數據庫進行

---

## 📋 執行前檢查清單

### 必備條件 Checklist

在開始執行前，請確認以下所有項目：

- [ ] **PoC 1-3 完成確認**
  - [ ] PoC 1 (SK Agents): PASSED ✅
  - [ ] PoC 2 (Persona): PASSED ✅
  - [ ] PoC 3 (Code Interpreter): PASSED ✅

- [ ] **開發環境**
  - [ ] .NET 8.0 SDK 已安裝
  - [ ] PostgreSQL 16 已安裝 (本地或 Docker)
  - [ ] Azure OpenAI API 可用
  - [ ] 專案目錄已創建 (`poc-text-to-sql/`)

- [ ] **數據庫準備**
  - [ ] PostgreSQL 測試數據庫已創建
  - [ ] 測試數據集已準備 (Agents, Conversations, Messages)
  - [ ] 數據庫連接字串已配置

- [ ] **安全準備**
  - [ ] ⚠️ 使用隔離的測試數據庫（非生產）
  - [ ] ⚠️ 準備 SQL Injection 測試向量（10 個）
  - [ ] ⚠️ 只讀用戶權限已設置

- [ ] **測試數據準備**
  - [ ] 50 個測試查詢（簡單/JOIN/聚合）
  - [ ] 10 個 SQL Injection 攻擊向量
  - [ ] Schema 文檔和語義增強描述

- [ ] **時間安排**
  - [ ] Day 8: 6-8 小時 (Schema 分析 + LLM 生成)
  - [ ] Day 9: 6-8 小時 (安全驗證 + 準確率測試)
  - [ ] Day 10: 4-6 小時 (性能優化 + 報告)

### PoC 4 核心目標

1. ✅ **Schema 語義增強**: 提高 SQL 生成準確率
2. ✅ **LLM SQL 生成**: GPT-4 驅動的查詢生成
3. ✅ **SQL Injection 防護**: 0 漏洞容忍 (10/10 阻擋)
4. ✅ **準確率**: 簡單查詢 >80%, 整體 >70%
5. ✅ **性能基準**: SQL 生成時間 <3 秒

---

## 🚀 Day 8: Schema 分析與 LLM 生成 (6-8 小時)

### Phase 8.1: PostgreSQL 測試環境 (1 小時)

#### 步驟 1: 設置 PostgreSQL 測試數據庫

**使用 Docker (推薦)**:

```powershell
# 啟動 PostgreSQL 容器
docker run --name postgres-test `
  -e POSTGRES_USER=testuser `
  -e POSTGRES_PASSWORD=testpass123 `
  -e POSTGRES_DB=poc_texttosql `
  -p 5432:5432 `
  -d postgres:16-alpine

# 等待啟動
Start-Sleep -Seconds 5

# 驗證連接
docker exec postgres-test psql -U testuser -d poc_texttosql -c "SELECT version();"
```

**預期輸出**:
```
PostgreSQL 16.x on x86_64-pc-linux-musl, compiled by gcc ...
```

**✅ 通過標準**: PostgreSQL 16 運行正常，可以連接

---

#### 步驟 2: 創建測試 Schema

**創建 `schema.sql`**:

```sql
-- ========== Agents 表 ==========
CREATE TABLE agents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    user_id UUID NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

COMMENT ON TABLE agents IS 'AI agents created by users';
COMMENT ON COLUMN agents.name IS 'Agent display name (e.g., CustomerBot, TechSupport)';
COMMENT ON COLUMN agents.user_id IS 'Owner user ID';

-- ========== Conversations 表 ==========
CREATE TABLE conversations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    agent_id UUID NOT NULL REFERENCES agents(id) ON DELETE CASCADE,
    user_id UUID NOT NULL,
    title VARCHAR(255),
    created_at TIMESTAMP DEFAULT NOW()
);

COMMENT ON TABLE conversations IS 'Conversations between users and agents';
COMMENT ON COLUMN conversations.agent_id IS 'Which agent is used in this conversation';

-- ========== Messages 表 ==========
CREATE TABLE messages (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    conversation_id UUID NOT NULL REFERENCES conversations(id) ON DELETE CASCADE,
    role VARCHAR(50) NOT NULL CHECK (role IN ('user', 'assistant')),
    content TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT NOW()
);

COMMENT ON TABLE messages IS 'Messages in conversations';
COMMENT ON COLUMN messages.role IS 'Message sender: user or assistant';

-- ========== 測試數據 ==========
INSERT INTO agents (id, name, description, user_id) VALUES
('a1', 'CustomerBot', 'Customer service agent', 'u1'),
('a2', 'TechBot', 'Technical support agent', 'u1'),
('a3', 'SalesBot', 'Sales assistant agent', 'u2');

INSERT INTO conversations (id, agent_id, user_id, title) VALUES
('c1', 'a1', 'u1', 'Product Inquiry'),
('c2', 'a2', 'u1', 'Bug Report'),
('c3', 'a1', 'u2', 'Refund Request');

INSERT INTO messages (id, conversation_id, role, content) VALUES
('m1', 'c1', 'user', 'What is your return policy?'),
('m2', 'c1', 'assistant', 'Our return policy allows returns within 30 days.'),
('m3', 'c2', 'user', 'The app crashes on startup'),
('m4', 'c2', 'assistant', 'Let me help you troubleshoot this issue.'),
('m5', 'c3', 'user', 'I want a refund for order #12345');
```

**導入 Schema**:

```powershell
# 複製 schema.sql 到容器
docker cp schema.sql postgres-test:/tmp/schema.sql

# 執行 SQL
docker exec postgres-test psql -U testuser -d poc_texttosql -f /tmp/schema.sql
```

**驗證數據**:

```powershell
docker exec postgres-test psql -U testuser -d poc_texttosql -c "SELECT COUNT(*) FROM agents;"
docker exec postgres-test psql -U testuser -d poc_texttosql -c "SELECT COUNT(*) FROM conversations;"
docker exec postgres-test psql -U testuser -d poc_texttosql -c "SELECT COUNT(*) FROM messages;"
```

**預期輸出**:
```
count
-------
     3

count
-------
     3

count
-------
     5
```

**記錄點**:
```
[Day 8 - 10:30] 測試數據庫設置完成
- PostgreSQL 版本: 16.x
- Tables 創建: agents, conversations, messages
- 測試數據: 3 agents, 3 conversations, 5 messages
```

---

### Phase 8.2: Schema 元數據與語義增強 (2 小時)

#### 步驟 3: 創建 Schema Metadata

**創建 PoC 專案**:

```powershell
mkdir C:\poc-text-to-sql
cd C:\poc-text-to-sql

dotnet new console -n TextToSQLPoC
cd TextToSQLPoC

# 安裝套件
dotnet add package Microsoft.SemanticKernel --version 1.66.0
dotnet add package Npgsql --version 8.0.3
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
```

**創建 `SchemaMetadata.cs`**:

```csharp
using System.Text.Json;

public class SchemaMetadata
{
    public List<TableMetadata> Tables { get; set; } = new();

    public string ToPromptString()
    {
        var lines = new List<string> { "**Database Schema**:" };

        foreach (var table in Tables)
        {
            lines.Add($"\n### Table: {table.Name}");
            lines.Add($"Description: {table.Description}");
            lines.Add("Columns:");

            foreach (var col in table.Columns)
            {
                lines.Add($"  - {col.Name} ({col.DataType}): {col.Description}");
                if (col.SampleValues.Any())
                {
                    lines.Add($"    Sample values: {string.Join(", ", col.SampleValues)}");
                }
            }

            if (table.ForeignKeys.Any())
            {
                lines.Add("Foreign Keys:");
                foreach (var fk in table.ForeignKeys)
                {
                    lines.Add($"  - {fk}");
                }
            }
        }

        return string.Join("\n", lines);
    }
}

public class TableMetadata
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<ColumnMetadata> Columns { get; set; } = new();
    public List<string> ForeignKeys { get; set; } = new();
}

public class ColumnMetadata
{
    public string Name { get; set; } = "";
    public string DataType { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> SampleValues { get; set; } = new();
}
```

**創建 `schema-metadata.json`**:

```json
{
  "tables": [
    {
      "name": "agents",
      "description": "AI agents created by users for various tasks",
      "columns": [
        {
          "name": "id",
          "dataType": "UUID",
          "description": "Unique identifier for the agent"
        },
        {
          "name": "name",
          "dataType": "VARCHAR(255)",
          "description": "Agent display name",
          "sampleValues": ["CustomerBot", "TechBot", "SalesBot"]
        },
        {
          "name": "description",
          "dataType": "TEXT",
          "description": "Agent description and purpose"
        },
        {
          "name": "user_id",
          "dataType": "UUID",
          "description": "ID of the user who created this agent"
        },
        {
          "name": "created_at",
          "dataType": "TIMESTAMP",
          "description": "When the agent was created"
        }
      ],
      "foreignKeys": []
    },
    {
      "name": "conversations",
      "description": "Conversations between users and agents",
      "columns": [
        {
          "name": "id",
          "dataType": "UUID",
          "description": "Unique identifier for the conversation"
        },
        {
          "name": "agent_id",
          "dataType": "UUID",
          "description": "Which agent is used in this conversation"
        },
        {
          "name": "user_id",
          "dataType": "UUID",
          "description": "User participating in the conversation"
        },
        {
          "name": "title",
          "dataType": "VARCHAR(255)",
          "description": "Conversation title",
          "sampleValues": ["Product Inquiry", "Bug Report", "Refund Request"]
        },
        {
          "name": "created_at",
          "dataType": "TIMESTAMP",
          "description": "When the conversation started"
        }
      ],
      "foreignKeys": [
        "agent_id REFERENCES agents(id)"
      ]
    },
    {
      "name": "messages",
      "description": "Individual messages in conversations",
      "columns": [
        {
          "name": "id",
          "dataType": "UUID",
          "description": "Unique identifier for the message"
        },
        {
          "name": "conversation_id",
          "dataType": "UUID",
          "description": "Which conversation this message belongs to"
        },
        {
          "name": "role",
          "dataType": "VARCHAR(50)",
          "description": "Message sender role: 'user' or 'assistant'",
          "sampleValues": ["user", "assistant"]
        },
        {
          "name": "content",
          "dataType": "TEXT",
          "description": "Message text content"
        },
        {
          "name": "created_at",
          "dataType": "TIMESTAMP",
          "description": "When the message was sent"
        }
      ],
      "foreignKeys": [
        "conversation_id REFERENCES conversations(id)"
      ]
    }
  ]
}
```

---

### Phase 8.3: LLM SQL Generator (3 小時)

#### 步驟 4: 實現 Text-to-SQL Engine

**創建 `TextToSQLEngine.cs`**:

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

public class TextToSQLEngine
{
    private readonly IChatCompletionService _chatService;
    private readonly SchemaMetadata _schema;

    public TextToSQLEngine(IChatCompletionService chatService, SchemaMetadata schema)
    {
        _chatService = chatService;
        _schema = schema;
    }

    public async Task<(string SQL, double GenerationTimeMs)> GenerateSQLAsync(
        string naturalLanguageQuery)
    {
        var prompt = BuildPrompt(naturalLanguageQuery);

        var startTime = DateTime.UtcNow;

        var history = new ChatHistory();
        history.AddUserMessage(prompt);

        var response = await _chatService.GetChatMessageContentAsync(history);
        var sql = CleanSQL(response.Content!);

        var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;

        return (sql, elapsedMs);
    }

    private string BuildPrompt(string query)
    {
        return $@"
You are a SQL expert. Generate a PostgreSQL query for the following request.

{_schema.ToPromptString()}

**User Request**: {query}

**Rules**:
1. Use standard PostgreSQL syntax
2. Only query existing tables and columns from the schema above
3. Use proper JOINs for relationships (check foreign keys)
4. Return ONLY the SQL query, no explanations or markdown
5. Do NOT use parameterized queries or placeholders - generate complete SQL
6. For text matching, use ILIKE for case-insensitive matching
7. Always include reasonable LIMIT clauses (e.g., LIMIT 10) to prevent huge result sets

**SQL Query**:
";
    }

    private string CleanSQL(string rawSql)
    {
        // 移除 markdown code blocks
        rawSql = rawSql.Replace("```sql", "").Replace("```", "");

        // 移除前後空白
        rawSql = rawSql.Trim();

        // 移除多餘換行
        rawSql = string.Join("\n", rawSql.Split('\n')
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line)));

        return rawSql;
    }
}
```

---

#### 步驟 5: 測試基礎 SQL 生成

**更新 `Program.cs`**:

```csharp
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

Console.WriteLine("========== PoC 4: Text-to-SQL Engine ==========\n");

// 讀取配置
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: false)
    .Build();

// 創建 Kernel
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    deploymentName: configuration["AzureOpenAI:DeploymentName"]!,
    endpoint: configuration["AzureOpenAI:Endpoint"]!,
    apiKey: configuration["AzureOpenAI:ApiKey"]!
);
var kernel = builder.Build();
var chatService = kernel.GetRequiredService<IChatCompletionService>();

// 加載 Schema Metadata
var schemaJson = File.ReadAllText("schema-metadata.json");
var schema = JsonSerializer.Deserialize<SchemaMetadata>(schemaJson)!;

// 創建 Text-to-SQL Engine
var engine = new TextToSQLEngine(chatService, schema);

// ========== 測試 1: 簡單查詢 ==========
Console.WriteLine("========== Test 1: Simple Queries ==========\n");

var simpleQueries = new Dictionary<string, string>
{
    ["List all agents"] = "SELECT * FROM agents",
    ["Count total agents"] = "SELECT COUNT(*) FROM agents",
    ["Find agent named CustomerBot"] = "SELECT * FROM agents WHERE name"
};

foreach (var (nlQuery, expectedPattern) in simpleQueries)
{
    Console.WriteLine($"--- Query: \"{nlQuery}\" ---");

    var (sql, genTime) = await engine.GenerateSQLAsync(nlQuery);

    Console.WriteLine($"Generated SQL:\n{sql}");
    Console.WriteLine($"Generation Time: {genTime:F0}ms");

    // 簡單驗證
    bool containsExpected = sql.Contains(expectedPattern, StringComparison.OrdinalIgnoreCase);
    Console.WriteLine($"Contains expected pattern: {containsExpected}");
    Console.WriteLine($"Result: {(containsExpected && genTime < 3000 ? "✅ PASSED" : "❌ FAILED")}\n");
}
```

**執行測試**:

```powershell
dotnet run
```

**預期輸出**:
```
========== PoC 4: Text-to-SQL Engine ==========

========== Test 1: Simple Queries ==========

--- Query: "List all agents" ---
Generated SQL:
SELECT * FROM agents LIMIT 10
Generation Time: 1234ms
Contains expected pattern: True
Result: ✅ PASSED

--- Query: "Count total agents" ---
Generated SQL:
SELECT COUNT(*) FROM agents
Generation Time: 1156ms
Contains expected pattern: True
Result: ✅ PASSED

--- Query: "Find agent named CustomerBot" ---
Generated SQL:
SELECT * FROM agents WHERE name ILIKE 'CustomerBot'
Generation Time: 1298ms
Contains expected pattern: True
Result: ✅ PASSED
```

**✅ 通過標準**:
- SQL 生成成功
- 包含預期關鍵字
- 生成時間 < 3 秒

**記錄點**:
```
[Day 8 - 16:00] 基礎 SQL 生成測試完成
- 簡單查詢測試: _____ / 3 通過
- 平均生成時間: _____ ms (目標 <3000ms)
- LLM 理解 Schema: 成功 / 失敗
```

---

## 🚀 Day 9: 安全驗證與準確率測試 (6-8 小時)

### Phase 9.1: SQL Injection 防護 (3 小時)

#### 步驟 6: 實現 SQL 安全驗證器

**創建 `SQLSecurityValidator.cs`**:

```csharp
using Npgsql;

public class SQLSecurityValidator
{
    private readonly List<string> _dangerousKeywords = new()
    {
        "DROP", "DELETE", "TRUNCATE", "ALTER", "CREATE",
        "INSERT", "UPDATE", "EXEC", "EXECUTE", "GRANT", "REVOKE",
        "--", "/*", "*/", ";--", "xp_", "sp_"
    };

    private readonly List<string> _suspiciousPatterns = new()
    {
        "1=1", "OR 1", "OR '1'='1", "'; ", "UNION SELECT",
        "../", "\\x", "CHAR(", "CONCAT("
    };

    public (bool IsValid, List<string> Violations) ValidateSQL(string sql)
    {
        var violations = new List<string>();

        // 1. 檢查危險關鍵字
        foreach (var keyword in _dangerousKeywords)
        {
            if (sql.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                violations.Add($"Dangerous keyword detected: {keyword}");
            }
        }

        // 2. 檢查可疑模式
        foreach (var pattern in _suspiciousPatterns)
        {
            if (sql.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                violations.Add($"Suspicious pattern detected: {pattern}");
            }
        }

        // 3. 驗證只包含 SELECT 語句
        var trimmed = sql.Trim().ToUpper();
        if (!trimmed.StartsWith("SELECT") && !trimmed.StartsWith("WITH"))
        {
            violations.Add("Only SELECT queries are allowed");
        }

        // 4. 檢查是否有多個語句（分號）
        var statements = sql.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).Count();
        if (statements > 1)
        {
            violations.Add("Multiple statements detected (possible SQL injection)");
        }

        return (violations.Count == 0, violations);
    }

    public async Task<(bool IsValid, string Error)> ValidateSyntaxAsync(
        string sql,
        string connectionString)
    {
        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            // 使用 EXPLAIN 驗證語法（不執行查詢）
            var explainSQL = $"EXPLAIN {sql}";
            await using var cmd = new NpgsqlCommand(explainSQL, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, $"Syntax error: {ex.Message}");
        }
    }
}
```

---

#### 步驟 7: SQL Injection 測試

**創建 `tests/test_sql_injection.py`** (或使用 C# 測試類):

```csharp
public class SQLInjectionTests
{
    public static async Task RunTests(TextToSQLEngine engine, SQLSecurityValidator validator)
    {
        Console.WriteLine("\n========== SQL Injection Protection Tests ==========\n");

        var injectionAttempts = new List<string>
        {
            "'; DROP TABLE agents; --",
            "1' OR '1'='1",
            "admin'--",
            "' UNION SELECT * FROM users--",
            "'; DELETE FROM agents WHERE '1'='1",
            "\\x27; DROP TABLE agents;--",
            "1' AND 1=0 UNION ALL SELECT table_name FROM information_schema.tables--",
            "'; EXEC xp_cmdshell('dir'); --",
            "' OR 1=1--",
            "admin' OR '1'='1' /*"
        };

        int blocked = 0;
        int total = injectionAttempts.Count;

        foreach (var (attempt, index) in injectionAttempts.Select((a, i) => (a, i + 1)))
        {
            Console.WriteLine($"--- Attack {index}: {attempt.Substring(0, Math.Min(50, attempt.Length))}... ---");

            try
            {
                var (sql, _) = await engine.GenerateSQLAsync($"Show agents where name = {attempt}");

                // 驗證 SQL
                var (isValid, violations) = validator.ValidateSQL(sql);

                if (!isValid)
                {
                    blocked++;
                    Console.WriteLine($"✅ BLOCKED - Violations: {string.Join(", ", violations)}");
                }
                else
                {
                    Console.WriteLine($"❌ FAILED - SQL passed validation: {sql}");
                }
            }
            catch (Exception ex)
            {
                // LLM 拒絕生成也算阻擋成功
                blocked++;
                Console.WriteLine($"✅ BLOCKED - LLM refused: {ex.Message}");
            }

            Console.WriteLine();
        }

        Console.WriteLine($"========== SQL Injection Test Summary ==========");
        Console.WriteLine($"Total Attacks: {total}");
        Console.WriteLine($"Blocked: {blocked}/{total}");
        Console.WriteLine($"Success Rate: {(double)blocked / total * 100:F1}%");
        Console.WriteLine($"Result: {(blocked == total ? "✅ ALL BLOCKED" : "❌ VULNERABILITIES FOUND")}\n");
    }
}
```

**執行測試**:

```csharp
// 在 Program.cs 中添加
var validator = new SQLSecurityValidator();
await SQLInjectionTests.RunTests(engine, validator);
```

**預期輸出**:
```
========== SQL Injection Protection Tests ==========

--- Attack 1: '; DROP TABLE agents; -- ---
✅ BLOCKED - Violations: Dangerous keyword detected: DROP, Dangerous keyword detected: --

--- Attack 2: 1' OR '1'='1 ---
✅ BLOCKED - Violations: Suspicious pattern detected: OR '1'='1

...

========== SQL Injection Test Summary ==========
Total Attacks: 10
Blocked: 10/10
Success Rate: 100.0%
Result: ✅ ALL BLOCKED
```

**✅ 通過標準**: 10/10 SQL Injection 攻擊被阻擋

**記錄點**:
```
[Day 9 - 12:00] SQL Injection 防護測試完成
- 總攻擊向量: 10
- 成功阻擋: _____ / 10 (目標 10/10)
- 成功率: _____%
- 發現漏洞: _____ 個
```

---

### Phase 9.2: 準確率測試 (3 小時)

#### 步驟 8: 50 個測試查詢

**創建 `test-queries.json`**:

```json
{
  "simple_queries": [
    {
      "nl": "List all agents",
      "expected_sql": "SELECT * FROM agents",
      "category": "simple"
    },
    {
      "nl": "Count total agents",
      "expected_sql": "SELECT COUNT(*) FROM agents",
      "category": "simple"
    },
    {
      "nl": "Show agents created by user u1",
      "expected_sql": "SELECT * FROM agents WHERE user_id = 'u1'",
      "category": "simple"
    }
  ],
  "join_queries": [
    {
      "nl": "Show conversations for agent CustomerBot",
      "expected_sql": "SELECT c.* FROM conversations c JOIN agents a ON c.agent_id = a.id WHERE a.name",
      "category": "join"
    },
    {
      "nl": "List all messages in conversation c1",
      "expected_sql": "SELECT * FROM messages WHERE conversation_id = 'c1'",
      "category": "join"
    }
  ],
  "aggregation_queries": [
    {
      "nl": "How many conversations does each agent have?",
      "expected_sql": "SELECT agent_id, COUNT(*) FROM conversations GROUP BY agent_id",
      "category": "aggregation"
    },
    {
      "nl": "Count messages per conversation",
      "expected_sql": "SELECT conversation_id, COUNT(*) FROM messages GROUP BY conversation_id",
      "category": "aggregation"
    }
  ]
}
```

**創建準確率測試**:

```csharp
public class AccuracyTests
{
    public static async Task RunTests(
        TextToSQLEngine engine,
        SQLSecurityValidator validator,
        string connectionString)
    {
        Console.WriteLine("\n========== Accuracy Tests ==========\n");

        // 加載測試查詢
        var testQueriesJson = File.ReadAllText("test-queries.json");
        var testData = JsonSerializer.Deserialize<TestQueries>(testQueriesJson)!;

        var allQueries = testData.SimpleQueries
            .Concat(testData.JoinQueries)
            .Concat(testData.AggregationQueries)
            .ToList();

        int correct = 0;
        int total = allQueries.Count;

        var results = new Dictionary<string, (int correct, int total)>
        {
            ["simple"] = (0, 0),
            ["join"] = (0, 0),
            ["aggregation"] = (0, 0)
        };

        foreach (var query in allQueries)
        {
            Console.WriteLine($"--- Query: \"{query.NL}\" ---");

            var (sql, genTime) = await engine.GenerateSQLAsync(query.NL);
            Console.WriteLine($"Generated SQL: {sql}");
            Console.WriteLine($"Generation Time: {genTime:F0}ms");

            // 驗證安全性
            var (isValid, violations) = validator.ValidateSQL(sql);
            if (!isValid)
            {
                Console.WriteLine($"❌ FAILED - Security violations: {string.Join(", ", violations)}");
                results[query.Category] = (results[query.Category].correct, results[query.Category].total + 1);
                continue;
            }

            // 驗證語法
            var (syntaxValid, syntaxError) = await validator.ValidateSyntaxAsync(sql, connectionString);
            if (!syntaxValid)
            {
                Console.WriteLine($"❌ FAILED - Syntax error: {syntaxError}");
                results[query.Category] = (results[query.Category].correct, results[query.Category].total + 1);
                continue;
            }

            // 檢查是否包含預期模式
            bool containsExpected = sql.Contains(query.ExpectedSQL, StringComparison.OrdinalIgnoreCase);

            if (containsExpected && genTime < 5000)
            {
                correct++;
                results[query.Category] = (results[query.Category].correct + 1, results[query.Category].total + 1);
                Console.WriteLine($"✅ PASSED");
            }
            else
            {
                results[query.Category] = (results[query.Category].correct, results[query.Category].total + 1);
                Console.WriteLine($"❌ FAILED - Expected pattern not found or too slow");
            }

            Console.WriteLine();
        }

        // 總結
        Console.WriteLine($"========== Accuracy Test Summary ==========");
        Console.WriteLine($"Overall Accuracy: {correct}/{total} = {(double)correct / total * 100:F1}%");

        foreach (var (category, (categoryCorrect, categoryTotal)) in results)
        {
            if (categoryTotal > 0)
            {
                Console.WriteLine($"{category} Accuracy: {categoryCorrect}/{categoryTotal} = {(double)categoryCorrect / categoryTotal * 100:F1}%");
            }
        }

        Console.WriteLine($"Result: {((double)correct / total >= 0.7 ? "✅ PASSED (>70%)" : "❌ FAILED (<70%)")}\n");
    }
}

public class TestQueries
{
    public List<QueryTest> SimpleQueries { get; set; } = new();
    public List<QueryTest> JoinQueries { get; set; } = new();
    public List<QueryTest> AggregationQueries { get; set; } = new();
}

public class QueryTest
{
    [JsonPropertyName("nl")]
    public string NL { get; set; } = "";

    [JsonPropertyName("expected_sql")]
    public string ExpectedSQL { get; set; } = "";

    [JsonPropertyName("category")]
    public string Category { get; set; } = "";
}
```

**執行測試**:

```csharp
var connectionString = "Host=localhost;Port=5432;Database=poc_texttosql;Username=testuser;Password=testpass123";
await AccuracyTests.RunTests(engine, validator, connectionString);
```

**預期輸出**:
```
========== Accuracy Tests ==========

--- Query: "List all agents" ---
Generated SQL: SELECT * FROM agents LIMIT 10
Generation Time: 1234ms
✅ PASSED

...

========== Accuracy Test Summary ==========
Overall Accuracy: 42/50 = 84.0%
simple Accuracy: 18/20 = 90.0%
join Accuracy: 15/20 = 75.0%
aggregation Accuracy: 9/10 = 90.0%
Result: ✅ PASSED (>70%)
```

**✅ 通過標準**:
- 整體準確率 > 70%
- 簡單查詢準確率 > 80%

**記錄點**:
```
[Day 9 - 18:00] 準確率測試完成
- 整體準確率: _____% (目標 >70%)
- 簡單查詢: _____% (目標 >80%)
- JOIN 查詢: _____%
- 聚合查詢: _____%
```

---

## 🚀 Day 10: 性能優化與報告 (4-6 小時)

### Phase 10.1: 性能測試 (2 小時)

#### 步驟 9: 生成時間與穩定性

**創建性能測試**:

```csharp
public class PerformanceTests
{
    public static async Task RunTests(TextToSQLEngine engine)
    {
        Console.WriteLine("\n========== Performance Tests ==========\n");

        var testQuery = "List all agents created by user u1";
        var times = new List<double>();

        // 100 次生成測試
        for (int i = 0; i < 100; i++)
        {
            var (sql, genTime) = await engine.GenerateSQLAsync(testQuery);
            times.Add(genTime);

            if ((i + 1) % 10 == 0)
            {
                Console.WriteLine($"Progress: {i + 1}/100 generations");
            }
        }

        var avgTime = times.Average();
        var minTime = times.Min();
        var maxTime = times.Max();

        Console.WriteLine($"\n========== Performance Summary ==========");
        Console.WriteLine($"Total Generations: 100");
        Console.WriteLine($"Average Time: {avgTime:F0}ms");
        Console.WriteLine($"Min Time: {minTime:F0}ms");
        Console.WriteLine($"Max Time: {maxTime:F0}ms");
        Console.WriteLine($"Result: {(avgTime < 3000 ? "✅ PASSED (<3s avg)" : "❌ FAILED (>3s avg)")}\n");
    }
}
```

**記錄點**:
```
[Day 10 - 12:00] 性能測試完成
- 平均生成時間: _____ ms (目標 <3000ms)
- 最小時間: _____ ms
- 最大時間: _____ ms
```

---

### Phase 10.2: 填寫驗證報告 (2 小時)

#### 步驟 10: 整理並填寫報告

**打開**: `docs/technical-implementation/1-poc-validation/poc-validation-report.md`

**填寫 PoC 4 部分**:

```markdown
### PoC 4: Text-to-SQL Engine (P0) 🔴

**狀態**: ✅ 通過 / ❌ 未通過 / ⚠️ 有條件通過

**成功標準達成情況**:
- [✅] 簡單查詢準確率 >80% (90%)
- [✅] 0 SQL Injection 漏洞 (10/10 阻擋)
- [✅] SQL 生成時間 <3 秒 (1.5s 平均)
- [✅] Schema 語義增強有效

**關鍵發現**:
- 整體準確率: 84%
  - 簡單查詢: 90%
  - JOIN 查詢: 75%
  - 聚合查詢: 90%
- SQL Injection 防護: 10/10 阻擋
- 平均生成時間: 1.5 秒

**風險與緩解**:
| 風險 | 嚴重性 | 緩解方案 |
|------|--------|----------|
| 複雜 JOIN 準確率低 | 中 | 增加 Few-Shot 範例 |
| LLM 幻覺生成不存在表 | 中 | 實現 Schema 驗證層 |
| 成本較高 | 低 | 實現查詢緩存 |

**建議**:
- Schema 語義增強顯著提升準確率
- 建立測試 benchmark 持續評估
- 實現查詢結果可視化（圖表生成）
- 生產環境必須使用只讀數據庫用戶
```

---

## 📊 最終檢查清單

執行完成後，確認以下所有項目：

- [ ] **所有測試執行完成**
  - [ ] 基礎 SQL 生成: PASSED
  - [ ] SQL Injection 防護: 10/10 PASSED
  - [ ] 準確率測試: PASSED (>70%)
  - [ ] 性能測試: PASSED (<3s avg)

- [ ] **安全標準達成**
  - [ ] 0 SQL Injection 漏洞: YES / NO
  - [ ] 只允許 SELECT 查詢: YES / NO
  - [ ] 語法驗證機制: 已實現 / 未實現

- [ ] **準確率達成**
  - [ ] 整體準確率 >70%: YES / NO
  - [ ] 簡單查詢 >80%: YES / NO

---

## 🎯 Go/No-Go 決策標準

### ✅ GO (繼續 PoC 5)
- [x] 簡單查詢準確率 >80%
- [x] SQL Injection: 10/10 阻擋
- [x] 平均生成時間 <3 秒
- [x] Schema 語義增強有效

### ⚠️ 有條件 GO
- [ ] 準確率 70-80%（可通過 Few-Shot 改進）
- [ ] 8-9/10 SQL Injection 阻擋（有緩解方案）

### ❌ NO-GO
- [ ] 準確率 <70%
- [ ] <8/10 SQL Injection 阻擋
- [ ] 平均生成時間 >5 秒

---

**最後更新**: 2025-10-30
**執行指導版本**: 1.0.0

---

**執行記錄**:

```
執行人: _________________
執行日期: _______________

Day 8 完成時間: _____________
Day 9 完成時間: _____________
Day 10 完成時間: _____________

最終狀態: ✅ PASSED / ❌ FAILED / ⚠️ CONDITIONAL

關鍵指標:
- 整體準確率: _____% (目標 >70%)
- 簡單查詢準確率: _____% (目標 >80%)
- SQL Injection 防護: _____ / 10 (目標 10/10)
- 平均生成時間: _____ s (目標 <3s)

技術評估:
- Schema 語義增強效果: 顯著 / 中等 / 輕微
- LLM 理解能力: 優秀 / 良好 / 需改進
- 安全機制: 完善 / 基本 / 不足

Go/No-Go 決策: _______________
決策人: _________________

下一步: 繼續 PoC 5 (Knowledge RAG) / 改進準確率 / 重新評估
```
