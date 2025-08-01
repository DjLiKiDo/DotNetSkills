# DotNet Coding Principles - DotNetSkills Project

> Specific coding standards and best practices for the DotNetSkills project, built with .NET 9, Clean Architecture, and Domain-Driven Design.

---

## Table of Contents

1. [Core SOLID Principles in .NET Context](#core-solid-principles-in-net-context)
2. [Clean Architecture Principles](#clean-architecture-principles)
3. [Domain-Driven Design Guidelines](#domain-driven-design-guidelines)
4. [.NET 9 Specific Best Practices](#net-9-specific-best-practices)
5. [Entity Framework Core Guidelines](#entity-framework-core-guidelines)
6. [Minimal API Conventions](#minimal-api-conventions)
7. [Testing Principles](#testing-principles)
8. [Security Guidelines](#security-guidelines)
9. [Performance and Scalability](#performance-and-scalability)
10. [Code Quality Standards](#code-quality-standards)
11. [What to Avoid](#what-to-avoid)

---

## Core SOLID Principles in .NET Context

### 1. **Single Responsibility Principle (SRP)**
Every class, method, and module must have one clear, well-defined purpose.

**✅ Good Examples:**
```csharp
// Domain Entity - Only handles user business logic
public class User : BaseEntity
{
    public void UpdateEmail(string newEmail) { /* validation + update */ }
    public bool CanJoinTeam(Team team) { /* business rule */ }
}

// Repository - Only handles data access
public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id);
    Task<User> AddAsync(User user);
}

// Use Case - Only orchestrates one business operation
public class CreateUserCommand : IRequest<UserResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
```

**❌ Avoid:**
```csharp
// God class that does everything
public class UserService
{
    public void CreateUser() { }
    public void SendEmail() { }
    public void ValidatePermissions() { }
    public void LogActivity() { }
    public void GenerateReports() { }
}
```

### 2. **Open/Closed Principle (OCP)**
Classes should be open for extension but closed for modification.

**✅ Good Examples:**
```csharp
// Base notification strategy
public abstract class NotificationStrategy
{
    public abstract Task SendAsync(string message, string recipient);
}

// Extension without modifying existing code
public class EmailNotificationStrategy : NotificationStrategy
{
    public override Task SendAsync(string message, string recipient)
        => emailService.SendAsync(recipient, message);
}

// Domain events enable extension
public class TaskAssignedDomainEvent : IDomainEvent
{
    public UserId UserId { get; }
    public TaskId TaskId { get; }
}
```

### 3. **Liskov Substitution Principle (LSP)**
Derived classes must be substitutable for their base classes.

**✅ Good Examples:**
```csharp
public interface IRepository<T, TId> where T : BaseEntity<TId>
{
    Task<T?> GetByIdAsync(TId id);
    Task<T> AddAsync(T entity);
}

// All implementations must honor the contract
public class UserRepository : IRepository<User, UserId>
{
    // Must return null if not found, as per interface contract
    public async Task<User?> GetByIdAsync(UserId id) { }
}
```

### 4. **Interface Segregation Principle (ISP)**
Clients shouldn't depend on interfaces they don't use.

**✅ Good Examples:**
```csharp
// Focused interfaces
public interface IUserReader
{
    Task<User?> GetByIdAsync(UserId id);
    Task<IEnumerable<User>> GetAllAsync();
}

public interface IUserWriter
{
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(UserId id);
}

// Compose when needed
public interface IUserRepository : IUserReader, IUserWriter { }
```

### 5. **Dependency Inversion Principle (DIP)**
High-level modules shouldn't depend on low-level modules. Both should depend on abstractions.

**✅ Good Examples:**
```csharp
// Application layer depends on abstraction
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository; // Abstraction
    private readonly IDomainEventDispatcher _eventDispatcher; // Abstraction
    
    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IDomainEventDispatcher eventDispatcher)
    {
        _userRepository = userRepository;
        _eventDispatcher = eventDispatcher;
    }
}

// Infrastructure implements abstractions
public class EfUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    // Implementation details
}
```

---

## Clean Architecture Principles

### **Dependency Rule**
Dependencies must point inward. Inner layers cannot know about outer layers.

```
API → Application → Domain
Infrastructure → Application → Domain
```

**✅ Correct Dependencies:**
```csharp
// Domain Layer - No dependencies on other layers
namespace DotNetSkills.Domain.Entities;
public class User : BaseEntity<UserId> { }

// Application Layer - Only depends on Domain
namespace DotNetSkills.Application.Users.Commands;
public class CreateUserCommand : IRequest<UserResponse>
{
    // Uses Domain entities and value objects
    public UserId? Id { get; init; }
}

// Infrastructure - Depends on Application interfaces
namespace DotNetSkills.Infrastructure.Repositories;
public class EfUserRepository : IUserRepository // Application interface
{
    // Implementation uses EF Core
}
```

### **Layer Responsibilities**

#### **Domain Layer** (`DotNetSkills.Domain`)
- **Entities**: Rich domain models with business logic
- **Value Objects**: Immutable objects defined by their values
- **Domain Events**: Significant business occurrences
- **Enums**: Domain-specific enumerations
- **Interfaces**: Contracts needed by domain logic

```csharp
// Domain Entity with business logic
public class Team : BaseEntity<TeamId>
{
    private readonly List<TeamMember> _members = new();
    public IReadOnlyList<TeamMember> Members => _members.AsReadOnly();
    
    public void AddMember(User user, TeamRole role = TeamRole.Developer)
    {
        if (_members.Any(m => m.UserId == user.Id))
            throw new DomainException("User is already a team member");
            
        var member = new TeamMember(user.Id, Id, role);
        _members.Add(member);
        
        // Raise domain event
        RaiseDomainEvent(new UserJoinedTeamDomainEvent(user.Id, Id));
    }
}
```

#### **Application Layer** (`DotNetSkills.Application`)
- **Commands/Queries**: Use case implementations (CQRS)
- **DTOs**: Data transfer objects for API contracts
- **Interfaces**: Repository and service contracts
- **Validators**: Input validation rules
- **Mappers**: Entity ↔ DTO transformations

```csharp
// Command Handler (Use Case)
public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, TeamResponse>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<TeamResponse> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = Team.Create(request.Name, request.Description);
        
        await _teamRepository.AddAsync(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return TeamMapper.ToResponse(team);
    }
}
```

#### **Infrastructure Layer** (`DotNetSkills.Infrastructure`)
- **Repositories**: EF Core implementations
- **DbContext**: Database access configuration
- **External Services**: Third-party integrations
- **Configurations**: Entity configurations

```csharp
// Repository Implementation
public class EfTeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<Team?> GetByIdAsync(TeamId id)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
```

#### **API Layer** (`DotNetSkills.API`)
- **Endpoints**: Minimal API definitions
- **Middleware**: Cross-cutting concerns
- **Authentication**: JWT configuration
- **Validation**: Input validation integration

```csharp
// Minimal API Endpoint
public static class TeamEndpoints
{
    public static void MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/teams")
            .WithTags("Teams")
            .RequireAuthorization();
            
        group.MapPost("/", CreateTeam)
            .WithSummary("Create a new team");
    }
    
    private static async Task<IResult> CreateTeam(
        CreateTeamCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return Results.Created($"/api/v1/teams/{result.Id}", result);
    }
}
```

---

## Domain-Driven Design Guidelines

### **Rich Domain Models**
Business logic belongs in domain entities, not in services.

**✅ Rich Domain Model:**
```csharp
public class Task : BaseEntity<TaskId>
{
    public TaskStatus Status { get; private set; }
    public UserId? AssignedUserId { get; private set; }
    
    public void AssignTo(User user)
    {
        if (Status == TaskStatus.Completed)
            throw new DomainException("Cannot assign completed tasks");
            
        AssignedUserId = user.Id;
        RaiseDomainEvent(new TaskAssignedDomainEvent(Id, user.Id));
    }
    
    public void MarkAsCompleted()
    {
        if (AssignedUserId == null)
            throw new DomainException("Cannot complete unassigned task");
            
        Status = TaskStatus.Completed;
        RaiseDomainEvent(new TaskCompletedDomainEvent(Id, AssignedUserId.Value));
    }
}
```

**❌ Anemic Domain Model:**
```csharp
public class Task
{
    public TaskStatus Status { get; set; } // Public setter
    public UserId? AssignedUserId { get; set; } // No business logic
}

// Business logic scattered in services
public class TaskService
{
    public void AssignTask(Task task, User user)
    {
        // Business logic outside domain model
        if (task.Status == TaskStatus.Completed)
            throw new Exception("Cannot assign completed tasks");
        task.AssignedUserId = user.Id;
    }
}
```

### **Value Objects**
Use value objects for concepts defined by their attributes, not identity.

**✅ Value Object Examples:**
```csharp
public record UserId(Guid Value) : IStronglyTypedId<Guid>
{
    public static UserId New() => new(Guid.NewGuid());
    public static implicit operator Guid(UserId id) => id.Value;
}

public record EmailAddress
{
    public string Value { get; }
    
    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty");
            
        if (!IsValidEmail(value))
            throw new ArgumentException("Invalid email format");
            
        Value = value.ToLowerInvariant();
    }
    
    private static bool IsValidEmail(string email)
        => email.Contains('@') && email.Contains('.');
}
```

### **Domain Events**
Use domain events for decoupled communication between aggregates.

**✅ Domain Events:**
```csharp
public record TaskAssignedDomainEvent(TaskId TaskId, UserId UserId) : IDomainEvent;

public class TaskAssignedDomainEventHandler : IDomainEventHandler<TaskAssignedDomainEvent>
{
    private readonly INotificationService _notificationService;
    
    public async Task Handle(TaskAssignedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Send notification without coupling aggregates
        await _notificationService.NotifyTaskAssignedAsync(
            domainEvent.UserId, 
            domainEvent.TaskId);
    }
}
```

### **Aggregate Boundaries**
Each aggregate should be a consistency boundary with a single root entity.

**✅ Proper Aggregate Design:**
```csharp
// Team aggregate
public class Team : BaseEntity<TeamId> // Aggregate Root
{
    private readonly List<TeamMember> _members = new();
    
    // All operations go through the root
    public void AddMember(UserId userId, TeamRole role)
    {
        // Invariant enforcement
        if (_members.Count >= MaxMembers)
            throw new DomainException("Team is at maximum capacity");
            
        var member = new TeamMember(userId, Id, role);
        _members.Add(member);
    }
}

// TeamMember is part of Team aggregate, not separate
public class TeamMember : BaseEntity<TeamMemberId>
{
    internal TeamMember(UserId userId, TeamId teamId, TeamRole role)
    {
        UserId = userId;
        TeamId = teamId;
        Role = role;
    }
}
```

---

## .NET 9 Specific Best Practices

### **Nullable Reference Types**
Enable nullable reference types and handle nullability explicitly.

**✅ Proper Nullable Usage:**
```csharp
#nullable enable

public class User : BaseEntity<UserId>
{
    public string Name { get; private set; } = string.Empty; // Non-nullable
    public string? Bio { get; private set; } // Explicitly nullable
    public EmailAddress Email { get; private set; } = null!; // Set in constructor
    
    public User(string name, EmailAddress email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }
    
    public void UpdateBio(string? bio)
    {
        Bio = bio; // Can be null
    }
}
```

### **Record Types**
Use records for DTOs and value objects.

**✅ Record Usage:**
```csharp
// DTOs as records
public record CreateUserRequest(string Name, string Email);
public record UserResponse(Guid Id, string Name, string Email, DateTime CreatedAt);

// Value objects as records
public record TaskId(Guid Value) : IStronglyTypedId<Guid>;
public record ProjectStatus(string Value)
{
    public static readonly ProjectStatus Active = new("Active");
    public static readonly ProjectStatus Completed = new("Completed");
    public static readonly ProjectStatus OnHold = new("OnHold");
}
```

### **Pattern Matching**
Use modern C# pattern matching for cleaner conditionals.

**✅ Pattern Matching:**
```csharp
public string GetStatusDescription(TaskStatus status) => status switch
{
    TaskStatus.ToDo => "Ready to start",
    TaskStatus.InProgress => "Work in progress",
    TaskStatus.InReview => "Under review",
    TaskStatus.Done => "Completed",
    _ => throw new ArgumentOutOfRangeException(nameof(status))
};

// Property patterns
public bool CanAssignTask(Task task, User user) => task switch
{
    { Status: TaskStatus.Done } => false,
    { AssignedUserId: not null } => false,
    _ when user.IsActive => true,
    _ => false
};
```

### **Global Using Statements**
Organize common using statements globally.

**✅ GlobalUsings.cs:**
```csharp
// Global using statements
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// Domain layer globals
global using DotNetSkills.Domain.Common;
global using DotNetSkills.Domain.Events;

// Application layer globals  
global using MediatR;
global using FluentValidation;
```

### **Minimal APIs with EndpointRouteBuilder Extensions**
Organize endpoints using extension methods.

**✅ Endpoint Organization:**
```csharp
public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/users")
            .WithTags("Users")
            .RequireAuthorization();
            
        group.MapGet("/{id:guid}", GetUser)
            .WithName("GetUser")
            .WithSummary("Get user by ID")
            .Produces<UserResponse>()
            .ProducesValidationProblem();
            
        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .RequireAuthorization(Policies.AdminOnly);
    }
}
```

---

## Entity Framework Core Guidelines

### **Entity Configuration**
Use `IEntityTypeConfiguration<T>` for clean entity mapping.

**✅ Entity Configuration:**
```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));
                
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);
        });
        
        // Optimistic concurrency
        builder.Property(u => u.UpdatedAt)
            .IsRowVersion();
    }
}
```

### **Repository Pattern Implementation**
Implement repositories with proper abstractions.

**✅ Repository Pattern:**
```csharp
// Application layer interface
public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id);
    Task<User?> GetByEmailAsync(EmailAddress email);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(UserId id);
}

// Infrastructure implementation
public class EfUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public EfUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByIdAsync(UserId id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        return user;
    }
}
```

### **Unit of Work Pattern**
Coordinate transactions across repositories.

**✅ Unit of Work:**
```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        await DispatchDomainEventsAsync();
        
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### **Query Optimization**
Use projection and Include strategically.

**✅ Optimized Queries:**
```csharp
// Projection for read-only scenarios
public async Task<IReadOnlyList<UserSummaryDto>> GetUserSummariesAsync()
{
    return await _context.Users
        .Select(u => new UserSummaryDto
        {
            Id = u.Id.Value,
            Name = u.Name,
            Email = u.Email.Value,
            TeamCount = u.TeamMemberships.Count
        })
        .ToListAsync();
}

// Strategic Include for related data
public async Task<Team?> GetTeamWithMembersAsync(TeamId id)
{
    return await _context.Teams
        .Include(t => t.Members)
        .ThenInclude(m => m.User)
        .FirstOrDefaultAsync(t => t.Id == id);
}
```

---

## Minimal API Conventions

### **Endpoint Organization**
Group related endpoints and use consistent patterns.

**✅ Endpoint Patterns:**
```csharp
public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/tasks")
            .WithTags("Tasks")
            .RequireAuthorization();
            
        // RESTful patterns
        group.MapGet("/", GetTasks)
            .WithName("GetTasks")
            .Produces<PagedResponse<TaskResponse>>();
            
        group.MapGet("/{id:guid}", GetTask)
            .WithName("GetTask")
            .Produces<TaskResponse>()
            .Produces(404);
            
        group.MapPost("/", CreateTask)
            .WithName("CreateTask")
            .Produces<TaskResponse>(201);
            
        group.MapPut("/{id:guid}", UpdateTask)
            .WithName("UpdateTask")
            .Produces<TaskResponse>();
            
        group.MapDelete("/{id:guid}", DeleteTask)
            .WithName("DeleteTask")
            .Produces(204);
            
        // Business operations
        group.MapPost("/{id:guid}/assign", AssignTask)
            .WithName("AssignTask")
            .RequireAuthorization(Policies.ProjectManager);
    }
}
```

### **Input Validation**
Integrate FluentValidation with minimal APIs.

**✅ Validation Integration:**
```csharp
public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
            
        RuleFor(x => x.ProjectId)
            .NotEmpty();
            
        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}

// Endpoint with validation
private static async Task<IResult> CreateTask(
    CreateTaskCommand command,
    IValidator<CreateTaskCommand> validator,
    IMediator mediator)
{
    var validationResult = await validator.ValidateAsync(command);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    
    var result = await mediator.Send(command);
    return Results.Created($"/api/v1/tasks/{result.Id}", result);
}
```

### **Error Handling Middleware**
Centralized exception handling.

**✅ Global Exception Handler:**
```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problemDetails) = exception switch
        {
            DomainException domainEx => (400, new ProblemDetails
            {
                Title = "Domain Error",
                Detail = domainEx.Message,
                Status = 400
            }),
            
            UnauthorizedAccessException => (401, new ProblemDetails
            {
                Title = "Unauthorized",
                Status = 401
            }),
            
            _ => (500, new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = 500
            })
        };
        
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
```

---

## Testing Principles

### **Test Structure (AAA Pattern)**
Arrange, Act, Assert pattern for clear test structure.

**✅ Unit Test Example:**
```csharp
public class UserTests
{
    [Fact]
    public void AssignToTeam_WithValidTeam_ShouldAddTeamMembership()
    {
        // Arrange
        var user = UserBuilder.Default().Build();
        var team = TeamBuilder.Default().Build();
        var role = TeamRole.Developer;
        
        // Act
        user.JoinTeam(team, role);
        
        // Assert
        user.TeamMemberships.Should().HaveCount(1);
        user.TeamMemberships.First().TeamId.Should().Be(team.Id);
        user.TeamMemberships.First().Role.Should().Be(role);
    }
    
    [Fact]
    public void JoinTeam_WhenAlreadyMember_ShouldThrowDomainException()
    {
        // Arrange
        var user = UserBuilder.Default().Build();
        var team = TeamBuilder.Default().Build();
        user.JoinTeam(team, TeamRole.Developer);
        
        // Act & Assert
        var action = () => user.JoinTeam(team, TeamRole.ProjectManager);
        action.Should().Throw<DomainException>()
            .WithMessage("User is already a member of this team");
    }
}
```

### **Test Builders**
Use the Builder pattern for test data creation.

**✅ Test Builders:**
```csharp
public class UserBuilder
{
    private string _name = "Test User";
    private EmailAddress _email = new("test@example.com");
    private bool _isActive = true;
    
    public static UserBuilder Default() => new();
    
    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public UserBuilder WithEmail(string email)
    {
        _email = new EmailAddress(email);
        return this;
    }
    
    public UserBuilder Inactive()
    {
        _isActive = false;
        return this;
    }
    
    public User Build()
    {
        var user = new User(_name, _email);
        if (!_isActive)
            user.Deactivate();
        return user;
    }
}
```

### **Integration Test Setup**
Use TestContainers for database integration tests.

**✅ Integration Test Base:**
```csharp
public class IntegrationTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithDatabase("dotnetskills_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();
        
    protected ApplicationDbContext DbContext { get; private set; } = null!;
    
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;
            
        DbContext = new ApplicationDbContext(options);
        await DbContext.Database.MigrateAsync();
    }
    
    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _container.DisposeAsync();
    }
}
```

### **Test Categories**
Organize tests by execution speed and scope.

**✅ Test Categories:**
```csharp
// Fast unit tests
[Trait("Category", "Unit")]
public class UserDomainTests
{
    // Tests that run in milliseconds
}

// Slower integration tests
[Trait("Category", "Integration")]
public class UserRepositoryTests : IntegrationTestBase
{
    // Tests that use database
}

// End-to-end tests
[Trait("Category", "E2E")]
public class UserEndpointTests : WebApplicationFactory<Program>
{
    // Full HTTP request/response tests
}
```

---

## Security Guidelines

### **Authentication & Authorization**
Implement JWT-based security with role-based access control.

**✅ JWT Configuration:**
```csharp
// Startup configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOnly, policy =>
        policy.RequireRole(UserRole.Admin.Value));
        
    options.AddPolicy(Policies.ProjectManager, policy =>
        policy.RequireRole(UserRole.Admin.Value, UserRole.ProjectManager.Value));
});
```

### **Input Validation**
Validate all inputs at multiple layers.

**✅ Multi-layer Validation:**
```csharp
// Domain validation
public class EmailAddress
{
    public string Value { get; }
    
    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email is required");
            
        if (!IsValidFormat(value))
            throw new ArgumentException("Invalid email format");
            
        Value = value;
    }
}

