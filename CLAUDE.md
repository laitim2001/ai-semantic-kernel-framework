# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

---

## Project Overview

**IPA Platform** (Intelligent Process Automation) is an enterprise-grade AI Agent orchestration platform built on **Microsoft Agent Framework**. The platform enables intelligent workflow automation with multi-agent collaboration, human-in-the-loop checkpoints, and cross-system integration.

**Core Framework**: Microsoft Agent Framework (unifies Semantic Kernel + AutoGen capabilities)
**Target Users**: Mid-size enterprises (500-2000 employees) for IT operations and customer service
**Current Phase**: Sprint 0 - Infrastructure Foundation (MVP)

---

## Development Commands

### Local Development Environment

```bash
# Start all services (PostgreSQL, Redis, RabbitMQ, Backend API)
docker-compose up -d

# Check service health
docker-compose ps
curl http://localhost:8000/health

# Stop services
docker-compose down
# Stop and remove volumes
docker-compose down -v
```

### Backend (Python FastAPI)

```bash
cd backend/

# Code Quality
black .                              # Format code
isort .                              # Sort imports
flake8 .                             # Lint
mypy .                               # Type check

# Testing
pytest                               # Run all tests
pytest tests/unit/                   # Unit tests only
pytest tests/integration/            # Integration tests only
pytest -v --cov=src                  # With coverage report
pytest -m "not slow"                 # Skip slow tests
pytest tests/test_specific.py::test_function  # Single test

# Run backend locally
uvicorn main:app --reload --host 0.0.0.0 --port 8000
```

### Database

```bash
# Connect to PostgreSQL
docker-compose exec postgres psql -U ipa_user -d ipa_platform

# Run migrations (when available)
alembic upgrade head
alembic downgrade -1
alembic revision --autogenerate -m "description"
```

### Redis

```bash
# Connect to Redis CLI
docker-compose exec redis redis-cli -a redis_password

# Common commands
KEYS *
GET key
SET key value
DEL key
```

---

## Architecture

### System Architecture (MVP Phase)

```
Frontend (React 18)
    ↓ HTTPS
Azure App Service (Python FastAPI)
    ├─ Workflow Module (CRUD, Validation)
    ├─ Execution Module (Scheduler, State Management)
    └─ Agent Module (Semantic Kernel, Tools)
    ↓
Azure Service Bus / RabbitMQ (local dev)
    ↓
PostgreSQL 16 (State, Data)
Redis 7 (Cache, Session)
Azure Blob Storage (Files)
```

### Core Modules

1. **Workflow Service** (`backend/src/workflow/`)
   - Workflow CRUD operations
   - YAML/JSON validation
   - Version management

2. **Execution Service** (`backend/src/execution/`)
   - Task scheduling (APScheduler)
   - State machine management
   - Checkpoint handling
   - Retry logic with exponential backoff

3. **Agent Service** (`backend/src/agent/`)
   - Microsoft Agent Framework integration
   - Semantic Kernel orchestration
   - Multi-agent coordination
   - Tool/Plugin management

### Data Models

- **Workflows**: Graph-based definitions with nodes and edges
- **Executions**: State tracking with checkpoints for human-in-the-loop
- **Agents**: Configurable with tools, prompts, and parameters
- **Audit Logs**: Complete execution trail for compliance

---

## Project Structure

```
.
├── backend/                    # Python FastAPI application
│   ├── src/
│   │   ├── agent/             # Agent Framework integration
│   │   ├── execution/         # Execution engine
│   │   └── workflow/          # Workflow management
│   ├── tests/
│   │   ├── unit/              # Unit tests
│   │   └── integration/       # Integration tests
│   ├── main.py                # FastAPI entry point
│   ├── requirements.txt       # Python dependencies
│   └── pyproject.toml         # Tool configurations
│
├── frontend/                   # React TypeScript (planned)
│
├── docs/                       # Comprehensive documentation
│   ├── 00-discovery/          # Product brief, brainstorming
│   ├── 01-planning/           # PRD, UI/UX specs, features
│   ├── 02-architecture/       # Technical architecture
│   └── 03-implementation/     # Sprint planning, dev guides
│
├── claudedocs/                 # AI Assistant system
│   ├── AI-ASSISTANT-INSTRUCTIONS.md  # 10 core Instructions
│   └── prompts/               # 9 standardized Prompts
│       ├── PROMPT-01-PROJECT-ONBOARDING.md
│       ├── PROMPT-04-SPRINT-DEVELOPMENT.md
│       ├── PROMPT-06-PROGRESS-SAVE.md  # Most used
│       └── ...
│
├── scripts/                    # Utility scripts
│   └── init-db.sql            # Database initialization
│
├── docker-compose.yml         # Local development stack
├── .env.example               # Environment template
└── .bmad/                     # BMAD methodology framework
```

---

## BMAD Workflow Integration

This project follows the **BMAD (BMad Agile Development) Method**, a structured AI-assisted development workflow:

### Current Status

Track project phase and Sprint progress:

