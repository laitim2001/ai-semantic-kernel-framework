# PoC 2: Persona Builder - 實際執行指導

**目的**: 逐步執行 PoC 2 驗證，構建可配置的 Persona 系統

**預計時間**: 2 days (Day 3-4)
**執行人**: AI/ML Tech Lead
**執行日期**: _____________

**前置條件**: PoC 1 已通過 ✅

---

## 📋 執行前檢查清單

### 必備條件 Checklist

在開始執行前，請確認以下所有項目：

- [ ] **PoC 1 完成確認**
  - [ ] PoC 1 狀態: PASSED ✅
  - [ ] Semantic Kernel Agent 運行正常
  - [ ] 開發環境已就緒

- [ ] **開發環境**
  - [ ] .NET 8.0 SDK 已安裝
  - [ ] PoC 2 專案目錄已創建 (`poc-persona-builder/`)
  - [ ] Azure OpenAI API 可用 (用於 GPT-4 as Judge)

- [ ] **NuGet 套件準備**
  - [ ] Scriban (模板引擎)
  - [ ] Json.NET (JSON Schema 驗證)
  - [ ] Semantic Kernel (Agent 整合)

- [ ] **測試數據準備**
  - [ ] 至少 3 個測試 Persona 配置
  - [ ] 10+ 個測試響應樣本

- [ ] **時間安排**
  - [ ] Day 3: 4-6 小時 (Schema + 模板引擎)
  - [ ] Day 4: 4-6 小時 (GPT-4 Judge + 一致性測試)

### PoC 2 核心目標

1. ✅ **Persona 配置驗證**: JSON Schema 定義與驗證
2. ✅ **System Prompt 生成**: 模板引擎實現 (Scriban)
3. ✅ **Few-Shot 範例注入**: 動態範例管理
4. ✅ **一致性驗證**: GPT-4 as Judge 評估機制
5. ✅ **性能基準**: Prompt 生成時間 <100ms

---

## 🚀 Day 3: Schema 驗證與模板引擎 (4-6 小時)

### Phase 3.1: 專案設置 (30 分鐘)

#### 步驟 1: 創建 PoC 2 專案

```powershell
# 創建專案目錄
mkdir C:\poc-persona-builder
cd C:\poc-persona-builder

# 創建 .NET Console 專案
dotnet new console -n PersonaBuilderPoC
cd PersonaBuilderPoC

# 安裝必要套件
dotnet add package Microsoft.SemanticKernel --version 1.66.0
dotnet add package Scriban --version 5.10.0
dotnet add package Newtonsoft.Json.Schema --version 3.0.15
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json

# 驗證專案創建
dotnet build
```

**預期輸出**:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

**✅ 通過標準**: 專案創建成功，所有套件安裝完成

**記錄點**:
```
[Day 3 - 10:00] 專案設置完成
- 專案路徑: C:\poc-persona-builder\PersonaBuilderPoC
- Scriban 版本: 5.10.0
- Build 時間: _____ 秒
```

---

#### 步驟 2: 創建 Persona 配置模型

**創建 `PersonaConfig.cs`**:

```csharp
using System.Text.Json.Serialization;

public class PersonaConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("role")]
    public string Role { get; set; } = "";

    [JsonPropertyName("personality")]
    public PersonalityConfig Personality { get; set; } = new();

    [JsonPropertyName("expertise")]
    public List<string> Expertise { get; set; } = new();

    [JsonPropertyName("constraints")]
    public List<string> Constraints { get; set; } = new();

    [JsonPropertyName("few_shot_examples")]
    public List<FewShotExample> FewShotExamples { get; set; } = new();
}

public class PersonalityConfig
{
    [JsonPropertyName("tone")]
    public string Tone { get; set; } = "professional"; // formal, casual, friendly, professional

    [JsonPropertyName("style")]
    public string Style { get; set; } = "concise"; // concise, detailed, conversational, technical

    [JsonPropertyName("traits")]
    public List<string> Traits { get; set; } = new();
}

public class FewShotExample
{
    [JsonPropertyName("user_input")]
    public string UserInput { get; set; } = "";

    [JsonPropertyName("assistant_response")]
    public string AssistantResponse { get; set; } = "";
}
```

