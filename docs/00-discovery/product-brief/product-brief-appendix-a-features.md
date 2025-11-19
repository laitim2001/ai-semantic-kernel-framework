# 附錄 A: MVP 核心功能詳細規格

**返回**: [Product Brief 主文檔](./product-brief.md)

**日期**: 2025-11-17  
**版本**: v1.0

---

## 📋 功能清單總覽

本附錄詳細說明 MVP 的 **14 個核心功能**，包含功能描述、商業價值、技術實現、開發時間和優先級。

### 功能分類

| 類別 | 功能數量 | 開發週數 | 優先級分布 |
|------|---------|---------|-----------|
| **核心編排引擎** | 2 | 2 週 | 2 × P0 |
| **創新整合功能** | 3 | 5 週 | 1 × P0, 2 × P1 |
| **開發效率工具** | 2 | 5 週 | 2 × P0 |
| **可靠性和可觀測性** | 5 | 6.5 週 | 5 × P0 |
| **用戶界面** | 1 | 2 週 | 1 × P0 |
| **數據和性能** | 1 | 1 週 | 1 × P0 |
| **總計** | **14** | **~18-20 週（串行）** | **12 × P0, 2 × P1** |

**優化後總時長**: 12-14 週（並行開發）

---

## 🏗️ 類別 1: 核心編排引擎（Foundation）

### ✅ 功能 1: Sequential Agent 編排

#### 功能描述
使用 Python 代碼定義 Agent 工作流，支持順序執行、條件分支、循環邏輯和異步並發。

#### 來源決策
- **決策 2**: 純程式碼 Sequential 編排
- **決策 5**: 允許不同場景用不同編排方式

#### 商業價值
🟢 **核心基礎** - 所有自動化場景的執行引擎

#### 用戶價值
- 開發者可以使用熟悉的 Python 語法快速創建工作流
- 完全靈活，支持任意複雜的業務邏輯
- IDE 支持（代碼提示、調試、重構）

#### 技術實現
```python
# 示例：IT 場景 - 服務器健康巡檢
async def server_health_check_workflow(server_list: List[str]):
    """
    順序執行：
    1. 並行檢查所有服務器
    2. 分析異常
    3. 生成報告
    4. 發送通知
    """
    # Step 1: 並行檢查（異步）
    tasks = [health_check_agent.run(server=s) for s in server_list]
    results = await asyncio.gather(*tasks)
    
    # Step 2: 分析異常（條件邏輯）
    anomalies = [r for r in results if r.status != "healthy"]
    
    if len(anomalies) > 0:
        # Step 3: 智能分析
        analysis = await analyzer_agent.run(
            task="分析異常原因",
            context=anomalies
        )
        
        # Step 4: Checkpoint（如果有高風險異常）
        if analysis.risk_level == "high":
            checkpoint_id = await save_checkpoint({
                "analysis": analysis,
                "recommended_actions": analysis.actions
            })
            return {"status": "pending_approval", "checkpoint_id": checkpoint_id}
    
    # Step 5: 生成報告
    report = await report_generator.run(results=results)
    
    # Step 6: 發送通知
    await teams_notifier.send(report)
    
    return {"status": "completed", "report": report}
```

#### 關鍵特性
- ✅ 純 Python 代碼定義
- ✅ 支持異步並發（`asyncio.gather`）
- ✅ 條件分支（`if/else`）
- ✅ 循環邏輯（`for/while`）
- ✅ 錯誤處理（`try/except`）
- ✅ 完整 IDE 支持

#### 開發時間
設計階段（核心架構，Week 1-3）

#### 技術複雜度
⭐⭐⭐ 中等

#### MVP 優先級
🔴 **P0（最高，基礎設施）**

---

### ✅ 功能 2: Human-in-the-loop Checkpointing

#### 功能描述
工作流執行到關鍵點時自動暫停，等待人工審批後繼續執行。支持斷點恢復、狀態持久化和審批決策記錄。

