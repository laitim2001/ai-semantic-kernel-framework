# PoC 2: Persona Builder 驗證

**優先級**: 🔴 P0 (最高優先級)
**預計時間**: 2 days (Day 3-4)
**狀態**: ⏳ 待開始
**負責人**: AI/ML Tech Lead

**基於**: Semantic Kernel 1.66+ + Jinja2/Handlebars (TECH-STACK-ANALYSIS v2.0.0)

---

## 🎯 驗證目標

驗證 Persona Builder 框架的核心功能，實現用戶友好的 Persona 配置系統：

1. ✅ **Persona 配置 Schema**: JSON Schema 定義與驗證
2. ✅ **System Prompt 生成**: 模板引擎實現
3. ✅ **Few-Shot 範例注入**: 動態範例管理
4. ✅ **一致性驗證**: GPT-4 as Judge 評估機制
5. ✅ **性能基準**: Prompt 生成時間與質量

---

## 📋 技術背景

### Persona Builder 架構

```
┌─────────────────────────────────────────┐
│         Persona Builder System          │
│  ┌─────────────────────────────────┐   │
│  │   Persona Configuration         │   │
│  │  - JSON Schema Definition       │   │
│  │  - Validation Engine            │   │
│  │  - Default Templates            │   │
│  └─────────────────────────────────┘   │
│  ┌─────────────────────────────────┐   │
│  │   Prompt Template Engine        │   │
│  │  - Jinja2 / Handlebars          │   │
│  │  - Variable Interpolation       │   │
│  │  - Few-Shot Injection           │   │
│  └─────────────────────────────────┘   │
│  ┌─────────────────────────────────┐   │
│  │   Consistency Validation        │   │
│  │  - GPT-4 as Judge               │   │
│  │  - Scoring Mechanism            │   │
│  └─────────────────────────────────┘   │
└─────────────────────────────────────────┘
```

---

## 🔧 環境準備

### Persona 配置 Schema (JSON)

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "required": ["name", "role", "personality", "expertise"],
  "properties": {
    "name": {
      "type": "string",
      "description": "Persona 名稱"
    },
    "role": {
      "type": "string",
      "description": "角色定義（如：客服助理、技術專家）"
    },
    "personality": {
      "type": "object",
      "properties": {
        "tone": { "enum": ["formal", "casual", "friendly", "professional"] },
        "style": { "enum": ["concise", "detailed", "conversational", "technical"] },
        "traits": { "type": "array", "items": { "type": "string" } }
      }
    },
    "expertise": {
      "type": "array",
      "items": { "type": "string" },
      "description": "專業領域"
    },
    "constraints": {
      "type": "array",
      "items": { "type": "string" },
      "description": "行為約束"
    },
    "few_shot_examples": {
      "type": "array",
      "items": {
        "type": "object",
        "properties": {
          "user_input": { "type": "string" },
          "assistant_response": { "type": "string" }
        }
      }
    }
  }
}
```

---

## 💻 實現步驟

### 步驟 1: Persona 配置驗證

**測試 Persona 配置**:

```json
{
  "name": "TechSupportBot",
  "role": "Technical Support Specialist",
  "personality": {
    "tone": "friendly",
    "style": "detailed",
    "traits": ["patient", "helpful", "knowledgeable"]
  },
  "expertise": ["Windows", "macOS", "Linux", "Networking"],
  "constraints": [
    "Never provide admin passwords",
    "Always verify user identity before sensitive operations",
    "Escalate complex issues to human agents"
  ],
  "few_shot_examples": [
    {
      "user_input": "My computer won't start",
      "assistant_response": "I understand how frustrating that must be. Let's troubleshoot step by step. First, can you tell me if you see any lights or hear any sounds when you press the power button?"
    }
  ]
}
```

### 步驟 2: Prompt 模板引擎 (Jinja2 示例)

**C# 使用 Scriban (Jinja2-like)**:

```csharp
using Scriban;

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
User: {{ example.user_input }}
Assistant: {{ example.assistant_response }}

{{ end }}
{{ end }}

