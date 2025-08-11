# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Build & Test
```bash
dotnet restore && dotnet build
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

### Database Operations
```bash
# Update database with migrations
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Create new migration
dotnet ef migrations add <MigrationName> --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

### Running the API
```bash
dotnet run --project src/DotNetSkills.API
```

### Docker Development
```bash
docker-compose up -d  # Start SQL Server
```

## Architecture Overview

This is a Clean Architecture .NET 9 project implementing Domain-Driven Design with these layers:

```
API → Application → Domain
Infrastructure → Application → Domain  
```

### Key Architectural Patterns
- **Clean Architecture**: Dependency rule - inner layers never reference outer layers
- **Domain-Driven Design**: Rich domain models with business logic in entities
- **CQRS with MediatR**: Commands/Queries for all use cases
- **Repository Pattern**: With caching decorators in Infrastructure
- **Domain Events**: Decoupled communication between aggregates
- **Strongly-Typed IDs**: `record UserId(Guid Value)` pattern throughout

### Project Structure
- `DotNetSkills.Domain`: Entities, value objects, domain events, business rules
- `DotNetSkills.Application`: Use cases (commands/queries), DTOs, validation, mapping
- `DotNetSkills.Infrastructure`: EF Core repositories, database context, external services
- `DotNetSkills.API`: Minimal APIs, middleware, authentication, endpoint routing

### Bounded Contexts
The domain is organized into four bounded contexts:
- `UserManagement`: User entities, roles, authentication
- `TeamCollaboration`: Teams, team members, roles  
- `ProjectManagement`: Projects, project lifecycle
- `TaskExecution`: Tasks, assignments, status management

## Error Handling Contract (ADR-0001)

**CRITICAL**: The Application layer uses an exception-only contract. Never return `Result<T>` wrappers.

### Handler Pattern
```csharp
// Correct
public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken ct)
{
    // Throw exceptions for failures
    if (!emailIsUnique)
        throw new DomainException("Email already exists");
    
    return new UserResponse(...); // Direct return for success
}
```

### Exception Types
- `ValidationException` (FluentValidation): Request validation failures
- `DomainException`: Business rule violations
- `NotFoundException`: Resource not found
- `BusinessRuleViolationException`: Domain invariant violations

## Development Patterns

### Adding New Features
1. Define domain changes in `Domain/<Context>/`
2. Add repository interface method in `Application/<Context>/Contracts/`  
3. Create Command/Query + Validator + Handler + Mapping in `Application`
4. Implement repository method + caching decorator in `Infrastructure`
5. Add endpoint in `API/Endpoints/<Context>/` calling MediatR
6. Add tests in appropriate test projects

### Domain Modeling Rules
- Rich entities with business logic in domain methods, not services
- Private setters, public methods for state changes
- Strongly-typed IDs: `record ProjectId(Guid Value)`  
- Value objects for constrained primitives: `record EmailAddress`
- Domain events raised in aggregate methods, dispatched post-handler

### Repository Pattern
- Application defines interfaces, Infrastructure implements
- Use caching decorators: `CachedUserRepository` wraps `UserRepository`
- Never expose `IQueryable` - provide specific query methods
- Repository methods should be async and return domain entities

### Validation & Mapping
- FluentValidation validators beside commands in `Application`
- AutoMapper profiles per bounded context
- ValidationBehavior ensures handlers receive valid input

### Endpoint Conventions
```csharp
var group = app.MapGroup("/api/v1/users")
    .WithTags("Users")
    .RequireAuthorization();

// Handler pattern
group.MapPost("/", async (CreateUserCommand command, IMediator mediator) =>
    await mediator.Send(command));
```

## Technology Stack

- **.NET 9**: Latest framework with nullable reference types enabled
- **Entity Framework Core**: SQL Server with retry policies
- **Minimal APIs**: Lightweight endpoint definitions
- **JWT Authentication**: Role-based authorization (Admin, ProjectManager, Developer, Viewer)
- **MediatR**: CQRS implementation with pipeline behaviors
- **FluentValidation**: Input validation
- **AutoMapper**: Entity ↔ DTO mapping
- **xUnit + FluentAssertions**: Testing framework
- **Testcontainers**: Integration tests with real databases
- **Serilog**: Structured logging

## Key Conventions

### Naming Patterns
- Commands: `CreateUserCommand`, `UpdateTaskStatusCommand`
- Queries: `GetUserByIdQuery`, `GetProjectTasksQuery`
- Handlers: `CreateUserCommandHandler`, `GetUserByIdQueryHandler`
- Validators: `CreateUserCommandValidator`
- DTOs: `UserResponse`, `CreateUserRequest`

### Pipeline Behaviors (Order Matters)
1. LoggingBehavior
2. ValidationBehavior (throws ValidationException)
3. PerformanceBehavior  
4. DomainEventDispatchBehavior

### Authentication & Authorization
- JWT optional via `JwtOptions.Enabled`
- Use `ICurrentUserService` in handlers, not HttpContext
- Role-based policies: `Policies.AdminOnly`, `Policies.ProjectManager`

## Common Anti-Patterns to Avoid

- ❌ Business logic in Application handlers instead of Domain entities
- ❌ Returning `Result<T>` wrappers (use exceptions per ADR-0001)
- ❌ Exposing `IQueryable` from repositories
- ❌ Accessing `ApplicationDbContext` outside Infrastructure layer
- ❌ Using primitive IDs instead of strongly-typed ones
- ❌ Bypassing MediatR in endpoints
- ❌ Catching and swallowing exceptions in endpoints

## Testing Approach

- **Unit Tests**: Fast tests for domain logic and business rules
- **Integration Tests**: Repository tests with Testcontainers
- **API Tests**: Full HTTP request/response with WebApplicationFactory
- **Test Builders**: Builder pattern for maintainable test data
- Run specific test project: `dotnet test tests/DotNetSkills.Domain.UnitTests`

## Security Notes

- JWT authentication with role-based authorization
- Input validation at multiple layers (domain, application, API)
- Password hashing with BCrypt
- Secrets stored in User Secrets (dev) or Azure Key Vault (prod)
- Never commit secrets to repository

## Performance Considerations

- Use projection queries (`Select`) for read-only scenarios
- Strategic `Include` for related data, consider `AsSplitQuery()` for multiple collections
- Caching decorators with 5-10 minute expiration
- Async/await throughout, never use `.Result` or `.Wait()`
- Pagination for large result sets