#### 來源決策
- **決策 4**: 自行實現 Checkpointing（PostgreSQL State Manager）
- **決策 21**: 靈活配置化 Checkpointing（YAML 配置）

#### 商業價值
🟢 **核心差異化** - 確保高風險操作的安全性和可控性

#### 用戶價值
- **IT/CS 團隊**: 可以控制 Agent 的關鍵決策，避免錯誤操作
- **管理層**: 符合企業治理要求，審計追蹤完整
- **系統**: 工作流可恢復，避免重複執行

#### 技術實現

##### 1. State Manager 設計
```python
class StateManager:
    """自行實現的 Checkpoint 狀態管理器"""
    
    def __init__(self, db: PostgreSQL):
        self.db = db
    
    async def save_checkpoint(
        self,
        workflow_id: str,
        step: str,
        data: dict,
        risk_level: str = "medium"
    ) -> str:
        """
        保存 Checkpoint 狀態到 PostgreSQL
        
        Returns:
            checkpoint_id: 用於後續恢復
        """
        checkpoint = {
            "id": str(uuid.uuid4()),
            "workflow_id": workflow_id,
            "step": step,
            "data": json.dumps(data),
            "status": "pending_approval",
            "risk_level": risk_level,
            "created_at": datetime.utcnow(),
        }
        
        await self.db.checkpoints.insert(checkpoint)
        
        # 記錄審計日誌
        await self.audit_log(
            action="checkpoint_created",
            checkpoint_id=checkpoint["id"]
        )
        
        return checkpoint["id"]
    
    async def load_checkpoint(self, checkpoint_id: str) -> dict:
        """從 PostgreSQL 載入 Checkpoint 狀態"""
        checkpoint = await self.db.checkpoints.get(checkpoint_id)
        
        if checkpoint.status != "approved":
            raise CheckpointNotApprovedException()
        
        return {
            "workflow_id": checkpoint.workflow_id,
            "step": checkpoint.step,
            "data": json.loads(checkpoint.data),
        }
    
    async def approve_checkpoint(
        self,
        checkpoint_id: str,
        approver: str,
        comment: str = None,
        modifications: dict = None
    ):
        """審批 Checkpoint（批准或修改）"""
        await self.db.checkpoints.update(
            id=checkpoint_id,
            status="approved",
            approver=approver,
            approved_at=datetime.utcnow(),
            comment=comment,
            modifications=json.dumps(modifications) if modifications else None
        )
        
        # 記錄學習數據（Few-shot）
        if modifications:
            await self.save_learning_case(checkpoint_id, modifications)
        
        # 記錄審計日誌
        await self.audit_log(
            action="checkpoint_approved",
            checkpoint_id=checkpoint_id,
            approver=approver
        )
```

##### 2. YAML 配置化 Checkpoint
```yaml
# checkpoints.yaml - 配置 Checkpoint 觸發條件
checkpoints:
  - name: "high_risk_operation"
    trigger:
      risk_level: "high"
      operation_type: ["delete", "update", "permission_change"]
    approval_required: true
    auto_approve_rules:
      - condition: "risk_score < 30"
        action: "auto_approve"
      - condition: "user_role == 'admin'"
        action: "auto_approve"
    timeout: 24h
    escalation:
      - after: 2h
        notify: ["manager"]
      - after: 12h
        notify: ["director"]

  - name: "medium_risk_operation"
    trigger:
      risk_level: "medium"
    approval_required: false  # 僅記錄，不阻塞
    audit_only: true
```

##### 3. 工作流恢復機制
```python
async def resume_workflow_from_checkpoint(checkpoint_id: str):
    """從 Checkpoint 恢復工作流執行"""
    # 1. 載入狀態
    state = await state_manager.load_checkpoint(checkpoint_id)
    
    # 2. 獲取批准的修改（如果有）
    checkpoint = await db.checkpoints.get(checkpoint_id)
    if checkpoint.modifications:
        modifications = json.loads(checkpoint.modifications)
        state["data"].update(modifications)
    
    # 3. 繼續執行工作流（從暫停的步驟開始）
    workflow = get_workflow(state["workflow_id"])
    result = await workflow.resume_from_step(
        step=state["step"],
        context=state["data"]
    )
    
    # 4. 更新執行記錄
    await db.executions.update(
        workflow_id=state["workflow_id"],
        status="completed",
        result=result
    )
    
    return result
```

