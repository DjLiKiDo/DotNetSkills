# DotNetSkills

[![Build Status](https://img.shields.io/badge/build-passing-green?style=flat-square)](https://github.com/DjLiKiDo/DotNetSkills)
[![.NET 9](https://img.shields.io/badge/.NET-9.0-5027D5?style=flat-square&logo=.net)](https://dotnet.microsoft.com)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-blue?style=flat-square)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![Domain-Driven Design](https://img.shields.io/badge/Design-DDD-orange?style=flat-square)](https://domainlanguage.com/ddd/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)](LICENSE)

> A modern, secure, and scalable project management API built with .NET 9, demonstrating Clean Architecture and Domain-Driven Design principles.

## Overview

DotNetSkills is a comprehensive project management API designed to showcase enterprise-grade development practices. Built with .NET 9 and following Clean Architecture principles, it provides a solid foundation for teams to organize and execute their projects efficiently.

The API serves as both a functional project management tool and a demonstration of modern .NET development techniques, including Domain-Driven Design, JWT authentication, and comprehensive testing strategies.

## Features

- **Clean Architecture**: Well-organized layers with proper dependency inversion
- **Domain-Driven Design**: Rich domain models with business logic encapsulated in entities
- **JWT Authentication**: Stateless security with role-based access control
- **Minimal APIs**: Lean, performance-focused endpoint implementations
- **Entity Framework Core**: Modern data access with SQL Server
- **Comprehensive Testing**: Unit, integration, and end-to-end test coverage
- **Domain Events**: Decoupled communication between aggregates
- **Strong Typing**: Value objects and strongly-typed identifiers throughout

## Architecture

The solution follows Clean Architecture principles with these layers:

```
┌─────────────────────────────────────────────┐
│                 API Layer                   │
│        (Controllers, Middleware)            │
└─────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────┐
│             Application Layer               │
│     (Use Cases, DTOs, Interfaces)           │
└─────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────┐
│              Domain Layer                   │
│    (Entities, Value Objects, Events)        │
└─────────────────────────────────────────────┘
                      ▲
┌─────────────────────────────────────────────┐
│            Infrastructure Layer             │
│   (Repositories, Database, External APIs)   │
└─────────────────────────────────────────────┘
```

### Project Structure

```
src/
├── DotNetSkills.API/              # Minimal APIs, authentication, validation
├── DotNetSkills.Application/      # Use cases, DTOs, service interfaces
├── DotNetSkills.Domain/           # Core business logic and domain models
└── DotNetSkills.Infrastructure/   # Database, repositories, external services

tests/
├── DotNetSkills.API.UnitTests/
├── DotNetSkills.Application.UnitTests/
├── DotNetSkills.Domain.UnitTests/
└── DotNetSkills.Infrastructure.UnitTests/
```

## Core Entities

- **User**: Core entity with role-based permissions (Admin, ProjectManager, Developer, Viewer)
- **Team**: Collection of users working together with many-to-many relationships
- **Project**: Belongs to one team, contains tasks with status management
- **Task**: Single-user assignment with one-level subtask nesting support

## Technology Stack

- **.NET 9**: Latest framework with nullable reference types enabled
- **Entity Framework Core**: Modern ORM with SQL Server provider
- **Minimal APIs**: Lightweight, performance-focused endpoints
- **JWT Authentication**: Stateless security implementation
- **AutoMapper**: Entity ↔ DTO transformations
- **FluentValidation**: Comprehensive input validation
- **xUnit + FluentAssertions**: Testing framework and assertions
- **Testcontainers**: Integration testing with real databases
- **Serilog**: Structured logging
- **Docker**: Containerization support

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express/LocalDB)
- [Docker](https://www.docker.com/products/docker-desktop) (optional, for containerized development)

### Local Development

1. **Clone the repository:**
   ```bash
   git clone https://github.com/DjLiKiDo/DotNetSkills.git
   cd DotNetSkills
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the solution:**
   ```bash
   dotnet build
   ```

4. **Update connection strings** in `src/DotNetSkills.API/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DotNetSkills;Trusted_Connection=true;"
     }
   }
   ```

5. **Run database migrations:**
   ```bash
   dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
   ```

6. **Start the API:**
   ```bash
   dotnet run --project src/DotNetSkills.API
   ```

The API will be available at `https://localhost:5001` with Swagger documentation at `https://localhost:5001/swagger`.

### Using Docker

For a complete development environment with SQL Server:

1. **Start the development stack:**
   ```bash
   docker-compose up -d
   ```

2. **Run migrations:**
   ```bash
   dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
   ```

## API Endpoints

### Authentication
- `POST /api/v1/auth/login` - Authenticate user and get JWT token
- `POST /api/v1/auth/register` - Create new user (Admin only)

### User Management
- `GET /api/v1/users` - List all users
- `GET /api/v1/users/{id}` - Get user by ID
- `PUT /api/v1/users/{id}` - Update user
- `DELETE /api/v1/users/{id}` - Delete user (Admin only)

### Team Management
- `GET /api/v1/teams` - List all teams
- `POST /api/v1/teams` - Create new team
- `PUT /api/v1/teams/{id}` - Update team
- `DELETE /api/v1/teams/{id}` - Delete team
- `POST /api/v1/teams/{id}/members` - Add team member
- `DELETE /api/v1/teams/{id}/members/{userId}` - Remove team member

### Project Management
- `GET /api/v1/projects` - List all projects
- `POST /api/v1/projects` - Create new project
- `PUT /api/v1/projects/{id}` - Update project
- `DELETE /api/v1/projects/{id}` - Delete project
- `GET /api/v1/projects/{id}` - Get project details

### Task Management
- `GET /api/v1/projects/{projectId}/tasks` - List project tasks
- `POST /api/v1/projects/{projectId}/tasks` - Create new task
- `PUT /api/v1/tasks/{id}` - Update task
- `PUT /api/v1/tasks/{id}/assign` - Assign task to user
- `PUT /api/v1/tasks/{id}/status` - Update task status
- `DELETE /api/v1/tasks/{id}` - Delete task

## Testing

Run all tests:
```bash
dotnet test
```

Run specific test project:
```bash
dotnet test tests/DotNetSkills.Domain.UnitTests
```

Run tests with coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Test Organization

- **Unit Tests**: Fast tests for business logic and domain rules
- **Integration Tests**: API endpoints with real database using Testcontainers
- **Test Builders**: Builder pattern for maintainable test data creation
- **80% Coverage Minimum**: Focus on Domain and Application layers

## Security

- **JWT Authentication**: Stateless token-based authentication
- **Role-based Authorization**: Admin, ProjectManager, Developer, Viewer roles
- **Input Validation**: Comprehensive validation using FluentValidation
- **SQL Injection Protection**: Parameterized queries through EF Core
- **Password Hashing**: BCrypt with unique salts

> [!NOTE]
> This MVP version does not include user self-registration or password recovery features. All user creation is admin-only.

## Exception Handling

The API uses a standardized exception handling system with specific HTTP status codes and error formats:

| Exception Type | HTTP Status | Error Code | Usage | Example |
|---|---|---|---|---|
| `NotFoundException` | 404 | `not_found` | Resource not found by ID | Task with ID 123 not found |
| `BusinessRuleViolationException` | 409 | `business_rule_violation` | Domain business rule violations | Cannot assign task to inactive user |
| `ValidationException` (Application) | 400 | `validation_error` | Application-level validation failures | Invalid email format |
| `ValidationException` (FluentValidation) | 400 | `validation_error` | Request validation failures | Required field missing |
| `DomainException` | 400 | `domain_rule_violation` | Domain invariant violations | Task priority cannot be null |
| `ApplicationExceptionBase` (custom) | varies | custom | Application-specific errors | Custom error codes as needed |
| Generic exceptions | 500 | `internal_server_error` | Unexpected errors | Database connection failed |

### Error Response Format

All errors return standardized ProblemDetails format with additional metadata:

```json
{
  "title": "Not Found",
  "detail": "Task with ID '123e4567-e89b-12d3-a456-426614174000' was not found",
  "status": 404,
  "extensions": {
    "errorCode": "not_found"
  }
}
```

### Validation Pipeline

The system uses a multi-layered validation approach:

1. **Request Validation**: FluentValidation rules applied via `ValidationBehavior`  
2. **Business Rules**: Domain entity validation during state changes
3. **Application Rules**: Cross-cutting concerns in command/query handlers

The pipeline order ensures validation failures are caught early and consistently formatted.

#### Application Layer Contract (ADR-0001)

The Application layer now uses an **exception-only contract** for signaling failures (see `docs/adr/0001-result-handling-decision.md`). The previously used `Result` / `Result<T>` wrapper pattern has been deprecated and SHOULD NOT be used in new code. All `IRequest` implementations return their successful DTO/primitive directly:

```csharp
// Before (deprecated)
public sealed record CreateUserCommand(...) : IRequest<Result<UserResponse>>;

// After (current contract)
public sealed record CreateUserCommand(...) : IRequest<UserResponse>;

// Handler excerpt
public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken ct)
{
   // Throw for failures
   if (!emailIsUnique)
      throw new DomainException("Email already exists");

   return new UserResponse(...); // Success path
}
```

Guidelines:
1. Validation failures are surfaced as `FluentValidation.ValidationException` (thrown by the pipeline behavior before the handler runs).
2. Domain rule or invariant violations throw `DomainException` (inside aggregates or handler guards).
3. Cross-cutting / orchestration issues throw a dedicated `ApplicationException` (or derive from `ApplicationExceptionBase`).
4. Handlers MUST NOT return null for successes (use nullable return types only when absence is a legitimate, documented outcome, e.g., `UserResponse?`).
5. Endpoints rely on global exception middleware to translate exceptions to consistent ProblemDetails responses—DO NOT catch and convert to ad‑hoc error objects in handlers.

Migration Impact: Legacy handlers using `Result` were refactored; the `Result` / `Result<T>` utilities and related extension methods have been fully removed (see ADR-0001). New contributions MUST NOT reintroduce wrapper result patterns.

### HTTP Request Pipeline

The API implements a carefully ordered middleware pipeline for optimal request processing:

```
Request → CorrelationId → ExceptionHandling → HTTPS → CORS → Authentication → Authorization → Endpoints
```

#### Correlation ID Middleware

The `CorrelationIdMiddleware` provides request tracing capabilities:

- **Auto-generation**: Creates a unique correlation ID if not provided in request headers
- **Header Support**: Accepts incoming `X-Correlation-Id` header from clients
- **Response Headers**: Always includes `X-Correlation-Id` in response headers
- **Error Integration**: Correlation IDs are included in ProblemDetails for error responses
- **Logging Context**: Adds correlation ID to logging scope for structured logging

**Usage Example:**
```http
# Request with correlation ID
GET /api/v1/users
X-Correlation-Id: my-custom-trace-123

# Response includes the same ID
HTTP/1.1 200 OK
X-Correlation-Id: my-custom-trace-123
```

**Error Response Integration:**
```json
{
  "title": "Validation Failed",
  "status": 400,
  "extensions": {
    "errorCode": "validation_failed",
    "correlationId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    "requestId": "0HMEQ9H6J4D1S:00000001",
    "timestamp": "2025-01-10T15:30:00.000Z"
  }
}

## Development Guidelines

The project follows strict coding principles documented in [DotNet Coding Principles](docs/DotNet%20Coding%20Principles.md), including:

- **SOLID Principles**: Single responsibility, open/closed, Liskov substitution, interface segregation, and dependency inversion
- **Clean Architecture**: Proper layer separation with dependency inversion
- **Domain-Driven Design**: Rich domain models with business logic in entities
- **Testing Best Practices**: Comprehensive test coverage with clear test organization

## Documentation

- [Product Requirements Document](docs/prd.md) - Complete specifications and business requirements
- [Implementation Plan](plan/feature-dotnetskills-mvp-1.md) - Detailed development roadmap
- [DotNet Coding Principles](docs/DotNet%20Coding%20Principles.md) - Project-specific coding standards
- [Architecture Decision Records](docs/adr) - Key architectural decisions (see ADR-0001 for error handling contract)

## Roadmap

Current implementation status can be tracked in the [MVP Implementation Plan](plan/feature-dotnetskills-mvp-1.md):

- ✅ **Phase 1**: Domain models and infrastructure setup
- 🚧 **Phase 2**: JWT authentication and user management
- 📋 **Phase 3**: Core CRUD operations for all entities
- 📋 **Phase 4**: Testing and CI/CD pipeline

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For questions or support, please open an issue on GitHub.
