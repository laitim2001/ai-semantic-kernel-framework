# SCAMPER 詳細分析: A - Adapt (調整)

**返回**: [Overview](../02-scamper-method-overview.md) | [導航](../02-scamper-method.md)

---

    - å•é¡Œ: {checkpoint['issue']}
    - å»ºè­°æ–¹æ¡ˆ: {checkpoint['solution']}
    - ç‚ºä»€éº¼é¸é€™å€‹æ–¹æ¡ˆ: {checkpoint['reasoning']}
    
    èªžæ°£: å‹å¥½ã€å°ˆæ¥­ã€ç°¡æ½”
    """
    
    explanation = await llm.generate(conversation_prompt)
    
    # ç­‰å¾…ç”¨æˆ¶å›žè¦†
    user_response = await get_user_input()
    
    # ç†è§£ç”¨æˆ¶æ„åœ–
    intent = await parse_approval_intent(user_response)
    
    if intent["action"] == "approve":
        await resume_workflow(checkpoint_id, approved=True)
    elif intent["action"] == "reject":
        await resume_workflow(checkpoint_id, approved=False, reason=intent["reason"])
    elif intent["action"] == "modify":
        # ç”¨æˆ¶æå‡ºä¿®æ”¹å»ºè­°
        modified_solution = await agent.refine_solution(
            original=checkpoint['solution'],
            user_feedback=intent["modification"]
        )
        # é¡¯ç¤ºä¿®æ”¹å¾Œçš„æ–¹æ¡ˆï¼Œå†æ¬¡ç¢ºèª
        await show_modified_solution(modified_solution)
```

**MVP å¯¦ç¾**: ðŸ”„ Phase 2 (MVP å…ˆç”¨å‚³çµ±å¯©æ‰¹ï¼Œé«”é©—æ›´å¥½å¾Œå†å‡ç´š)

---

#### **ç”¢å“åˆ†æž B**: Dify AI (AI æ‡‰ç”¨é–‹ç™¼å¹³å°)

**æ ¸å¿ƒå„ªå‹¢**:
```
1. Visual Workflow Builder
   - LLM Node (èª¿ç”¨æ¨¡åž‹)
   - Tool Node (èª¿ç”¨å·¥å…·)
   - Conditional Node (æ¢ä»¶åˆ†æ”¯)
   - Variable Management

2. Prompt ç®¡ç†
   - Prompt Template åº«
   - Version Control
   - A/B Testing

3. å¯è§€æ¸¬æ€§
   - Trace æ¯æ¬¡ LLM èª¿ç”¨
   - Token ä½¿ç”¨çµ±è¨ˆ
   - æˆæœ¬åˆ†æž
```

**å¯å€Ÿé‘’åˆ° Agent Platform**:

#### å€Ÿé‘’é»ž 6: **Prompt Template ç®¡ç†** âœ… é«˜å„ªå…ˆç´š

```
å•é¡Œ: Agent çš„ Prompt æ•£è½åœ¨ä»£ç¢¼ä¸­
- é›£ä»¥ç®¡ç†å’Œæ›´æ–°
- ç„¡æ³• A/B æ¸¬è©¦
- æ²’æœ‰ç‰ˆæœ¬æŽ§åˆ¶

Dify çš„è§£æ±ºæ–¹æ¡ˆ:
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Prompt Template Library        â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ cs_ticket_analyzer_v1.2        â”‚
â”‚ cs_solution_generator_v1.0     â”‚
â”‚ it_diagnostic_agent_v2.1       â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

æˆ‘å€‘çš„è¨­è¨ˆ:
# prompts/cs_ticket_analyzer.yaml
name: "CS Ticket Analyzer"
version: "1.2"
description: "åˆ†æžå®¢æœå·¥å–®ï¼Œæå–é—œéµä¿¡æ¯"
system_message: |
  ä½ æ˜¯å°ˆæ¥­çš„å®¢æœå·¥å–®åˆ†æžå°ˆå®¶ã€‚
  
  åˆ†æžå·¥å–®æ™‚ï¼Œä½ éœ€è¦:
  1. è­˜åˆ¥å•é¡Œé¡žåž‹ (æŠ€è¡“/å¸³è™Ÿ/è¨ˆè²»/å…¶ä»–)
  2. åˆ¤æ–·å„ªå…ˆç´š (é«˜/ä¸­/ä½Ž)
  3. æå–é—œéµä¿¡æ¯ (å®¢æˆ¶ IDã€å½±éŸ¿ç¯„åœã€ç·Šæ€¥ç¨‹åº¦)
  
  è¼¸å‡ºæ ¼å¼ (JSON):
  {
    "issue_type": "...",
    "priority": "...",
    "customer_id": "...",
    "impact": "...",
    "urgency": "..."
  }

variables:
  - name: "ticket_id"
    type: "string"
    required: true
  - name: "ticket_content"
    type: "string"
    required: true

examples:  # Few-shot learning
  - input: "å®¢æˆ¶åé¥‹ç„¡æ³•ç™»éŒ„ï¼Œè¨‚å–®è™Ÿ #12345"
    output: |
      {
        "issue_type": "technical",
        "priority": "high",
        "customer_id": "extracted_from_order",
        "impact": "customer_blocked",
        "urgency": "immediate"
      }

# ä½¿ç”¨ Prompt Template
from jinja2 import Template

def load_prompt_template(template_name: str, version: str = "latest"):
    template_file = f"prompts/{template_name}.yaml"
    config = yaml.load(open(template_file))
    
    if version != "latest":
        # è¼‰å…¥ç‰¹å®šç‰ˆæœ¬ (å¾ž Git æˆ–æ•¸æ“šåº«)
        config = load_version(template_name, version)
    
    return config

# å‰µå»º Agent æ™‚ä½¿ç”¨ Template
async def create_agent_from_template(template_name: str):
    template = load_prompt_template(template_name)
    
    agent = AssistantAgent(
        name=template["name"],
        model_client=model_client,
        system_message=template["system_message"]
    )
    
    return agent
```

**MVP å¯¦ç¾**: âœ… å¿…é ˆæœ‰ Prompt ç®¡ç† (YAML é…ç½®æ–‡ä»¶)

---

#### å€Ÿé‘’é»ž 7: **LLM èª¿ç”¨è¿½è¹¤å’Œæˆæœ¬åˆ†æž** âœ… MVP å¿…é ˆ

```
Dify çš„ Trace åŠŸèƒ½:
æ¯æ¬¡ LLM èª¿ç”¨è¨˜éŒ„:
- Request (prompt)
- Response (completion)
- Tokens (input/output)
- Cost (è¨ˆç®—è²»ç”¨)
- Latency (éŸ¿æ‡‰æ™‚é–“)

æˆ‘å€‘çš„è¨­è¨ˆ:
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ LLM Call Trace                   â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ Execution ID: exec_12345         â”‚
â”‚ Agent: ticket_analyzer           â”‚
â”‚ Timestamp: 2025-11-17 10:30:00   â”‚
â”‚                                  â”‚
â”‚ Input:                           â”‚
â”‚ â€¢ Prompt Tokens: 250             â”‚
â”‚ â€¢ System Message: "ä½ æ˜¯..."      â”‚
â”‚ â€¢ User Message: "åˆ†æžå·¥å–®..."    â”‚
â”‚                                  â”‚
â”‚ Output:                          â”‚
â”‚ â€¢ Completion Tokens: 150         â”‚
â”‚ â€¢ Response: {...}                â”‚
â”‚                                  â”‚
â”‚ Metrics:                         â”‚
â”‚ â€¢ Total Tokens: 400              â”‚
â”‚ â€¢ Cost: $0.004                   â”‚
â”‚ â€¢ Latency: 1.2s                  â”‚
â”‚ â€¢ Model: gpt-4o                  â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

å¯¦ç¾æ–¹æ¡ˆ:
class LLMTracer:
    async def trace_call(self, agent_name: str, prompt: str, response: str, usage: dict):
        trace_record = {
            "execution_id": current_execution_id(),
            "agent_name": agent_name,
            "timestamp": datetime.now(),
            "prompt": prompt,
            "response": response,
            "prompt_tokens": usage["prompt_tokens"],
            "completion_tokens": usage["completion_tokens"],
            "total_tokens": usage["total_tokens"],
            "cost": self.calculate_cost(usage, model="gpt-4o"),
            "latency": usage.get("latency", 0),
            "model": "gpt-4o"
        }
        
        await llm_trace_db.save(trace_record)
        
        # å¯¦æ™‚æˆæœ¬ç›£æŽ§
        await cost_monitor.update(trace_record)
    
    def calculate_cost(self, usage: dict, model: str) -> float:
        # Azure OpenAI GPT-4o å®šåƒ¹ (2025)
        # Input: $0.005 / 1K tokens
        # Output: $0.015 / 1K tokens
        input_cost = (usage["prompt_tokens"] / 1000) * 0.005
        output_cost = (usage["completion_tokens"] / 1000) * 0.015
        return input_cost + output_cost

# æˆæœ¬åˆ†æž Dashboard
async def get_cost_analytics(time_range: str = "last_30_days"):
    traces = await llm_trace_db.query(time_range=time_range)
    
    analytics = {
        "total_cost": sum(t["cost"] for t in traces),
        "total_calls": len(traces),
        "total_tokens": sum(t["total_tokens"] for t in traces),
        "by_agent": group_by(traces, "agent_name"),
        "by_workflow": group_by(traces, "execution_id"),
        "daily_trend": calculate_daily_trend(traces)
    }
    
    return analytics
```

**MVP å¯¦ç¾**: âœ… å¿…é ˆæœ‰ LLM è¿½è¹¤å’Œæˆæœ¬ç›£æŽ§

---

### 3. å¾žä¼æ¥­è»Ÿä»¶å€Ÿé‘’

#### **ç”¢å“åˆ†æž A**: Dynamics 365 (CE/FO)

**æ ¸å¿ƒå„ªå‹¢**:
```
1. æ¨¡çµ„åŒ–è¨­è¨ˆ
   - CE (Customer Engagement): Sales, Service, Marketing
   - FO (Finance & Operations): ERP
   - å¯ç¨ç«‹éƒ¨ç½²æˆ–çµ„åˆ

2. çµ±ä¸€æ•¸æ“šæ¨¡åž‹
   - Common Data Model (CDM)
   - è·¨æ¨¡çµ„æ•¸æ“šå…±äº«
   - æ¨™æº–åŒ– API

3. æ¬Šé™å’Œå®‰å…¨
   - Role-based Access Control
   - Field-level Security
   - Audit Trail
```

**å¯å€Ÿé‘’åˆ° Agent Platform**:

#### å€Ÿé‘’é»ž 8: **æ¨¡çµ„åŒ– Agent æž¶æ§‹** âœ… MVP æ ¸å¿ƒè¨­è¨ˆ

```
Dynamics 365 çš„æ¨¡çµ„è¨­è¨ˆ:
Sales Module â”€â”
Service Module â”¼â”€â†’ Common Data Platform
Marketing Module â”€â”˜

æˆ‘å€‘çš„ Agent æ¨¡çµ„è¨­è¨ˆ:
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Agent Platform Core                  â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ â€¢ Agent Runtime Engine               â”‚
â”‚ â€¢ Checkpoint Manager                 â”‚
â”‚ â€¢ Tool Registry                      â”‚
â”‚ â€¢ Common Data Model                  â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
         â†“         â†“         â†“
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â” â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â” â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ CS Module  â”‚ â”‚ IT Mod  â”‚ â”‚ HR Module â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤ â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤ â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ â€¢ Ticket   â”‚ â”‚ â€¢ Alert â”‚ â”‚ â€¢ Leave   â”‚
â”‚   Analyzer â”‚ â”‚   Diag  â”‚ â”‚   Request â”‚
â”‚ â€¢ Solution â”‚ â”‚ â€¢ Auto  â”‚ â”‚ â€¢ Approvalâ”‚
â”‚   Gen      â”‚ â”‚   Fix   â”‚ â”‚   Flow    â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜ â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜ â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

å„ªå‹¢:
- ç¨ç«‹é–‹ç™¼å’Œéƒ¨ç½²
- å…±äº«æ ¸å¿ƒèƒ½åŠ›
- å¯é¸å®‰è£æ¨¡çµ„
```

**MVP å¯¦ç¾**: âœ… æ¨¡çµ„åŒ–æž¶æ§‹è¨­è¨ˆ

---

#### å€Ÿé‘’é»ž 9: **å¯©è¨ˆè¿½è¹¤ (Audit Trail)** âœ… ä¼æ¥­å¿…é ˆ

```
Dynamics 365 çš„å¯©è¨ˆåŠŸèƒ½:
- è¨˜éŒ„æ‰€æœ‰æ•¸æ“šè®Šæ›´
- Who, What, When, Why
- ä¸å¯ç¯¡æ”¹
- åˆè¦è¦æ±‚

æˆ‘å€‘çš„å¯©è¨ˆè¨­è¨ˆ:
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Audit Trail                              â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ è¨˜éŒ„æ‰€æœ‰é—œéµæ“ä½œ:                        â”‚
â”‚ â€¢ Agent åŸ·è¡Œé–‹å§‹/çµæŸ                    â”‚
â”‚ â€¢ Checkpoint å‰µå»º/æ¢å¾©                   â”‚
â”‚ â€¢ äººå·¥å¯©æ‰¹æ±ºç­–                           â”‚
â”‚ â€¢ ç³»çµ±æ•¸æ“šä¿®æ”¹                           â”‚
â”‚ â€¢ é…ç½®è®Šæ›´                               â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

æ•¸æ“šçµæ§‹:
{
  "audit_id": "aud_12345",
  "timestamp": "2025-11-17T10:30:00Z",
  "action": "workflow_approved",
  "actor": {
    "type": "human",
    "user_id": "user_001",
    "username": "john@company.com",
    "ip_address": "192.168.1.100"
  },
  "target": {
    "type": "checkpoint",
    "checkpoint_id": "ckpt_789",
    "workflow": "cs_ticket",
    "ticket_id": "TICKET-12345"
  },
  "details": {
    "decision": "approved",
    "original_solution": "...",
    "modified_solution": "...",
    "reason": "Solution looks good"
  },
  "metadata": {
    "execution_id": "exec_456",
    "session_id": "sess_789"
  }
}

å¯¦ç¾:
class AuditLogger:
    async def log(self, action: str, actor: dict, target: dict, details: dict):
        audit_record = {
            "audit_id": generate_id(),
            "timestamp": datetime.now(timezone.utc),
            "action": action,
            "actor": actor,
            "target": target,
            "details": details,
            "metadata": self.get_context()
        }
        
        # å¯«å…¥ append-only å¯©è¨ˆè¡¨ (ä¸å¯ä¿®æ”¹)
        await audit_db.append(audit_record)
        
        # å¯é¸: ç™¼é€åˆ° SIEM ç³»çµ±
        await siem.send(audit_record)

# ä½¿ç”¨å¯©è¨ˆ
await audit_logger.log(
    action="checkpoint_approved",
    actor=get_current_user(),
    target={"checkpoint_id": checkpoint_id},
    details={"decision": "approved", "modifications": modifications}
)
```

**MVP å¯¦ç¾**: âœ… å¿…é ˆæœ‰å¯©è¨ˆæ—¥èªŒ (åˆè¦å’Œè¿½æº¯)

---

#### **ç”¢å“åˆ†æž B**: ServiceNow

**æ ¸å¿ƒå„ªå‹¢**:
```
1. ITIL æœ€ä½³å¯¦è¸
   - Incident Management
   - Problem Management
   - Change Management

2. å·¥ä½œæµè‡ªå‹•åŒ–
   - Approval Workflows
   - SLA Management
   - Escalation Rules

3. CMDB (é…ç½®ç®¡ç†æ•¸æ“šåº«)
   - è³‡ç”¢é—œè¯
   - å½±éŸ¿åˆ†æž
```

**å¯å€Ÿé‘’åˆ° Agent Platform**:

#### å€Ÿé‘’é»ž 10: **SLA ç®¡ç†** ðŸ”„ Phase 2

```
ServiceNow çš„ SLA:
Incident Priority: P1 (Critical)
  â†’ Response Time: 15 åˆ†é˜
  â†’ Resolution Time: 4 å°æ™‚
  â†’ è¶…æ™‚è‡ªå‹•å‡ç´š

æˆ‘å€‘çš„ Agent SLA è¨­è¨ˆ:
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Agent Workflow SLA                 â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ CS Ticket (VIP å®¢æˆ¶)              â”‚
â”‚ â€¢ éŸ¿æ‡‰æ™‚é–“: 5 åˆ†é˜                â”‚
â”‚ â€¢ å¯©æ‰¹æ™‚é™: 30 åˆ†é˜               â”‚
â”‚ â€¢ è§£æ±ºæ™‚é™: 2 å°æ™‚                â”‚
â”‚                                    â”‚
â”‚ è¶…æ™‚è™•ç†:                         â”‚
â”‚ â€¢ 5 åˆ†é˜æœªé–‹å§‹ â†’ Alert ç®¡ç†å“¡     â”‚
â”‚ â€¢ 30 åˆ†é˜æœªå¯©æ‰¹ â†’ è‡ªå‹•å‡ç´šåˆ°ä¸»ç®¡  â”‚
â”‚ â€¢ 2 å°æ™‚æœªè§£æ±º â†’ è½‰äººå·¥è™•ç†       â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

å¯¦ç¾ (Phase 2):
class SLAManager:
    async def monitor_sla(self, execution_id: str):
        execution = await get_execution(execution_id)
        sla_config = await get_sla_config(execution["workflow_type"])
        
        # æª¢æŸ¥å„éšŽæ®µ SLA
        for stage, time_limit in sla_config["stages"].items():
            elapsed = datetime.now() - execution["stages"][stage]["started_at"]
            
            if elapsed > time_limit:
                # SLA é•å
                await handle_sla_breach(
                    execution_id=execution_id,
                    stage=stage,
                    elapsed=elapsed,
                    limit=time_limit
                )
```

**MVP å¯¦ç¾**: â¸ï¸ Phase 2 (MVP å…ˆåšåŸºç¤Žç›£æŽ§)

---

#### **ç”¢å“åˆ†æž C**: Microsoft 365 (Outlook, Teams, SharePoint)

**æ ¸å¿ƒå„ªå‹¢**:
```
1. æ·±åº¦æ•´åˆ
   - Teams <-> SharePoint <-> Outlook
   - çµ±ä¸€çš„ç”¨æˆ¶é«”é©—
   - Single Sign-On (SSO)

2. å”ä½œåŠŸèƒ½
   - Teams Channel é€šçŸ¥
   - @mention æé†’
   - æ–‡ä»¶å”ä½œ

3. Power Platform æ•´åˆ
   - Power Automate (å·¥ä½œæµ)
   - Power Apps (ä½Žä»£ç¢¼æ‡‰ç”¨)
   - Power BI (åˆ†æž)
```

**å¯å€Ÿé‘’åˆ° Agent Platform**:

#### å€Ÿé‘’é»ž 11: **Microsoft 365 åŽŸç”Ÿæ•´åˆ** âœ… é«˜å„ªå…ˆç´š

```
æ•´åˆé»ž 1: Teams é€šçŸ¥
Agent å®Œæˆä»»å‹™ â†’ ç™¼é€ Teams è¨Šæ¯

# å¯¦ç¾
import requests

async def send_teams_notification(channel_webhook: str, message: dict):
    teams_message = {
        "@type": "MessageCard",
        "@context": "http://schema.org/extensions",
        "summary": message["title"],
        "sections": [{
            "activityTitle": message["title"],
            "activitySubtitle": message["subtitle"],
            "facts": [
                {"name": "å·¥å–® ID", "value": message["ticket_id"]},
                {"name": "ç‹€æ…‹", "value": message["status"]},
                {"name": "è™•ç†æ™‚é–“", "value": message["duration"]}
            ],
            "markdown": True
        }],
        "potentialAction": [{
            "@type": "OpenUri",
            "name": "æŸ¥çœ‹è©³æƒ…",
            "targets": [{
                "os": "default",
                "uri": message["url"]
            }]
        }]
    }
    
    await requests.post(channel_webhook, json=teams_message)

# ä½¿ç”¨
await send_teams_notification(
    channel_webhook=config.teams_webhook,
    message={
        "title": "âœ… CS å·¥å–®å·²è§£æ±º",
        "subtitle": "Agent è‡ªå‹•è™•ç†å®Œæˆ",
        "ticket_id": "TICKET-12345",
        "status": "å·²è§£æ±º",
        "duration": "15 åˆ†é˜",
        "url": "https://platform.company.com/executions/exec_789"
    }
)

---

æ•´åˆé»ž 2: Outlook éƒµä»¶è§¸ç™¼
æ”¶åˆ°ç‰¹å®šéƒµä»¶ â†’ è§¸ç™¼ Agent

# ä½¿ç”¨ n8n æˆ– Power Automate
Outlook (æ”¶åˆ°éƒµä»¶: ä¸»æ—¨åŒ…å« "ç·Šæ€¥å·¥å–®")
  â†’ è§£æžéƒµä»¶å…§å®¹
  â†’ èª¿ç”¨ Agent API
  â†’ å‰µå»ºå·¥å–®ä¸¦è‡ªå‹•è™•ç†

---

æ•´åˆé»ž 3: SharePoint æ–‡ä»¶å”ä½œ
Agent ç”Ÿæˆå ±å‘Š â†’ ä¸Šå‚³åˆ° SharePoint

# Microsoft Graph API
from microsoft.graph import GraphServiceClient

async def upload_to_sharepoint(file_path: str, sharepoint_folder: str):
    graph_client = GraphServiceClient(credentials)
    
    with open(file_path, 'rb') as file:
        await graph_client.sites[site_id].drives[drive_id].items[folder_id].children.post(
            file_name=os.path.basename(file_path),
            content=file
        )

# Agent å®Œæˆå¾Œä¸Šå‚³å ±å‘Š
report = await agent.generate_report()
await upload_to_sharepoint(report, "/Shared Documents/Agent Reports/")
```

**MVP å¯¦ç¾**: âœ… Teams é€šçŸ¥å¿…é ˆï¼ŒSharePoint å¯é¸

---

### 4. å¾ž RPA ç”¢å“å€Ÿé‘’ - UiPath

#### **ç”¢å“åˆ†æž**: UiPath (RPA å¹³å°)

**æ ¸å¿ƒå„ªå‹¢**:
```
1. Orchestrator (ç·¨æŽ’å™¨)
   - é›†ä¸­ç®¡ç†æ‰€æœ‰ Robot
   - ä»»å‹™èª¿åº¦
   - è² è¼‰å¹³è¡¡
   - ç›£æŽ§å’Œå ±å‘Š

2. åŸ·è¡Œæ¨¡å¼
   - Attended (æœ‰äººå€¼å®ˆ)
   - Unattended (ç„¡äººå€¼å®ˆ)
   - Semi-attended (åŠè‡ªå‹•)

3. ç•°å¸¸è™•ç†
   - Try-Catch æ¡†æž¶
   - æ¥­å‹™ç•°å¸¸ vs ç³»çµ±ç•°å¸¸
   - è‡ªå‹•é‡è©¦ç­–ç•¥
   - è£œå„Ÿäº‹å‹™
```

**å¯å€Ÿé‘’åˆ° Agent Platform**:

#### å€Ÿé‘’é»ž 12: **Agent åŸ·è¡Œæ¨¡å¼** âœ… MVP è¨­è¨ˆè€ƒæ…®

```
UiPath çš„åŸ·è¡Œæ¨¡å¼:
1. Attended: éœ€è¦äººå·¥è§¸ç™¼ï¼Œäººå·¥ç›£ç£
2. Unattended: å®Œå…¨è‡ªå‹•ï¼Œå®šæ™‚æˆ–äº‹ä»¶è§¸ç™¼
3. Semi-attended: é–‹å§‹è‡ªå‹•ï¼Œé—œéµé»žéœ€è¦äººå·¥

æˆ‘å€‘çš„ Agent åŸ·è¡Œæ¨¡å¼:
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Agent Execution Modes                   â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ 1. Fully Automated (Unattended)        â”‚
â”‚    â€¢ å®šæ™‚è§¸ç™¼ (n8n Cron)               â”‚
â”‚    â€¢ äº‹ä»¶è§¸ç™¼ (Webhook)                â”‚
â”‚    â€¢ å®Œå…¨è‡ªå‹•åŸ·è¡Œ                      â”‚
â”‚    â€¢ é©ç”¨: IT å¥åº·æª¢æŸ¥ã€æ—¥å¸¸å·¡æª¢       â”‚
â”‚                                         â”‚
â”‚ 2. Human-in-the-loop (Semi-attended)   â”‚
â”‚    â€¢ Agent è‡ªå‹•åˆ†æž                    â”‚
â”‚    â€¢ é—œéµæ±ºç­–éœ€è¦äººå·¥å¯©æ‰¹              â”‚
â”‚    â€¢ å¯©æ‰¹å¾Œç¹¼çºŒè‡ªå‹•åŸ·è¡Œ                â”‚
â”‚    â€¢ é©ç”¨: CS å·¥å–®ã€IT è®Šæ›´            â”‚
â”‚                                         â”‚
â”‚ 3. Interactive (Attended)              â”‚
â”‚    â€¢ äººå·¥è§¸ç™¼å•Ÿå‹•                      â”‚
â”‚    â€¢ Agent è¼”åŠ©äººå·¥æ±ºç­–                â”‚
â”‚    â€¢ å¯¦æ™‚äº¤äº’å’Œèª¿æ•´                    â”‚
â”‚    â€¢ é©ç”¨: è¤‡é›œå•é¡Œã€æŽ¢ç´¢æ€§åˆ†æž        â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

é…ç½®ç¤ºä¾‹:
# workflows/cs_ticket_config.yaml
execution_mode: "semi-attended"

checkpoints:
  - name: "solution_approval"
    condition: "always"  # ç¸½æ˜¯éœ€è¦å¯©æ‰¹
    timeout: 30m  # 30 åˆ†é˜å…§å¿…é ˆå¯©æ‰¹
    escalation: "supervisor"  # è¶…æ™‚å‡ç´š
  
  - name: "high_risk_action"
    condition: "if impact.level == 'high'"  # é«˜é¢¨éšªæ‰éœ€è¦å¯©æ‰¹
    timeout: 15m
    escalation: "manager"

auto_approve:
  enabled: true
  conditions:
    - "ticket.priority == 'low'"
    - "customer.vip_level < 3"
    - "solution.confidence > 0.9"
```

**MVP å¯¦ç¾**: âœ… æ”¯æŒ 3 ç¨®åŸ·è¡Œæ¨¡å¼é…ç½®

---

#### å€Ÿé‘’é»ž 13: **Orchestrator æ¦‚å¿µ - Agent èª¿åº¦å™¨** ðŸ”„ Phase 2

```
UiPath Orchestrator åŠŸèƒ½:
- Robot æ± ç®¡ç†
- ä»»å‹™éšŠåˆ—
- è² è¼‰å¹³è¡¡
- å„ªå…ˆç´šèª¿åº¦

æˆ‘å€‘çš„ Agent Orchestrator (Phase 2):
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Agent Orchestrator                 â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ Agent Pool                         â”‚
â”‚ â€¢ ticket_analyzer x 3 (å‰¯æœ¬)      â”‚
â”‚ â€¢ solution_generator x 2           â”‚
â”‚ â€¢ action_executor x 5              â”‚
â”‚                                    â”‚
â”‚ Task Queue (å„ªå…ˆç´šéšŠåˆ—)            â”‚
â”‚ â€¢ P1 (Critical): 2 å€‹ä»»å‹™          â”‚
â”‚ â€¢ P2 (High): 5 å€‹ä»»å‹™              â”‚
â”‚ â€¢ P3 (Normal): 10 å€‹ä»»å‹™           â”‚
â”‚                                    â”‚
â”‚ Load Balancing                     â”‚
â”‚ â€¢ ç›£æŽ§ Agent è² è¼‰                  â”‚
â”‚ â€¢ æ™ºèƒ½åˆ†é…ä»»å‹™                     â”‚
â”‚ â€¢ è‡ªå‹•æ“´å±• Agent                   â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

å¯¦ç¾ (Phase 2):
class AgentOrchestrator:
    async def schedule_task(self, workflow_name: str, priority: int):
        # 1. åŠ å…¥å„ªå…ˆç´šéšŠåˆ—
        await task_queue.enqueue(
            workflow_name=workflow_name,
            priority=priority,
            timestamp=datetime.now()
        )
        
        # 2. æª¢æŸ¥å¯ç”¨ Agent
        available_agents = await agent_pool.get_available(workflow_name)
        
        if available_agents:
            # æœ‰ç©ºé–’ Agentï¼Œç«‹å³åŸ·è¡Œ
            agent = available_agents[0]
            await agent.execute(workflow_name)
        else:
            # æ²’æœ‰ç©ºé–’ Agentï¼Œç­‰å¾…èª¿åº¦
            await self.wait_for_agent(workflow_name, priority)
    
    async def auto_scale(self):
        # ç›£æŽ§éšŠåˆ—é•·åº¦ï¼Œè‡ªå‹•æ“´å±• Agent
        queue_length = await task_queue.length()
        
        if queue_length > threshold:
            # å‰µå»ºæ–°çš„ Agent å‰¯æœ¬
            await agent_pool.scale_up(workflow_name, replicas=2)
```

**MVP å¯¦ç¾**: â¸ï¸ Phase 2 (MVP å…ˆå–® Agent åŸ·è¡Œ)

---

### 5. å¾žé–‹ç™¼è€…å·¥å…·å€Ÿé‘’

#### **ç”¢å“åˆ†æž A**: Postman (API æ¸¬è©¦)

**æ ¸å¿ƒå„ªå‹¢**:
```
1. Collections (æ¸¬è©¦é›†åˆ)
   - çµ„ç¹” API è«‹æ±‚
   - å¯åˆ†äº«å’Œå°Žå…¥
   - åœ˜éšŠå”ä½œ

2. Environment è®Šé‡
   - Dev / Staging / Prod
   - ä¸€éµåˆ‡æ›ç’°å¢ƒ
   - è®Šé‡ç®¡ç†

3. Testing
   - Pre-request Scripts
   - Tests (æ–·è¨€)
   - è‡ªå‹•åŒ–æ¸¬è©¦
```

**å¯å€Ÿé‘’åˆ° Agent Platform**:

#### å€Ÿé‘’é»ž 14: **ç’°å¢ƒç®¡ç†** âœ… MVP å¿…é ˆ

```
Postman çš„ Environment è¨­è¨ˆ:
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Environments               â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ â€¢ Development              â”‚
â”‚   - API_URL: localhost     â”‚
â”‚   - DB: dev_db             â”‚
â”‚                            â”‚
â”‚ â€¢ Staging                  â”‚
â”‚   - API_URL: staging.com   â”‚
â”‚   - DB: staging_db         â”‚
â”‚                            â”‚
â”‚ â€¢ Production               â”‚
â”‚   - API_URL: prod.com      â”‚
â”‚   - DB: prod_db            â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

æˆ‘å€‘çš„ç’°å¢ƒç®¡ç†:
# config/environments/dev.yaml
environment: development

openai:
  api_key: ${AZURE_OPENAI_API_KEY_DEV}
  endpoint: https://dev-openai.azure.com
  deployment: gpt-4o-dev

connectors:
  servicenow:
    url: https://dev.servicenow.com
    username: ${SERVICENOW_USER_DEV}
    password: ${SERVICENOW_PASS_DEV}
  
  dynamics365:
    url: https://dev.dynamics.com
    client_id: ${D365_CLIENT_ID_DEV}
    client_secret: ${D365_SECRET_DEV}

agent_config:
  retry_limit: 5  # Dev ç’°å¢ƒå¤šé‡è©¦å¹¾æ¬¡
  timeout: 300    # Dev ç’°å¢ƒè¶…æ™‚æ™‚é–“é•·ä¸€é»ž
  dry_run: true   # Dev é»˜èª dry-run

# config/environments/prod.yaml
environment: production

openai:
  api_key: ${AZURE_OPENAI_API_KEY_PROD}
  endpoint: https://prod-openai.azure.com
  deployment: gpt-4o-prod

connectors:
  servicenow:
    url: https://company.servicenow.com
    username: ${SERVICENOW_USER_PROD}
    password: ${SERVICENOW_PASS_PROD}
  
  dynamics365:
    url: https://company.dynamics.com
    client_id: ${D365_CLIENT_ID_PROD}
    client_secret: ${D365_SECRET_PROD}

agent_config:
  retry_limit: 3
  timeout: 120
  dry_run: false  # ç”Ÿç”¢ç’°å¢ƒå¯¦éš›åŸ·è¡Œ

# è¼‰å…¥ç’°å¢ƒé…ç½®
import os
import yaml

def load_config(env: str = None):
    if env is None:
        env = os.getenv("ENVIRONMENT", "development")
    
    config_file = f"config/environments/{env}.yaml"
    with open(config_file) as f:
        config = yaml.safe_load(f)
    
    # æ›¿æ›ç’°å¢ƒè®Šé‡
    config = substitute_env_vars(config)
    
    return config

# ä¸€éµåˆ‡æ›ç’°å¢ƒ
# export ENVIRONMENT=production
# python main.py
```

**MVP å¯¦ç¾**: âœ… å¿…é ˆæ”¯æŒå¤šç’°å¢ƒé…ç½®

---

#### **ç”¢å“åˆ†æž B**: Kubernetes (å®¹å™¨ç·¨æŽ’)

**æ ¸å¿ƒå„ªå‹¢**:
```
1. è²æ˜Žå¼é…ç½®
   - YAML å®šç¾©æœŸæœ›ç‹€æ…‹
   - K8s è‡ªå‹•ç¶­æŒç‹€æ…‹
   - GitOps å·¥ä½œæµ

2. å¥åº·æª¢æŸ¥
   - Liveness Probe
   - Readiness Probe
   - è‡ªå‹•é‡å•Ÿä¸å¥åº·å®¹å™¨

3. æ»¾å‹•æ›´æ–°
   - Zero-downtime éƒ¨ç½²
   - è‡ªå‹•å›žæ»¾
```

**å¯å€Ÿé‘’åˆ° Agent Platform**:

#### å€Ÿé‘’é»ž 15: **è²æ˜Žå¼ Agent é…ç½®** âœ… MVP è¨­è¨ˆ

```
Kubernetes é¢¨æ ¼çš„é…ç½®:
# k8s-deployment.yaml
apiVersion: apps/v1
kind: Deployment
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: web
        image: nginx:1.14

æˆ‘å€‘çš„ Agent é…ç½®:
# agents/cs_ticket_analyzer.yaml
apiVersion: agent.platform/v1
kind: Agent
metadata:
  name: cs-ticket-analyzer
  version: "1.2"
  labels:
    category: customer-service
    environment: production

spec:
  model:
    provider: azure_openai
    deployment: gpt-4o
    temperature: 0.7
    max_tokens: 1500
  
  system_message: |
    ä½ æ˜¯å°ˆæ¥­çš„å®¢æœå·¥å–®åˆ†æžå°ˆå®¶...
  
  tools:
    - query_servicenow
    - query_dynamics365
  
  retry:
    max_attempts: 3
    backoff: exponential
  
  timeout: 120s
  
  healthcheck:
    enabled: true
    interval: 60s
    test_prompt: "é€™æ˜¯æ¸¬è©¦"

# éƒ¨ç½² Agent
from agent_platform import AgentDeployer

deployer = AgentDeployer()
agent = deployer.deploy_from_yaml("agents/cs_ticket_analyzer.yaml")
```

**MVP å¯¦ç¾**: âœ… YAML è²æ˜Žå¼é…ç½®

---

#### **ç”¢å“åˆ†æž C**: Grafana (å¯è¦–åŒ–ç›£æŽ§)

**æ ¸å¿ƒå„ªå‹¢**:
```
1. å¼·å¤§çš„ Dashboard
   - æ™‚åºåœ–è¡¨
   - å„€è¡¨æ¿
   - ç†±åŠ›åœ–

2. Alert è¦å‰‡
   - é–¾å€¼å‘Šè­¦
   - ç•°å¸¸æª¢æ¸¬
   - å¤šæ¸ é“é€šçŸ¥

3. æ•¸æ“šæºæ•´åˆ
   - Prometheus
   - InfluxDB
   - Elasticsearch
```

**å¯å€Ÿé‘’åˆ° Agent Platform**:

#### å€Ÿé‘’é»ž 16: **ç›£æŽ§ Dashboard è¨­è¨ˆ** âœ… MVP å¿…é ˆ

```
Grafana é¢¨æ ¼çš„ Dashboard:
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Agent Platform - Overview Dashboard     â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ ðŸ“Š Real-time Metrics (æœ€è¿‘ 1 å°æ™‚)      â”‚
â”‚ â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¬â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¬â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”   â”‚
â”‚ â”‚ ç¸½åŸ·è¡Œæ•¸ â”‚ æˆåŠŸçŽ‡   â”‚ å¹³å‡æ™‚é•·   â”‚   â”‚
â”‚ â”‚   145    â”‚  94.5%   â”‚   45s      â”‚   â”‚
â”‚ â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”´â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”´â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜   â”‚
â”‚                                         â”‚
â”‚ ðŸ“ˆ Execution Trend (æŠ˜ç·šåœ–)            â”‚
â”‚ [åŸ·è¡Œæ¬¡æ•¸éš¨æ™‚é–“è®ŠåŒ–]                    â”‚
â”‚                                         â”‚
â”‚ ðŸŽ¯ By Workflow (é¤…åœ–)                  â”‚
â”‚ â€¢ CS Ticket: 65%                       â”‚
â”‚ â€¢ IT Ops: 30%                          â”‚
â”‚ â€¢ Other: 5%                            â”‚
â”‚                                         â”‚
â”‚ âš ï¸ Recent Failures (è¡¨æ ¼)              â”‚
â”‚ | Time   | Workflow | Error | Action |â”‚
â”‚ |--------|----------|-------|--------|â”‚
â”‚ | 10:30  | CS-001   | API   | Retry  |â”‚
â”‚ | 10:15  | IT-005   | Timeout| Alert|â”‚
â”‚                                         â”‚
â”‚ ðŸ’° Cost Analysis                       â”‚
â”‚ â€¢ Today: $45.30                        â”‚
â”‚ â€¢ This Month: $1,230.50                â”‚
â”‚ â€¢ Trend: â†“ 15% vs last month           â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

å¯¦ç¾ (React Dashboard):
// Dashboard.tsx
import { Line, Pie, Table } from 'recharts';

function AgentDashboard() {
  const [metrics, setMetrics] = useState(null);
  
  useEffect(() => {
    // å¯¦æ™‚æ›´æ–°æ•¸æ“š
    const interval = setInterval(async () => {
      const data = await fetch('/api/metrics/realtime');
      setMetrics(await data.json());
    }, 5000);  // æ¯ 5 ç§’æ›´æ–°
    
    return () => clearInterval(interval);
  }, []);
  
  return (
    <div className="dashboard">
      <MetricsCards metrics={metrics} />
      <ExecutionTrendChart data={metrics.trend} />
      <WorkflowDistribution data={metrics.by_workflow} />
      <RecentFailures failures={metrics.recent_failures} />
      <CostAnalysis costs={metrics.costs} />
    </div>
  );
}
```

**MVP å¯¦ç¾**: âœ… åŸºç¤Ž Dashboard (å¯¦æ™‚æŒ‡æ¨™ + åœ–è¡¨)

---

#### **ç”¢å“åˆ†æž D**: Git (ç‰ˆæœ¬æŽ§åˆ¶)

**æ ¸å¿ƒå„ªå‹¢**:
```
1. ç‰ˆæœ¬æ­·å²
   - æ¯æ¬¡è®Šæ›´è¨˜éŒ„
   - Commit message
   - Diff æŸ¥çœ‹

2. åˆ†æ”¯ç®¡ç†
   - Feature branch
   - åˆä½µå’Œè¡çªè§£æ±º

3. å›žæ»¾
   - git revert
   - git reset
   - å¿«é€Ÿæ¢å¾©
```

**å¯å€Ÿé‘’åˆ° Agent Platform**:

#### å€Ÿé‘’é»ž 17: **Agent é…ç½®ç‰ˆæœ¬æŽ§åˆ¶** ðŸ”„ Phase 2

```
Git é¢¨æ ¼çš„ç‰ˆæœ¬ç®¡ç†:
commit abc123
Author: John Doe
Date: 2025-11-17

  Updated CS ticket analyzer prompt
  - Improved issue classification accuracy
  - Added VIP customer detection

æˆ‘å€‘çš„ Agent ç‰ˆæœ¬æŽ§åˆ¶ (Phase 2):
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Agent Configuration History            â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ v1.3 (Current) - 2025-11-17           â”‚
â”‚ â€¢ æ”¹é€²å•é¡Œåˆ†é¡žæº–ç¢ºåº¦                  â”‚
â”‚ â€¢ æ·»åŠ  VIP å®¢æˆ¶æª¢æ¸¬                   â”‚
â”‚ â€¢ ä¿®å¾©: ServiceNow API è¶…æ™‚           â”‚
â”‚                                        â”‚
â”‚ v1.2 - 2025-11-10                     â”‚
â”‚ â€¢ æ·»åŠ  Dynamics 365 æ•´åˆ              â”‚
â”‚ â€¢ å„ªåŒ– prompt æ¸›å°‘ token ä½¿ç”¨         â”‚
â”‚                                        â”‚
â”‚ v1.1 - 2025-11-03                     â”‚
â”‚ â€¢ åˆå§‹ç™¼å¸ƒ                            â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

å¯¦ç¾:
class AgentVersionControl:
    async def commit(self, agent_name: str, config: dict, message: str):
        version = {
            "agent_name": agent_name,
            "version": self.get_next_version(agent_name),
            "config": config,
            "commit_message": message,
            "author": get_current_user(),
            "timestamp": datetime.now()
        }
        
        await version_db.save(version)
        
        # æ›´æ–°ç•¶å‰ç‰ˆæœ¬
        await agent_registry.update_active_version(agent_name, version["version"])
    
    async def rollback(self, agent_name: str, target_version: str):
        # è¼‰å…¥ç›®æ¨™ç‰ˆæœ¬é…ç½®
        version = await version_db.get(agent_name, target_version)
        
        # æ¢å¾©é…ç½®
        await agent_registry.update(agent_name, version["config"])
        
        # å¯©è¨ˆæ—¥èªŒ
        await audit_logger.log(
            action="agent_rollback",
            details={"from": current_version, "to": target_version}
        )
```

**MVP å¯¦ç¾**: â¸ï¸ Phase 2 (MVP å…ˆç”¨æ–‡ä»¶å‚™ä»½)

---

### 6. æœ€çµ‚å€Ÿé‘’ç­–ç•¥ç¸½çµ

#### âœ… MVP å¿…é ˆå¯¦ç¾çš„å€Ÿé‘’

| å€Ÿé‘’ä¾†æº | å€Ÿé‘’åŠŸèƒ½ | å„ªå…ˆç´š | é–‹ç™¼æ™‚é–“ |
|---------|---------|-------|---------|
| **n8n** | è§¸ç™¼æ©Ÿåˆ¶ (Cron/Webhook) | ðŸŸ¢ é«˜ | 1 é€± |
| **n8n** | éŒ¯èª¤è™•ç†å’Œé‡è©¦ | ðŸŸ¢ é«˜ | 1 é€± |
| **n8n** | Dry-run æ¸¬è©¦æ¨¡å¼ | ðŸŸ¢ é«˜ | 3 å¤© |
| **Dify** | Prompt Template ç®¡ç† | ðŸŸ¢ é«˜ | 1 é€± |
| **Dify** | LLM èª¿ç”¨è¿½è¹¤å’Œæˆæœ¬ | ðŸŸ¢ é«˜ | 1 é€± |
| **Dynamics 365** | æ¨¡çµ„åŒ–æž¶æ§‹ | ðŸŸ¢ é«˜ | è¨­è¨ˆéšŽæ®µ |
| **Dynamics 365** | å¯©è¨ˆè¿½è¹¤ | ðŸŸ¢ é«˜ | 1 é€± |
| **M365** | Teams é€šçŸ¥æ•´åˆ | ðŸŸ¢ é«˜ | 3 å¤© |
| **UiPath** | åŸ·è¡Œæ¨¡å¼è¨­è¨ˆ | ðŸŸ¢ é«˜ | 1 é€± |
| **Postman** | ç’°å¢ƒé…ç½®ç®¡ç† | ðŸŸ¢ é«˜ | 3 å¤© |
| **Kubernetes** | è²æ˜Žå¼ YAML é…ç½® | ðŸŸ¢ é«˜ | 1 é€± |
| **Grafana** | ç›£æŽ§ Dashboard | ðŸŸ¢ é«˜ | 2 é€± |

**ç¸½è¨ˆ**: ç´„ 9-10 é€±

#### ðŸ”„ Phase 2 å¯¦ç¾çš„å€Ÿé‘’

| å€Ÿé‘’ä¾†æº | å€Ÿé‘’åŠŸèƒ½ | å¯¦æ–½æ™‚æ©Ÿ |
|---------|---------|---------|
| **ChatGPT** | å°è©±å¼å¯©æ‰¹ä»‹é¢ | 6 å€‹æœˆå¾Œ |
| **ServiceNow** | SLA ç®¡ç† | 6 å€‹æœˆå¾Œ |
| **UiPath** | Agent Orchestrator | 12 å€‹æœˆå¾Œ |
| **Git** | é…ç½®ç‰ˆæœ¬æŽ§åˆ¶ | 12 å€‹æœˆå¾Œ |

---

### 7. æ ¸å¿ƒæ´žå¯Ÿ (Adapt ç¶­åº¦)

#### ðŸ’¡ æ´žå¯Ÿ 1: ä¸è¦é‡é€ è¼ªå­

```
æˆç†Ÿç”¢å“å·²ç¶“è§£æ±ºçš„å•é¡Œ:
â€¢ n8n: å·¥ä½œæµè§¸ç™¼å’ŒéŒ¯èª¤è™•ç†
â€¢ Dify: Prompt ç®¡ç†å’Œ LLM è¿½è¹¤
â€¢ Dynamics: ä¼æ¥­ç´šæž¶æ§‹å’Œå¯©è¨ˆ
â€¢ UiPath: åŸ·è¡Œæ¨¡å¼å’Œç•°å¸¸è™•ç†

æˆ‘å€‘çš„ç­–ç•¥:
â†’ å€Ÿé‘’è¨­è¨ˆç†å¿µå’Œæœ€ä½³å¯¦è¸
â†’ ä¸è¦å¾žé›¶é–‹å§‹è¨­è¨ˆ
â†’ ç«™åœ¨å·¨äººè‚©è†€ä¸Š
```

---

#### ðŸ’¡ æ´žå¯Ÿ 2: ä¼æ¥­ç´šåŠŸèƒ½ä¸å¯å°‘

```
å‰µæ¥­ç”¢å“å¸¸çŠ¯çš„éŒ¯èª¤: å¿½è¦–ä¼æ¥­éœ€æ±‚
â€¢ æ²’æœ‰å¯©è¨ˆæ—¥èªŒ â†’ ç„¡æ³•åˆè¦
â€¢ æ²’æœ‰æ¬Šé™ç®¡ç† â†’ å®‰å…¨é¢¨éšª
â€¢ æ²’æœ‰ç’°å¢ƒéš”é›¢ â†’ æ¸¬è©¦å½±éŸ¿ç”Ÿç”¢

å¾ž D365 å’Œ ServiceNow å­¸åˆ°:
â†’ å¯©è¨ˆè¿½è¹¤æ˜¯å¿…éœ€å“ä¸æ˜¯å¥¢ä¾ˆå“
â†’ æ¬Šé™ç®¡ç†å¾ž MVP å°±è¦è€ƒæ…®
â†’ ç’°å¢ƒç®¡ç†é¿å…ç½é›£æ€§éŒ¯èª¤
```

---

#### ðŸ’¡ æ´žå¯Ÿ 3: é–‹ç™¼è€…é«”é©—æ±ºå®šæˆæ•—

```
Postman ç‚ºä»€éº¼æµè¡Œ? â†’ é–‹ç™¼è€…é«”é©—å¥½
Kubernetes ç‚ºä»€éº¼æˆåŠŸ? â†’ è²æ˜Žå¼é…ç½®ç›´è§€

æˆ‘å€‘çš„é‡é»ž:
â†’ YAML é…ç½® (ä¸æ˜¯ JSON æˆ– Python dict)
â†’ æ¸…æ™°çš„éŒ¯èª¤è¨Šæ¯
â†’ Dry-run æ¨¡å¼ (å®‰å…¨æ¸¬è©¦)
â†’ å®Œå–„çš„æ–‡æª”å’Œç¤ºä¾‹
```

---

#### ðŸ’¡ æ´žå¯Ÿ 4: å¯è§€æ¸¬æ€§æ˜¯åŸºç¤Žè¨­æ–½

```
Grafana å’Œ Datadog è­‰æ˜Ž:
â†’ çœ‹ä¸è¦‹å°±ç„¡æ³•å„ªåŒ–
â†’ æ²’æœ‰æ•¸æ“šå°±ç„¡æ³•æ±ºç­–

æˆ‘å€‘çš„æŠ•è³‡:
â†’ MVP å°±è¦æœ‰å®Œæ•´ç›£æŽ§
â†’ LLM èª¿ç”¨è¿½è¹¤ (æˆæœ¬å„ªåŒ–)
â†’ Dashboard (å¯¦æ™‚å¯è¦‹)
â†’ Alert (åŠæ™‚éŸ¿æ‡‰)
```

---

### 8. è¡Œå‹•å»ºè­° (åŸºæ–¼ Adapt åˆ†æž)

#### ç«‹å³è¡Œå‹• (MVP Week 1-2)

1. âœ… **è¨­ç½®é–‹ç™¼ç’°å¢ƒ**
   - Dev / Staging / Prod é…ç½®æ–‡ä»¶
   - ç’°å¢ƒè®Šé‡ç®¡ç† (.env)
   - ä¸€éµåˆ‡æ›è…³æœ¬

2. âœ… **å»ºç«‹ Prompt Template åº«**
   - YAML æ ¼å¼ Prompt å®šç¾©
   - ç‰ˆæœ¬æ¨™è¨˜
   - è¼‰å…¥å™¨é–‹ç™¼

3. âœ… **å¯¦ç¾éŒ¯èª¤è™•ç†æ¡†æž¶**
   - Try-Catch åŒ…è£
   - è‡ªå‹•é‡è©¦é‚è¼¯
   - Error Handler

#### çŸ­æœŸè¡Œå‹• (MVP Week 3-6)

1. âœ… **æ•´åˆ n8n**
   - Cron è§¸ç™¼ Agent
   - Webhook æŽ¥æ”¶å™¨
   - éŒ¯èª¤é€šçŸ¥

2. âœ… **é–‹ç™¼ç›£æŽ§ Dashboard**
   - å¯¦æ™‚æŒ‡æ¨™
   - åŸ·è¡Œæ­·å²
   - æˆæœ¬åˆ†æž

3. âœ… **M365 æ•´åˆ**
   - Teams é€šçŸ¥
   - Outlook è§¸ç™¼ (å¯é¸)

#### ä¸­æœŸå®Œå–„ (MVP Week 7-10)

1. âœ… **å¯©è¨ˆç³»çµ±**
   - æ‰€æœ‰æ“ä½œè¨˜éŒ„
   - ä¸å¯ç¯¡æ”¹è¨­è¨ˆ
   - æŸ¥è©¢ä»‹é¢

2. âœ… **LLM è¿½è¹¤**
   - Token ä½¿ç”¨çµ±è¨ˆ
   - æˆæœ¬åˆ†æž
   - å„ªåŒ–å»ºè­°

3. âœ… **æ¸¬è©¦å·¥å…·**
   - Dry-run æ¨¡å¼
   - Agent å–®å…ƒæ¸¬è©¦
   - æ•´åˆæ¸¬è©¦

---

## ðŸŽ¯ Adapt ç¶­åº¦å®Œæˆ

**æ ¸å¿ƒç™¼ç¾**: å¾žæˆç†Ÿç”¢å“å€Ÿé‘’å¯ä»¥åŠ é€Ÿ 50% é–‹ç™¼æ™‚é–“

**é—œéµå€Ÿé‘’**:
1. ðŸŸ¢ n8n: è§¸ç™¼å’ŒéŒ¯èª¤è™•ç† (å·¥ä½œæµåŸºç¤Ž)
2. ðŸŸ¢ Dify: Prompt å’Œ LLM ç®¡ç† (AI æœ€ä½³å¯¦è¸)
3. ðŸŸ¢ Dynamics 365: ä¼æ¥­æž¶æ§‹ (å¯æ“´å±•æ€§)