#### 實施細節
- **數據庫**: PostgreSQL 存儲狀態（JSON 類型）
- **序列化**: Python `json.dumps/loads`
- **事務**: 使用 PostgreSQL 事務確保一致性
- **超時**: 支持審批超時自動升級
- **通知**: Teams Adaptive Card 通知審批請求

#### 開發時間
2 週

#### 技術複雜度
⭐⭐⭐⭐ 高（狀態管理、事務一致性）

#### MVP 優先級
🔴 **P0（最高，核心功能）**

---

## 🔗 類別 2: 創新整合功能（Innovation）

### ✅ 功能 3: 跨系統智能關聯分析

#### 功能描述
自動關聯 ServiceNow、Dynamics 365、SharePoint 的數據，提供統一視圖和智能洞察。使用 LLM 分析跨系統數據模式，提供智能建議。

#### 來源決策
- **決策 7**: 跨系統關聯分析（MVP 必須，核心差異化）

#### 商業價值
🟢 **核心差異化** - 創造指數級價值（1×1×1=5+，不是簡單的 1+1+1=3）

#### 用戶價值

**CS 團隊**:
- 查客戶信息時自動顯示：
  - Dynamics 365: 客戶基本信息、銷售記錄
  - ServiceNow: 歷史工單、問題類型統計
  - SharePoint: 合同文件、會議記錄
- 智能洞察：「該客戶最近 3 個工單都是『登錄慢』，建議檢查服務器負載」

**IT 團隊**:
- 排查問題時自動關聯：
  - Azure Monitor: 服務器監控數據、性能指標
  - ServiceNow: 相關工單、變更記錄
  - SharePoint: 配置文檔、運維手冊
- 智能診斷：「數據庫慢查詢與 2 天前的索引變更相關」

#### 技術實現

##### 1. 跨系統查詢 Agent
```python
class CrossSystemCorrelationAgent:
    """跨系統智能關聯 Agent"""
    
    def __init__(
        self,
        servicenow_client: ServiceNowAPI,
        dynamics_client: Dynamics365API,
        sharepoint_client: SharePointAPI,
        llm: AzureOpenAI,
        cache: Redis
    ):
        self.servicenow = servicenow_client
        self.dynamics = dynamics_client
        self.sharepoint = sharepoint_client
        self.llm = llm
        self.cache = cache
    
    async def get_customer_360_view(self, customer_id: str) -> dict:
        """
        獲取客戶 360 度視圖（跨系統關聯）
        
        流程：
        1. 並行查詢 3 個系統
        2. 檢查 Redis 緩存
        3. LLM 智能關聯和總結
        4. 返回統一視圖
        """
        # 檢查緩存
        cache_key = f"customer_360:{customer_id}"
        cached = await self.cache.get(cache_key)
        if cached:
            return json.loads(cached)
        
        # 並行查詢 3 個系統
        tasks = [
            self.query_dynamics(customer_id),      # 客戶信息
            self.query_servicenow(customer_id),    # 工單歷史
            self.query_sharepoint(customer_id),    # 文檔和合同
        ]
        
        dynamics_data, servicenow_data, sharepoint_data = await asyncio.gather(*tasks)
        
        # LLM 智能關聯分析
        correlation_result = await self.llm_correlate({
            "customer": dynamics_data,
            "tickets": servicenow_data,
            "documents": sharepoint_data
        })
        
        # 構建統一視圖
        unified_view = {
            "customer_info": dynamics_data,
            "ticket_history": servicenow_data,
            "documents": sharepoint_data,
            "insights": correlation_result["insights"],
            "recommendations": correlation_result["recommendations"]
        }
        
        # 緩存結果（TTL 1 天）
        await self.cache.setex(cache_key, 86400, json.dumps(unified_view))
        
        return unified_view
    
    async def llm_correlate(self, data: dict) -> dict:
        """使用 LLM 分析跨系統數據並提供洞察"""
        prompt = f"""
        分析以下客戶的跨系統數據，提供智能洞察和建議：

        客戶信息（Dynamics 365）:
        {json.dumps(data["customer"], indent=2)}

        工單歷史（ServiceNow）:
        {json.dumps(data["tickets"], indent=2)}

        相關文檔（SharePoint）:
        {json.dumps(data["documents"], indent=2)}

        請分析：
        1. 是否有重複出現的問題模式？
        2. 客戶的服務器負載趨勢如何？
        3. 是否需要採取預防性措施？
        4. 給出 3 條具體的行動建議

        回答格式（JSON）:
        {{
            "insights": ["洞察1", "洞察2", ...],
            "patterns": ["模式1", "模式2", ...],
            "recommendations": [
                {{"action": "建議1", "priority": "high", "reason": "理由"}},
                ...
            ]
        }}
        """
        
        response = await self.llm.chat.completions.create(
            model="gpt-4o",
            messages=[{"role": "user", "content": prompt}],
            response_format={"type": "json_object"}
        )
        
        return json.loads(response.choices[0].message.content)
```

