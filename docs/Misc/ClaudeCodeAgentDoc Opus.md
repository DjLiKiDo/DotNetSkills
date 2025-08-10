# Claude Code Agent Configuration Guide

Based on comprehensive research of Anthropic's official documentation, **Claude Code uses a sophisticated agent system built around the `.claude` folder structure** that enables developers to create specialized AI agents for different software engineering roles. This system provides granular control over agent behavior, task routing, and quality assurance through YAML-based configuration files and JSON settings.

The agent configuration system is designed around **single-responsibility principles** where each agent focuses on specific expertise areas like backend development, frontend engineering, or security auditing. Claude Code automatically routes tasks to appropriate agents based on context analysis and semantic matching, while also supporting explicit agent invocation and multi-agent coordination patterns.

## Project structure and configuration hierarchy

The `.claude` folder follows a specific hierarchy that determines how settings and agents are loaded and prioritized:

```
project/
├── .claude/
│   ├── settings.json              # Project settings (checked into git)
│   ├── settings.local.json        # Local settings (auto-ignored by git)
│   ├── agents/                    # Project-level subagents
│   │   ├── backend-architect.md   # Individual agent definitions
│   │   └── frontend-developer.md
│   ├── commands/                  # Custom slash commands
│   │   ├── fix-issue.md          # Command definition files
│   │   ├── frontend/             # Organized by category
│   │   └── backend/
│   └── hooks/                    # Hook scripts for automation
└── CLAUDE.md                     # Project context file
```

**Configuration precedence follows a strict hierarchy**: Enterprise policies override command line arguments, which override local project settings (`.claude/settings.local.json`), which override shared project settings (`.claude/settings.json`), which override user settings (`~/.claude/settings.json`). Project-level subagents in `.claude/agents/` always take precedence over user-level agents.

## Agent specialization and development roles

**Effective agent design centers on focused expertise areas** rather than generalist approaches. The recommended strategy involves creating agents with single, clear responsibilities that align with specific development roles.

### Backend development agent configuration

```yaml
---
name: backend-architect
description: Design RESTful APIs, microservice boundaries, and database schemas. Use proactively for backend development tasks.
tools: Read, Write, Edit, Bash, Grep, WebFetch
model: claude-3-5-sonnet-20241022
---
You are a senior backend architect specializing in scalable system design.

**Core Responsibilities:**
- Design RESTful APIs with proper versioning
- Architect microservice boundaries and communication patterns  
- Design database schemas with proper normalization
- Implement authentication and authorization systems
- Follow security best practices for API development

**Technical Stack Expertise:**
- Node.js/Express, Python/FastAPI, Go, Java Spring
- PostgreSQL, MongoDB, Redis
- Docker, Kubernetes, AWS/GCP

Always consider scalability, security, and maintainability in your solutions.
```

### Frontend development specialist

```yaml
---
name: frontend-developer  
description: Build React components, implement responsive layouts, and handle client-side state management. Use for UI development tasks.
tools: Read, Write, Edit, Bash, WebFetch
---
You are an expert frontend developer specializing in modern React applications.

**Core Responsibilities:**
- Build reusable React components with TypeScript
- Implement responsive layouts using CSS Grid/Flexbox
- Handle client-side state with Context API or Redux
- Ensure accessibility (WCAG 2.1) compliance

**Technical Expertise:**
- React 18+, TypeScript, Next.js
- CSS Modules, Styled Components, Tailwind CSS  
- React Query, SWR for data fetching
- Testing with Jest, React Testing Library

Focus on performance, accessibility, and user experience.
```

## Multi-agent coordination patterns

Claude Code supports several coordination patterns for complex software engineering tasks. **Sequential delegation** routes tasks through multiple specialists in order: user request → backend-architect → frontend-developer → test-automator → security-auditor. **Parallel execution** runs multiple agents simultaneously on different aspects of the same problem, then merges results.

The system provides both automatic routing based on context analysis and explicit agent invocation. Users can override automatic selection with commands like `> Use the code-reviewer subagent to check my recent changes` or `> Have the security-auditor subagent analyze this endpoint`.

**Agent management uses interactive commands**: `/agents` opens the comprehensive management interface, `/agents create` provides guided setup for new agents, and `/agents edit` modifies existing configurations. All agents can also be managed directly through file manipulation in the `.claude/agents/` directory.

## Configuration file format and syntax

### YAML frontmatter structure

