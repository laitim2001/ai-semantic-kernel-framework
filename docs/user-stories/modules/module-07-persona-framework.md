# 模組 07: Persona Framework

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 07
**User Stories**: US 7.1-7.4
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 7: Persona Framework ⭐ 核心差異化能力

**Epic**: 實現可配置的 Agent 個性化系統，讓每個 Agent 具有獨特的對話風格、行為模式和專業領域

**差異化價值**:
- 超越 Copilot Studio 的個性化能力
- 讓 Agent 不只是工具，而是具有個性的助手
- 提升用戶體驗和情感連結

**User Stories 數量**: 4 個（US 7.1 - 7.4）

---

#### US 7.1 - Persona 模板配置

**作為** 業務分析師
**我想要** 透過配置檔定義 Agent 的個性和行為
**以便** 快速建立具有特定風格的 Agent

**驗收標準**:

✅ 必須項（Persona 配置）:
- [ ] 支援 JSON/YAML 格式的 Persona 配置檔
- [ ] 定義 Agent 的姓名、角色、專業領域
- [ ] 定義對話風格（正式/輕鬆、簡潔/詳細）
- [ ] 定義回應模式（主動/被動、探索式/指令式）
- [ ] 定義專業術語使用偏好
- [ ] 定義禁止使用的詞彙或主題（安全守則）

✅ 必須項（預設 Persona 模板）:
- [ ] 提供 5+ 個預設模板（專業顧問、技術專家、客服助理、創意夥伴、數據分析師）
- [ ] 每個模板包含完整配置範例
- [ ] 模板可直接使用或作為起點修改

✅ 必須項（驗證機制）:
- [ ] 配置檔格式驗證（Schema Validation）
- [ ] 必填欄位檢查
- [ ] 禁止詞彙清單驗證
- [ ] 配置衝突檢測（例如：正式風格 + 過於隨意的用詞）

**配置範例**:
```yaml
# persona-config.yaml
persona:
  name: "Alice"
  role: "技術顧問"
  expertise: ["雲端架構", "DevOps", "安全性"]

  communication_style:
    formality: "professional"      # professional, casual, friendly
    verbosity: "balanced"           # concise, balanced, detailed
    tone: "encouraging"             # neutral, encouraging, assertive

  response_pattern:
    proactivity: "moderate"         # passive, moderate, proactive
    style: "exploratory"            # directive, exploratory, collaborative

  language_preferences:
    technical_terms: "always_explain"  # use_freely, explain_when_complex, always_explain
    examples: "provide_when_helpful"   # minimal, provide_when_helpful, abundant

  safety_guardrails:
    forbidden_topics: ["政治", "宗教"]
    forbidden_words: ["絕對", "保證", "一定"]
    max_speculation: "low"          # none, low, moderate

  personality_traits:
    - "耐心解釋複雜概念"
    - "使用類比和實例"
    - "鼓勵最佳實踐"
    - "避免過度技術術語"
```

**技術實現**:
- JSON Schema 或 YAML Schema 驗證
- Persona 配置載入器
- 預設模板資料庫
- 配置熱重載（開發模式）

**📊 優先級**: P0 (MVP 必須) - 核心差異化能力
**🎯 UX 目標**: 配置檔載入 <100ms
**🔗 相關**: US 1.2 (Agent 配置), US 7.2 (Prompt Engineering)

---

#### US 7.2 - Persona-Driven Prompt Engineering

**作為** AI Agent
**我想要** 根據 Persona 配置動態調整我的 System Prompt
**以便** 展現一致的個性和行為

**驗收標準**:

✅ 必須項（Prompt 模板系統）:
- [ ] System Prompt 模板引擎（支援變數插值）
- [ ] Persona 配置自動轉換為 Prompt 指令
- [ ] 多層次 Prompt 組合（基礎 Prompt + Persona Prompt + Task Prompt）
- [ ] Prompt 版本控制和 A/B 測試支援