**編譯驗證**:

```powershell
dotnet build
```

**✅ 通過標準**: 編譯成功，無警告

---

### Phase 3.2: JSON Schema 驗證 (1 小時)

#### 步驟 3: 實現 Schema 驗證器

**創建 `PersonaValidator.cs`**:

```csharp
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

public class PersonaValidator
{
    private readonly JSchema _schema;

    public PersonaValidator()
    {
        // 定義 JSON Schema
        _schema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""required"": [""name"", ""role"", ""personality"", ""expertise""],
            ""properties"": {
                ""name"": { ""type"": ""string"", ""minLength"": 1 },
                ""role"": { ""type"": ""string"", ""minLength"": 1 },
                ""personality"": {
                    ""type"": ""object"",
                    ""properties"": {
                        ""tone"": {
                            ""type"": ""string"",
                            ""enum"": [""formal"", ""casual"", ""friendly"", ""professional""]
                        },
                        ""style"": {
                            ""type"": ""string"",
                            ""enum"": [""concise"", ""detailed"", ""conversational"", ""technical""]
                        },
                        ""traits"": {
                            ""type"": ""array"",
                            ""items"": { ""type"": ""string"" }
                        }
                    }
                },
                ""expertise"": {
                    ""type"": ""array"",
                    ""items"": { ""type"": ""string"" },
                    ""minItems"": 1
                },
                ""constraints"": {
                    ""type"": ""array"",
                    ""items"": { ""type"": ""string"" }
                },
                ""few_shot_examples"": {
                    ""type"": ""array"",
                    ""items"": {
                        ""type"": ""object"",
                        ""required"": [""user_input"", ""assistant_response""],
                        ""properties"": {
                            ""user_input"": { ""type"": ""string"" },
                            ""assistant_response"": { ""type"": ""string"" }
                        }
                    }
                }
            }
        }");
    }

    public (bool IsValid, List<string> Errors) Validate(string jsonConfig)
    {
        try
        {
            var jsonObject = JObject.Parse(jsonConfig);
            bool isValid = jsonObject.IsValid(_schema, out IList<string> errorMessages);

            return (isValid, errorMessages.ToList());
        }
        catch (Exception ex)
        {
            return (false, new List<string> { $"JSON parsing error: {ex.Message}" });
        }
    }

    public (bool IsValid, List<string> Errors) Validate(PersonaConfig config)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(config);
        return Validate(json);
    }
}
```

---

#### 步驟 4: 測試 Schema 驗證

**更新 `Program.cs` 添加測試**:

```csharp
using System.Text.Json;

Console.WriteLine("========== PoC 2: Persona Builder ==========\n");

// ========== 測試 1: Schema 驗證 ==========
Console.WriteLine("========== Test 1: Schema Validation ==========");

var validator = new PersonaValidator();

// Test 1.1: 有效配置
var validConfig = new PersonaConfig
{
    Name = "TechSupportBot",
    Role = "Technical Support Specialist",
    Personality = new PersonalityConfig
    {
        Tone = "friendly",
        Style = "detailed",
        Traits = new List<string> { "patient", "helpful", "knowledgeable" }
    },
    Expertise = new List<string> { "Windows", "macOS", "Linux", "Networking" },
    Constraints = new List<string>
    {
        "Never provide admin passwords",
        "Always verify user identity before sensitive operations"
    },
    FewShotExamples = new List<FewShotExample>
    {
        new()
        {
            UserInput = "My computer won't start",
            AssistantResponse = "I understand how frustrating that must be. Let's troubleshoot step by step."
        }
    }
};

var (isValid1, errors1) = validator.Validate(validConfig);
Console.WriteLine($"\n--- Test 1.1: Valid Configuration ---");
Console.WriteLine($"   Result: {(isValid1 ? "✅ PASSED" : "❌ FAILED")}");
if (!isValid1)
{
    Console.WriteLine($"   Errors: {string.Join(", ", errors1)}");
}

// Test 1.2: 無效配置 (缺少必需字段)
var invalidConfig = @"{
    ""name"": ""InvalidBot"",
    ""personality"": {
        ""tone"": ""invalid_tone"",
        ""style"": ""concise""
    }
}";

var (isValid2, errors2) = validator.Validate(invalidConfig);
Console.WriteLine($"\n--- Test 1.2: Invalid Configuration (Missing Required Fields) ---");
Console.WriteLine($"   Result: {(!isValid2 ? "✅ PASSED (Expected failure)" : "❌ FAILED (Should have failed)")}");
Console.WriteLine($"   Errors detected: {errors2.Count}");
foreach (var error in errors2)
{
    Console.WriteLine($"     - {error}");
}

// Test 1.3: 無效枚舉值
var invalidEnumConfig = new PersonaConfig
{
    Name = "TestBot",
    Role = "Tester",
    Personality = new PersonalityConfig
    {
        Tone = "invalid_tone", // 無效枚舉
        Style = "concise"
    },
    Expertise = new List<string> { "Testing" }
};

var (isValid3, errors3) = validator.Validate(invalidEnumConfig);
Console.WriteLine($"\n--- Test 1.3: Invalid Enum Value ---");
Console.WriteLine($"   Result: {(!isValid3 ? "✅ PASSED (Expected failure)" : "❌ FAILED (Should have failed)")}");
Console.WriteLine($"   Errors: {errors3.Count}");
```

**執行測試**:

```powershell
dotnet run
```

**預期輸出**:
```
========== PoC 2: Persona Builder ==========

========== Test 1: Schema Validation ==========

--- Test 1.1: Valid Configuration ---
   Result: ✅ PASSED

--- Test 1.2: Invalid Configuration (Missing Required Fields) ---
   Result: ✅ PASSED (Expected failure)
   Errors detected: 2
     - Required properties are missing from object: role, expertise.

--- Test 1.3: Invalid Enum Value ---
   Result: ✅ PASSED (Expected failure)
   Errors: 1
```

**✅ 通過標準**:
- 有效配置驗證通過
- 無效配置正確檢測錯誤
- 錯誤訊息清晰

**記錄點**:
```
[Day 3 - 12:00] Schema 驗證完成
- 有效配置測試: PASSED / FAILED
- 無效配置檢測: PASSED / FAILED
- 錯誤訊息質量: 清晰 / 需改進
```

---

### Phase 3.3: Prompt 模板引擎 (2 小時)

#### 步驟 5: 實現 Scriban 模板引擎

**創建 `PromptGenerator.cs`**:

```csharp
using Scriban;
using System.Diagnostics;

public class PromptGenerator
{
    private readonly Template _systemPromptTemplate;

    public PromptGenerator()
    {
        var templateSource = @"
You are {{ name }}, a {{ role }}.

## Personality
- Tone: {{ personality.tone }}
- Style: {{ personality.style }}
- Traits: {{ personality.traits | array.join ', ' }}

## Expertise
{{ for domain in expertise }}
- {{ domain }}
{{ end }}

## Constraints
{{ for constraint in constraints }}
- {{ constraint }}
{{ end }}

{{ if few_shot_examples }}
## Example Interactions
{{ for example in few_shot_examples }}
**User**: {{ example.user_input }}
**Assistant**: {{ example.assistant_response }}

{{ end }}
{{ end }}

Always maintain your persona throughout the conversation.
Remember to be {{ personality.tone }} and provide {{ personality.style }} responses.
";

        _systemPromptTemplate = Template.Parse(templateSource);
    }

    public (string Prompt, long GenerationTimeMs) GenerateSystemPrompt(PersonaConfig config)
    {
        var stopwatch = Stopwatch.StartNew();

        var prompt = _systemPromptTemplate.Render(config);

        stopwatch.Stop();

        return (prompt, stopwatch.ElapsedMilliseconds);
    }
}
```

