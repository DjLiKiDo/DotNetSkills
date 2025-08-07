# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Common Development Commands

### Building and Running
```bash
# Restore dependencies
dotnet restore

# Build the entire solution
dotnet build

# Run the API project
dotnet run --project src/DotNetSkills.API

# Run with hot reload for development
dotnet watch run --project src/DotNetSkills.API
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/DotNetSkills.Domain.UnitTests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Database Operations
```bash
# Create new migration
dotnet ef migrations add <MigrationName> --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Update database
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Remove last migration
dotnet ef migrations remove --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

### Useful Development Scripts
```bash
# Run a single test by name
dotnet test --filter "MethodName=TestName"

# Run tests for specific category
dotnet test --filter "Category=Unit"

# Build in release mode
dotnet build --configuration Release
```

## Architecture Overview

This project follows **Clean Architecture** principles with **Domain-Driven Design** patterns, organized into 4 main layers:

### Project Structure
```
src/
├── DotNetSkills.API/           # Presentation Layer - Minimal APIs, middleware, endpoints
├── DotNetSkills.Application/   # Application Layer - Use cases, CQRS, DTOs, validators  
├── DotNetSkills.Domain/        # Domain Layer - Entities, value objects, domain events
└── DotNetSkills.Infrastructure/# Infrastructure Layer - Repositories, DbContext, external services
```

### Domain Organization by Bounded Contexts

The domain is organized into 4 bounded contexts:

1. **UserManagement** (`src/DotNetSkills.Domain/UserManagement/`)
   - Core entity: `User` 
   - Handles user lifecycle, roles (Admin, ProjectManager, Developer, Viewer)
   - Key operations: Create, Activate, Deactivate, Role management

2. **TeamCollaboration** (`src/DotNetSkills.Domain/TeamCollaboration/`)
   - Core entity: `Team` with `TeamMember` entities
   - Manages team formation and member roles
   - Key operations: Create teams, add/remove members, role assignments

3. **ProjectManagement** (`src/DotNetSkills.Domain/ProjectManagement/`)
   - Core entity: `Project`
   - Project lifecycle and status management
   - Key operations: Create, archive, status transitions

4. **TaskExecution** (`src/DotNetSkills.Domain/TaskExecution/`)
   - Core entity: `Task` 
   - Task assignment and progress tracking
   - Key operations: Create, assign, update status, subtask support

### Key Architectural Patterns

**Dependency Injection Architecture**: Each layer has a `DependencyInjection.cs` file that registers services:
- Domain: Uses factory pattern (no DI dependencies)  
- Application: Registers MediatR, validators, AutoMapper
- Infrastructure: Registers EF Core, repositories
- API: Orchestrates all layers + web-specific services

**CQRS with MediatR**: Commands and queries are separated:
- Commands: `CreateUserCommand`, `UpdateProjectCommand`, etc.
- Queries: `GetUserByIdQuery`, `GetProjectTasksQuery`, etc.
- Handlers: One handler per command/query following Single Responsibility

**Repository Pattern**: Interfaces in Application layer, implementations in Infrastructure:
```csharp
// Application layer
public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id);
    Task<User> AddAsync(User user);
}

// Infrastructure layer  
public class EfUserRepository : IUserRepository
```

**Domain Events**: Cross-aggregate communication via events:
```csharp
// Domain events raised by aggregate roots
public record UserCreatedDomainEvent(UserId UserId) : IDomainEvent;
public record TaskAssignedDomainEvent(TaskId TaskId, UserId UserId) : IDomainEvent;
```

**Strong Typing**: Value objects for all identifiers:
```csharp
public record UserId(Guid Value) : IStronglyTypedId<Guid>;
public record EmailAddress(string Value);
```

## Development Guidelines

### Code Organization
- Follow existing folder structure and naming conventions
- Each bounded context is self-contained within its layer
- Use `GlobalUsings.cs` files - they're already configured per layer
- Endpoint organization: Group by bounded context in `/Endpoints/{Context}/` folders

### Entity Framework Patterns  
- Entities use strong-typed IDs with value converters
- All configurations in separate `IEntityTypeConfiguration<T>` classes
- Repository base class provides common async operations including `IAsyncEnumerable<T>`
- Use `ConfigureAwait(false)` consistently in repository methods

### Testing Strategy
- Unit tests for domain logic and business rules
- Integration tests using the existing `BaseRepository<TEntity, TId>` pattern
- Test builders available for entity creation (Builder pattern)
- Test projects mirror source structure

### Performance Considerations
- The codebase has excellent async pattern implementation 
- Uses `IAsyncEnumerable<T>` for memory-efficient large result processing
- Performance monitoring service is available (`PerformanceMonitoringService`)
- **Critical issue to fix**: Avoid `ContinueWith` patterns found in `TeamRepository:180` and `TaskRepository:418`

### Security Implementation  
- JWT authentication planned (currently commented out in `Program.cs`)
- Role-based authorization with enum-based roles
- Input validation using FluentValidation (handlers expect this pattern)
- Never commit secrets - use User Secrets for development, environment variables for production

## Important Files and Conventions

### Configuration Files
- `appsettings.json` / `appsettings.Development.json` - Application configuration
- Each layer's `DependencyInjection.cs` - Service registration 
- Entity configurations in `Infrastructure/Persistence/Configurations/`

### Key Extension Points
- Add new entities: Follow the bounded context pattern 
- Add new use cases: Create Command/Query + Handler + Validator
- Add new endpoints: Extend existing endpoint groups in `/Endpoints/`
- Add domain events: Implement `IDomainEvent` and corresponding handlers

### Validation Patterns
The codebase follows multi-layer validation:
- Domain validation: In entity constructors and methods using `Ensure.BusinessRule()`
- Application validation: FluentValidation validators for commands/queries  
- API validation: Automatic validation via validation behaviors

### Common Pitfalls to Avoid
- Don't mix business logic in Application layer - keep it in Domain entities
- Don't expose `IQueryable` from repositories - use specific methods
- Don't break dependency rules - Domain should have no external dependencies
- Don't skip async/await patterns - the codebase is consistently async

This codebase demonstrates excellent clean architecture implementation with modern .NET 9 patterns. Follow the existing conventions and you'll maintain the high code quality standards already established.