✅ 必須項（Persona 行為注入）:
- [ ] 對話風格指令自動生成（"以專業但友善的語氣回應..."）
- [ ] 回應長度控制（根據 verbosity 設定）
- [ ] 範例提供策略（根據 examples 偏好）
- [ ] 專業術語解釋策略（根據 technical_terms 設定）

✅ 必須項（安全守則注入）:
- [ ] 禁止主題和詞彙自動加入 Prompt
- [ ] 思辨限制設定（"避免過度推測..."）
- [ ] 道德和法律守則（"拒絕不當請求..."）
- [ ] 品牌和語氣一致性守則

✅ 必須項（動態 Prompt 調整）:
- [ ] 根據對話歷史調整 Prompt（例如：用戶多次要求簡潔 → 自動切換到簡潔模式）
- [ ] 根據任務類型調整（例如：程式碼生成 → 增加技術細節）
- [ ] A/B 測試不同 Prompt 版本

**Prompt 生成範例**:
```python
# 基於 Persona 配置自動生成的 System Prompt

# === 基礎 Prompt ===
You are Alice, a professional technical consultant specializing in cloud architecture, DevOps, and security.

# === 風格指令（基於 communication_style）===
- Maintain a professional and encouraging tone
- Provide balanced detail level (neither too brief nor too verbose)
- Use exploratory approach: ask clarifying questions when needed

# === 語言策略（基於 language_preferences）===
- Always explain technical terms when first introduced
- Provide examples when helpful for understanding
- Use analogies to clarify complex concepts

# === 安全守則（基於 safety_guardrails）===
- NEVER discuss politics or religion
- AVOID using absolute terms like "絕對", "保證", "一定"
- Keep speculation to minimum; clearly mark uncertain information

# === 個性特質（基於 personality_traits）===
- Be patient when explaining complex concepts
- Use real-world examples and analogies
- Encourage best practices
- Avoid unnecessary technical jargon
```

**技術實現**:
- Jinja2 或 Handlebars 模板引擎
- Prompt 組合邏輯
- 動態 Prompt 調整演算法
- A/B 測試框架

**📊 優先級**: P0 (MVP 必須) - 核心實現
**🎯 技術目標**: Prompt 生成 <50ms
**🔗 相關**: US 7.1 (Persona 配置), US 1.3 (LLM 整合)

---

#### US 7.3 - 對話風格一致性驗證

**作為** 品質保證人員
**我想要** 驗證 Agent 的回應是否符合 Persona 定義
**以便** 確保對話品質和一致性

**驗收標準**:

✅ 必須項（風格指標檢測）:
- [ ] 語氣分析（Sentiment Analysis）：檢測回應是否符合設定的 tone
- [ ] 詳細程度檢測（Response Length Analysis）：驗證是否符合 verbosity 設定
- [ ] 專業術語使用檢測：確認術語解釋策略是否執行
- [ ] 禁用詞彙掃描：自動檢測 forbidden_words 和 forbidden_topics

✅ 必須項（一致性評分）:
- [ ] 對每個回應計算 Persona 一致性分數（0-100%）
- [ ] 分數過低時自動告警（例如 <70%）
- [ ] 提供改進建議（"回應過於簡短，建議增加範例"）
- [ ] 持續監控和趨勢分析

✅ 必須項（測試框架）:
- [ ] Persona 單元測試（給定相同問題，不同 Persona 應有不同回應風格）
- [ ] 回歸測試（Persona 更新後，確保風格改變符合預期）
- [ ] 對話流程測試（多輪對話中 Persona 保持一致）

✅ 必須項（視覺化儀表板）:
- [ ] 即時風格一致性監控
- [ ] 違反安全守則的告警
- [ ] 對話品質趨勢圖
- [ ] Persona 表現對比（不同 Agent 的風格差異）