```bash
# View workflow status
cat docs/bmm-workflow-status.yaml

# View Sprint status
cat docs/03-implementation/sprint-status.yaml
```

**Current Phase**: Phase 3 - Implementation
**Current Sprint**: Sprint 0 (Infrastructure Foundation)
**Sprint Duration**: 2 weeks
**Team Size**: 8 members
**Velocity Target**: 40 story points

### Sprint Workflow

```bash
# 1. Start Sprint Story
# Use PROMPT-02 to prepare
"@claudedocs/prompts/PROMPT-02-NEW-SPRINT-PREP.md Sprint-0 S0-1"

# 2. Execute Development
# Use PROMPT-04 for guided development
"@claudedocs/prompts/PROMPT-04-SPRINT-DEVELOPMENT.md Sprint-0 S0-1"

# 3. Save Progress
# Use PROMPT-06 (most commonly used)
"@claudedocs/prompts/PROMPT-06-PROGRESS-SAVE.md Sprint-0 S0-1"

# 4. End Session
# Use PROMPT-09 for session summary
"@claudedocs/prompts/PROMPT-09-SESSION-END.md"
```

### Available BMAD Commands

The project includes AI-assisted workflow commands:

```bash
# Project onboarding
/bmad:bmm:workflows:workflow-init

# Sprint management
/bmad:bmm:workflows:sprint-planning
/bmad:bmm:workflows:create-story
/bmad:bmm:workflows:dev-story

# Architecture and review
/bmad:bmm:workflows:architecture
/bmad:bmm:workflows:code-review

# Documentation
/bmad:bmm:workflows:document-project
```

---

## AI Assistant Instructions

This repository includes a comprehensive AI assistant system in `claudedocs/`:

### 10 Core Instructions

| ID | Purpose | Usage |
|----|---------|-------|
| Instruction 1 | Update Sprint Status YAML | `"請使用 Instruction 1 更新狀態"` |
| Instruction 3 | Git Standard Workflow | `"請使用 Instruction 3 提交代碼"` |
| Instruction 6 | Document Consistency Check | `"請使用 Instruction 6 檢查文檔"` |
| Instruction 8 | Quick Progress Sync | `"請使用 Instruction 8 快速同步"` |

### 9 Standardized Prompts

- **PROMPT-01**: Project onboarding for new developers/AI
- **PROMPT-02**: Prepare new Sprint Story
- **PROMPT-04**: Execute Sprint development
- **PROMPT-06**: Save progress and update status (most used)
- **PROMPT-08**: Code review

Full documentation: `claudedocs/AI-ASSISTANT-INSTRUCTIONS.md`

---

## Git Workflow

### Branch Naming

```bash
# Feature branches
feature/sprint-{N}-{story-id}-{description}
# Example: feature/sprint-0-s0-1-dev-environment

# Bug fixes
bugfix/{bug-id}-{description}
# Example: bugfix/bug-001-docker-network

# Hotfixes
hotfix/{issue-id}-{description}
```

### Commit Message Format

Follow Conventional Commits:

```
<type>(<scope>): <description>

[optional body]

🤖 Generated with Claude Code
Co-Authored-By: Claude <noreply@anthropic.com>
```

**Types**: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`
**Scopes**: `sprint-{N}`, `backend`, `frontend`, `docs`, `docker`

Example:
```bash
git commit -m "feat(sprint-0): complete S0-1 development environment setup

- Configure Docker Compose for local development
- Add PostgreSQL, Redis, RabbitMQ containers
- Create initialization scripts

🤖 Generated with Claude Code
Co-Authored-By: Claude <noreply@anthropic.com>"
```

---

## Code Standards

### Python

- **Style Guide**: PEP 8
- **Formatter**: Black (line-length: 100)
- **Import Sorter**: isort (profile: black)
- **Linter**: flake8
- **Type Checker**: mypy (strict mode)
- **Testing**: pytest with coverage >= 80%

Configuration: `backend/pyproject.toml`

### Testing Strategy

```python
# Test file naming: test_*.py
# Test class naming: Test*
# Test function naming: test_*

# Markers
@pytest.mark.unit          # Fast, isolated tests
@pytest.mark.integration   # Tests with external dependencies
@pytest.mark.slow          # Long-running tests
```

---

## Environment Variables

Copy `.env.example` to `.env` and configure:

```bash
# Database
DB_NAME=ipa_platform
DB_USER=ipa_user
DB_PASSWORD=<secure-password>
DB_PORT=5432

# Redis
REDIS_PASSWORD=<secure-password>
REDIS_PORT=6379

# RabbitMQ (local dev)
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
RABBITMQ_PORT=5672

# Azure OpenAI (required for Agent Framework)
AZURE_OPENAI_ENDPOINT=https://<resource>.openai.azure.com/
AZURE_OPENAI_API_KEY=<key>
AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4o