// Application validation
public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (email, ct) => 
                await userRepository.GetByEmailAsync(new EmailAddress(email)) == null)
            .WithMessage("Email already exists");
    }
}

// API validation (automatic with FluentValidation)
```

### **Secrets Management**
Never hardcode secrets in source code.

**✅ Secrets Configuration:**
```csharp
// Development: User Secrets
dotnet user-secrets set "Jwt:Key" "your-development-key"

// Production: Environment Variables or Key Vault
builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["KeyVault:Uri"]!),
    new DefaultAzureCredential());

// Accessing secrets
public class JwtService
{
    private readonly string _secretKey;
    
    public JwtService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:Key"] 
            ?? throw new InvalidOperationException("JWT key not configured");
    }
}
```

### **Data Protection**
Protect sensitive data at rest and in transit.

**✅ Data Protection:**
```csharp
// Password hashing
public class PasswordService
{
    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, 12);
        
    public bool VerifyPassword(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}

// Sensitive data encryption
[PersonalData]
public class User
{
    [ProtectedPersonalData]
    public string Email { get; private set; } = string.Empty;
}
```

---

## Performance and Scalability

### **Async/Await Best Practices**
Use async/await correctly to avoid blocking.

**✅ Proper Async Usage:**
```csharp
// Repository methods
public async Task<User?> GetByIdAsync(UserId id)
{
    return await _context.Users
        .FirstOrDefaultAsync(u => u.Id == id);
}

