# Architecture Overview

*System design, patterns, and architectural decisions for DotNetSkills*

## High-Level Architecture

DotNetSkills follows **Clean Architecture** principles with **Domain-Driven Design** patterns, creating a maintainable and testable enterprise application.

```
┌─────────────────────────────────────────────┐
│                 API Layer                   │
│     (Minimal APIs, Middleware, Auth)        │
└─────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────┐
│             Application Layer               │
│  (CQRS, Handlers, DTOs, Validation)        │
└─────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────┐
│              Domain Layer                   │
│  (Entities, Value Objects, Events)         │
└─────────────────────────────────────────────┘
                      ▲
┌─────────────────────────────────────────────┐
│            Infrastructure Layer             │
│   (EF Core, Repositories, External APIs)   │
└─────────────────────────────────────────────┘
```

### Dependency Rules
- **API** → Application → Domain
- **Infrastructure** → Application → Domain
- **Domain** has no external dependencies
- Dependencies always point inward

## Bounded Contexts

The system is organized around 4 main bounded contexts:

### 1. **UserManagement**
**Responsibility**: User accounts, authentication, and authorization
- **Entities**: User
- **Value Objects**: UserId, EmailAddress
- **Events**: UserCreatedDomainEvent
- **Key Features**: CRUD operations, role management, authentication

### 2. **TeamCollaboration** 
**Responsibility**: Team formation and member management
- **Entities**: Team, TeamMember
- **Value Objects**: TeamId, TeamMemberId
- **Events**: TeamCreatedDomainEvent, UserJoinedTeamDomainEvent
- **Key Features**: Team creation, member roles, many-to-many relationships

### 3. **ProjectManagement**
**Responsibility**: Project lifecycle and organization
- **Entities**: Project
- **Value Objects**: ProjectId
- **Events**: ProjectCreatedDomainEvent, ProjectStatusChangedDomainEvent
- **Key Features**: Project CRUD, status tracking, team association

### 4. **TaskExecution**
**Responsibility**: Task assignment and tracking
- **Entities**: Task
- **Value Objects**: TaskId
- **Events**: TaskCreatedDomainEvent, TaskAssignedDomainEvent
- **Key Features**: Task management, assignment, subtasks, status workflow

## Layer Responsibilities

### 📱 API Layer (`src/DotNetSkills.API/`)

**Purpose**: HTTP interface and cross-cutting concerns

**Key Components**:
- **Minimal APIs**: Lightweight endpoint definitions grouped by bounded context
- **Middleware Pipeline**: Correlation ID, exception handling, performance logging
- **Authentication**: JWT token validation and authorization policies
- **Configuration**: Options validation and environment-specific settings

**Endpoint Pattern**:
```csharp
// Standard pattern
async (CreateCommand command, IMediator mediator) => await mediator.Send(command)

// Grouped by context
app.MapGroup("/api/v1/users")
   .RequireAuthorization()
   .MapUserManagementEndpoints();
```

### 🎯 Application Layer (`src/DotNetSkills.Application/`)

**Purpose**: Use cases, business workflows, and orchestration

**Key Components**:
- **CQRS Pattern**: Commands for writes, Queries for reads
- **MediatR Pipeline**: Behaviors for logging, validation, performance, events
- **DTOs**: Data transfer objects for API contracts
- **Validation**: FluentValidation rules for all inputs
- **AutoMapper**: Entity ↔ DTO transformations

**Handler Pattern**:
```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken ct)
    {
        // Validation is handled by pipeline behavior
        // Domain logic is in entities
        // This layer orchestrates the workflow
    }
}
```

### 🏛️ Domain Layer (`src/DotNetSkills.Domain/`)

**Purpose**: Core business logic and domain rules

**Key Components**:
- **Entities**: Rich domain models with business logic
- **Value Objects**: Immutable objects representing domain concepts
- **Domain Events**: Communicate between aggregates
- **Domain Services**: Complex business rules spanning multiple entities
- **Strongly-Typed IDs**: Type-safe identifiers