##### 2. 實際使用示例
```python
# CS Agent 使用跨系統關聯
async def handle_customer_inquiry(ticket_id: str):
    """處理客戶諮詢工單"""
    
    # 1. 獲取工單信息
    ticket = await servicenow.get_ticket(ticket_id)
    customer_id = ticket.customer_id
    
    # 2. 獲取客戶 360 視圖（跨系統關聯）
    customer_360 = await cross_system_agent.get_customer_360_view(customer_id)
    
    # 3. 智能洞察
    print("=== 客戶 360 視圖 ===")
    print(f"客戶名稱: {customer_360['customer_info']['name']}")
    print(f"歷史工單: {len(customer_360['ticket_history'])} 個")
    print(f"相關文檔: {len(customer_360['documents'])} 個")
    
    print("\n=== 智能洞察 ===")
    for insight in customer_360["insights"]:
        print(f"• {insight}")
    
    print("\n=== 行動建議 ===")
    for rec in customer_360["recommendations"]:
        print(f"• [{rec['priority']}] {rec['action']}")
        print(f"  理由: {rec['reason']}")
    
    # 4. 基於洞察生成解決方案
    solution = await solution_generator.run(
        ticket=ticket,
        context=customer_360
    )
    
    return solution
```

##### 3. 系統 API 適配器
```python
class ServiceNowAdapter:
    """ServiceNow REST API 適配器"""
    
    async def get_customer_tickets(self, customer_id: str) -> List[dict]:
        """獲取客戶所有工單"""
        response = await self.client.get(
            f"/api/now/table/incident",
            params={
                "sysparm_query": f"caller_id={customer_id}",
                "sysparm_fields": "number,short_description,state,priority,created_on",
                "sysparm_limit": 100
            }
        )
        return response.json()["result"]

class Dynamics365Adapter:
    """Dynamics 365 Web API 適配器"""
    
    async def get_customer_info(self, customer_id: str) -> dict:
        """獲取客戶基本信息和銷售記錄"""
        response = await self.client.get(
            f"/api/data/v9.2/accounts({customer_id})",
            params={
                "$select": "name,accountnumber,revenue,industrycode",
                "$expand": "opportunity_customer_accounts($select=name,estimatedvalue,actualvalue)"
            }
        )
        return response.json()

class SharePointAdapter:
    """SharePoint Search API 適配器"""
    
    async def search_customer_documents(self, customer_id: str) -> List[dict]:
        """搜索客戶相關文檔"""
        query = f"ContentType:Document AND Customer:{customer_id}"
        response = await self.client.post(
            "/_api/search/postquery",
            json={
                "request": {
                    "Querytext": query,
                    "RowLimit": 50,
                    "SelectProperties": ["Title", "Path", "LastModifiedTime", "Author"]
                }
            }
        )
        return response.json()["PrimaryQueryResult"]["RelevantResults"]["Table"]["Rows"]
```