// Don't mix sync and async
// ❌ Bad: GetByIdAsync(id).Result
// ✅ Good: await GetByIdAsync(id)

// Use ConfigureAwait(false) in library code
public async Task<User> CreateUserAsync(CreateUserCommand command)
{
    var user = new User(command.Name, new EmailAddress(command.Email));
    await _repository.AddAsync(user).ConfigureAwait(false);
    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
    return user;
}
```

### **Database Query Optimization**
Write efficient queries and avoid N+1 problems.

**✅ Query Optimization:**
```csharp
// Use projection for read scenarios
public async Task<IReadOnlyList<TaskSummaryDto>> GetTaskSummariesAsync(ProjectId projectId)
{
    return await _context.Tasks
        .Where(t => t.ProjectId == projectId)
        .Select(t => new TaskSummaryDto
        {
            Id = t.Id.Value,
            Title = t.Title,
            Status = t.Status.Value,
            AssigneeName = t.AssignedUser != null ? t.AssignedUser.Name : null
        })
        .ToListAsync();
}

// Use Include strategically
public async Task<Project?> GetProjectWithTasksAsync(ProjectId id)
{
    return await _context.Projects
        .Include(p => p.Tasks.Where(t => t.Status != TaskStatus.Deleted))
        .FirstOrDefaultAsync(p => p.Id == id);
}