Agent definition files use a **strict YAML frontmatter format** within `.md` files:

```yaml
---
name: agent-name                    # Required: lowercase letters and hyphens only
description: When this should be used  # Required: natural language description
tools: tool1, tool2, tool3         # Optional: comma-separated list
model: claude-3-5-sonnet-20241022   # Optional: specific Claude model
---
# System Prompt Content

Your detailed agent instructions go here as Markdown content.
```

**Required fields** include `name` (using kebab-case only) and `description` (clear natural language explanation). **Optional fields** include `tools` (comma-separated list inheriting all tools if omitted) and `model` (uses system default if not specified).

### Settings configuration format

Project settings use **JSON structure** in `.claude/settings.json`:

```json
{
  "permissions": {
    "allow": [
      "Bash(npm run lint)",
      "Bash(npm run test:*)",
      "Edit",
      "Write"
    ],
    "deny": [
      "Bash(curl:*)",
      "Read(./.env)",
      "Read(./.env.*)",
      "Read(./secrets/**)"
    ]
  },
  "env": {
    "CLAUDE_CODE_ENABLE_TELEMETRY": "1"
  },
  "hooks": {
    "PreToolUse": {
      "Edit|Write": [{
        "type": "command", 
        "command": "prettier --write \"$CLAUDE_FILE_PATHS\""
      }]
    }
  }
}
```

## Task routing and assignment mechanisms

**Claude Code uses intelligent routing algorithms** that analyze task descriptions against agent `description` fields, perform keyword and semantic matching, and consider current project context and file types. The system evaluates subagent expertise alignment, tool requirements, and model capability requirements to select the most appropriate agent.

For complex tasks, the system performs **automatic task decomposition** and assigns parallel agents. For example, "Build new feature" might route to backend-architect for API design, frontend-developer for UI implementation, test-automation-engineer for test coverage, and security-auditor for security review.

The **Task tool enables programmatic sub-agent spawning** where the main agent can invoke specialized sub-agents for specific subtasks, creating supervisor-style coordination for complex workflows.

## Quality assurance and validation systems

### Hook-based quality gates

**Claude Code implements a comprehensive hooks system** for enforcing quality standards:

```json
{
  "hooks": {
    "PostToolUse": [{
      "matcher": "Edit|Write",
      "hooks": [{
        "type": "command",
        "command": "prettier --write \"$CLAUDE_FILE_PATHS\""
      }]
    }]
  }
}
```

**Available hook types** include `PreToolUse` for blocking dangerous operations, `PostToolUse` for quality checks after operations, `UserPromptSubmit` for prompt validation, and `SubagentStop` for ensuring task completion.

### Permission-based access control

**Fine-grained permissions system** prevents agents from accessing sensitive files or executing dangerous commands:

```json
{
  "permissions": {
    "deny": [
      "Read(./.env)",
      "Read(./.env.*)",
      "Read(./secrets/**)",
      "Bash(rm:*)",
      "Bash(curl:*)"
    ]
  }
}
```

**Tool restrictions can be applied per-agent** by specifying limited tool sets in agent configurations, enabling read-only reviewers or write-restricted specialists.

## Validation rules and schema requirements

**Strict validation requirements** ensure proper agent functionality. Agent files must use `.md` extension with valid YAML frontmatter delimited by `---`. Agent names must use lowercase letters and hyphens only (no spaces, underscores, or special characters). The `tools` field requires comma-separated, case-sensitive tool names.

**Settings files must conform to valid JSON structure** with specific permission patterns and hook configuration schemas. Environment variables accept string values only, and hook exit codes follow specific meanings: `0` for success, `2` for blocking errors, and other codes for non-blocking errors.

**File organization follows strict conventions**: agent files in `.claude/agents/`, command files in `.claude/commands/`, and settings files with exact naming requirements. Invalid configurations prevent agent loading, while malformed settings are ignored.

## Conclusion

Claude Code's agent system provides a **sophisticated framework for organizing AI assistance around software engineering specializations**. The combination of hierarchical configuration, intelligent task routing, multi-agent coordination, and comprehensive quality controls creates a powerful development environment that adapts to complex project needs.

**Key implementation recommendations** include starting with essential agents (code-reviewer, security-auditor, test-suite-generator), using single-responsibility design principles, implementing quality gates through hooks, and leveraging the permission system for security. The system's flexibility supports both simple single-agent workflows and complex multi-agent orchestration for enterprise-scale development projects.