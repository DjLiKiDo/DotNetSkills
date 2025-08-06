## DotNetSkills - Component Cross-Reference Guide

### Bounded Context Relationships

```mermaid
graph TD
    UM[UserManagement] --> TC[TeamCollaboration]
    TC --> PM[ProjectManagement]
    PM --> TE[TaskExecution]
    
    UM -.-> PM
    UM -.-> TE
    TC -.-> TE
```

### Domain Entity Relationships

| Aggregate Root | Owns | References | Events Raised |
|----------------|------|------------|---------------|
| User | TeamMember (via Team) | - | UserCreated, UserDeactivated |
| Team | TeamMember | User | TeamCreated, UserJoinedTeam, UserLeftTeam |
| Project | - | Team | ProjectCreated, ProjectStatusChanged |
| Task | Subtasks | Project, User | TaskCreated, TaskAssigned, TaskStatusChanged |

### Application Service Dependencies

| Bounded Context | Depends On | Reason |
|-----------------|------------|--------|
| TeamCollaboration | UserManagement | User validation and retrieval |
| ProjectManagement | TeamCollaboration | Team membership validation |
| TaskExecution | ProjectManagement, UserManagement | Project context and user assignment |

### Interface Contracts

| Interface | Implementation | Location | Purpose |
|-----------|----------------|----------|---------|
| IUserRepository | EfUserRepository | Infrastructure | User data access |
| ITeamRepository | EfTeamRepository | Infrastructure | Team data access |
| IProjectRepository | EfProjectRepository | Infrastructure | Project data access |
| ITaskRepository | EfTaskRepository | Infrastructure | Task data access |

### Event Flow

```mermaid
sequenceDiagram
    participant API
    participant App as Application
    participant Dom as Domain
    participant Inf as Infrastructure
    
    API->>App: Command/Query
    App->>Dom: Business Operation
    Dom->>Dom: Raise Domain Event
    App->>Inf: Persist Changes
    Inf->>App: Dispatch Events
    App->>API: Response
```