---

#### 步驟 6: 測試 Prompt 生成

**更新 `Program.cs` 添加 Prompt 生成測試**:

```csharp
// ========== 測試 2: Prompt 生成 ==========
Console.WriteLine("\n========== Test 2: Prompt Generation ==========");

var generator = new PromptGenerator();

// Test 2.1: 生成 System Prompt
var (prompt, genTime) = generator.GenerateSystemPrompt(validConfig);

Console.WriteLine($"\n--- Test 2.1: Generate System Prompt ---");
Console.WriteLine($"   Generation Time: {genTime}ms");
Console.WriteLine($"   Prompt Length: {prompt.Length} characters");
Console.WriteLine($"   Performance: {(genTime < 100 ? "✅ PASSED (<100ms)" : "❌ FAILED (>100ms)")}");

Console.WriteLine($"\n   Generated Prompt Preview:");
Console.WriteLine("   " + new string('-', 60));
var previewLines = prompt.Split('\n').Take(15);
foreach (var line in previewLines)
{
    Console.WriteLine($"   {line}");
}
Console.WriteLine("   " + new string('-', 60));

// Test 2.2: 驗證 Few-Shot 注入
bool fewShotInjected = prompt.Contains("My computer won't start");
Console.WriteLine($"\n--- Test 2.2: Few-Shot Example Injection ---");
Console.WriteLine($"   Few-Shot examples found in prompt: {fewShotInjected}");
Console.WriteLine($"   Result: {(fewShotInjected ? "✅ PASSED" : "❌ FAILED")}");

// Test 2.3: 驗證所有字段注入
bool allFieldsInjected = prompt.Contains("TechSupportBot") &&
                         prompt.Contains("Technical Support Specialist") &&
                         prompt.Contains("friendly") &&
                         prompt.Contains("Windows");

Console.WriteLine($"\n--- Test 2.3: All Fields Injected ---");
Console.WriteLine($"   All required fields present: {allFieldsInjected}");
Console.WriteLine($"   Result: {(allFieldsInjected ? "✅ PASSED" : "❌ FAILED")}");

// Test 2.4: 性能測試 (100 次生成)
Console.WriteLine($"\n--- Test 2.4: Performance Test (100 iterations) ---");
var times = new List<long>();

for (int i = 0; i < 100; i++)
{
    var (_, time) = generator.GenerateSystemPrompt(validConfig);
    times.Add(time);
}

double avgTime = times.Average();
Console.WriteLine($"   Average Generation Time: {avgTime:F2}ms");
Console.WriteLine($"   Min Time: {times.Min()}ms");
Console.WriteLine($"   Max Time: {times.Max()}ms");
Console.WriteLine($"   Result: {(avgTime < 100 ? "✅ PASSED (<100ms avg)" : "❌ FAILED (>100ms avg)")}");
```

**執行測試**:

```powershell
dotnet run
```

**預期輸出**:
```
========== Test 2: Prompt Generation ==========

--- Test 2.1: Generate System Prompt ---
   Generation Time: 12ms
   Prompt Length: 543 characters
   Performance: ✅ PASSED (<100ms)

   Generated Prompt Preview:
   ------------------------------------------------------------
   You are TechSupportBot, a Technical Support Specialist.

   ## Personality
   - Tone: friendly
   - Style: detailed
   - Traits: patient, helpful, knowledgeable
   ...
   ------------------------------------------------------------

--- Test 2.2: Few-Shot Example Injection ---
   Few-Shot examples found in prompt: True
   Result: ✅ PASSED

--- Test 2.3: All Fields Injected ---
   All required fields present: True
   Result: ✅ PASSED

--- Test 2.4: Performance Test (100 iterations) ---
   Average Generation Time: 8.45ms
   Min Time: 6ms
   Max Time: 23ms
   Result: ✅ PASSED (<100ms avg)
```

