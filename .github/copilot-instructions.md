# DotNetSkills AI Coding Agent Instructions

## Project Overview

This is a .NET 9 project management API built with **Clean Architecture** and **Domain-Driven Design** principles. The project demonstrates enterprise-grade development practices for a portfolio/skills showcase.

## Architecture Structure

**Clean Architecture Layers** (dependency flow: API → Application → Domain):
- `DotNetSkills.Domain/` - Core business entities, domain events, and business rules
- `DotNetSkills.Application/` - Use cases, DTOs, repository interfaces, orchestration logic  
- `DotNetSkills.Infrastructure/` - EF Core repositories, external services, database implementations
- `DotNetSkills.API/` - Minimal APIs, JWT auth, validation, mapping layer

**Testing Structure**: Each layer has corresponding unit test projects in `tests/` directory.

## Key Technical Patterns

### Domain-Driven Design Implementation
- **Rich Domain Models**: Business logic lives in entity methods (e.g., `Task.AssignTo()`, `Team.AddMember()`)
- **Domain Events**: Use domain events for cross-aggregate communication (see PRD section 3.4.3)
- **Aggregates**: User, Team, Project, Task are separate aggregates with clear boundaries
- **Value Objects**: Use for immutable data structures

### Authentication & Authorization
- **JWT-based**: Stateless authentication with role-based authorization
- **Roles**: Admin, ProjectManager, Developer, Viewer with specific permissions
- **Admin-only user creation**: No self-registration in MVP

### Data Relationships
- **Team ↔ User**: Many-to-many via `TeamMember` entity
- **Project → Team**: Each project belongs to one team
- **Task → Project**: Tasks belong to projects
- **Task → User**: Single assignment model
- **Task → Task**: One-level subtask nesting only

## Development Workflow

### Build & Test Commands
```bash
# Build solution
dotnet build

# Run API project
dotnet run --project src/DotNetSkills.API

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/DotNetSkills.Domain.UnitTests
```

### Project Structure Conventions
- **Entities**: Domain models in `Domain/Entities/`
- **DTOs**: Request/response models in `Application/DTOs/`
- **Repositories**: Interfaces in Application, implementations in Infrastructure
- **Endpoints**: Minimal API endpoints organized by feature in `API/Endpoints/`

## Critical Business Rules

### Task Management
- **Single assignment**: Each task assigned to exactly one user
- **Status transitions**: Enforce valid state changes (cannot go from Done to ToDo)
- **Subtask constraints**: Only one level of nesting allowed
- **Cascading deletes**: Parent task deletion affects subtasks

### Team & Project Logic
- **Project leadership**: No explicit leader field - use ProjectManager role within team
- **Deletion constraints**: Cannot delete teams with active projects, projects with active tasks

### Authentication Flow
- **Token-based**: JWT tokens for stateless authentication
- **Role validation**: Endpoint access controlled by user roles
- **Admin privileges**: User CRUD operations restricted to Admin role

## Technology Stack Specifics

- **.NET 9**: Latest framework features, nullable reference types enabled
- **Minimal APIs**: Use for lean endpoint definitions
- **EF Core**: Repository pattern with DbContext
- **JWT**: For authentication (no refresh tokens in MVP)
- **AutoMapper**: Entity ↔ DTO mapping
- **FluentValidation**: Input validation
- **xUnit + Moq**: Testing framework and mocking

## Implementation Priorities

**Phase 1**: Domain models and basic infrastructure
**Phase 2**: JWT authentication and user management  
**Phase 3**: Core CRUD operations for all entities
**Phase 4**: Testing and CI/CD pipeline

Refer to `doc/PRD.md` section 7 for detailed implementation phases and acceptance criteria.

Refer to `doc/General Coding Principles.md` for coding standards and best practices.