// Use Split Queries for multiple collections
public async Task<Team?> GetTeamFullAsync(TeamId id)
{
    return await _context.Teams
        .AsSplitQuery()
        .Include(t => t.Members)
        .Include(t => t.Projects)
        .ThenInclude(p => p.Tasks)
        .FirstOrDefaultAsync(t => t.Id == id);
}
```

### **Caching Strategy**
Implement appropriate caching for frequently accessed data.

**✅ Caching Implementation:**
```csharp
public class CachedUserRepository : IUserRepository
{
    private readonly IUserRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);
    
    public async Task<User?> GetByIdAsync(UserId id)
    {
        var cacheKey = $"user:{id.Value}";
        
        if (_cache.TryGetValue(cacheKey, out User? cachedUser))
            return cachedUser;
            
        var user = await _innerRepository.GetByIdAsync(id);
        
        if (user != null)
        {
            _cache.Set(cacheKey, user, _cacheExpiry);
        }
        
        return user;
    }
}
```

### **Pagination**
Implement efficient pagination for large result sets.

**✅ Pagination Implementation:**
```csharp
public record PagedRequest(int Page = 1, int PageSize = 20)
{
    public int Skip => (Page - 1) * PageSize;
}

public record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

// Repository implementation
public async Task<PagedResponse<User>> GetUsersPagedAsync(PagedRequest request)
{
    var totalCount = await _context.Users.CountAsync();
    
    var users = await _context.Users
        .OrderBy(u => u.Name)
        .Skip(request.Skip)
        .Take(request.PageSize)
        .ToListAsync();
        
    return new PagedResponse<User>(users, totalCount, request.Page, request.PageSize);
}
```

---

## Code Quality Standards

### **Naming Conventions**
Use clear, descriptive names following .NET conventions.

**✅ Naming Examples:**
```csharp
// Classes: PascalCase
public class UserManagementService { }
public class CreateUserCommandHandler { }