**✅ 通過標準**:
- Prompt 生成時間 < 100ms
- Few-Shot 範例正確注入
- 所有字段正確渲染
- 性能穩定

**記錄點**:
```
[Day 3 - 15:00] Prompt 模板引擎完成
- 生成時間: _____ ms (目標 <100ms)
- 100 次平均時間: _____ ms
- Few-Shot 注入: 成功 / 失敗
- 字段完整性: 100% / ____%
```

---

## 🚀 Day 4: GPT-4 Judge 與一致性驗證 (4-6 小時)

### Phase 4.1: GPT-4 as Judge 實現 (2 小時)

#### 步驟 7: 實現一致性驗證器

**創建 `PersonaConsistencyValidator.cs`**:

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

public class PersonaConsistencyValidator
{
    private readonly IChatCompletionService _judgeService;

    public PersonaConsistencyValidator(IChatCompletionService judgeService)
    {
        _judgeService = judgeService;
    }

    public async Task<ConsistencyScore> ValidateConsistencyAsync(
        string personaPrompt,
        List<string> responses)
    {
        var prompt = $@"
You are evaluating AI responses for consistency with a given persona description.

**Persona Description**:
{personaPrompt}

**Responses to Evaluate**:
{string.Join("\n\n", responses.Select((r, i) => $"{i + 1}. {r}"))}

**Task**:
Rate each response on a scale of 0-10 for persona consistency, considering:
- Tone alignment (friendly, professional, etc.)
- Style alignment (concise, detailed, etc.)
- Expertise demonstration
- Constraint adherence

**Output Format (JSON only, no explanations)**:
{{
  ""scores"": [score1, score2, ...],
  ""average"": average_score,
  ""reasoning"": ""brief explanation""
}}
";

        var history = new ChatHistory();
        history.AddUserMessage(prompt);

        var response = await _judgeService.GetChatMessageContentAsync(history);
        var result = ParseConsistencyResult(response.Content!);

        return result;
    }

    private ConsistencyScore ParseConsistencyResult(string jsonResponse)
    {
        try
        {
            // 提取 JSON (可能包含 markdown 格式)
            var jsonStart = jsonResponse.IndexOf('{');
            var jsonEnd = jsonResponse.LastIndexOf('}') + 1;
            var json = jsonResponse.Substring(jsonStart, jsonEnd - jsonStart);

            var result = JsonSerializer.Deserialize<ConsistencyScore>(json);
            return result ?? new ConsistencyScore();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   [Warning] Failed to parse GPT-4 Judge response: {ex.Message}");
            return new ConsistencyScore { Scores = new List<double>(), Average = 0, Reasoning = "Parse error" };
        }
    }
}

public class ConsistencyScore
{
    [System.Text.Json.Serialization.JsonPropertyName("scores")]
    public List<double> Scores { get; set; } = new();

    [System.Text.Json.Serialization.JsonPropertyName("average")]
    public double Average { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("reasoning")]
    public string Reasoning { get; set; } = "";
}
```

---

#### 步驟 8: 測試 GPT-4 Judge

**更新 `Program.cs` 添加一致性測試**:

```csharp
// ========== 測試 3: GPT-4 as Judge 一致性驗證 ==========
Console.WriteLine("\n========== Test 3: Persona Consistency Validation ==========");

// 配置 GPT-4 Judge (使用 Azure OpenAI)
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: false)
    .Build();

var judgeBuilder = Kernel.CreateBuilder();
judgeBuilder.AddAzureOpenAIChatCompletion(
    deploymentName: configuration["AzureOpenAI:DeploymentName"]!,
    endpoint: configuration["AzureOpenAI:Endpoint"]!,
    apiKey: configuration["AzureOpenAI:ApiKey"]!
);
var judgeKernel = judgeBuilder.Build();
var judgeService = judgeKernel.GetRequiredService<IChatCompletionService>();