# Azure Service Bus (production)
AZURE_SERVICE_BUS_CONNECTION_STRING=<connection-string>
```

---

## Key Documentation

### Planning & Architecture
- `docs/01-planning/prd/prd-main.md` - Product Requirements
- `docs/02-architecture/technical-architecture.md` - Technical design
- `docs/02-architecture/gate-check/solutioning-gate-check.md` - Architecture validation

### Implementation
- `docs/03-implementation/sprint-planning/` - All Sprint plans
- `docs/03-implementation/sprint-status.yaml` - Current Sprint tracking
- `docs/03-implementation/local-development-guide.md` - Dev setup

### AI Assistant System
- `claudedocs/AI-ASSISTANT-INSTRUCTIONS.md` - Complete instruction manual
- `claudedocs/prompts/README.md` - Prompts library guide

---

## Microsoft Agent Framework

The platform is built on Microsoft Agent Framework (Preview), which unifies:
- **Semantic Kernel**: LLM orchestration and prompt management
- **AutoGen**: Multi-agent collaboration patterns
- **Native Features**: Checkpoints, human-in-the-loop, state management

Key concepts:
- **Agents**: AI-powered decision makers with tools and context
- **Workflows**: Graph-based execution flows with conditional logic
- **Checkpoints**: State snapshots for human approval steps
- **Tools**: External system integrations (ServiceNow, Dynamics 365, etc.)

Agent Framework is in Preview - expect API changes. Monitor:
- https://github.com/microsoft/semantic-kernel
- https://github.com/microsoft/autogen

---

## Common Workflows

### Starting a New Sprint Story

1. Prepare Story: `@PROMPT-02-NEW-SPRINT-PREP.md Sprint-0 S0-X`
2. Create feature branch: `git checkout -b feature/sprint-0-s0-X-description`
3. Develop: `@PROMPT-04-SPRINT-DEVELOPMENT.md Sprint-0 S0-X`
4. Test: `pytest tests/` (ensure >= 80% coverage)
5. Save progress: `@PROMPT-06-PROGRESS-SAVE.md Sprint-0 S0-X`
6. Code review: `@PROMPT-08-CODE-REVIEW.md backend/src/`

### Quick Bug Fix

1. Prepare: `@PROMPT-03-BUG-FIX-PREP.md BUG-XXX`
2. Create bugfix branch: `git checkout -b bugfix/bug-XXX-description`
3. Fix and test
4. Quick sync: Use `Instruction 8` for fast commit

### Ending Work Session

Always run before finishing:
```bash
"@claudedocs/prompts/PROMPT-09-SESSION-END.md"
```

This ensures:
- All changes are committed
- Documentation is synchronized
- Sprint status is updated
- Next session todos are created

---

## Architecture Decision Records

Key technical decisions:

1. **MVP Deployment**: Azure App Service (not Kubernetes) for faster launch
2. **Message Queue**: Azure Service Bus (production) / RabbitMQ (local dev)
3. **Monitoring**: Hybrid approach - Azure Monitor + Application Insights + Prometheus
4. **Agent Framework**: Preview version accepted with upgrade plan for GA
5. **Workflow Definition**: Pure Python code (no visual designer in MVP)

Full details: `docs/02-architecture/technical-architecture.md`

---

## Project Phases

Based on `docs/bmm-workflow-status.yaml`:

- ✅ **Phase 0: Discovery** - Product brief, brainstorming (Completed)
- ✅ **Phase 1: Planning** - PRD, UI/UX design (Completed)
- ✅ **Phase 2: Solutioning** - Architecture, gate check (Completed)
- 🔄 **Phase 3: Implementation** - Sprint 0-5 execution (Current)

Sprint Overview:
- **Sprint 0**: Infrastructure & Foundation (Current)
- **Sprint 1**: Core Services (Agent, Workflow, Execution)
- **Sprint 2**: Integrations (ServiceNow, Dynamics, n8n)
- **Sprint 3**: Security & Observability
- **Sprint 4**: UI & Frontend
- **Sprint 5**: Testing & Launch

---

## Notes for AI Assistants

1. **Always check Sprint status** before starting work:
   - Current Sprint: `docs/03-implementation/sprint-status.yaml`
   - Workflow phase: `docs/bmm-workflow-status.yaml`

2. **Use AI Assistant system** for standardized workflows:
   - Instructions for quick operations
   - Prompts for complete workflows
   - See `claudedocs/AI-ASSISTANT-INSTRUCTIONS.md`

3. **Follow BMAD methodology**:
   - Phase-based progression
   - Sprint-driven development
   - Document-first approach

4. **Code quality is mandatory**:
   - Black formatting (100 char line length)
   - Type hints (mypy strict)
   - Test coverage >= 80%
   - No unhandled exceptions

5. **Agent Framework is Preview**:
   - API may change
   - Document workarounds
   - Prepare for GA migration

6. **All Chinese documentation** is intentional:
   - Target market is Taiwan/Hong Kong
   - Technical terms in English
   - Comments in Traditional Chinese

---

**Last Updated**: 2025-11-20
**Project Start**: 2025-11-14
**Current Sprint**: Sprint 0 (Infrastructure Foundation)
**Next Milestone**: Sprint 0 completion (2025-12-06)
