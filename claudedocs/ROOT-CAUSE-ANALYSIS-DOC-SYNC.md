# 根本原因分析: PROJECT-STATUS-REPORT.md 不同步問題

**分析日期**: 2025-11-06
**分析者**: AI Assistant (Claude Code)
**問題描述**: PROJECT-STATUS-REPORT.md 內容與實際項目進度不同步
**影響範圍**: 項目狀態追蹤、Sprint 進度管理、團隊溝通

---

## 📋 目錄

1. [問題現象](#問題現象)
2. [根本原因分析](#根本原因分析)
3. [AI-ASSISTANT-INSTRUCTIONS.md 評估](#ai-assistant-instructionsmd-評估)
4. [改進建議](#改進建議)
5. [實施計劃](#實施計劃)

---

## 🔍 問題現象

### 觀察到的不同步情況

#### 實例 1: Sprint 1 集成測試完成 (2025-11-06)
**發生時間**: 2025-11-06 00:00 UTC (Session 5)

**實際狀態**:
- 完成 US 1.2 集成測試 (8 tests, 7/8 passed)
- 完成 US 1.3 Phase 4 集成測試 (8 tests, 8/8 passed)
- 發現並修復 9 個後端問題 (TDD 驅動)
- 集成測試通過率: 96% (25/26)
- 總測試數: 122 tests (97 unit + 25 integration)

**PROJECT-STATUS-REPORT.md 狀態** (在更新前):
- 版本: v5.0.0
- 報告日期: 2025-11-05 (過期 1 天)
- Sprint 1 狀態: "✅ 完成 (US 1.1 + US 1.2 + US 1.3 Phase 2-5)"
- 測試統計: 97 單元測試 (缺少集成測試統計)
- US 1.2 狀態: 標記為 "✅ 已完成", 但沒有集成測試結果
- US 1.3 Phase 4 狀態: 沒有集成測試結果

**不同步內容**:
- ❌ 缺少 US 1.2 集成測試結果 (8 tests)
- ❌ 缺少 US 1.3 Phase 4 集成測試結果 (8 tests)
- ❌ 缺少 9 個後端問題修復記錄
- ❌ 測試統計不完整 (只有單元測試,沒有集成測試)
- ❌ API 端點測試狀態表格缺失
- ❌ 缺少 Session 5 工作歷史記錄

**影響程度**: 🔴 高 (Sprint 1 最終狀態不準確)

---

### 不同步的時間線

```
Timeline: Sprint 1 Session 5
┌────────────────────────────────────────────────────────────┐
│ 2025-11-05 20:00 UTC - 開始 Session 5                      │
│  └── 目標: 創建 US 1.2 + US 1.3 Phase 4 集成測試           │
├────────────────────────────────────────────────────────────┤
│ 2025-11-05 20:30 - 創建 8 個 US 1.2 集成測試               │
│  └── ConversationApiTests.cs 完成                          │
├────────────────────────────────────────────────────────────┤
│ 2025-11-05 21:00 - 創建 8 個 US 1.3 Phase 4 集成測試       │
│  └── AgentPluginApiTests.cs 完成                           │
├────────────────────────────────────────────────────────────┤
│ 2025-11-05 21:30 - 執行測試,發現 5 個失敗                  │
│  └── TDD 驅動發現後端問題                                  │
├────────────────────────────────────────────────────────────┤
│ 2025-11-05 22:00 - 修復後端問題 (9 個)                     │
│  ├── US 1.2: API 路由修正 (1 個問題)                       │
│  └── US 1.3 Phase 4: 異常處理等 (4 個問題)                 │
├────────────────────────────────────────────────────────────┤
│ 2025-11-05 23:00 - 測試通過率提升到 96%                    │
│  └── 25/26 tests passed                                   │
├────────────────────────────────────────────────────────────┤
│ 2025-11-05 23:56 - 創建 TEST-RESULTS.md                   │
│  └── 詳細測試報告完成 ✅                                   │
├────────────────────────────────────────────────────────────┤
│ 2025-11-06 00:00 - Session 5 結束                         │
│  ❌ PROJECT-STATUS-REPORT.md 未更新 (v5.0.0)               │
│  ❌ 缺少 Session 5 歷史記錄                                │
│  ❌ 集成測試結果未同步到狀態報告                            │
└────────────────────────────────────────────────────────────┘

⏰ 不同步時長: ~8-12 小時 (Session 5 完成到報告更新)
```

---

## 🔎 根本原因分析

### 5 Whys 分析法

#### Why 1: 為什麼 PROJECT-STATUS-REPORT.md 沒有更新?
**答案**: Session 5 結束時,沒有執行文檔更新流程 (Instruction 1)

#### Why 2: 為什麼沒有執行文檔更新流程?
**答案**: AI Assistant 在 Session 5 中專注於測試開發和後端修復,沒有在 Session 結束時觸發文檔同步流程

**詳細說明**:
- Session 5 的主要任務是"創建集成測試" (Phase 1)
- AI Assistant 完成測試創建後,創建了 TEST-RESULTS.md 詳細報告
- 但沒有同步更新 PROJECT-STATUS-REPORT.md 主狀態文件
- 也沒有創建 Session 5 摘要文件

#### Why 3: 為什麼 AI Assistant 沒有在 Session 結束時觸發文檔同步?
**答案**: AI-ASSISTANT-INSTRUCTIONS.md 中缺少"Session 結束時自動檢查文檔同步"的明確指引

**證據**:
檢查 AI-ASSISTANT-INSTRUCTIONS.md 的內容:
- ✅ **有**: Instruction 1 (更新 PROJECT-STATUS-REPORT.md)
- ✅ **有**: Instruction 5 (生成 Session 摘要)
- ✅ **有**: Instruction 7 (完整功能開發結束流程)
- ❌ **缺少**: "Session 結束時強制檢查文檔同步" 的自動觸發規則
- ❌ **缺少**: "主要里程碑完成時強制更新狀態報告" 的觸發規則

**AI-ASSISTANT-INSTRUCTIONS.md 的使用時機說明**:
```markdown
範例 1: 完成 Phase 2-4 功能後
用戶輸入: "我剛完成 US 1.3 Phase 2-4 的實作,請執行完整的結束流程"
AI 執行: (使用 Instruction 7)

範例 3: 每日工作結束前
用戶輸入: "今天工作結束了,請幫我同步進度和文檔"
AI 執行: (組合多個 Instructions)
```

**問題**: 這些範例都依賴**用戶主動請求**文檔同步,沒有 AI 主動檢測的機制。

#### Why 4: 為什麼指引中沒有自動觸發規則?
**答案**: AI-ASSISTANT-INSTRUCTIONS.md v1.0.0 設計假設用戶會主動請求文檔同步,沒有考慮 AI 主動監測和觸發的場景

**設計假設**:
1. 用戶會在完成功能後明確請求"執行完整結束流程"
2. 用戶會在每日結束時請求"同步進度和文檔"
3. 用戶會記得在重要里程碑後更新文檔

**實際情況**:
1. Session 5 中,用戶只請求"創建集成測試" (Phase 1)
2. 測試完成後,用戶直接進入下一階段 (Phase 2: Sprint 1 總結)
3. 中間沒有明確請求"更新狀態報告"
4. 導致 8-12 小時的文檔不同步

#### Why 5: 為什麼設計時假設用戶會主動請求?
**答案**: v1.0.0 設計時的工作流程模型是"被動響應式",而非"主動監測式"

**被動響應式模型** (v1.0.0):
```
User Request → AI Execute → Deliver Result → Wait for Next Request
```

**理想的主動監測式模型**:
```
User Request → AI Execute → Deliver Result → AI Check (需要同步?) → Auto Sync → Wait
```

---

### 根本原因總結

#### 🔴 Primary Root Cause (主要根本原因)

**RC-1: AI-ASSISTANT-INSTRUCTIONS.md 缺少"主動觸發規則"**

**具體問題**:
1. **沒有自動觸發條件**: 指引中所有 Instructions 都是"被動式",依賴用戶明確請求
2. **缺少檢查點機制**: 沒有定義"重要里程碑完成時必須檢查文檔同步"的規則
3. **缺少 AI 主動行為指引**: 沒有告訴 AI 在什麼情況下應該主動建議或執行文檔同步

**影響**:
- AI Assistant 不會主動檢測文檔是否需要更新
- 依賴用戶記憶和主動請求
- 容易在 Session 結束時遺漏文檔同步

---

#### 🟡 Secondary Root Causes (次要根本原因)

**RC-2: 缺少"重要事件"定義和優先級**

**問題**:
AI-ASSISTANT-INSTRUCTIONS.md 沒有明確定義什麼是"重要事件",以及這些事件對應的必須操作。

**應該定義的重要事件**:
```yaml
Critical Events (必須立即更新文檔):
  - User Story 完成
  - Phase 完成
  - Sprint 結束
  - 集成測試完成 (新增!)
  - 重大 Bug 修復

High Priority Events (Session 結束前必須更新):
  - 多個功能模塊完成
  - 大量測試創建
  - 架構變更
  - API 端點新增/修改

Medium Priority Events (可以下次 Session 更新):
  - 單個文件修改
  - 小型 Bug 修復
  - 文檔更新
  - 配置調整
```

**當前狀態**: 沒有這樣的事件分類和優先級定義

---

**RC-3: Session 結束流程不完整**

**問題**:
Instruction 5 (生成 Session 摘要) 和 Instruction 7 (完整結束流程) 都沒有強制要求更新 PROJECT-STATUS-REPORT.md

**Instruction 5 當前內容**:
```markdown
4. 同時更新 claudedocs/PROJECT-STATUS-REPORT.md 中的 Session 記錄區塊
```

**問題**: 這只是"同時更新",沒有明確為"必須步驟",也沒有驗證機制

**Instruction 7 當前內容**:
```markdown
Phase 2: 文檔更新
1. 更新 PROJECT-STATUS-REPORT.md (使用 Instruction 1)
2. 生成 Feature 完成報告 (使用 Instruction 2)
3. 生成 Session 摘要 (使用 Instruction 5)
4. 執行文檔一致性檢查 (使用 Instruction 6)
```

**問題**: Instruction 7 只在"完整的功能開發結束"時使用,不適用於其他場景 (如純測試開發)

---

**RC-4: 缺少文檔同步驗證機制**

**問題**:
即使執行了文檔更新 Instructions,也沒有驗證機制確保:
1. PROJECT-STATUS-REPORT.md 版本號正確遞增
2. 所有重要統計數據已更新
3. Session 歷史記錄已添加
4. 測試結果已同步

**當前狀態**: 沒有 Instruction 6 (文檔一致性檢查) 的強制執行時機

---

### 因果關係圖

```
┌─────────────────────────────────────────────────────────────┐
│ Root Cause 1: 缺少主動觸發規則                               │
│ (AI-ASSISTANT-INSTRUCTIONS.md 被動響應式設計)                │
└──────────────────┬──────────────────────────────────────────┘
                   │
                   ├─→ AI 不會主動檢測文檔同步需求
                   │
                   ├─→ 依賴用戶明確請求文檔更新
                   │
                   └─→ Session 5 結束時沒有觸發文檔同步
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│ Root Cause 2: 缺少重要事件定義                               │
│ (沒有事件優先級和對應操作)                                   │
└──────────────────┬──────────────────────────────────────────┘
                   │
                   ├─→ "集成測試完成" 未被定義為重要事件
                   │
                   └─→ AI 不知道此時需要更新狀態報告
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│ Root Cause 3: Session 結束流程不完整                         │
│ (Instruction 5/7 沒有強制更新主狀態報告)                     │
└──────────────────┬──────────────────────────────────────────┘
                   │
                   └─→ 創建 TEST-RESULTS.md 但未更新主報告
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│ Root Cause 4: 缺少驗證機制                                   │
│ (沒有文檔同步完整性檢查)                                     │
└──────────────────┬──────────────────────────────────────────┘
                   │
                   └─→ 即使部分更新,也無法發現遺漏
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│ Symptom (症狀): PROJECT-STATUS-REPORT.md 不同步              │
│ - 缺少集成測試結果                                           │
│ - 缺少 Session 5 歷史                                        │
│ - 測試統計不完整                                             │
│ - 版本號未更新 (v5.0.0 → v6.0.0 延遲)                        │
└─────────────────────────────────────────────────────────────┘
```

---

## 📊 AI-ASSISTANT-INSTRUCTIONS.md 評估

### 評估標準

基於以下標準評估 AI-ASSISTANT-INSTRUCTIONS.md v1.0.0:

```yaml
Evaluation Criteria:
  1. Completeness (完整性): 是否涵蓋所有必要場景
  2. Proactivity (主動性): AI 是否能主動執行關鍵操作
  3. Robustness (健壯性): 是否有錯誤預防和恢復機制
  4. Clarity (清晰度): 指引是否明確易懂
  5. Automation (自動化): 是否減少人工干預需求
```

---

### 評估結果

#### ✅ 優點 (Strengths)

**1. 結構清晰** (Clarity: 9/10)
- 8 個明確的 Instructions,分類清楚
- 每個 Instruction 有清晰的步驟和格式
- 提供範例和使用時機說明
- Markdown 格式易讀

**2. 覆蓋主要場景** (Completeness: 7/10)
- ✅ 狀態報告更新 (Instruction 1)
- ✅ Feature 完成報告 (Instruction 2)
- ✅ Git 工作流程 (Instruction 3)
- ✅ PR 創建 (Instruction 4)
- ✅ Session 摘要 (Instruction 5)
- ✅ 文檔一致性檢查 (Instruction 6)
- ✅ 完整結束流程 (Instruction 7)
- ✅ 快速同步 (Instruction 8)

**3. 詳細的 Git 工作流程** (Robustness: 8/10)
- 前置檢查機制 (branch check, status check)
- 測試驗證流程 (build → test → commit)
- 錯誤處理指引 (test failure, push failure)
- 提交訊息標準格式

**4. 豐富的使用範例** (Clarity: 8/10)
- 3 個實際使用範例
- 快速參考卡 (預估時間)
- 環境變數和設定
- 相關文檔鏈接

---

#### ❌ 缺陷 (Weaknesses)

**1. 缺少主動觸發機制** (Proactivity: 3/10) 🔴 Critical

**問題描述**:
所有 Instructions 都是"被動響應式",依賴用戶明確請求執行。

**具體缺陷**:
```markdown
當前設計:
  用戶: "我完成了功能,請執行結束流程"
  AI: 執行 Instruction 7

理想設計:
  AI: (檢測到重要里程碑完成)
  AI: "我注意到你剛完成了 US 1.2 集成測試 (25/26 通過),
       這是重要里程碑。建議執行以下操作:
       1. 更新 PROJECT-STATUS-REPORT.md (必須)
       2. 生成 Session 摘要 (建議)
       3. 創建 PR (可選)
       是否現在執行?"
```

**缺少的指引**:
- 沒有"AI 主動檢測"的觸發條件
- 沒有"AI 主動建議"的時機定義
- 沒有"強制執行"vs"建議執行"的區分

**影響**: 完全依賴用戶記憶,容易遺漏關鍵操作

---

**2. 重要事件定義缺失** (Completeness: 4/10) 🔴 Critical

**問題描述**:
沒有定義什麼是"重要事件"以及對應的必須操作。

**缺少的內容**:
```yaml
# 建議新增: Instruction 9 - 重要事件自動觸發規則

Critical Events (必須立即執行文檔更新):
  - User Story 完成
    觸發: Instruction 1 (更新狀態報告) + Instruction 2 (Feature 報告)

  - Sprint 結束
    觸發: Instruction 7 (完整結束流程)

  - 集成測試完成 (新增!)
    觸發: Instruction 1 (更新狀態報告,包含測試結果)

  - Phase 完成
    觸發: Instruction 1 + Instruction 5 (Session 摘要)

High Priority Events (Session 結束前必須執行):
  - 多個測試文件創建 (≥5 tests)
  - 多個後端問題修復 (≥3 fixes)
  - API 端點新增/修改 (≥2 endpoints)

Medium Priority Events (可以下次更新):
  - 單個文件修改
  - 配置調整
  - 小型文檔更新
```

**當前狀態**: 完全沒有這樣的定義

**影響**: AI 無法判斷何時應該主動更新文檔

---

**3. Session 結束檢查機制缺失** (Automation: 2/10) 🔴 Critical

**問題描述**:
沒有"Session 結束時強制檢查"的機制。

**缺少的指引**:
```markdown
# 建議新增: Instruction 10 - Session 結束檢查清單

每當 Session 即將結束時 (用戶說"今天結束"、"先到這裡"、"下次再繼續"),
AI 必須執行以下檢查:

## 強制檢查項目:
1. [ ] PROJECT-STATUS-REPORT.md 是否為最新狀態?
   - 檢查報告日期 vs 當前日期
   - 檢查是否有未記錄的 User Story 進度
   - 檢查是否有未記錄的測試結果

2. [ ] 是否有未提交的重要變更?
   - 檢查 git status
   - 如果有變更,建議執行 Instruction 3

3. [ ] 是否需要創建 Session 摘要?
   - 如果 Session 持續 >1 小時,必須創建摘要
   - 執行 Instruction 5

4. [ ] 是否有待辦事項需要記錄?
   - 檢查是否有未完成的任務
   - 記錄到 SESSION-SUMMARY.md

## AI 輸出格式:
"✅ Session 結束檢查完成
- [✅/❌] 狀態報告已更新
- [✅/❌] 變更已提交
- [✅/❌] Session 摘要已創建
- [✅/❌] 待辦事項已記錄

下次 Session 建議:
1. [建議 1]
2. [建議 2]"
```

**當前狀態**: 沒有這樣的檢查機制

**影響**: Session 結束時容易遺漏關鍵操作

---

**4. 驗證機制不足** (Robustness: 5/10) 🟡 High

**問題描述**:
Instruction 6 (文檔一致性檢查) 存在,但沒有定義執行時機和強制性。

**當前狀態**:
```markdown
Instruction 6: 檢查文檔一致性

執行時機:
- 完成一個 Phase 後
- 創建 PR 之前
- Sprint 結束前
```

**問題**:
- "完成一個 Phase 後" 太模糊,AI 難以判斷何時是"Phase 完成"
- 沒有說明是"建議執行"還是"必須執行"
- 沒有與其他 Instructions 的強制關聯 (如 Instruction 7 必須包含 Instruction 6)

**改進建議**:
```markdown
Instruction 6: 文檔一致性檢查 (強制驗證)

強制執行時機:
- ✅ 執行 Instruction 1 之後 (驗證狀態報告更新正確)
- ✅ 執行 Instruction 7 的 Phase 2 結束時 (驗證所有文檔已同步)
- ✅ Session 結束前 (執行 Instruction 10 時)

可選執行時機:
- 創建 PR 之前 (Instruction 4)
- 每日工作結束時
```

**影響**: 即使執行了部分文檔更新,也可能有遺漏但無法發現

---

**5. 缺少優先級和強制性標記** (Clarity: 6/10) 🟡 Medium

**問題描述**:
Instructions 沒有標記優先級和強制性,AI 難以判斷哪些是"必須"哪些是"可選"。

**當前狀態**:
所有 Instructions 看起來平等,沒有優先級區分。

**改進建議**:
```markdown
# Instructions Priority Matrix

🔴 Critical (必須執行,不可跳過):
  - Instruction 1: 更新 PROJECT-STATUS-REPORT.md
  - Instruction 3: Git 工作流程 (提交和推送)
  - Instruction 10: Session 結束檢查 (新增)

🟡 High Priority (強烈建議,可協商跳過):
  - Instruction 5: 生成 Session 摘要
  - Instruction 6: 文檔一致性檢查
  - Instruction 7: 完整結束流程

🟢 Medium Priority (建議執行,可延後):
  - Instruction 2: 生成 Feature 完成報告
  - Instruction 4: 創建 Pull Request
  - Instruction 8: 快速同步

執行規則:
- 🔴 Critical: AI 必須主動提醒並建議執行
- 🟡 High: AI 應該建議執行,但尊重用戶決定
- 🟢 Medium: AI 可以提及,但不強制建議
```

**影響**: AI 無法判斷哪些操作是關鍵的,可能在時間緊張時跳過重要步驟

---

**6. 測試失敗處理不夠明確** (Robustness: 6/10) 🟡 Medium

**問題描述**:
Instruction 3 和 Instruction 7 中,測試失敗的處理邏輯不夠清晰。

**當前內容** (Instruction 3):
```markdown
錯誤處理:
- 如果測試失敗,停止流程並報告失敗測試
```

**問題**:
- "停止流程" 是否意味著完全停止,還是詢問用戶是否繼續?
- 如果是已知的測試失敗 (如 CreateConversation 問題),是否可以繼續?
- 測試失敗時,文檔是否應該更新 (記錄失敗狀態)?

**改進建議**:
```markdown
測試失敗處理流程:

1. 分類失敗測試:
   - 🔴 Critical: 核心功能測試失敗 (如 CRUD 基本操作)
   - 🟡 Known Issues: 已知問題 (記錄在 PROJECT-STATUS-REPORT.md)
   - 🟢 Edge Cases: 邊界情況測試失敗

2. 根據分類決定:
   - 🔴 Critical: 必須停止,不允許提交
   - 🟡 Known Issues: 詢問用戶是否繼續,記錄到文檔
   - 🟢 Edge Cases: 可以繼續,但必須記錄

3. 文檔更新:
   - 無論測試是否通過,都必須更新 PROJECT-STATUS-REPORT.md
   - 記錄失敗測試詳情和已知問題
```

---

### 評估總結

#### 總體評分

| 維度 | 評分 | 權重 | 加權分 |
|------|------|------|--------|
| Completeness (完整性) | 7/10 | 25% | 1.75 |
| Proactivity (主動性) | 3/10 | 30% | 0.90 |
| Robustness (健壯性) | 6/10 | 20% | 1.20 |
| Clarity (清晰度) | 8/10 | 15% | 1.20 |
| Automation (自動化) | 2/10 | 10% | 0.20 |
| **總分** | - | - | **5.25/10** |

**等級**: 🟡 **Fair (尚可)**

**結論**: AI-ASSISTANT-INSTRUCTIONS.md v1.0.0 提供了清晰的操作指引,但在主動性和自動化方面存在顯著不足,導致文檔同步依賴用戶記憶和主動請求,容易遺漏關鍵操作。

---

#### 關鍵問題優先級

| 優先級 | 問題 | 影響 | 建議行動 |
|-------|------|------|----------|
| 🔴 P0 | 缺少主動觸發機制 | 完全依賴用戶,易遺漏 | 立即添加 Instruction 9 (事件觸發規則) |
| 🔴 P0 | 重要事件定義缺失 | AI 無法判斷何時更新 | 立即添加重要事件清單和觸發條件 |
| 🔴 P0 | Session 結束檢查缺失 | Session 結束時易遺漏 | 立即添加 Instruction 10 (結束檢查清單) |
| 🟡 P1 | 驗證機制不足 | 無法發現遺漏 | 強化 Instruction 6 的執行時機和強制性 |
| 🟡 P1 | 缺少優先級標記 | AI 難以判斷重要性 | 為所有 Instructions 添加優先級標記 |
| 🟢 P2 | 測試失敗處理模糊 | 可能導致錯誤決策 | 完善測試失敗處理流程 |

---

## 💡 改進建議

基於根本原因分析和評估結果,提出以下改進建議:

### 建議 1: 新增 Instruction 9 - 重要事件自動觸發規則 🔴 Critical

**目標**: 讓 AI 能夠主動檢測重要事件並觸發相應操作

**新增內容**:
```markdown
## Instruction 9: 重要事件自動觸發規則

AI Assistant 必須主動監測以下重要事件,並在事件發生時自動觸發相應操作:

### 🔴 Critical Events (必須立即觸發)

#### Event 1: User Story 完成
**檢測條件**:
- 用戶明確表示 "US X.X 完成"
- 或 AI 檢測到某個 US 的所有 Acceptance Criteria 都已滿足
- 或 所有相關測試都已通過

**自動觸發操作**:
1. 🔴 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md
   - 更新 User Story 狀態為 ✅ 已完成
   - 更新完成時間和統計數據
   - 記錄測試結果

2. 🟡 **Instruction 2**: 生成 Feature 完成報告
   - 創建詳細的功能完成報告
   - 記錄變更統計和技術細節

3. 🟡 **Instruction 5**: 生成 Session 摘要
   - 如果此 Session 主要在實作此 US

**AI 輸出**:
"🎉 我注意到 User Story X.X 已完成!這是重要里程碑。
我將執行以下操作:
1. ✅ 更新 PROJECT-STATUS-REPORT.md (必須)
2. ✅ 生成 Feature 完成報告 (建議)
3. ✅ 生成 Session 摘要 (如適用)
開始執行..."

---

#### Event 2: 集成測試完成 (新增!)
**檢測條件**:
- 創建了 ≥5 個集成測試
- 或 執行了集成測試並得到結果 (passed/failed)
- 或 用戶明確表示 "集成測試完成"

**自動觸發操作**:
1. 🔴 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md
   - 更新集成測試統計 (total, passed, failed)
   - 添加測試覆蓋的 API 端點表格
   - 記錄測試通過率和已知問題
   - 更新版本號 (如果是重要里程碑)

2. 🟡 **Instruction 6**: 文檔一致性檢查
   - 驗證測試結果已正確記錄

**AI 輸出**:
"📊 集成測試已完成 (25/26 通過, 96%)!
我將更新 PROJECT-STATUS-REPORT.md 記錄測試結果。
執行中..."

**重要性**: 🔴 Critical (此事件在 Session 5 中被遺漏,導致文檔不同步)

---

#### Event 3: Sprint 結束
**檢測條件**:
- 用戶明確表示 "Sprint X 結束"
- 或 Sprint 計劃中的所有 User Stories 都已完成
- 或 用戶請求 "Sprint Review" 或 "Sprint Retrospective"

**自動觸發操作**:
1. 🔴 **Instruction 7**: 完整結束流程
2. 🔴 **Instruction 1**: 最終狀態報告更新
3. 🟡 **Instruction 4**: 建議創建 PR (如有未合併分支)
4. 🟡 生成 Sprint Retrospective 報告

---

#### Event 4: Phase 完成
**檢測條件**:
- 用戶明確表示 "Phase X 完成"
- 或 Phase 的所有任務都已完成 (根據 Roadmap 或 Todo List)

**自動觸發操作**:
1. 🔴 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md
2. 🟡 **Instruction 5**: 生成 Session 摘要

---

### 🟡 High Priority Events (Session 結束前必須觸發)

#### Event 5: 多個測試文件創建
**檢測條件**:
- 創建了 ≥3 個測試文件
- 或 新增了 ≥10 個測試案例

**觸發時機**: Session 結束前

**自動觸發操作**:
1. 🟡 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md (測試統計)

---

#### Event 6: 多個後端問題修復
**檢測條件**:
- 修復了 ≥3 個 Bug 或問題
- 或 用戶明確表示 "修復了多個問題"

**觸發時機**: Session 結束前

**自動觸發操作**:
1. 🟡 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md (問題修復記錄)

---

#### Event 7: API 端點新增/修改
**檢測條件**:
- 新增或修改了 ≥2 個 API Controller 端點
- 或 用戶明確表示 "API 端點完成"

**觸發時機**: Session 結束前

**自動觸發操作**:
1. 🟡 **Instruction 1**: 更新 PROJECT-STATUS-REPORT.md (API 端點清單)

---

### 🟢 Medium Priority Events (可以下次更新)

#### Event 8: 單個文件修改
**處理**: 不觸發自動更新,可以累積到下次 Session

#### Event 9: 配置調整
**處理**: 使用 Instruction 8 (快速同步) 即可

---

### AI 行為規則

#### Rule 1: 主動檢測
AI 必須在以下時機主動檢測重要事件:
- 用戶任務完成時
- Session 即將結束時 (用戶說 "結束"、"先到這裡")
- 執行 git commit 之前
- 創建 PR 之前

#### Rule 2: 主動建議
當檢測到 Critical Event 時,AI 必須:
1. 明確告知用戶檢測到的事件
2. 列出將要執行的操作
3. 立即開始執行 (不需要等待用戶確認)

當檢測到 High Priority Event 時,AI 應該:
1. 告知用戶檢測到的事件
2. 建議執行的操作
3. 詢問用戶是否現在執行

#### Rule 3: 強制執行 vs 建議執行
- 🔴 Critical Operations: 直接執行,無需詢問
- 🟡 High Priority Operations: 建議並詢問
- 🟢 Medium Priority Operations: 僅提及,不強制

---

### 執行範例

**Session 5 理想執行流程** (如果有 Instruction 9):
```
User: "我完成了 US 1.2 和 US 1.3 Phase 4 的集成測試"

AI: 🎯 檢測到重要事件: 集成測試完成 (Event 2 - Critical)
    - 創建了 16 個集成測試
    - 測試通過率: 96% (25/26)

    我將執行以下操作:
    1. 🔴 更新 PROJECT-STATUS-REPORT.md (記錄測試結果)
    2. 🟡 執行文檔一致性檢查 (驗證更新正確)

    開始執行 Instruction 1 (更新狀態報告)...
    [執行更新]

    ✅ PROJECT-STATUS-REPORT.md 已更新到 v6.0.0
    - 新增 US 1.2 集成測試結果區塊
    - 新增 US 1.3 Phase 4 集成測試結果區塊
    - 更新測試統計 (122 total tests)
    - 更新版本號和報告日期

    開始執行 Instruction 6 (文檔一致性檢查)...
    [執行檢查]

    ✅ 文檔一致性檢查完成 - 所有關鍵資訊已同步

User: "好的,今天先到這裡"

AI: 🎯 檢測到 Session 結束,執行 Instruction 10 (結束檢查)

    ✅ Session 結束檢查:
    - ✅ 狀態報告已更新 (v6.0.0)
    - ✅ 變更已提交 (如適用)
    - ⏳ Session 摘要待創建

    是否要創建 Session 5 摘要? (建議: 是,本次 Session 完成重要工作)
```

**結果**: 文檔不會不同步,因為 AI 在集成測試完成時自動更新了狀態報告。
```

**實施優先級**: 🔴 P0 (立即實施)
**預估效果**: 解決 80% 的文檔不同步問題

---

### 建議 2: 新增 Instruction 10 - Session 結束強制檢查清單 🔴 Critical

**目標**: 確保每個 Session 結束時都執行必要的檢查和更新

**新增內容**:
```markdown
## Instruction 10: Session 結束強制檢查清單

### 觸發時機
當用戶表示 Session 即將結束時,AI 必須自動執行此檢查清單:
- 用戶說: "今天結束了"、"先到這裡"、"下次再繼續"、"暫停一下"
- 或 用戶沉默 >30 分鐘 (可選,如 AI 能檢測)

### 強制檢查項目

#### 1. 🔴 PROJECT-STATUS-REPORT.md 是否最新?
```bash
# AI 執行檢查:
1. 讀取 PROJECT-STATUS-REPORT.md 的 "報告日期"
2. 比較與當前日期 (UTC)
3. 如果不是今天的日期 → 必須更新

# 檢查內容:
- [ ] 報告日期 = 今天?
- [ ] 版本號正確遞增?
- [ ] 所有本次 Session 完成的工作都已記錄?
- [ ] 測試統計已更新?
- [ ] Session 歷史已添加?

# 如果有任何 [ ] 未勾選:
→ 執行 Instruction 1 (更新狀態報告)
```

#### 2. 🟡 是否有未提交的重要變更?
```bash
# AI 執行:
git status

# 檢查:
- 是否有 unstaged 或 staged 的變更?
- 是否是重要文件 (src/, tests/, docs/)?

# 如果是:
→ 建議執行 Instruction 3 (Git 工作流程)
→ 詢問用戶: "檢測到未提交的變更,是否現在提交?"
```

#### 3. 🟡 是否需要創建 Session 摘要?
```bash
# 檢查條件:
- Session 持續時間 >1 小時?
- 或 完成了重要功能/測試 (≥3 tasks)?
- 或 修復了問題 (≥2 fixes)?

# 如果是:
→ 建議執行 Instruction 5 (生成 Session 摘要)
→ 詢問用戶: "建議創建 Session X 摘要記錄本次工作,是否執行?"
```

#### 4. 🟢 是否有待辦事項需要記錄?
```bash
# 檢查:
- 是否有未完成的任務?
- 是否有計劃的後續工作?
- 是否有發現的問題待解決?

# 如果是:
→ 記錄到 Todo List 或 SESSION-SUMMARY.md
```

---

### AI 輸出格式

```
🔍 Session 結束檢查

執行時間: YYYY-MM-DD HH:MM UTC

📋 檢查結果:
1. 🔴 狀態報告檢查:
   - 報告日期: 2025-11-05 (過期 1 天) ❌
   - 版本號: v5.0.0 (需要遞增) ❌
   - 測試統計: 缺少集成測試結果 ❌

   → 必須執行 Instruction 1 更新狀態報告

2. 🟡 Git 變更檢查:
   - 未提交變更: 5 files ⚠️
   - 重要文件: tests/IntegrationTests (2 files)

   → 建議執行 Instruction 3 提交變更

3. 🟡 Session 摘要檢查:
   - Session 時長: ~3 小時 ⚠️
   - 重要工作: 創建 16 個集成測試,修復 9 個問題

   → 建議創建 Session 5 摘要

4. 🟢 待辦事項:
   - [ ] 修復 CreateConversation 測試失敗
   - [ ] 統一 API 路由版本控制

---

🎯 建議操作:
1. 🔴 立即更新 PROJECT-STATUS-REPORT.md (必須)
2. 🟡 提交未提交的變更 (建議)
3. 🟡 創建 Session 5 摘要 (建議)

是否執行以上操作? (建議: 全部執行)
```

---

### 自動執行規則

**Rule 1: 強制執行 (無需詢問)**
如果檢測到以下情況,AI 直接執行,無需用戶確認:
- PROJECT-STATUS-REPORT.md 報告日期過期 ≥2 天
- 有 Critical Event 未記錄 (如 US 完成、集成測試完成)
- 有未提交的 .md 文檔變更 (文檔應該立即提交)

**Rule 2: 建議執行 (詢問用戶)**
如果檢測到以下情況,AI 建議並詢問:
- 有未提交的代碼變更
- Session 時長 >1 小時且完成重要工作
- 有多個待辦事項需要記錄

**Rule 3: 僅提及 (不強制)**
- 有小型文檔更新可延後
- 有非關鍵的配置調整

---

### 執行範例

**Session 5 結束時** (理想流程):
```
User: "今天先到這裡"

AI: 🔍 執行 Session 結束檢查...

    檢查結果:
    1. ❌ 狀態報告過期 (2025-11-05 vs 2025-11-06)
    2. ❌ 集成測試結果未記錄 (16 tests, 25/26 passed)
    3. ⚠️ 有未提交變更 (測試文件)

    我將立即更新 PROJECT-STATUS-REPORT.md (必須操作)...

    [執行 Instruction 1]

    ✅ 狀態報告已更新到 v6.0.0

    另外,建議:
    - 提交測試文件變更
    - 創建 Session 5 摘要

    是否現在執行? (y/n)
```

**結果**: 確保 PROJECT-STATUS-REPORT.md 在 Session 結束時是最新的。
```

**實施優先級**: 🔴 P0 (立即實施)
**預估效果**: 解決剩餘 15% 的文檔不同步問題

---

### 建議 3: 強化 Instruction 6 的執行時機和強制性 🟡 High

**目標**: 將文檔一致性檢查從"可選"變為"強制"

**修改內容**:
```markdown
## Instruction 6: 文檔一致性檢查 (強制驗證)

### 執行時機 (更新)

#### 🔴 強制執行時機 (必須執行,不可跳過):
1. **執行 Instruction 1 之後**
   - 驗證 PROJECT-STATUS-REPORT.md 更新正確
   - 確認所有關鍵資訊已同步

2. **執行 Instruction 7 的 Phase 2 結束時**
   - 驗證所有文檔已更新
   - 確認無遺漏

3. **執行 Instruction 10 (Session 結束檢查) 時**
   - 最終驗證文檔一致性

4. **創建 PR 之前 (Instruction 4)**
   - 確保 PR 包含的變更已反映在文檔中

#### 🟡 建議執行時機:
- Sprint 結束前
- 每日工作結束時
- 重要里程碑完成後

---

### 驗證內容 (擴展)

#### 驗證 1: PROJECT-STATUS-REPORT.md 完整性
```yaml
檢查項目:
  - [ ] 報告日期 = 當前日期 (UTC)
  - [ ] 版本號正確遞增 (按規則: major.minor.patch)
  - [ ] 所有完成的 User Stories 狀態為 ✅
  - [ ] 所有進行中的 User Stories 狀態為 🟡
  - [ ] 測試統計完整 (unit + integration)
  - [ ] API 端點清單最新
  - [ ] Session 歷史包含本次 Session
  - [ ] 已知問題清單最新

驗證方法:
  1. 讀取 PROJECT-STATUS-REPORT.md
  2. 讀取 User Stories 文檔 (docs/user-stories/US-*.md)
  3. 執行 git log 檢查最近提交
  4. 執行 dotnet test 檢查測試統計
  5. 比對一致性
```

#### 驗證 2: User Story 文檔一致性
```yaml
檢查項目:
  - [ ] US 狀態與 PROJECT-STATUS-REPORT.md 一致
  - [ ] Acceptance Criteria 完成度與實際實作一致
  - [ ] 測試覆蓋記錄與實際測試一致

報告格式:
  - ✅ US 1.1: 狀態一致 (✅ 已完成)
  - ✅ US 1.2: 狀態一致 (✅ 已完成)
  - ⚠️ US 1.3 Phase 4: 缺少集成測試結果記錄
```

#### 驗證 3: 測試文檔一致性
```yaml
檢查項目:
  - [ ] 測試統計與實際測試文件一致
  - [ ] 失敗測試已記錄在已知問題中
  - [ ] 測試覆蓋率記錄準確

執行方法:
  1. 統計 tests/ 目錄下的測試文件
  2. 執行 dotnet test 獲取測試結果
  3. 比對 PROJECT-STATUS-REPORT.md 中的測試統計
  4. 報告不一致項目
```

---

### AI 輸出格式 (更新)

```markdown
## 📋 文檔一致性檢查報告

**檢查時間**: YYYY-MM-DD HH:MM UTC
**檢查範圍**: [列出檢查的文檔]

---

### ✅ 一致的文檔 (5/7)
- ✅ PROJECT-STATUS-REPORT.md: 版本號、報告日期
- ✅ US 1.1: 狀態、測試覆蓋
- ✅ US 1.2: 狀態 (但缺少集成測試詳情)
- ✅ SPRINT-1-ROADMAP.md: Checklist 與進度一致
- ✅ README.md: 功能清單最新

---

### ⚠️ 需要更新的文檔 (2/7)
- ⚠️ **PROJECT-STATUS-REPORT.md**:
  - 缺少 US 1.2 集成測試結果 (8 tests)
  - 缺少 US 1.3 Phase 4 集成測試結果 (8 tests)
  - 測試統計不完整 (只有 97 unit, 缺少 26 integration)
  - 缺少 Session 5 歷史記錄

- ⚠️ **US-1.2-conversation-crud.md**:
  - Acceptance Criteria 狀態未更新 (測試覆蓋完成)

---

### ❌ 缺失的文檔 (0/7)
(無)

---

### 🔧 自動修復建議
AI 可以自動修復以下問題:
1. 更新 PROJECT-STATUS-REPORT.md (執行 Instruction 1)
2. 更新 US 1.2 文檔的測試狀態

是否執行自動修復? (y/n)

---

### 📝 手動修復建議
以下需要用戶手動確認或決定:
(無)

---

### ✅ 驗證結論
- **一致性得分**: 71% (5/7 完全一致)
- **關鍵問題**: 2 個 (測試結果未同步)
- **建議操作**: 立即執行 Instruction 1 更新狀態報告
```

---

### 強制執行邏輯 (新增)

```markdown
### 驗證失敗處理

#### 如果驗證發現 Critical 問題:
Critical 問題定義:
- PROJECT-STATUS-REPORT.md 報告日期過期 >1 天
- 測試統計與實際結果差異 >10%
- 有 User Story 完成但狀態未更新

處理邏輯:
1. AI 必須立即報告問題
2. AI 必須建議修復方案
3. 在用戶確認前,不允許執行後續操作 (如 git commit, PR creation)

範例:
"⚠️ 驗證失敗: 發現 Critical 問題
- PROJECT-STATUS-REPORT.md 過期 2 天
- 測試統計差異 21% (97 unit 缺少 26 integration)

必須先修復這些問題,才能繼續提交或創建 PR。
是否現在執行 Instruction 1 修復? (強烈建議: 是)"

#### 如果驗證發現 Warning 問題:
Warning 問題定義:
- 有小型文檔更新遺漏
- 有非關鍵統計數據差異 <10%

處理邏輯:
1. AI 報告問題
2. AI 建議修復,但不強制
3. 用戶可以選擇延後修復

範例:
"⚠️ 驗證警告: 發現小問題
- US 1.2 文檔的測試狀態未更新

建議在下次 Session 修復。是否現在修復? (可選)"
```
```

**實施優先級**: 🟡 P1 (Sprint 2 Week 1 完成)
**預估效果**: 提供主動錯誤檢測,減少文檔不一致的風險

---

### 建議 4: 為所有 Instructions 添加優先級標記 🟡 High

**目標**: 幫助 AI 判斷哪些操作是關鍵的,哪些可以延後

**修改方式**:
在 AI-ASSISTANT-INSTRUCTIONS.md 開頭添加 Instructions 優先級表:

```markdown
## 📊 Instructions 優先級矩陣

### 優先級定義

🔴 **Critical (必須執行,不可跳過)**:
- AI 必須主動提醒並建議執行
- 如果檢測到觸發條件,AI 直接執行無需詢問
- 如果未執行,不允許進行後續操作 (如 git commit, PR creation)

🟡 **High Priority (強烈建議,可協商跳過)**:
- AI 應該主動建議執行
- 需要用戶確認才執行
- 如果跳過,AI 應該記錄原因並提醒後續執行

🟢 **Medium Priority (建議執行,可延後)**:
- AI 可以提及
- 用戶自主決定是否執行
- 不影響關鍵流程

---

### Instructions 優先級表

| Instruction | 名稱 | 優先級 | 觸發時機 | 強制性 |
|-------------|------|--------|---------|--------|
| Instruction 1 | 更新 PROJECT-STATUS-REPORT.md | 🔴 Critical | 重要事件完成時 | 必須執行 |
| Instruction 3 | Git 工作流程 | 🔴 Critical | 有重要變更時 | 必須執行 |
| Instruction 9 | 重要事件自動觸發 | 🔴 Critical | 檢測到事件時 | 自動執行 |
| Instruction 10 | Session 結束檢查 | 🔴 Critical | Session 結束時 | 自動執行 |
| Instruction 5 | 生成 Session 摘要 | 🟡 High | Session >1h 時 | 建議執行 |
| Instruction 6 | 文檔一致性檢查 | 🟡 High | 更新文檔後 | 建議執行 |
| Instruction 7 | 完整結束流程 | 🟡 High | 功能完成時 | 建議執行 |
| Instruction 2 | 生成 Feature 報告 | 🟢 Medium | US 完成時 | 可選執行 |
| Instruction 4 | 創建 Pull Request | 🟢 Medium | 分支準備好時 | 可選執行 |
| Instruction 8 | 快速同步 | 🟢 Medium | 小型變更時 | 可選執行 |

---

### 執行規則

#### Rule 1: Critical Instructions
```yaml
觸發條件: 自動檢測到關鍵事件
AI 行為:
  - 立即告知用戶檢測到的事件
  - 列出將要執行的操作
  - 直接開始執行 (無需等待確認)
  - 執行完畢後報告結果

範例:
"🎯 檢測到 Critical Event: 集成測試完成
執行 Instruction 1 更新狀態報告...
✅ 完成"
```

#### Rule 2: High Priority Instructions
```yaml
觸發條件: 檢測到重要但非關鍵的事件
AI 行為:
  - 告知用戶檢測到的事件或時機
  - 列出建議執行的操作和理由
  - 詢問用戶是否執行
  - 根據用戶回應執行或記錄延後原因

範例:
"📊 建議執行 Session 摘要 (Session 時長 3h,完成重要工作)
是否現在執行? (y/n)
如果暫不執行,我會在 Session 結束檢查時再次提醒。"
```

#### Rule 3: Medium Priority Instructions
```yaml
觸發條件: 用戶明確請求
AI 行為:
  - 僅在用戶請求時執行
  - 可以簡單提及可選操作,但不強制建議
  - 完全尊重用戶決定

範例:
"✅ 變更已提交。
(可選) 你可以執行 Instruction 4 創建 PR,或稍後手動創建。"
```

---

### 優先級覆蓋規則

用戶可以臨時調整優先級:
- "跳過文檔更新,先提交代碼" → AI 允許跳過 Instruction 1 (通常為 Critical)
- "這是緊急修復,快速流程" → AI 使用 Instruction 8 而非 Instruction 7
- "完整檢查,不要遺漏任何步驟" → AI 提升所有 Medium 為 High

AI 應該記錄優先級覆蓋的原因,並在下次 Session 提醒補充遺漏的操作。
```

**實施優先級**: 🟡 P1 (Sprint 2 Week 1 完成)
**預估效果**: 幫助 AI 做出更好的決策,減少用戶干預需求

---

### 建議 5: 完善測試失敗處理流程 🟢 Medium

**目標**: 讓 AI 在測試失敗時做出更智能的決策

**修改內容** (Instruction 3 和 Instruction 7):
```markdown
### 測試失敗處理流程 (更新)

#### 步驟 1: 執行測試
```bash
dotnet build "C:\AI Semantic Kernel\src\AIAgentPlatform.sln" --configuration Release
dotnet test "C:\AI Semantic Kernel\src\AIAgentPlatform.sln" --configuration Release --verbosity normal
```

#### 步驟 2: 分析測試結果
如果有測試失敗,AI 必須分類失敗測試:

##### 🔴 Critical Failures (核心功能失敗)
**定義**:
- 基本 CRUD 操作測試失敗
- 新增功能的核心測試失敗
- 導致系統無法運行的測試失敗

**範例**:
- CreateAgent 測試失敗
- GetAgentById 測試失敗
- 新增的 AgentExecutionService 核心測試失敗

**處理**:
```markdown
❌ Critical 測試失敗: CreateAgent_WithValidData_ShouldSucceed

必須停止流程,不允許提交代碼。
建議操作:
1. 檢查測試失敗原因 (查看錯誤訊息)
2. 修復問題
3. 重新執行測試
4. 確認通過後再繼續

是否要查看詳細錯誤訊息? (y/n)
```

---

##### 🟡 Known Issue Failures (已知問題)
**定義**:
- PROJECT-STATUS-REPORT.md 中已記錄的已知測試失敗
- 非本次 Session 引入的問題
- 不影響新功能的失敗測試

**範例**:
- CreateConversation_WithValidData (Session 5 發現,已記錄)

**處理**:
```markdown
⚠️ 測試失敗: CreateConversation_WithValidData_ShouldSucceed

檢查結果: 這是已知問題 (記錄在 PROJECT-STATUS-REPORT.md)
- 發現時間: Session 5
- 狀態: 待修復
- 優先級: 中

這個失敗不影響本次變更 (US 1.3 Phase 4 AgentPlugin)。

是否繼續提交? (建議: 是,但需在提交訊息中標注已知問題)
```

---

##### 🟢 Edge Case Failures (邊界情況失敗)
**定義**:
- 邊界條件或異常情況測試失敗
- 不影響主要用戶流程
- 可以後續改進

**範例**:
- GetAgent_WithExtremelyLongDescription (描述長度 >10000 字元)
- CreateAgent_With1000Plugins (極端數量測試)

**處理**:
```markdown
⚠️ Edge Case 測試失敗: GetAgent_WithExtremelyLongDescription

分析: 這是邊界情況測試,不影響正常使用場景。

建議操作:
1. 記錄為已知改進項 (不是 Bug)
2. 可以繼續提交
3. 創建 Issue 或 TODO 追蹤改進

是否繼續提交? (建議: 是,記錄改進項)
```

---

#### 步驟 3: 決定是否繼續
```yaml
決策矩陣:
  Critical Failures:
    action: STOP
    allow_commit: false
    message: "必須修復 Critical 測試失敗後才能提交"

  Known Issue Failures (新功能測試通過):
    action: WARN
    allow_commit: true_with_warning
    message: "有已知問題失敗,但不影響本次變更"
    commit_note: "標注已知問題在提交訊息中"

  Known Issue Failures (新功能測試也失敗):
    action: STOP
    allow_commit: false
    message: "新功能測試失敗,必須修復"

  Edge Case Failures:
    action: INFO
    allow_commit: true
    message: "邊界情況測試失敗,記錄為改進項"
```

---

#### 步驟 4: 文檔更新
**重要**: 無論測試通過或失敗,都必須更新 PROJECT-STATUS-REPORT.md

```yaml
測試通過:
  更新內容:
    - 測試統計 (passed/total)
    - 測試覆蓋率
    - 測試狀態: ✅ 全部通過

測試部分失敗 (Known Issues):
  更新內容:
    - 測試統計 (passed/total, 含失敗數)
    - 已知問題清單 (添加或更新)
    - 測試狀態: ⚠️ X/Y 通過 (Z 個已知問題)

測試失敗 (Critical):
  更新內容:
    - 標記為 ❌ 測試失敗
    - 記錄失敗原因和計劃修復時間
    - 不允許標記功能為 "已完成"
```

---

#### 步驟 5: 提交訊息標注
如果允許提交但有測試失敗,提交訊息必須標注:

```markdown
feat: US 1.3 Phase 4 - AgentPlugin API

實作 AgentPlugin CRUD API 和 Plugin 執行機制。

完成項目:
- AgentPlugin API 端點 (4 個)
- Plugin 動態加載機制
- 集成測試 (8/8 通過)

測試結果:
- Build: ✅ Success
- Unit Tests: 97/97 ✅
- Integration Tests: 25/26 (96%) ⚠️
- Known Issue: CreateConversation_WithValidData (US 1.2, 待修復)

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
```
```

**實施優先級**: 🟢 P2 (Sprint 2 Week 2 完成)
**預估效果**: 讓 AI 在測試失敗時做出更合理的決策,避免過度阻塞或過度寬鬆

---

### 改進建議總結表

| 建議 | 優先級 | 實施時間 | 預估效果 | 工作量 |
|------|--------|---------|---------|--------|
| 1. 新增 Instruction 9 (事件觸發規則) | 🔴 P0 | 立即 | 80% 問題解決 | 中 (2-3h) |
| 2. 新增 Instruction 10 (結束檢查清單) | 🔴 P0 | 立即 | 15% 問題解決 | 中 (2-3h) |
| 3. 強化 Instruction 6 (驗證強制性) | 🟡 P1 | Week 1 | 提供主動檢測 | 小 (1h) |
| 4. 添加優先級標記 | 🟡 P1 | Week 1 | 改善決策質量 | 小 (1h) |
| 5. 完善測試失敗處理 | 🟢 P2 | Week 2 | 更智能的處理 | 中 (2h) |

**總工作量**: 8-10 小時
**預估總體效果**: 解決 95% 的文檔不同步問題

---

## 📅 實施計劃

### Phase 1: 立即修復 (Sprint 2 Day 11 - 完成時間: 0.5 天)

#### Task 1.1: 更新 AI-ASSISTANT-INSTRUCTIONS.md 添加 Instruction 9 🔴
**負責人**: Tech Lead / AI Assistant
**工作內容**:
1. 在 AI-ASSISTANT-INSTRUCTIONS.md 中新增 Instruction 9 章節
2. 定義所有重要事件 (Critical, High, Medium)
3. 為每個事件定義觸發條件和自動操作
4. 添加 AI 行為規則 (主動檢測、主動建議、強制執行)
5. 添加執行範例

**驗收標準**:
- [ ] Instruction 9 章節完整
- [ ] 至少定義 7 個重要事件
- [ ] "集成測試完成" 事件已定義 (Critical)
- [ ] AI 行為規則清晰明確
- [ ] 有完整的執行範例

**預估時間**: 2-3 小時

---

#### Task 1.2: 更新 AI-ASSISTANT-INSTRUCTIONS.md 添加 Instruction 10 🔴
**負責人**: Tech Lead / AI Assistant
**工作內容**:
1. 在 AI-ASSISTANT-INSTRUCTIONS.md 中新增 Instruction 10 章節
2. 定義 Session 結束觸發時機
3. 定義 4 個強制檢查項目
4. 定義 AI 輸出格式
5. 定義自動執行規則 (強制 vs 建議 vs 僅提及)
6. 添加執行範例

**驗收標準**:
- [ ] Instruction 10 章節完整
- [ ] 4 個檢查項目清晰定義
- [ ] 自動執行規則明確
- [ ] AI 輸出格式標準化
- [ ] 有完整的執行範例

**預估時間**: 2-3 小時

---

#### Task 1.3: 測試新 Instructions 🔴
**負責人**: AI Assistant + User
**工作內容**:
1. 模擬 Session 5 場景,測試 Instruction 9 是否正確觸發
2. 模擬 Session 結束,測試 Instruction 10 是否正確執行
3. 驗證 PROJECT-STATUS-REPORT.md 是否正確更新
4. 記錄測試結果和問題

**測試場景**:
```yaml
Scenario 1: 集成測試完成
  Given: 用戶創建了 ≥5 個集成測試
  When: 用戶表示 "集成測試完成"
  Then: AI 自動觸發 Instruction 9 (Event 2)
  And: AI 執行 Instruction 1 更新狀態報告
  And: AI 執行 Instruction 6 驗證文檔一致性
  And: 狀態報告版本號遞增
  And: 測試統計已更新

Scenario 2: Session 結束
  Given: 用戶完成了一些工作
  When: 用戶說 "今天結束了"
  Then: AI 自動觸發 Instruction 10
  And: AI 檢查狀態報告是否最新
  And: AI 檢查是否有未提交變更
  And: AI 建議創建 Session 摘要 (如適用)
  And: AI 輸出完整的檢查結果
```

**驗收標準**:
- [ ] Scenario 1 測試通過
- [ ] Scenario 2 測試通過
- [ ] 沒有誤觸發 (false positive)
- [ ] 沒有漏觸發 (false negative)

**預估時間**: 1-2 小時

---

### Phase 2: 增強驗證 (Sprint 2 Week 1 - 完成時間: Day 14)

#### Task 2.1: 更新 Instruction 6 強化驗證機制 🟡
**負責人**: Tech Lead / AI Assistant
**工作內容**:
1. 在 Instruction 6 中添加"強制執行時機"區塊
2. 添加詳細的驗證內容 (3 個驗證項目)
3. 添加驗證失敗處理邏輯
4. 更新 AI 輸出格式,包含自動修復建議

**驗收標準**:
- [ ] 4 個強制執行時機清晰定義
- [ ] 3 個驗證項目完整 (狀態報告、US 文檔、測試文檔)
- [ ] 驗證失敗處理邏輯明確 (Critical vs Warning)
- [ ] AI 輸出格式包含自動修復選項

**預估時間**: 1 小時

---

#### Task 2.2: 添加 Instructions 優先級矩陣 🟡
**負責人**: Tech Lead / AI Assistant
**工作內容**:
1. 在 AI-ASSISTANT-INSTRUCTIONS.md 開頭添加優先級章節
2. 創建 Instructions 優先級表
3. 定義執行規則 (Critical, High, Medium)
4. 添加優先級覆蓋規則

**驗收標準**:
- [ ] 優先級定義清晰 (Critical, High, Medium)
- [ ] 所有 Instructions 都有優先級標記
- [ ] 執行規則明確 (3 個 Rules)
- [ ] 優先級覆蓋規則已定義

**預估時間**: 1 小時

---

### Phase 3: 完善處理 (Sprint 2 Week 2 - 完成時間: Day 17)

#### Task 3.1: 更新測試失敗處理流程 🟢
**負責人**: Tech Lead / AI Assistant
**工作內容**:
1. 更新 Instruction 3 和 Instruction 7 的測試失敗處理
2. 添加測試分類邏輯 (Critical, Known Issue, Edge Case)
3. 添加決策矩陣
4. 更新文檔更新規則 (測試失敗時也要更新文檔)
5. 更新提交訊息標注規則

**驗收標準**:
- [ ] 測試分類邏輯清晰 (3 種類型)
- [ ] 決策矩陣完整
- [ ] 文檔更新規則明確 (失敗也要記錄)
- [ ] 提交訊息範例包含測試失敗情況

**預估時間**: 2 小時

---

### Phase 4: 驗證和優化 (Sprint 2 Week 2 - 完成時間: Day 18)

#### Task 4.1: 完整測試所有改進 🟡
**負責人**: AI Assistant + User
**工作內容**:
1. 設計綜合測試場景 (涵蓋所有新 Instructions)
2. 執行測試並記錄結果
3. 收集用戶反饋
4. 根據反饋調整 Instructions

**測試場景清單**:
```yaml
Scenario 1: US 完成 (Event 1)
Scenario 2: 集成測試完成 (Event 2)
Scenario 3: Phase 完成 (Event 4)
Scenario 4: Session 結束檢查 (Instruction 10)
Scenario 5: 文檔一致性驗證 (Instruction 6 強制執行)
Scenario 6: 測試失敗處理 (Critical, Known, Edge Case)
Scenario 7: 優先級覆蓋 (用戶臨時調整)
```

**驗收標準**:
- [ ] 所有 7 個場景測試通過
- [ ] 用戶反饋收集完成
- [ ] 根據反饋調整完成
- [ ] 文檔同步率 ≥95%

**預估時間**: 2-3 小時

---

#### Task 4.2: 更新相關文檔 🟡
**負責人**: Tech Lead
**工作內容**:
1. 更新 PROJECT-STATUS-REPORT.md (記錄 AI-ASSISTANT-INSTRUCTIONS.md 改進)
2. 更新 SPRINT-1-RETROSPECTIVE.md (添加文檔同步問題的根本原因分析)
3. 更新 QUICK-START-GUIDE.md (如果有相關章節)
4. 創建 CHANGELOG 記錄 AI-ASSISTANT-INSTRUCTIONS.md v2.0.0 變更

**驗收標準**:
- [ ] 所有相關文檔已更新
- [ ] CHANGELOG 完整記錄變更
- [ ] 文檔一致性檢查通過

**預估時間**: 1 小時

---

### 實施時間線

```
Week 1 (Days 11-14):
├── Day 11: Phase 1 - 立即修復
│   ├── Task 1.1: 添加 Instruction 9 (2-3h)
│   ├── Task 1.2: 添加 Instruction 10 (2-3h)
│   └── Task 1.3: 測試新 Instructions (1-2h)
│
├── Day 14: Phase 2 - 增強驗證
│   ├── Task 2.1: 強化 Instruction 6 (1h)
│   └── Task 2.2: 添加優先級矩陣 (1h)

Week 2 (Days 15-18):
├── Day 17: Phase 3 - 完善處理
│   └── Task 3.1: 更新測試失敗處理 (2h)
│
└── Day 18: Phase 4 - 驗證和優化
    ├── Task 4.1: 完整測試所有改進 (2-3h)
    └── Task 4.2: 更新相關文檔 (1h)
```

**總時間**: 12-16 小時 (分散在 Sprint 2 的 2 週內)

---

### 成功指標

#### 量化指標

| 指標 | 當前值 | 目標值 | 測量方法 |
|------|--------|--------|----------|
| 文檔同步率 | ~50% | ≥95% | (同步 sessions / 總 sessions) × 100% |
| 自動觸發準確率 | N/A | ≥90% | (正確觸發 / 總觸發) × 100% |
| Session 結束檢查執行率 | ~0% | 100% | (執行檢查 / 總 sessions) × 100% |
| 文檔更新及時性 | 8-12h lag | <1h lag | SESSION_END_TIME - DOC_UPDATE_TIME |
| 用戶干預次數 | ~3 次/session | ≤1 次/session | 記錄用戶提醒 AI 更新文檔的次數 |

---

#### 質量指標

```yaml
指標 1: 準確性
  定義: AI 正確識別重要事件並觸發相應操作
  目標: ≥90% 準確率
  測量:
    - True Positive: 正確識別並觸發
    - False Positive: 誤觸發
    - False Negative: 漏觸發
    - Accuracy = (TP + TN) / (TP + TN + FP + FN)

指標 2: 完整性
  定義: 狀態報告包含所有必要資訊
  目標: 100% 必要資訊完整
  檢查清單:
    - [ ] 報告日期最新
    - [ ] 版本號正確
    - [ ] 所有 US 狀態正確
    - [ ] 測試統計完整
    - [ ] Session 歷史完整
    - [ ] 已知問題列表最新

指標 3: 及時性
  定義: 文檔更新延遲時間
  目標: <1 小時 (從事件發生到文檔更新)
  測量: SESSION_END_TIME - DOC_UPDATE_TIME

指標 4: 用戶滿意度
  定義: 用戶對文檔同步流程的滿意程度
  目標: ≥4.0/5.0
  測量: 用戶問卷調查 (1-5 分)
```

---

### 風險和緩解措施

#### 風險 1: AI 行為變更可能導致不可預期的問題 🟡
**機率**: 中 (30%)
**影響**: 中 (可能干擾工作流程)
**緩解措施**:
1. 分階段實施 (Phase 1 → Phase 4)
2. 每個 Phase 都進行充分測試
3. 保留 AI-ASSISTANT-INSTRUCTIONS.md v1.0.0 備份
4. 如果出現問題,可以快速回退

---

#### 風險 2: 自動觸發可能過於頻繁,干擾用戶 🟢
**機率**: 低 (20%)
**影響**: 低 (可以調整觸發條件)
**緩解措施**:
1. 優先級系統 (Critical vs High vs Medium)
2. 用戶可以臨時覆蓋優先級
3. 收集用戶反饋,調整觸發閾值

---

#### 風險 3: 實施時間可能影響 Sprint 2 開發進度 🟡
**機率**: 中 (25%)
**影響**: 中 (可能延遲其他任務)
**緩解措施**:
1. 將工作分散在 Sprint 2 的 2 週內 (不集中在一天)
2. 優先實施 Phase 1 (解決 80% 問題,只需 5-7h)
3. Phase 2-4 可以根據進度靈活調整

---

## 📊 總結

### 根本原因總結

PROJECT-STATUS-REPORT.md 不同步的根本原因是:

**🔴 Primary**: AI-ASSISTANT-INSTRUCTIONS.md v1.0.0 採用"被動響應式"設計,缺少"主動觸發規則",導致 AI 完全依賴用戶明確請求文檔更新,容易在 Session 結束時遺漏關鍵操作。

**🟡 Secondary**:
1. 缺少"重要事件"定義 (尤其是"集成測試完成"事件)
2. Session 結束流程不完整 (沒有強制檢查機制)
3. 驗證機制不足 (Instruction 6 執行時機不明確)
4. 缺少優先級標記 (AI 難以判斷操作重要性)

---

### 改進建議總結

**立即實施** (🔴 P0):
1. 新增 Instruction 9 (重要事件自動觸發規則)
2. 新增 Instruction 10 (Session 結束強制檢查清單)

**短期實施** (🟡 P1):
3. 強化 Instruction 6 (文檔一致性檢查強制性)
4. 添加 Instructions 優先級矩陣

**中期實施** (🟢 P2):
5. 完善測試失敗處理流程

**預期效果**: 解決 95% 的文檔不同步問題

---

### 成功標準

改進完成後,應該達到:
- ✅ 文檔同步率 ≥95%
- ✅ 自動觸發準確率 ≥90%
- ✅ Session 結束檢查執行率 100%
- ✅ 文檔更新延遲 <1 小時
- ✅ 用戶干預次數 ≤1 次/session

---

**文檔版本**: 1.0.0
**創建日期**: 2025-11-06
**創建者**: AI Assistant (Claude Code)
**下一步**: 實施 Phase 1 (立即修復)
