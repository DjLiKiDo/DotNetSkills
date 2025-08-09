# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

Build the entire solution:
```bash
dotnet build
```

Run tests:
```bash
dotnet test
```

Run specific test project:
```bash
dotnet test tests/DotNetSkills.Domain.UnitTests
dotnet test tests/DotNetSkills.Application.UnitTests
dotnet test tests/DotNetSkills.Infrastructure.UnitTests
dotnet test tests/DotNetSkills.API.UnitTests
```

Run the API:
```bash
dotnet run --project src/DotNetSkills.API
```

Database migrations (when needed):
```bash
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

## Architecture Overview

This is a .NET 9 Clean Architecture solution following Domain-Driven Design principles with CQRS pattern implementation using MediatR.

### Layer Structure and Dependencies
```
API Layer (Minimal APIs)
    ↓ depends on
Application Layer (Use Cases, CQRS)
    ↓ depends on  
Domain Layer (Entities, Business Logic)
    ↑ implemented by
Infrastructure Layer (Database, External Services)
```

### Project Structure
- **DotNetSkills.Domain**: Core business entities, value objects, domain events, and business rules
- **DotNetSkills.Application**: Use cases implemented as Commands/Queries, DTOs, validators, and application interfaces
- **DotNetSkills.Infrastructure**: EF Core repositories, database configuration, and external service implementations
- **DotNetSkills.API**: Minimal API endpoints, authentication, and middleware

### Bounded Contexts
The solution is organized into four main bounded contexts:

1. **UserManagement**: User lifecycle, roles, and authentication
   - Entry point: `Domain.UserManagement.Entities.User`
   - Key operations: Create, activate/deactivate users, role management

2. **TeamCollaboration**: Team management and membership
   - Entry point: `Domain.TeamCollaboration.Entities.Team`  
   - Key operations: Create teams, manage membership, assign roles

3. **ProjectManagement**: Project lifecycle and organization
   - Entry point: `Domain.ProjectManagement.Entities.Project`
   - Key operations: Create/update projects, project status management

4. **TaskExecution**: Task management and assignment
   - Entry point: `Domain.TaskExecution.Entities.Task`
   - Key operations: Create/assign tasks, status transitions, subtask support

## Key Patterns and Conventions

### Domain Layer
- Rich domain models with business logic encapsulated in entities
- Strongly-typed IDs using records: `record UserId(Guid Value) : IStronglyTypedId<Guid>`
- Domain events for decoupled communication between aggregates
- Value objects for concepts defined by attributes (e.g., `EmailAddress`)
- Business rule validation using `Ensure.BusinessRule()` pattern

### Application Layer
- CQRS pattern with separate Commands and Queries
- MediatR for command/query handling
- FluentValidation for input validation with `ValidationBehavior`
- Repository abstractions (never expose IQueryable)
- DTOs for API contracts using record types
- AutoMapper for entity ↔ DTO transformations

### API Layer
- Minimal APIs organized in endpoint groups by bounded context
- JWT authentication with role-based authorization policies
- Global exception handling middleware
- Swagger/OpenAPI documentation with enum examples
- Request/response validation using FluentValidation integration

### Infrastructure Layer
- Entity Framework Core with SQL Server
- Repository pattern implementation
- Unit of Work pattern for transaction coordination
- Entity configurations using `IEntityTypeConfiguration<T>`
- Value converters for strongly-typed IDs

## Code Quality Standards

### Naming Conventions
- Classes/Methods: PascalCase (`CreateUserCommandHandler`)
- Properties: PascalCase (`public string Name { get; private set; }`)
- Fields: camelCase with underscore (`private readonly IUserRepository _userRepository`)
- Parameters: camelCase (`public User(string name, EmailAddress email)`)
- Constants: PascalCase (`public const int MaxTeamMembers = 50`)

### Testing Structure
- Unit tests for domain logic and business rules
- Integration tests using TestContainers for database scenarios
- Builder pattern for test data creation
- 80%+ coverage target, focusing on Domain and Application layers
- Test categories: `[Trait("Category", "Unit")]` and `[Trait("Category", "Integration")]`

### Security Guidelines
- JWT authentication with role-based policies
- Input validation at multiple layers (Domain, Application, API)
- Never expose secrets in code - use configuration/environment variables
- BCrypt password hashing
- Validate all user inputs using FluentValidation

## Current Implementation Status

This is an MVP in development. Key areas requiring attention:

### High Priority Tasks
1. **Authorization policies**: Need to implement policies referenced in endpoints (`AdminOnly`, `TeamManager`, `ProjectManagerOrAdmin`, `ProjectMemberOrAdmin`)
2. **MediatR integration**: Many endpoints have TODO placeholders that need to be wired to actual command/query handlers
3. **Application handlers**: Missing implementations for CQRS handlers across all bounded contexts
4. **Current user context**: Need service to extract current user ID from JWT claims

### Common TODO Patterns
- "Replace with MediatR.Send" in endpoint files
- "TODO: Implement [operation]" in Application handlers
- "Get current user ID from authentication context" 
- "TODO: Log exception" for error handling
- "Replace with AutoMapper when configured" for DTO mapping

When working on these areas, follow the established patterns and maintain consistency with the existing codebase architecture.

## Important Implementation Notes

- Always use `ConfigureAwait(false)` in Application/Infrastructure layers
- Domain entities should validate business rules and raise domain events
- Repository methods should return strongly-typed results, never IQueryable
- Use projection queries for read-only scenarios to improve performance
- Implement proper async/await patterns throughout
- Follow the existing GlobalUsings.cs patterns for each project layer
- Maintain Clean Architecture dependency rules - outer layers depend on inner layers only