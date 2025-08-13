# Developer Guide

*Daily workflows, commands, and development practices for DotNetSkills*

## Essential Commands Reference

### Build & Test
```bash
# Build and test (most common workflow)
make build && make test

# Individual commands
dotnet restore                    # Restore packages
dotnet build                     # Build solution  
dotnet test                      # Run all tests
dotnet test --collect:"XPlat Code Coverage"  # With coverage
```

### Local Development
```bash
# Run API locally (with hot reload)
dotnet run --project src/DotNetSkills.API
# Available at: https://localhost:5001

# Watch mode for continuous building
dotnet watch run --project src/DotNetSkills.API
```

### Docker Environment
```bash
# Start/stop environment
make up                          # Start API + Database
make full                        # Start API + Database + Redis
make down                        # Stop everything

# Environment management
make status                      # Check health of all services
make logs                        # Follow API logs
make health                      # Quick health check (returns 200/500)
make ps                          # Show running containers

# Debugging
docker compose logs api          # API logs only
docker compose logs db           # Database logs
docker compose restart api      # Restart API service
```

### Database Operations
```bash
# Entity Framework migrations
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Create new migration
dotnet ef migrations add MigrationName --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Generate SQL script for migration
dotnet ef migrations script --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

## Development Workflows

### 🔄 Daily Development Loop
```bash
# 1. Start environment
make up

# 2. Verify everything is working
make health

# 3. Make your changes
# ... code changes ...

# 4. Test changes
make test

# 5. Run locally if needed
dotnet run --project src/DotNetSkills.API
```

### 🆕 Adding a New Feature
```bash
# 1. Create feature branch
git checkout -b feature/your-feature-name

# 2. Follow the architecture pattern:
#    - Domain: Entities, Value Objects, Events
#    - Application: Commands/Queries, Handlers, DTOs
#    - Infrastructure: Repository implementations
#    - API: Endpoints

# 3. Run tests frequently
dotnet test

# 4. Create/update migrations if needed
dotnet ef migrations add AddYourFeature --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# 5. Test the complete flow
make up && make health
```

### 🧪 Testing Workflow
```bash
# Run specific test projects
dotnet test tests/DotNetSkills.Domain.UnitTests
dotnet test tests/DotNetSkills.Application.UnitTests

# Run tests with filters
dotnet test --filter "Category=Unit"
dotnet test --filter "TestMethod=CreateUser*"

# Debug failing tests
dotnet test --logger "console;verbosity=detailed"
```

## IDE Configuration

### VS Code Setup
**Recommended Extensions:**
- C# Dev Kit
- REST Client (for testing API endpoints)
- Docker
- GitLens

**Settings (.vscode/settings.json):**
```json
{
  "dotnet.defaultSolution": "DotNetSkills.sln",
  "files.exclude": {
    "**/bin": true,
    "**/obj": true
  }
}
```

### Debugging Configuration
**launch.json for API debugging:**
```json
{
  "name": "Launch API",
  "type": "coreclr",
  "request": "launch",
  "program": "${workspaceFolder}/src/DotNetSkills.API/bin/Debug/net9.0/DotNetSkills.API.dll",
  "args": [],
  "cwd": "${workspaceFolder}/src/DotNetSkills.API",
  "env": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```

## Common Development Patterns

### Adding a New Bounded Context Feature

**1. Domain Layer (src/DotNetSkills.Domain/[Context]/)**
```csharp
// Entity
public class YourEntity : BaseEntity<YourEntityId>
{
    // Domain logic here
}

// Value Object  
public record YourEntityId(Guid Value) : IStronglyTypedId;

// Domain Event
public record YourEntityCreatedDomainEvent(YourEntityId Id) : BaseDomainEvent;
```

**2. Application Layer (src/DotNetSkills.Application/[Context]/)**
```csharp
// Command
public record CreateYourEntityCommand(...) : IRequest<YourEntityResponse>;

// Handler
public class CreateYourEntityCommandHandler : IRequestHandler<CreateYourEntityCommand, YourEntityResponse>
{
    // Implementation using repository
}

// Validator
public class CreateYourEntityCommandValidator : AbstractValidator<CreateYourEntityCommand>
{
    // Validation rules
}
```

**3. Infrastructure Layer (src/DotNetSkills.Infrastructure/)**
```csharp
// Repository
public class YourEntityRepository : BaseRepository<YourEntity, YourEntityId>, IYourEntityRepository
{
    // Custom queries
}

// Register in DependencyInjection.cs
services.AddScoped<IYourEntityRepository, YourEntityRepository>();
```

**4. API Layer (src/DotNetSkills.API/)**
```csharp
// Endpoints group
public static class YourEntityEndpoints
{
    public static RouteGroupBuilder MapYourEntityEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateYourEntityCommand command, IMediator mediator) =>
            await mediator.Send(command));
        return group;
    }
}
```

### Repository Pattern Usage
```csharp
// In Application handlers - always use interfaces
public class YourHandler
{
    private readonly IYourRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<YourResponse> Handle(YourCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id, ct);
        // ... business logic
        await _unitOfWork.SaveChangesAsync(ct);
        return new YourResponse(...);
    }
}
```

## Environment Variables

### Development (.env or appsettings.Development.json)
```bash
# Database
DOTNETSKILLS_Database__ConnectionString="Server=(localdb)\\mssqllocaldb;Database=DotNetSkills;Trusted_Connection=true;"