#### 實施細節
- **並行查詢**: 使用 `asyncio.gather` 並發調用 3 個 API
- **超時控制**: 每個 API 調用 5 秒超時
- **降級策略**: 單個系統失敗不影響其他系統數據展示
- **緩存策略**: Redis 緩存查詢結果（TTL 1 天）
- **智能分析**: LLM (GPT-4o) 關聯分析和模式識別

#### 開發時間
2 週

#### 技術複雜度
⭐⭐⭐⭐ 高（多系統整合、LLM 調用、緩存策略）

#### MVP 優先級
🔴 **P0（最高，核心差異化功能）**

---

### ✅ 功能 4: 跨場景協作（CS ↔ IT）

#### 功能描述
CS 工單處理中可以自動觸發 IT 運維 Agent，實現跨部門協作。MVP 實現單向觸發（CS → IT），Phase 2 擴展雙向協作。

#### 來源決策
- **決策 10**: 跨場景協作（MVP 可行）

#### 商業價值
🟢 **高價值** - 打破部門牆，自動化跨部門協作

#### 用戶價值
- **CS 團隊**: 發現技術問題時，無需手動創建 IT 工單，自動觸發 IT 排查
- **IT 團隊**: 自動接收 CS 觸發的排查請求，結果自動反饋給 CS
- **客戶**: 問題解決更快，體驗更好

#### 技術實現
```python
# CS Agent 觸發 IT Agent 示例
async def cs_handle_technical_complaint(ticket_id: str):
    """CS 處理客戶技術投訴"""
    
    # 1. 分析工單
    ticket = await servicenow.get_ticket(ticket_id)
    analysis = await ticket_analyzer.run(ticket=ticket)
    
    # 2. 判斷是否需要 IT 支持
    if analysis.requires_it_support:
        # 3. 自動觸發 IT Agent（跨場景協作）
        it_workflow_id = await trigger_it_workflow(
            trigger_source="cs_ticket",
            cs_ticket_id=ticket_id,
            issue_description=analysis.technical_issue,
            priority=analysis.priority
        )
        
        # 4. 等待 IT 排查結果（異步，不阻塞 CS 工作流）
        # 結果會通過 callback 返回給 CS Agent
        await register_callback(
            workflow_id=it_workflow_id,
            callback_url=f"/api/cs/tickets/{ticket_id}/it_result"
        )
        
        # 5. 更新 CS 工單狀態
        await servicenow.update_ticket(
            ticket_id,
            status="waiting_for_it",
            it_workflow_id=it_workflow_id
        )
        
        return {"status": "it_triggered", "it_workflow_id": it_workflow_id}
    
    # 如果不需要 IT，繼續 CS 流程
    return await cs_standard_process(ticket)

# IT Agent 被觸發
async def it_diagnose_issue(request: ITDiagnoseRequest):
    """IT 自動排查技術問題（由 CS 觸發）"""
    
    # 1. 獲取問題描述
    issue = request.issue_description
    
    # 2. 自動排查
    diagnosis = await it_diagnostic_agent.run(
        issue=issue,
        systems_to_check=["server", "database", "network"]
    )
    
    # 3. 生成解決方案
    solution = await it_solution_agent.run(diagnosis=diagnosis)
    
    # 4. 結果反饋給 CS（callback）
    await notify_cs_agent(
        cs_ticket_id=request.cs_ticket_id,
        diagnosis=diagnosis,
        solution=solution
    )
    
    return solution
```

#### 實施細節
- **觸發機制**: CS Agent 通過 REST API 調用觸發 IT Workflow
- **異步通信**: 使用 Webhook callback 返回結果
- **狀態同步**: ServiceNow 工單狀態實時更新
- **優先級傳遞**: CS 工單優先級自動傳遞給 IT Workflow

#### 開發時間
2 週

#### 技術複雜度
⭐⭐⭐ 中等