var consistencyValidator = new PersonaConsistencyValidator(judgeService);

// Test 3.1: 一致的響應 (友善客服風格)
var consistentResponses = new List<string>
{
    "I understand how frustrating that must be! Let's work together to solve this step by step.",
    "Great question! I'm happy to help you with that. Here's what you can do...",
    "I appreciate your patience. Let me explain this in detail to make sure everything is clear."
};

Console.WriteLine($"\n--- Test 3.1: Consistent Responses (Friendly Support Style) ---");
Console.WriteLine($"   Evaluating {consistentResponses.Count} responses...");

var startTime = DateTime.UtcNow;
var score1 = await consistencyValidator.ValidateConsistencyAsync(prompt, consistentResponses);
var evalTime1 = DateTime.UtcNow - startTime;

Console.WriteLine($"   Average Consistency Score: {score1.Average:F1}/10");
Console.WriteLine($"   Individual Scores: {string.Join(", ", score1.Scores.Select(s => $"{s:F1}"))}");
Console.WriteLine($"   GPT-4 Reasoning: {score1.Reasoning}");
Console.WriteLine($"   Evaluation Time: {evalTime1.TotalMilliseconds:F0}ms");
Console.WriteLine($"   Result: {(score1.Average >= 7.0 ? "✅ PASSED (>7.0)" : "❌ FAILED (<7.0)")}");

// Test 3.2: 不一致的響應 (混合風格)
var inconsistentResponses = new List<string>
{
    "Your issue can be resolved by executing: sudo rm -rf /", // 違反約束
    "idk bro just restart it lol", // 不專業風格
    "Let me help you troubleshoot this carefully. First, check the power cable." // 正確風格
};

Console.WriteLine($"\n--- Test 3.2: Inconsistent Responses (Mixed Styles) ---");
Console.WriteLine($"   Evaluating {inconsistentResponses.Count} responses...");

startTime = DateTime.UtcNow;
var score2 = await consistencyValidator.ValidateConsistencyAsync(prompt, inconsistentResponses);
var evalTime2 = DateTime.UtcNow - startTime;

Console.WriteLine($"   Average Consistency Score: {score2.Average:F1}/10");
Console.WriteLine($"   Individual Scores: {string.Join(", ", score2.Scores.Select(s => $"{s:F1}"))}");
Console.WriteLine($"   GPT-4 Reasoning: {score2.Reasoning}");
Console.WriteLine($"   Evaluation Time: {evalTime2.TotalMilliseconds:F0}ms");
Console.WriteLine($"   Result: {(score2.Average < 6.0 ? "✅ PASSED (Detected inconsistency)" : "❌ FAILED (Should detect inconsistency)")}");
```

**執行測試**:

```powershell
dotnet run
```

**預期輸出**:
```
========== Test 3: Persona Consistency Validation ==========

--- Test 3.1: Consistent Responses (Friendly Support Style) ---
   Evaluating 3 responses...
   Average Consistency Score: 8.7/10
   Individual Scores: 9.0, 8.5, 8.5
   GPT-4 Reasoning: All responses maintain friendly, detailed support style with patience
   Evaluation Time: 2345ms
   Result: ✅ PASSED (>7.0)

--- Test 3.2: Inconsistent Responses (Mixed Styles) ---
   Evaluating 3 responses...
   Average Consistency Score: 4.3/10
   Individual Scores: 2.0, 3.0, 8.0
   GPT-4 Reasoning: First two responses violate constraints and professionalism
   Evaluation Time: 2567ms
   Result: ✅ PASSED (Detected inconsistency)
