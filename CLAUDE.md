# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Essential Commands

### Build & Test
```bash
# Primary development workflow
make build && make test
dotnet restore                    # Restore packages
dotnet build                     # Build solution
dotnet test                      # Run all tests
dotnet test --collect:"XPlat Code Coverage"  # With coverage

# Individual test projects
dotnet test tests/DotNetSkills.Domain.UnitTests
dotnet test tests/DotNetSkills.Application.UnitTests
dotnet test tests/DotNetSkills.Infrastructure.UnitTests
dotnet test tests/DotNetSkills.API.UnitTests
```

### Local Development
```bash
# Run API locally with hot reload
dotnet run --project src/DotNetSkills.API
# Available at: https://localhost:5001

# Watch mode for continuous building
dotnet watch run --project src/DotNetSkills.API
```

### Docker Environment
```bash
# Environment management
make up                          # Start API + Database
make down                        # Stop everything
make status                      # Check health of all services
make health                      # Quick health check (returns 200)
make logs                        # Follow API logs

# For development with caching
make full                        # Start API + Database + Redis
make cache                       # Start only Redis
```

### Database Operations
```bash
# Entity Framework migrations
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Create new migration
dotnet ef migrations add MigrationName --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

## Architecture Overview

DotNetSkills follows **Clean Architecture** with **Domain-Driven Design** patterns. The solution is organized into 4 main bounded contexts:

### Project Structure
```
src/
├── DotNetSkills.API/              # Minimal APIs, middleware, authentication
├── DotNetSkills.Application/      # CQRS handlers, DTOs, validation
├── DotNetSkills.Domain/           # Entities, value objects, domain events
└── DotNetSkills.Infrastructure/   # EF Core, repositories, external services

tests/
├── DotNetSkills.API.UnitTests/
├── DotNetSkills.Application.UnitTests/
├── DotNetSkills.Domain.UnitTests/
└── DotNetSkills.Infrastructure.UnitTests/
```

### Bounded Contexts
1. **UserManagement** - User accounts, authentication, roles
2. **TeamCollaboration** - Teams and member management
3. **ProjectManagement** - Project lifecycle and organization
4. **TaskExecution** - Task assignment and tracking

### Key Patterns
- **CQRS with MediatR**: Commands for writes, Queries for reads
- **Repository Pattern**: Data access with caching decorators
- **Domain Events**: Decoupled communication between aggregates
- **Exception-Only Contract**: Uses exceptions for control flow (no Result<T> wrappers)
- **Strongly-Typed IDs**: Type-safe identifiers throughout

## Development Patterns

### Adding New Features
When adding features, follow the architectural layers:

1. **Domain Layer** (`src/DotNetSkills.Domain/[Context]/`):
   - Create entities, value objects, and domain events
   - Implement business logic in entities
   - Add domain services for complex rules

2. **Application Layer** (`src/DotNetSkills.Application/[Context]/`):
   - Create command/query classes with MediatR
   - Implement handlers that orchestrate domain operations
   - Add validation using FluentValidation
   - Create DTOs and mapping profiles

3. **Infrastructure Layer** (`src/DotNetSkills.Infrastructure/`):
   - Implement repository interfaces
   - Add EF Core entity configurations
   - Register dependencies in DependencyInjection.cs

4. **API Layer** (`src/DotNetSkills.API/Endpoints/[Context]/`):
   - Create minimal API endpoints
   - Group endpoints by bounded context
   - Apply authorization policies

### Entity Framework Patterns
- Use strongly-typed IDs converted to GUIDs
- Configure entities in separate configuration files
- Avoid shadow foreign keys (explicit FK properties)
- Use value converters for domain value objects

### Testing Strategy
- **Domain Tests**: Focus on business logic and entity behavior
- **Application Tests**: Test handlers, validation, and mapping
- **Infrastructure Tests**: Repository logic and data access
- **API Tests**: Endpoint behavior and middleware

## Key Files and Locations

### Configuration
- `src/DotNetSkills.API/appsettings.json` - Base configuration
- `src/DotNetSkills.API/appsettings.Development.json` - Development settings
- `docker-compose.yml` - Container orchestration
- `Makefile` - Development commands

### Core Architecture Files
- `src/DotNetSkills.Domain/Common/` - Base domain abstractions
- `src/DotNetSkills.Application/Common/` - Application abstractions and behaviors
- `src/DotNetSkills.Infrastructure/Persistence/` - EF Core configuration

### Important Conventions
- Use `BaseEntity<TId>` for all entities
- Implement `IStronglyTypedId` for all ID value objects
- Follow naming pattern: `[Entity][Action]Command/Query`
- Place validation in `[Command/Query]Validator` classes
- Use `[Entity]Response` DTOs for API responses

### MediatR Pipeline Behaviors (execution order)
1. LoggingBehavior - Captures all operations
2. ValidationBehavior - Short-circuits invalid requests
3. PerformanceBehavior - Measures execution time
4. DomainEventDispatchBehavior - Dispatches events after success

## Technology Stack
- **.NET 9** with nullable reference types
- **Entity Framework Core** with SQL Server
- **MediatR** for CQRS implementation
- **JWT Authentication** with role-based authorization
- **AutoMapper** for entity-DTO mapping
- **FluentValidation** for input validation
- **xUnit + FluentAssertions** for testing
- **Docker** for containerization

## Development Workflow

### Daily Development
1. Start environment: `make up`
2. Verify health: `make health`
3. Make changes following architectural patterns
4. Run tests: `make test`
5. For database changes: Create migrations with EF commands

### Before Committing
1. Build and test: `make build && make test`
2. Ensure migrations are created for schema changes
3. Follow existing patterns and naming conventions
4. Verify no hardcoded secrets or connection strings

### Common Issues
- Port conflicts: Use `API_PORT=8081 make up`
- Database issues: Check with `make status`
- Build errors after git pull: `dotnet clean && dotnet restore && dotnet build`
- Failed tests: Ensure Docker containers are running

## Quick Reference

### Health Checks
- API: http://localhost:8080/health
- Swagger: http://localhost:8080/swagger
- Database: SQL Server on localhost:1433

### Test Filters
```bash
dotnet test --filter "Category=Unit"
dotnet test --filter "TestMethod=CreateUser*"
dotnet test --filter "FullyQualifiedName~UserTests"
```

### Debugging
```bash
make logs                        # View API logs
make status                      # Check all services
docker compose logs db           # Database logs
docker compose restart api      # Restart API only
```