// Methods: PascalCase, verbs
public async Task<User> CreateUserAsync(CreateUserCommand command) { }
public bool CanAssignTask(User user, Task task) { }

// Properties: PascalCase
public string Name { get; private set; }
public TaskStatus Status { get; private set; }

// Fields: camelCase with underscore prefix
private readonly IUserRepository _userRepository;
private readonly List<TeamMember> _members = new();

// Parameters: camelCase
public User(string name, EmailAddress email) { }

// Constants: PascalCase
public const int MaxTeamMembers = 50;
public static readonly TaskStatus DefaultStatus = TaskStatus.ToDo;

// Enums: PascalCase
public enum TaskStatus { ToDo, InProgress, InReview, Done }
```

### **Documentation**
Write meaningful XML documentation for public APIs.

**✅ Documentation Examples:**
```csharp
/// <summary>
/// Represents a project task with assignment and status tracking capabilities.
/// </summary>
/// <remarks>
/// Tasks can be assigned to a single user and support one level of subtask nesting.
/// Business rules are enforced through domain methods to maintain data integrity.
/// </remarks>
public class Task : BaseEntity<TaskId>
{
    /// <summary>
    /// Assigns the task to the specified user.
    /// </summary>
    /// <param name="user">The user to assign the task to.</param>
    /// <exception cref="DomainException">
    /// Thrown when the task is already completed or the user is invalid.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="user"/> is null.
    /// </exception>
    public void AssignTo(User user)
    {
        // Implementation
    }
}
```

### **Error Handling**
Implement consistent error handling patterns.

**✅ Error Handling:**
```csharp
// Domain exceptions for business rule violations
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) 
        : base(message, innerException) { }
}