**Entity Pattern**:
```csharp
public class User : BaseEntity<UserId>
{
    private User() { } // EF Constructor
    
    public static User Create(string firstName, string lastName, EmailAddress email)
    {
        // Domain validation and business rules
        var user = new User { /* ... */ };
        user.AddDomainEvent(new UserCreatedDomainEvent(user.Id));
        return user;
    }
}
```

### 🔧 Infrastructure Layer (`src/DotNetSkills.Infrastructure/`)

**Purpose**: External concerns and data persistence

**Key Components**:
- **Entity Framework Core**: Database access and migrations
- **Repository Pattern**: Data access abstractions with caching decorators
- **Unit of Work**: Transaction management
- **External Services**: Third-party API integrations
- **Domain Event Dispatcher**: Event handling infrastructure

**Repository Pattern**:
```csharp
public class UserRepository : BaseRepository<User, UserId>, IUserRepository
{
    // Custom queries specific to User aggregate
}

// With caching decorator
services.Decorate<IUserRepository, CachedUserRepository>();
```

## Key Architectural Patterns

### 🔄 CQRS (Command Query Responsibility Segregation)

**Commands**: Modify state, return minimal data
```csharp
public record CreateUserCommand(string FirstName, string LastName, string Email) 
    : IRequest<UserResponse>;
```

**Queries**: Read data, no side effects
```csharp
public record GetUserByIdQuery(UserId UserId) 
    : IRequest<UserResponse>;
```

### 🎭 MediatR Pipeline Behaviors

**Execution Order** (order matters!):
1. **LoggingBehavior** - Captures all operations early
2. **ValidationBehavior** - Short-circuits before expensive work  
3. **PerformanceBehavior** - Measures after validation passes
4. **DomainEventDispatchBehavior** - Dispatches after successful execution

### 🚨 Exception-Only Contract (ADR-0001)

**Philosophy**: Use exceptions for control flow instead of Result<T> wrappers

```csharp
// ✅ Current approach
public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken ct)
{
    if (!emailIsUnique)
        throw new DomainException("Email already exists");
    
    return new UserResponse(...); // Success path
}

// ❌ Deprecated approach  
public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken ct)
{
    // Don't use Result<T> wrappers
}
```

### 🎯 Domain Events

**Pattern**: Raise events in aggregates, handle in application layer

```csharp
// In Domain Entity
public void AssignToTeam(TeamId teamId)
{
    // Business logic
    AddDomainEvent(new UserJoinedTeamDomainEvent(Id, teamId));
}

// In Application Event Handler
public class UserJoinedTeamDomainEventHandler : INotificationHandler<UserJoinedTeamDomainEvent>
{
    // Cross-aggregate communication
}
```

## Data Flow

### Typical Request Flow
```
1. HTTP Request → API Endpoint
2. Endpoint → MediatR.Send(Command/Query)
3. MediatR → Pipeline Behaviors (Logging, Validation, Performance)
4. MediatR → Handler
5. Handler → Domain Service/Repository
6. Repository → Database (via EF Core)
7. Response ← DTO mapped from Entity
8. HTTP Response ← Serialized DTO
```

### Error Handling Flow
```
1. Exception thrown in Handler/Domain
2. Global Exception Middleware catches
3. Middleware maps to appropriate HTTP status
4. ProblemDetails response with correlation ID
5. Client receives structured error
```

## Database Design

### Entity Framework Configuration
- **Conventions**: Each bounded context has its own configuration files
- **Shadow Foreign Keys**: Explicit foreign key properties avoided ([see documentation](EFCore-Relationships-AntiShadowFKs.md))
- **Value Converters**: Strongly-typed IDs stored as GUIDs
- **Migrations**: Automatic on startup (configurable via `RUN_MIGRATIONS`)

### Repository Pattern
```csharp
// Base repository with common operations
public abstract class BaseRepository<TEntity, TId> 
    where TEntity : BaseEntity<TId>
    where TId : IStronglyTypedId

// Context-specific repositories
public interface IUserRepository : IRepository<User, UserId>
{
    Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken ct);
}

// Cached decorators for performance
public class CachedUserRepository : CachedRepositoryBase<User, UserId>, IUserRepository
```

