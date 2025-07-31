# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Architecture

This is a .NET 9 project management API following **Clean Architecture** and **Domain-Driven Design** principles. The solution demonstrates enterprise-grade development practices for a portfolio/skills showcase.

### Layer Structure
```
DotNetSkills.API/          ← Minimal APIs, JWT auth, validation, mapping
DotNetSkills.Application/  ← Use cases, DTOs, repository interfaces  
DotNetSkills.Infrastructure/ ← EF Core repos, external services, DB implementations
DotNetSkills.Domain/       ← Core entities, domain events, business rules
```

**Dependency Rule**: API → Application → Domain (Infrastructure also depends on Application)

### Key Domain Concepts
- **User**: Core entity with role-based permissions (Admin, ProjectManager, Developer, Viewer)
- **Team**: Collection of users working together, many-to-many via TeamMember entity
- **Project**: Belongs to one team, contains tasks
- **Task**: Assigned to single user, supports one-level subtask nesting only

## Development Commands

### Build & Test
```bash
# Build entire solution
dotnet build

# Run API (default port: https://localhost:7240)
dotnet run --project src/DotNetSkills.API

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/DotNetSkills.Domain.UnitTests
dotnet test tests/DotNetSkills.Application.UnitTests
dotnet test tests/DotNetSkills.Infrastructure.UnitTests
dotnet test tests/DotNetSkills.API.UnitTests

# Database migrations (when EF is configured)
dotnet ef migrations add <MigrationName> --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

### Docker Development
```bash
# Run with Docker Compose (when configured)
docker-compose up -d

# Build Docker image
docker build -t dotnetskills-api .
```

## Critical Business Rules

### Authentication & Authorization
- **JWT-based authentication** with stateless tokens
- **Admin-only user creation** - no self-registration in MVP
- **Role-based access control**: Admin > ProjectManager > Developer > Viewer
- **No refresh tokens** in initial MVP implementation

### Task Management Rules
- **Single assignment model**: Each task assigned to exactly one user
- **Status transition validation**: Cannot move from Done back to ToDo
- **Subtask nesting**: Only one level deep (task → subtask, but subtask cannot have children)
- **Cascading operations**: Deleting parent task affects all subtasks

### Team & Project Rules
- **Project-team relationship**: Each project belongs to exactly one team
- **Deletion constraints**: Cannot delete teams with active projects, or projects with active tasks
- **Team membership**: Many-to-many relationship between users and teams via TeamMember entity

## Development Patterns

### Domain-Driven Design Implementation
- **Rich domain models**: Business logic in entity methods (e.g., `Task.AssignTo()`, `Team.AddMember()`)
- **Domain events**: For cross-aggregate communication (TaskAssigned, UserJoinedTeam, etc.)
- **Value objects**: For immutable concepts like strongly-typed IDs
- **Repository pattern**: Interfaces in Application, implementations in Infrastructure

### Code Organization
- **Entities**: Domain models in `Domain/Entities/`
- **DTOs**: Request/response models in `Application/DTOs/`
- **Validators**: FluentValidation classes in `Application/Validators/`
- **Endpoints**: Minimal API endpoints organized by feature in `API/Endpoints/`
- **Services**: Business logic orchestration in `Application/Services/`

### Testing Strategy
- **Unit tests**: Fast tests for business logic and domain rules
- **Integration tests**: API endpoints with real database (TestContainers)
- **Test builders**: Builder pattern for test data creation
- **80% coverage minimum**: Focus on Domain and Application layers

## Technology Stack

- **.NET 9**: Latest framework with nullable reference types enabled
- **Entity Framework Core**: With SQL Server provider
- **Minimal APIs**: For lean, performance-focused endpoints
- **JWT Authentication**: For stateless security
- **AutoMapper**: Entity ↔ DTO transformations
- **FluentValidation**: Comprehensive input validation
- **xUnit + FluentAssertions**: Testing framework
- **Testcontainers**: Integration testing with real databases

## Implementation Status

Current project is in **planning phase**. Core implementation follows the plan in `plan/feature-dotnetskills-mvp-1.md`:

1. **Phase 1**: Domain models and infrastructure setup
2. **Phase 2**: JWT authentication and user management
3. **Phase 3**: Core CRUD operations for all entities
4. **Phase 4**: Testing and CI/CD pipeline

## Key Files to Reference

- `.github/instructions/dotnet.instructions.md`: Comprehensive .NET development guidelines, coding standards, patterns and best practices
- `plan/feature-dotnetskills-mvp-1.md`: Detailed MVP implementation plan
- `.github/copilot-instructions.md`: AI agent development guidelines
- `doc/prd.md`: Product requirements and business specifications

When implementing features, always follow the Clean Architecture dependency rules, implement rich domain models with business logic, and ensure comprehensive test coverage.