```

**✅ 通過標準**:
- 一致響應得分 > 7.0
- 不一致響應得分 < 6.0
- GPT-4 Judge 能正確檢測

**⚠️ 故障排查**:

1. **API 錯誤**:
```
Error: Rate limit exceeded
```
→ 等待 1 分鐘後重試，或降低測試頻率

2. **JSON 解析錯誤**:
```
Failed to parse GPT-4 Judge response
```
→ 檢查 GPT-4 回應格式，可能需要調整 prompt

3. **分數異常**:
```
All scores are 0 or 10
```
→ 檢查 persona prompt 是否清晰，調整評分標準

**記錄點**:
```
[Day 4 - 12:00] GPT-4 Judge 驗證完成
- 一致響應評分: _____ /10 (目標 >7.0)
- 不一致響應評分: _____ /10 (目標 <6.0)
- 評估時間: _____ ms
- GPT-4 Judge 準確性: 高 / 中 / 低
- 發現問題: _____________________
```

---

### Phase 4.2: 整合測試與報告 (2 小時)

#### 步驟 9: 端到端整合測試

**創建 `IntegrationTests.cs`**:

```csharp
public class IntegrationTests
{
    public static async Task RunFullPipeline(
        PersonaConfig config,
        IChatCompletionService judgeService)
    {
        Console.WriteLine("\n========== Integration Test: Full Pipeline ==========");

        // Step 1: 驗證配置
        var validator = new PersonaValidator();
        var (isValid, errors) = validator.Validate(config);
        Console.WriteLine($"\n✅ Step 1: Configuration Validation");
        Console.WriteLine($"   Valid: {isValid}");
        if (!isValid)
        {
            Console.WriteLine($"   Errors: {string.Join(", ", errors)}");
            return;
        }

        // Step 2: 生成 Prompt
        var generator = new PromptGenerator();
        var (prompt, genTime) = generator.GenerateSystemPrompt(config);
        Console.WriteLine($"\n✅ Step 2: Prompt Generation");
        Console.WriteLine($"   Generation Time: {genTime}ms");
        Console.WriteLine($"   Prompt Length: {prompt.Length} chars");

        // Step 3: 創建 Agent 並生成測試響應
        Console.WriteLine($"\n✅ Step 3: Generate Test Responses");
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(/* ... */)
            .Build();
        var chatService = kernel.GetRequiredService<IChatCompletionService>();

        var testInputs = new List<string>
        {
            "Hello, I need help with my computer.",
            "Can you explain how to reset my password?",
            "My laptop is running very slow lately."
        };

        var responses = new List<string>();
        foreach (var input in testInputs)
        {
            var history = new ChatHistory(prompt);
            history.AddUserMessage(input);
            var response = await chatService.GetChatMessageContentAsync(history);
            responses.Add(response.Content!);
            Console.WriteLine($"   Generated response {responses.Count}/3");
        }

        // Step 4: 驗證一致性
        var consistencyValidator = new PersonaConsistencyValidator(judgeService);
        var score = await consistencyValidator.ValidateConsistencyAsync(prompt, responses);

        Console.WriteLine($"\n✅ Step 4: Consistency Validation");
        Console.WriteLine($"   Average Score: {score.Average:F1}/10");
        Console.WriteLine($"   Result: {(score.Average >= 7.0 ? "✅ PASSED" : "❌ FAILED")}");

        // Step 5: 最終結果
        Console.WriteLine($"\n========== Integration Test Result ==========");
        bool allPassed = isValid && genTime < 100 && score.Average >= 7.0;
        Console.WriteLine($"   Overall: {(allPassed ? "✅ ALL TESTS PASSED" : "❌ SOME TESTS FAILED")}");
    }
}
```

---

#### 步驟 10: 填寫驗證報告

根據測試結果，填寫 `poc-validation-report.md` 中的 PoC 2 部分：

**打開**: `docs/technical-implementation/1-poc-validation/poc-validation-report.md`

**填寫 PoC 2 部分**:

```markdown
### PoC 2: Persona Builder (P0) 🔴

**狀態**: ✅ 通過 / ❌ 未通過 / ⚠️ 有條件通過

