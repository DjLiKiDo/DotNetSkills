# Copilot Instructions — DotNetSkills (.NET 9, Clean Architecture)

Use this as your operating manual for this repo. Keep changes aligned with Clean Architecture and DDD.

## Big picture
- Layers and deps: API → Application → Domain; Infrastructure → Application → Domain (no outward references from inner layers).
- Projects (see `src/`):
  - `DotNetSkills.API`: Minimal APIs, auth, middleware, options, Swagger.
  - `DotNetSkills.Application`: CQRS (MediatR), validators, mappings, behaviors.
  - `DotNetSkills.Domain`: Entities, value objects, domain events/exceptions.
  - `DotNetSkills.Infrastructure`: EF Core DbContext, repositories, caching, UoW.
- Read docs: `docs/DotNet Coding Principles.md` for project-specific patterns.

## Critical extension points
- API DI: `src/DotNetSkills.API/DependencyInjection.cs`
  - `AddApiServices(configuration)` wires: Application, Infrastructure, ProblemDetails, JSON (camelCase + `JsonStringEnumConverter`), Swagger, CORS, HealthChecks, ResponseCompression.
  - JWT is conditional via `JwtOptions.Enabled` (see `Configuration/Options`). When enabled, both `AddAuthentication` and `AddAuthorization()` are registered.
- App pipeline: `src/DotNetSkills.API/Program.cs`
  - Uses `UseExceptionHandling()` middleware, optional Swagger, HTTPS redirection, CORS, and conditional `UseAuthentication()`/`UseAuthorization()`.
  - Endpoint groups mapped by bounded context: `MapUserManagementEndpoints`, `MapTeamCollaborationEndpoints`, `MapProjectManagementEndpoints`, `MapTaskExecutionEndpoints`.
- Application DI: `src/DotNetSkills.Application/DependencyInjection.cs`
  - MediatR registered with pipeline behaviors in this order: Logging → Performance → Validation → DomainEventDispatch.
  - AutoMapper and FluentValidation auto-discovery added.
- Infrastructure DI: `src/DotNetSkills.Infrastructure/DependencyInjection.cs`
  - SQL Server via `ApplicationDbContext`; options validated via `DatabaseOptions`.
  - Repositories are wrapped with memory-cached decorators (e.g., `CachedUserRepository`).
  - `IUnitOfWork` and `IDomainEventDispatcher` provided.
- Global usings: `src/DotNetSkills.API/GlobalUsings.cs` centralizes layer imports and aliases (e.g., `DomainTaskStatus`). Prefer adding new using scopes here.

## Endpoint pattern (Minimal APIs)
- Endpoints live under `src/DotNetSkills.API/Endpoints/<BoundedContext>/*Endpoints.cs` and are grouped via extension methods (e.g., `UserEndpoints.MapUserEndpoints`).
- Typical shape:
  - `var group = app.MapGroup("/api/v1/<resource>").RequireAuthorization();`
  - Map methods with `.WithSummary()/.Produces<>()` and optional `.RequireAuthorization("PolicyName")`.
  - Handlers accept `IMediator` and send commands/queries. Return `Results.Ok/Created/...`.
  - Rely on global `ExceptionHandlingMiddleware` for errors; prefer throwing `DomainException`/`ValidationException` over try/catch in endpoints.

Example (from `UserEndpoints`): `.RequireAuthorization("AdminOnly")` and `await mediator.Send(command)`.

## Auth, options, and configuration
- Options pattern: `src/DotNetSkills.API/Configuration/Options/*` (e.g., `JwtOptions`, `CorsOptions`, `SwaggerOptions`) with validators under `Options/Validators/` and loaded in `AddApiServices`.
- Auth is JWT-based when enabled; policies are added via `services.AddAuthorization(...)` in API. Endpoints currently reference string policies (e.g., "AdminOnly"). Define policy constants and mappings in API (not Application/Infrastructure).

## Data and repositories
- Application depends on repository interfaces; Infrastructure provides EF Core implementations and caches. Do not use `ApplicationDbContext` in Application.
- For queries, prefer projection DTOs and pagination types from `Application.Common.Models`.
- Domain events are dispatched via the Application behavior/dispatcher; raise them from aggregates.

## Conventions and types
- Strongly-typed IDs and enums in Domain (`*.ValueObjects`, `*.Enums`). JSON uses camelCase and serializes enums as strings.
- Validation: FluentValidation in Application. Behaviors enforce validation before handlers.
- Mapping: AutoMapper profiles in Application (add per bounded context).

## Build, run, test (macOS zsh)
```bash
# Restore & build
dotnet restore
dotnet build

# Run API
dotnet run --project src/DotNetSkills.API

# EF migrations (uses Infrastructure for migrations, API as startup)
dotnet ef database update \
  --project src/DotNetSkills.Infrastructure \
  --startup-project src/DotNetSkills.API

# Tests
dotnet test
```

## Where to add new features
- Endpoint → `src/DotNetSkills.API/Endpoints/<Context>/*Endpoints.cs`
- Command/query + validator + profile → `src/DotNetSkills.Application/<Context>/Features/...`
- Repository interface (Application) → same context; implementation (Infrastructure) → `src/DotNetSkills.Infrastructure/Repositories`
- Options/policies/middleware → API layer only

Tip: Keep dependencies pointing inward, use MediatR for use-cases, raise domain events in aggregates, and let the exception middleware produce ProblemDetails.