# JWT (development only)
DOTNETSKILLS_Jwt__Enabled=true
DOTNETSKILLS_Jwt__SigningKey="YourDevelopmentKey"

# Swagger
DOTNETSKILLS_Swagger__Enabled=true

# Performance
DOTNETSKILLS_Performance__SlowRequestThresholdMs=500
```

### Docker Environment
```bash
# Auto-run migrations
RUN_MIGRATIONS=true

# Database settings
DB_SA_PASSWORD=DevPassword123
DB_NAME=DotNetSkills_Dev

# Performance monitoring
PERF_SLOW_THRESHOLD=500
PERF_ENABLE_METRICS=true
```

## Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| `dotnet: command not found` | Install [.NET 9 SDK](https://dotnet.microsoft.com/download) |
| Port 5001/8080 in use | `pkill -f dotnet` or `API_PORT=8081 make up` |
| Database connection failed | Check SQL Server is running: `make status` |
| EF migrations fail | Ensure connection string is correct |
| Tests fail randomly | Stop Docker containers: `make down` |
| Build errors after git pull | `dotnet clean && dotnet restore && dotnet build` |

### Debugging Steps
```bash
# 1. Check environment health
make status

# 2. Check API logs
make logs

# 3. Test database connection
docker compose exec db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P DevPassword123 -C -Q "SELECT 1"

# 4. Restart everything
make down && make up

# 5. Check health endpoint
curl http://localhost:8080/health
```

## Performance Tips

### Development Performance
- Use `dotnet watch` for hot reload during development
- Use cached repositories for frequently accessed data
- Run tests with `--no-build` flag after initial build

### Docker Performance
- Use `make up` instead of `docker compose up -d` (uses optimized Makefile)
- Restart only specific services: `docker compose restart api`
- Use `--profile cache` for Redis when needed

## Code Quality Standards

### Before Committing
```bash
# 1. Build and test
make build && make test

# 2. Check for lint/format issues (if configured)
dotnet format

# 3. Ensure migrations are up to date
dotnet ef migrations list --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

### Code Review Checklist
- [ ] Follows existing patterns and naming conventions
- [ ] Tests added for new functionality
- [ ] Database migrations included if schema changed
- [ ] Documentation updated if needed
- [ ] No hardcoded secrets or connection strings
- [ ] Error handling follows exception-only contract

## Additional Resources

- **Architecture Details**: [Architecture Guide](ARCHITECTURE.md)
- **Testing Practices**: [Testing Guide](TESTING-GUIDE.md)
- **Docker Operations**: [Docker Guide](../Docker/README.md)
- **Troubleshooting**: [Troubleshooting Guide](TROUBLESHOOTING.md)
- **Coding Standards**: [DotNet Coding Principles](DotNet%20Coding%20Principles.md)

---

**💡 Pro Tip**: Use `make help` to see all available Makefile commands at any time.