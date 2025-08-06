# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Build & Test
```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/DotNetSkills.Domain.UnitTests

# Start API in development mode
dotnet run --project src/DotNetSkills.API

# Hot reload during development
dotnet watch run --project src/DotNetSkills.API
```

### Database Operations
```bash
# Apply migrations
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Add new migration
dotnet ef migrations add MigrationName --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

## Architecture Overview

This codebase implements **Clean Architecture** with **Domain-Driven Design** patterns:

```
API Layer (Minimal APIs) → Application Layer (CQRS) → Domain Layer (Rich Models) ← Infrastructure Layer (EF Core)
```

### Project Structure
- **DotNetSkills.API**: Minimal APIs, endpoints, middleware
- **DotNetSkills.Application**: CQRS commands/queries, DTOs, validation
- **DotNetSkills.Domain**: Entities, value objects, domain events, business rules
- **DotNetSkills.Infrastructure**: EF Core, repositories, external services

### Key Patterns
- **CQRS with MediatR**: Commands for writes, Queries for reads
- **Rich Domain Models**: Business logic encapsulated in entities
- **Unit of Work**: Transaction management across repositories
- **Domain Events**: Cross-aggregate communication
- **Repository Pattern**: Data access abstraction

## Domain Structure

The system has **4 bounded contexts**:

1. **User Management**: Users, roles, authentication
2. **Team Collaboration**: Teams, memberships, collaboration
3. **Project Management**: Projects, lifecycle management
4. **Task Execution**: Tasks, assignments, tracking

Each context follows the same structure:
```
BoundedContext/
├── Contracts/     # DTOs, repository interfaces
├── Features/      # Commands/Queries by use case
└── Mappings/      # AutoMapper profiles
```

## Technology Stack
- **.NET 9** with C# 13 features
- **Entity Framework Core 9** with SQL Server
- **MediatR** for CQRS implementation
- **FluentValidation** for input validation
- **AutoMapper** for DTO mapping
- **xUnit, FluentAssertions, Moq** for testing

## Development Guidelines

### Entity Framework
- All entities inherit from `BaseEntity<TId>` or `AggregateRoot<TId>`
- Value objects are used for strongly-typed IDs
- Entity configurations use `IEntityTypeConfiguration<T>`
- Domain events are automatically dispatched on SaveChanges

### CQRS Implementation
- Commands modify state and return `Result<T>`
- Queries return data without side effects
- Each feature has its own folder with Command/Query/Handler/Validator
- Use `PagedRequest/PagedResponse` for list operations

### Domain Models
- Business rules are enforced in entity methods
- Use `Ensure.BusinessRule()` for validation
- Domain events are raised using `RaiseDomainEvent()`
- Value objects are immutable records

### Repository Pattern
- Each aggregate root has a corresponding repository interface
- Repositories are accessed through `IUnitOfWork`
- Use async methods for all database operations
- Follow naming convention: `GetByIdAsync`, `AddAsync`, etc.

### API Endpoints
- Use Minimal APIs with extension methods for organization
- Group endpoints by bounded context
- Apply validation middleware and error handling
- Return appropriate HTTP status codes

## Testing Strategy
- **Unit Tests**: Domain logic and business rules
- **Integration Tests**: Repository and database operations
- **API Tests**: End-to-end HTTP scenarios
- Use test builders for complex object creation
- Follow AAA pattern (Arrange, Act, Assert)

## Configuration
- `appsettings.json` for production settings
- `appsettings.Development.json` for local development
- Connection strings configured per environment
- Swagger available at `/swagger` in development

## Branch Strategy
- **main**: Production-ready code
- Feature branches follow naming convention
- Current development happens on feature branches like `janitor-plan-c3`