**檢測範例**:
```python
# 風格檢測報告
{
  "message_id": "msg_12345",
  "persona": "Alice (技術顧問)",
  "consistency_score": 85,  # 總體一致性分數

  "analysis": {
    "tone": {
      "expected": "professional + encouraging",
      "detected": "professional",
      "score": 90,
      "note": "缺少鼓勵性語氣"
    },
    "verbosity": {
      "expected": "balanced",
      "word_count": 150,
      "score": 95,
      "note": "符合預期"
    },
    "technical_terms": {
      "terms_used": ["Kubernetes", "CI/CD"],
      "explained": ["Kubernetes"],
      "not_explained": ["CI/CD"],
      "score": 70,
      "note": "CI/CD 應該解釋"
    },
    "forbidden_content": {
      "violations": [],
      "score": 100
    }
  },

  "recommendations": [
    "增加鼓勵性語句，例如「這是個好問題！」",
    "解釋 CI/CD 概念，因為設定為 always_explain"
  ]
}
```

**技術實現**:
- NLP 語氣分析（HuggingFace Transformers）
- 詞彙掃描和模式匹配
- 統計分析和評分演算法
- 即時監控儀表板（Grafana 或自訂前端）

**📊 優先級**: P1 (MVP 高優先) - 品質保證
**🎯 技術目標**: 檢測延遲 <200ms（不阻塞回應）
**🔗 相關**: US 7.2 (Prompt Engineering), US 10.2 (品質監控)

---

#### US 7.4 - Persona 演化和學習（Phase 2）

**作為** 系統管理員
**我想要** Agent 能從使用者互動中學習和優化 Persona
**以便** 持續改善用戶體驗

**驗收標準**:

✅ 必須項（使用者反饋收集）:
- [ ] 每個回應都有「滿意度」按鈕（👍 / 👎）
- [ ] 詳細反饋表單（"回應太簡短" / "語氣不對" / "太多技術術語"）
- [ ] 反饋自動關聯到 Persona 配置項目
- [ ] 反饋趨勢分析

✅ 必須項（自動優化建議）:
- [ ] 基於反饋數據分析 Persona 弱點
- [ ] 自動生成優化建議（"考慮將 verbosity 從 balanced 改為 detailed"）
- [ ] A/B 測試優化方案
- [ ] 優化效果驗證

✅ 可選項（AI 驅動的 Persona 調整 - Phase 2）:
- [ ] 使用 LLM 分析對話品質
- [ ] 自動微調 Persona 參數
- [ ] 個性化 Persona（每個用戶有客製化體驗）
- [ ] Persona 版本管理和回滾

**反饋分析範例**:
```yaml
# Persona 優化報告
agent_id: "alice_tech_consultant"
analysis_period: "2025-10-01 to 2025-10-31"

feedback_summary:
  total_interactions: 1500
  thumbs_up: 1200 (80%)
  thumbs_down: 300 (20%)

issues_identified:
  - issue: "回應太簡短"
    frequency: 120 occurrences (40% of complaints)
    related_config: "verbosity: balanced"
    recommendation: "建議改為 verbosity: detailed"

  - issue: "太多技術術語"
    frequency: 90 occurrences (30% of complaints)
    related_config: "technical_terms: use_freely"
    recommendation: "建議改為 technical_terms: explain_when_complex"

optimization_suggestions:
  - action: "調整 verbosity 設定"
    impact: "預期滿意度提升至 85%"
    ab_test: "已建立 A/B 測試（對照組 vs 實驗組）"
```

**技術實現**:
- 反饋資料收集和儲存
- 數據分析和模式識別
- A/B 測試框架
- （Phase 2）LLM-driven Persona 微調

**📊 優先級**: P2 (Phase 2) - 進階優化
**🎯 業務目標**: 滿意度提升 10%+
**🔗 相關**: US 7.3 (風格驗證), US 10.3 (使用者分析)

---