// Specific domain exceptions
public class TaskAssignmentException : DomainException
{
    public TaskAssignmentException(string message) : base(message) { }
}

// Result pattern for operations that can fail
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    
    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }
    
    private Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }
    
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);
}
```

### **Logging**
Implement structured logging with appropriate levels.

**✅ Logging Examples:**
```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly ILogger<CreateUserCommandHandler> _logger;
    
    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user with email {Email}", request.Email);
        
        try
        {
            var user = new User(request.Name, new EmailAddress(request.Email));
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Successfully created user {UserId}", user.Id);
            
            return UserMapper.ToResponse(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user with email {Email}", request.Email);
            throw;
        }
    }
}

// Structured logging with properties
_logger.LogInformation("Task {TaskId} assigned to user {UserId} by {AssignedByUserId}",
    task.Id, user.Id, currentUser.Id);
```

---

## What to Avoid

### **Anti-Patterns to Avoid**

#### **❌ Anemic Domain Model**
```csharp
// Don't: Business logic in services
public class Task
{
    public TaskStatus Status { get; set; } // Public setter
    public UserId? AssignedUserId { get; set; }
}

public class TaskService
{
    public void AssignTask(Task task, User user)
    {
        // Business logic outside domain
        task.AssignedUserId = user.Id;
    }
}
```

#### **❌ God Classes**
```csharp
// Don't: Classes that do everything
public class ProjectService
{
    public void CreateProject() { }
    public void AssignTasks() { }
    public void SendNotifications() { }
    public void GenerateReports() { }
    public void ManageUsers() { }
    public void HandlePayments() { }
}
```

#### **❌ Magic Numbers and Strings**
```csharp
// Don't: Magic values
if (user.TeamMemberships.Count > 5) // What does 5 mean?
{
    throw new Exception("Too many teams"); // What teams?
}