**成功標準達成情況**:
- [✅] Prompt 生成正確率 >90%
- [✅] Persona 一致性分數 >70%
- [✅] Few-Shot 範例注入成功
- [✅] 模板引擎穩定運行

**關鍵發現**:
- Prompt 生成正確率: 100%
- Persona 一致性分數: 87%
- 模板引擎選擇: Scriban (性能優異)
- GPT-4 as Judge 成本: ~$0.01/評估

**風險與緩解**:
| 風險 | 嚴重性 | 緩解方案 |
|------|--------|----------|
| GPT-4 Judge 成本高 | 中 | 實現緩存機制，批量評估 |
| Few-Shot 範例質量 | 中 | 建立範例庫和質量審查流程 |

**建議**:
- Scriban 模板引擎表現優異，推薦採用
- 建立 Persona 模板庫 (客服、技術、銷售等)
- 實現 Persona 測試集自動化評估
```

---

## 📊 最終檢查清單

執行完成後，確認以下所有項目：

- [ ] **所有測試執行完成**
  - [ ] Schema 驗證測試: PASSED
  - [ ] Prompt 生成測試: PASSED
  - [ ] Few-Shot 注入測試: PASSED
  - [ ] GPT-4 Judge 測試: PASSED
  - [ ] 整合測試: PASSED

- [ ] **性能指標達成**
  - [ ] Prompt 生成時間 < 100ms: YES / NO
  - [ ] Prompt 生成正確率 > 90%: YES / NO
  - [ ] Persona 一致性分數 > 70%: YES / NO

- [ ] **驗證報告填寫完成**
  - [ ] 成功標準達成情況已記錄
  - [ ] 關鍵發現已整理
  - [ ] 技術選擇建議已提出 (Scriban vs Handlebars)

- [ ] **決策準備**
  - [ ] 所有 P0 成功標準達成: YES / NO
  - [ ] Go/No-Go 決策: GO ✅ / NO-GO ❌

---

## 🎯 Go/No-Go 決策標準

### ✅ GO (繼續 PoC 3)
- [x] Prompt 生成正確率 > 90%
- [x] Persona 一致性分數 > 70%
- [x] Few-Shot 範例注入成功
- [x] 模板引擎穩定運行

### ⚠️ 有條件 GO
- [ ] Persona 一致性分數 60-70% (需改進 prompt 設計)
- [ ] GPT-4 Judge 成本較高 (需實現緩存)

### ❌ NO-GO
- [ ] Prompt 生成正確率 < 80%
- [ ] Persona 一致性分數 < 60%
- [ ] 模板引擎嚴重性能問題

---

## 🔗 相關資源

- **PoC 2 詳細文檔**: [02-persona-builder.md](./02-persona-builder.md)
- **PoC 1 執行指導**: [01-semantic-kernel-agents-EXECUTION-GUIDE.md](./01-semantic-kernel-agents-EXECUTION-GUIDE.md)
- **驗證報告模板**: [poc-validation-report.md](./poc-validation-report.md)
- **Scriban 文檔**: https://github.com/scriban/scriban

---

**最後更新**: 2025-10-30
**執行指導版本**: 1.0.0

---

**執行記錄**:

```
執行人: _________________
執行日期: _______________

Day 3 完成時間: _____________
Day 4 完成時間: _____________

最終狀態: ✅ PASSED / ❌ FAILED / ⚠️ CONDITIONAL

關鍵指標:
- Prompt 生成時間: _____ ms (目標 <100ms)
- Prompt 正確率: _____% (目標 >90%)
- Persona 一致性分數: _____ (目標 >7.0)
- GPT-4 Judge 準確性: _____

技術選擇建議:
- 模板引擎: Scriban / Handlebars / 其他
- 理由: _______________________________

Go/No-Go 決策: _______________
決策人: _________________
決策日期: _______________

下一步: 繼續 PoC 3 (Code Interpreter) / 調整方案 / 重新評估
```