#### MVP 優先級
🟡 **P1（高價值，可選）**

---

### ✅ 功能 5: 學習型人機協作（基礎版）

#### 功能描述
Agent 記錄人工審批時的修改和決策理由，使用 Few-shot Learning 提升未來的決策準確率。

#### 來源決策
- **決策 11**: 學習型人機協作（基礎版 MVP，完整 ML 訓練 Phase 2）

#### 商業價值
🟡 **體驗優化** - Agent 越用越智能，準確率隨時間提升

#### 用戶價值
- **準確率提升**: Month 1: 60% → Month 12: 90%+
- **減少審批**: 相同場景準確率提升後，人工介入率下降
- **個性化**: Agent 學習特定用戶/團隊的偏好

#### 技術實現
```python
# 1. 記錄人工修改（Learning 數據）
async def record_human_modification(checkpoint_id: str, modifications: dict):
    """記錄人工審批時的修改"""
    
    checkpoint = await db.checkpoints.get(checkpoint_id)
    
    learning_case = {
        "id": str(uuid.uuid4()),
        "checkpoint_id": checkpoint_id,
        "workflow_type": checkpoint.workflow_type,
        "original_decision": checkpoint.data,
        "human_modification": modifications,
        "approver": checkpoint.approver,
        "comment": checkpoint.comment,
        "created_at": datetime.utcnow()
    }
    
    # 存儲到 Learning 案例庫
    await db.learning_cases.insert(learning_case)

# 2. 提取 Few-shot 示例
async def get_few_shot_examples(workflow_type: str, limit: int = 3) -> List[dict]:
    """獲取相似場景的人工修改案例"""
    
    cases = await db.learning_cases.query(
        workflow_type=workflow_type,
        order_by="created_at DESC",
        limit=limit
    )
    
    examples = []
    for case in cases:
        examples.append({
            "input": case.original_decision,
            "output": case.human_modification,
            "reason": case.comment
        })
    
    return examples

# 3. LLM Prompt 注入 Few-shot 示例
async def llm_call_with_learning(prompt: str, workflow_type: str):
    """LLM 調用時動態注入 Few-shot 案例"""
    
    # 獲取相似案例
    examples = await get_few_shot_examples(workflow_type, limit=3)
    
    # 構建 Few-shot Prompt
    few_shot_prompt = f"""
    你是一個智能 Agent，正在處理 {workflow_type} 場景。
    
    以下是類似場景中人工修改的案例，請從中學習：
    
    """
    
    for i, ex in enumerate(examples, 1):
        few_shot_prompt += f"""
    案例 {i}:
    原始決策: {json.dumps(ex['input'], ensure_ascii=False)}
    人工修改: {json.dumps(ex['output'], ensure_ascii=False)}
    修改理由: {ex['reason']}
    
    """
    
    few_shot_prompt += f"""
    現在請處理當前場景：
    {prompt}
    
    請參考以上案例的修改模式，給出更準確的決策。
    """
    
    # 調用 LLM
    response = await llm.chat.completions.create(
        model="gpt-4o",
        messages=[{"role": "user", "content": few_shot_prompt}]
    )
    
    return response.choices[0].message.content
```

#### 實施細節
- **案例存儲**: PostgreSQL `learning_cases` 表
- **案例檢索**: 按 `workflow_type` 查詢最近 3-5 個案例
- **Prompt 注入**: 動態構建 Few-shot Prompt
- **隱私保護**: 敏感數據脫敏（如客戶姓名、電話）

#### 開發時間
1 週

#### 技術複雜度
⭐⭐ 低（基礎版本，不做複雜 ML 訓練）

#### MVP 優先級
🟡 **P1（體驗優化）**

---

## 🛠️ 類別 3: 開發效率工具（Developer Experience）

### ✅ 功能 6: Agent Marketplace（內部版）

#### 功能描述
內部 Agent 模板庫，開發者可以快速複製和部署常見場景 Agent。MVP 包含 6-8 個生產級模板。