// Do: Named constants
public const int MaxTeamMemberships = 5;
public const string MaxTeamMembershipsErrorMessage = "User cannot join more than 5 teams";

if (user.TeamMemberships.Count > MaxTeamMemberships)
{
    throw new DomainException(MaxTeamMembershipsErrorMessage);
}
```

#### **❌ Deep Inheritance Hierarchies**
```csharp
// Don't: Deep inheritance
public class Entity : BaseEntity { }
public class Person : Entity { }
public class Employee : Person { }
public class Developer : Employee { }
public class SeniorDeveloper : Developer { }

// Do: Composition and interfaces
public class User : BaseEntity<UserId>
{
    private readonly List<Skill> _skills = new();
    private readonly List<Role> _roles = new();
}
```

#### **❌ Primitive Obsession**
```csharp
// Don't: Primitive types for domain concepts
public class User
{
    public Guid Id { get; set; } // Which Guid? User? Team? Project?
    public string Email { get; set; } // No validation
    public int Status { get; set; } // What do the numbers mean?
}

// Do: Strong typing
public class User : BaseEntity<UserId>
{
    public EmailAddress Email { get; private set; }
    public UserStatus Status { get; private set; }
}
```

#### **❌ Repository Leakage**
```csharp
// Don't: Expose IQueryable
public interface IUserRepository
{
    IQueryable<User> GetUsers(); // Leaks data access concerns
}

