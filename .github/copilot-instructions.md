# GitHub Copilot Instructions - DotNetSkills

## Architecture Overview

DotNetSkills is a **Clean Architecture + Domain-Driven Design** project management API built with **.NET 9**. The solution follows strict architectural boundaries with **bounded contexts** organized by business domains.

### Layer Dependencies (Critical)
```
API → Application → Domain ← Infrastructure
```
- **Domain**: Pure business logic, no external dependencies
- **Application**: Use cases (CQRS), depends only on Domain
- **Infrastructure**: Data access, external services, depends on Application
- **API**: Minimal APIs, orchestrates all layers

### Bounded Context Organization
Each domain is organized into bounded contexts:
- `UserManagement/` - User entities, roles, authentication
- `TeamCollaboration/` - Teams, memberships, collaboration
- `ProjectManagement/` - Projects, status, team association
- `TaskExecution/` - Tasks, subtasks, assignments

**Key Pattern**: Each bounded context has its own `BoundedContextUsings.cs` to prevent cross-context coupling.

## Domain Layer Patterns

### Rich Domain Models
Business logic lives IN entities, not services:
```csharp
public class User : AggregateRoot<UserId>
{
    public void JoinTeam(Team team, TeamRole role)
    {
        // Validation + business rules here
        if (_teamMemberships.Count >= MaxTeamMemberships)
            throw new DomainException("Cannot exceed team limit");
    }
}
```

### Strongly-Typed IDs
ALL entities use value object IDs implementing `IStronglyTypedId<T>`:
```csharp
public record UserId(Guid Value) : IStronglyTypedId<Guid>
{
    public static UserId New() => new(Guid.NewGuid());
}
```

### Domain Events
Use for decoupled aggregate communication:
```csharp
RaiseDomainEvent(new UserJoinedTeamDomainEvent(user.Id, team.Id));
```

## Dependency Injection Architecture

### Critical DI Pattern
Each layer has `DependencyInjection.cs` that registers its services:
- **API**: `AddApiServices()` - orchestrates all layers
- **Application**: `AddApplicationServices()` - MediatR, AutoMapper, FluentValidation
- **Infrastructure**: `AddInfrastructureServices()` - EF Core, repositories
- **Domain**: Uses `DomainServiceFactory` (no DI dependencies)

**Domain DI Exception**: Domain layer uses factory pattern to avoid DI dependencies.

## Application Layer (CQRS)

### MediatR Pipeline Order (Critical)
Behaviors execute in this specific order:
1. `LoggingBehavior` - Captures all operations
2. `PerformanceBehavior` - Measures total time
3. `ValidationBehavior` - FluentValidation rules
4. `DomainEventDispatchBehavior` - Dispatches events

### Command/Query Pattern
```csharp
public class CreateUserCommand : IRequest<UserResponse>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
```

## Infrastructure Patterns

### Repository Pattern
Base repository with strongly-typed IDs:
```csharp
public abstract class BaseRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : IStronglyTypedId<Guid>
```

### Advanced Async Patterns
- Use `ConfigureAwait(false)` in library code
- Support `IAsyncEnumerable<T>` for streaming
- Implement batched processing with `GetBatchedAsync()`

### EF Core Configuration
- Entity configurations in `Persistence/Configurations/`
- Value converters for strongly-typed IDs in `ValueConverters.cs`
- Domain events handled via `SaveChangesAsync()` override

## API Layer (Minimal APIs)

### Endpoint Organization
Endpoints grouped by bounded context with extension methods:
```csharp
app.MapUserManagementEndpoints();
app.MapTeamCollaborationEndpoints();
app.MapProjectManagementEndpoints();
app.MapTaskExecutionEndpoints();
```

### Endpoint Patterns
- RESTful routes: `/api/v1/{resource}`
- Business operations: `/api/v1/{resource}/{id}/{action}`
- Validation through FluentValidation integration
- OpenAPI documentation with `WithSummary()`, `Produces()`

## Testing Patterns

### Test Organization
- **Unit Tests**: Fast, domain logic focused
- **Integration Tests**: Full HTTP pipeline with Testcontainers
- **Test Categories**: `[Trait("Category", "Unit|Integration|E2E")]`

### Test Builders
Use Builder pattern for test data (when implemented):
```csharp
var user = UserBuilder.Default()
    .WithEmail("test@example.com")
    .WithRole(UserRole.Developer)
    .Build();
```

## Development Workflows

### Build & Test Commands
```bash
# Build entire solution
dotnet build

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/DotNetSkills.Domain.UnitTests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Database Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Update database
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

### Development Server
```bash
dotnet run --project src/DotNetSkills.API
# API: https://localhost:5001
# Swagger: https://localhost:5001/swagger
```

## Critical Global Usings Strategy

### Layer-Specific Globals
Each layer has `GlobalUsings.cs` with layer-appropriate imports. **Domain** uses bounded context isolation - check `BoundedContextUsings.cs` files before adding cross-context dependencies.

### Naming Conflict Resolution
```csharp
// In Domain GlobalUsings.cs
global using TaskStatus = DotNetSkills.Domain.TaskExecution.Enums.TaskStatus;
global using Task = DotNetSkills.Domain.TaskExecution.Entities.Task;
```

## Project-Specific Conventions

### Exception Handling
- Domain: `DomainException` for business rule violations
- Global: `ExceptionHandlingMiddleware` converts to ProblemDetails
- Validation: FluentValidation automatic integration

### Security (When Implemented)
- JWT with role-based authorization
- Admin-only user creation pattern
- BCrypt password hashing
- Rate limiting and CORS configured

### Performance Requirements
- `< 200ms` for simple CRUD operations
- Support for `100+` concurrent users
- Memory-efficient streaming with `IAsyncEnumerable<T>`

## Documentation References
- Architecture details: `docs/DependencyInjection-Architecture.md`
- Coding standards: `.github/instructions/dotnet.instructions.md`
- Implementation plan: `plan/feature-dotnetskills-mvp-1.md`