Always maintain your persona throughout the conversation.
";

        _systemPromptTemplate = Template.Parse(templateSource);
    }

    public string GenerateSystemPrompt(PersonaConfig config)
    {
        return _systemPromptTemplate.Render(config);
    }
}
```

### 步驟 3: GPT-4 as Judge 一致性驗證

```csharp
public class PersonaConsistencyValidator
{
    private readonly IChatCompletionService _judgeService;

    public async Task<double> ValidateConsistencyAsync(
        string persona,
        string[] responses)
    {
        var prompt = $@"
Evaluate the following AI responses for consistency with the persona description.

**Persona**:
{persona}

**Responses to Evaluate**:
{string.Join("\n\n", responses.Select((r, i) => $"{i + 1}. {r}"))}

Rate each response on a scale of 0-10 for persona consistency.
Return only the scores as JSON: {{""scores"": [score1, score2, ...]}}
";

        var result = await _judgeService.GetChatMessageContentAsync(prompt);
        // 解析 JSON 並計算平均分
        var scores = ParseScores(result.Content);
        return scores.Average();
    }
}
```

---

## 🧪 測試用例

### 測試套件 1: Persona 配置驗證

| 測試場景 | 配置 | 預期結果 | 成功標準 |
|---------|------|----------|----------|
| TC-1.1 | 有效配置 | 驗證通過 | 100% 通過 |
| TC-1.2 | 缺少必需字段 | 驗證失敗 | 100% 失敗 |
| TC-1.3 | 無效枚舉值 | 驗證失敗 | 100% 失敗 |

### 測試套件 2: Prompt 生成

| 測試場景 | 操作 | 預期結果 | 成功標準 |
|---------|------|----------|----------|
| TC-2.1 | 生成 System Prompt | 正確生成 | 正確率 >90% |
| TC-2.2 | Few-Shot 範例注入 | 正確注入 | 100% 正確 |
| TC-2.3 | 生成時間 | 快速生成 | <100ms |

### 測試套件 3: 一致性驗證

| 測試場景 | Persona | 預期結果 | 成功標準 |
|---------|---------|----------|----------|
| TC-3.1 | 友善客服 | 一致性分數 >70% | 通過 |
| TC-3.2 | 技術專家 | 一致性分數 >70% | 通過 |
| TC-3.3 | 混合 Persona | 檢測不一致 | 分數 <60% |

---

## ✅ 成功標準驗證

### 1. Prompt 生成正確率 >90% ✅ / ❌

**結果**: ✅ / ❌ (實際正確率: _____%);

### 2. Persona 一致性分數 >70% ✅ / ❌

**結果**: ✅ / ❌ (實際分數: _____%);

### 3. Few-Shot 範例注入成功 ✅ / ❌

**結果**: ✅ / ❌

### 4. 模板引擎穩定運行 ✅ / ❌

**結果**: ✅ / ❌

---

## 🔍 關鍵發現

### 技術發現

1. **Prompt 模板引擎選擇**:
   - Scriban (Jinja2-like): ✅ / ❌
   - Handlebars.Net: ✅ / ❌
   - 推薦: _________________________

2. **GPT-4 as Judge 效果**:
   - 評估準確性: _________________________
   - 評估時間: _________________________
   - 成本: _________________________

---

## ⚠️ 風險識別

| 風險 | 嚴重性 | 緩解方案 |
|------|--------|----------|
| GPT-4 Judge 成本高 | 中 | 實現緩存機制 / 批量評估 |
| Persona 一致性難量化 | 中 | 多維度評分 / 人工抽查 |
| Few-Shot 範例質量 | 高 | 建立範例庫 / 質量審查流程 |

---

## 💡 建議與下一步

- 實現 Persona 模板庫
- 設計 Persona 測試集
- 開發 Persona 評估工具

---

**最後更新**: 2025-10-30
**PoC 負責人**: AI/ML Tech Lead

[← 上一個 PoC](./01-semantic-kernel-agents.md) | [下一個 PoC: Code Interpreter →](./03-code-interpreter-sandbox.md)