#### 來源決策
- **決策 8**: Agent Marketplace（內部版，戰略投資）

#### 商業價值
🟢 **戰略投資** - 加速部署 5 倍（2 週 → 1 天）

#### 用戶價值
- **開發者**: 不用從零開始，直接使用模板，10 分鐘完成部署
- **團隊**: 積累最佳實踐，標準化工作流
- **管理層**: 加速自動化覆蓋率

#### MVP 內置模板（6-8 個）

##### IT 場景模板
1. **服務器健康巡檢 Agent**
   - 自動檢查 CPU、內存、磁盤使用率
   - 異常檢測和告警
   - 生成巡檢報告

2. **用戶權限申請審批 Agent**
   - 分析權限申請風險
   - 低風險自動批准，高風險人工審批
   - 自動執行權限配置

3. **數據庫備份狀態檢查 Agent**
   - 檢查數據庫備份是否成功
   - 備份文件完整性驗證
   - 異常通知

##### CS 場景模板
4. **客戶信息智能查詢 Agent**
   - 跨系統查詢客戶數據（360 視圖）
   - 智能總結客戶歷史
   - 推薦相關知識庫文章

5. **工單自動分類 Agent**
   - NLP 分析工單描述
   - 自動分類（技術/產品/計費）
   - 智能路由到對應團隊

6. **知識庫文章推薦 Agent**
   - 基於工單描述推薦相關文章
   - 語義搜索 SharePoint 知識庫
   - 自動回覆常見問題

##### 通用模板
7. **定時報表生成 Agent**
   - 定時查詢數據
   - 生成可視化報表
   - 發送到 Teams 頻道

8. **異常告警處理 Agent**
   - 監聽系統告警
   - 自動排查常見問題
   - 升級或自動修復

#### 技術實現
```python
# Marketplace 數據模型
class AgentTemplate:
    id: str
    name: str
    description: str
    category: str  # "IT", "CS", "General"
    version: str
    author: str
    tags: List[str]
    code_template: str  # Python 代碼模板
    config_schema: dict  # YAML 配置 Schema
    usage_count: int
    rating: float
    created_at: datetime
    updated_at: datetime

# Marketplace API
@app.get("/api/marketplace/templates")
async def list_templates(category: Optional[str] = None):
    """列出所有模板"""
    if category:
        templates = await db.templates.query(category=category)
    else:
        templates = await db.templates.all()
    
    return {"templates": templates}

@app.post("/api/marketplace/templates/{template_id}/deploy")
async def deploy_template(template_id: str, config: dict):
    """部署模板（複製並配置）"""
    
    # 1. 獲取模板
    template = await db.templates.get(template_id)
    
    # 2. 替換配置參數
    agent_code = template.code_template
    for key, value in config.items():
        agent_code = agent_code.replace(f"{{{{ {key} }}}}", str(value))
    
    # 3. 創建新 Agent
    new_agent = {
        "id": str(uuid.uuid4()),
        "name": config["agent_name"],
        "template_id": template_id,
        "code": agent_code,
        "config": config,
        "status": "active",
        "created_at": datetime.utcnow()
    }
    
    await db.agents.insert(new_agent)
    
    # 4. 更新模板使用統計
    await db.templates.update(template_id, usage_count=template.usage_count + 1)
    
    return {"agent_id": new_agent["id"], "status": "deployed"}
```

#### 實施細節
- **模板存儲**: PostgreSQL `templates` 表
- **代碼模板**: Jinja2 變量替換 `{{ variable }}`
- **配置 Schema**: JSON Schema 驗證配置參數
- **UI**: Marketplace 瀏覽頁面（搜索、篩選、預覽）

#### 開發時間
3 週（包含 6-8 個模板開發）

#### 技術複雜度
⭐⭐⭐ 中等

#### MVP 優先級
🟢 **P0（戰略投資）**

---

### ✅ 功能 7: DevUI 整合（Microsoft DevUI）

#### 功能描述
整合微軟 DevUI 工具，可視化 Agent 執行過程、調試斷點、查看 LLM 調用鏈和 Token 使用。