## Security Architecture

### Authentication & Authorization
- **JWT Tokens**: Stateless authentication with configurable signing keys
- **Role-Based Access**: Admin, ProjectManager, Developer, Viewer
- **Claims Population**: Current user context available throughout application
- **Policy-Based Authorization**: Granular permissions per endpoint

### Input Validation
- **Request Validation**: FluentValidation behaviors in MediatR pipeline
- **Domain Validation**: Business rules enforced in entities
- **SQL Injection Protection**: Parameterized queries via EF Core

## Performance Considerations

### Caching Strategy
- **Repository Decorators**: Transparent caching layer
- **Redis Support**: Optional distributed caching (Docker profile: cache)
- **Memory Caching**: Default in-memory caching for development

### Database Performance
- **Connection Pooling**: EF Core connection management
- **Retry Policies**: Exponential backoff for transient failures
- **Query Optimization**: Proper indexing and query patterns

### Monitoring
- **Performance Logging**: Request timing middleware
- **Health Checks**: Database connectivity and application health
- **Correlation IDs**: Request tracing across service boundaries

## Testing Strategy

### Test Architecture
```
tests/
├── Domain.UnitTests/         # Domain logic and business rules
├── Application.UnitTests/    # Handlers, behaviors, validation
├── Infrastructure.UnitTests/ # Repository and data access
└── API.UnitTests/           # Endpoints and middleware
```

### Testing Patterns
- **Unit Tests**: Fast, isolated tests for business logic
- **Builder Pattern**: Maintainable test data creation
- **Test Doubles**: Mocks and stubs for external dependencies
- **Integration Tests**: Real database via Testcontainers

## Configuration Management

### Environment-Specific Configuration
- **Development**: Local settings, detailed logging, Swagger enabled
- **Staging**: Production-like settings, limited logging
- **Production**: Optimized settings, security hardened

### Configuration Sources (Priority Order)
1. Command line arguments
2. Environment variables (`DOTNETSKILLS_*`)
3. User secrets (Development only)
4. Azure Key Vault (Production)
5. appsettings.{Environment}.json
6. appsettings.json

## Deployment Architecture

### Docker Containerization
- **Multi-stage builds**: Optimized for size and security
- **Health checks**: Built-in container monitoring
- **Non-root execution**: Security best practices
- **Environment profiles**: Development, staging, production

### Service Dependencies
- **API Service**: Main application container
- **Database**: SQL Server 2022 Developer
- **Cache**: Redis (optional, profile-based)
- **Reverse Proxy**: Ready for nginx/traefik integration

## Extension Points

### Adding New Bounded Contexts
1. Create Domain entities and events in `src/DotNetSkills.Domain/[Context]/`
2. Add Application contracts and handlers in `src/DotNetSkills.Application/[Context]/`
3. Implement Infrastructure repositories in `src/DotNetSkills.Infrastructure/`
4. Expose API endpoints in `src/DotNetSkills.API/Endpoints/[Context]/`

### Custom Behaviors
```csharp
// Add to MediatR pipeline in Application DependencyInjection
cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(YourCustomBehavior<,>));
```

### Event Handlers
```csharp
// Implement INotificationHandler for domain events
public class YourEventHandler : INotificationHandler<YourDomainEvent>
```

## Related Documentation

- **[Coding Standards](DotNet%20Coding%20Principles.md)** - Detailed development guidelines
- **[Architecture Decision Records](adr/)** - Key architectural decisions
- **[EF Core Guidelines](EFCore-Relationships-AntiShadowFKs.md)** - Database relationship patterns
- **[Developer Guide](DEVELOPER-GUIDE.md)** - Day-to-day development practices

---

**📋 Architecture Principles Summary**:
- **Separation of Concerns**: Each layer has clear responsibilities
- **Dependency Inversion**: Dependencies point toward the domain
- **Explicit is Better**: No magic, clear contracts and interfaces
- **Fail Fast**: Validation early in the pipeline, meaningful errors
- **Testability**: Architecture supports comprehensive testing