// Do: Specific methods
public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetActiveUsersAsync();
    Task<PagedResponse<User>> GetUsersPagedAsync(PagedRequest request);
    Task<User?> GetByEmailAsync(EmailAddress email);
}
```

#### **❌ Service Locator Pattern**
```csharp
// Don't: Service locator
public class UserService
{
    public void CreateUser()
    {
        var repository = ServiceLocator.Get<IUserRepository>(); // Hidden dependency
        var emailService = ServiceLocator.Get<IEmailService>();
        // ...
    }
}

// Do: Dependency injection
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    
    public UserService(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }
}
```

---

## Useful Resources

### **.NET 9 & C# 13**
- [What's new in .NET 9](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9)
- [C# 13 Language Features](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13)

### **Clean Architecture & DDD**
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design by Eric Evans](https://domainlanguage.com/ddd/)
- [Implementing Domain-Driven Design by Vaughn Vernon](https://kalele.io/really-understanding-ddd/)

### **Entity Framework Core**
- [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [EF Core Performance Guidelines](https://docs.microsoft.com/en-us/ef/core/performance/)

### **Testing**
- [Test-Driven Development by Kent Beck](https://www.amazon.com/Test-Driven-Development-Kent-Beck/dp/0321146530)
- [xUnit Documentation](https://xunit.net/docs/getting-started/netcore/cmdline)
- [FluentAssertions Documentation](https://fluentassertions.com/introduction)

### **Security**
- [ASP.NET Core Security Documentation](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)

---

> **Note:**  
> These principles are living guidelines. When deviating from these patterns, document the reasoning and discuss with the team. The goal is maintainable, secure, and performant code that serves the business needs effectively.

---

**Last Updated:** July 31, 2025  
**Version:** 1.0  
**Project:** DotNetSkills - Project Management API