#### 來源決策
- **決策 9**: DevUI 整合（必要投資）

#### 商業價值
🟢 **開發體驗** - 提升開發和排查效率 4-6 倍

#### 用戶價值
- **開發者**: 快速定位問題，可視化調試，10-30 分鐘排查問題（vs 2-4 小時）
- **運維**: 生產問題排查更快，減少停機時間

#### 技術實現
- **安裝**: `pip install microsoft-devui`
- **配置**: 連接 Agent Framework Runtime
- **可視化**: 
  - Agent 執行流程圖
  - LLM 調用鏈追蹤
  - 變量和狀態查看
  - 斷點調試

#### 開發時間
2 週

#### 技術複雜度
⭐⭐⭐ 中等

#### MVP 優先級
🔴 **P0（必要投資）**

---

## 🔧 類別 4: 可靠性和可觀測性（Reliability）

### ✅ 功能 8-12

**詳細說明省略，請參閱主文檔功能清單**

- ✅ 功能 8: n8n 觸發 + 錯誤處理（1-2 週，P0）
- ✅ 功能 9: Prompt 管理和 LLM 追蹤（1 週，P0）
- ✅ 功能 10: 審計追蹤（1 週，P0）
- ✅ 功能 11: Teams 通知整合（3 天，P0）
- ✅ 功能 12: 監控 Dashboard 基礎版（2 週，P0）

---

## 🎨 類別 5: 用戶界面（User Experience）

### ✅ 功能 13: 現代化 Web UI

#### 功能描述
友好、時尚、現代化的 React UI，支持工作流管理、執行監控、審批處理。

#### 來源決策
- **決策 20**: UI/UX 不能簡化（戰略性投資，不可妥協）

#### 商業價值
⚠️ **戰略性投資** - 用戶體驗是產品成功的關鍵

#### MVP 核心頁面
1. Dashboard（執行狀態總覽）
2. Agent 列表和詳情
3. 執行歷史和日誌查看
4. Checkpoint 審批界面
5. Marketplace（瀏覽模板）

#### 開發時間
2 週（增加投資）

#### 技術複雜度
⭐⭐⭐⭐ 高

#### MVP 優先級
🔴 **P0（不可妥協）**

---

## 🗄️ 類別 6: 數據和性能（Data & Performance）

### ✅ 功能 14: Redis 緩存

#### 功能描述
Redis 緩存 LLM 響應、系統查詢結果，提升性能和降低成本。

#### 來源決策
- **決策 25**: Redis 必須實現（避免後期重構）

#### 商業價值
⚠️ **性能需求** - 響應速度提升 3-5 倍，LLM 成本降低 20%+

#### 開發時間
1 週

#### 技術複雜度
⭐⭐ 低

#### MVP 優先級
🔴 **P0（性能基礎）**

---

## 📊 功能優先級總結

### P0 功能（12 個，必須完成）
1. Sequential 編排
2. Checkpointing
3. 跨系統關聯
6. Marketplace
7. DevUI
8. n8n 觸發
9. Prompt 管理
10. 審計追蹤
11. Teams 通知
12. 監控 Dashboard
13. 現代化 UI
14. Redis 緩存

### P1 功能（2 個，高價值可選）
4. 跨場景協作
5. 學習型協作

---

## 📅 開發順序建議

基於依賴關係和風險，建議開發順序：

1. **Week 1-3**: 功能 1, 2, 14（核心引擎 + Checkpointing + Redis）
2. **Week 4-5**: 功能 8, 9, 10, 11（觸發 + Prompt + 審計 + Teams）
3. **Week 6-7**: 功能 3, 4（跨系統關聯 + 跨場景協作）
4. **Week 8-9**: 功能 6, 7, 5（Marketplace + DevUI + 學習協作）
5. **Week 10-11**: 功能 13, 12（UI + Dashboard）
6. **Week 12-13**: 整合測試和優化
7. **Week 14**: 部署上線

---

**返回**: [Product Brief 主文檔](./product-brief.md)
