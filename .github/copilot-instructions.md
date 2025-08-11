
# DotNetSkills Copilot Instructions

> Read this first. For details, see `docs/`, `README.md`, and `CLAUDE.md`.

## Architecture & Layering
- Clean Architecture: API → Application → Domain; Infrastructure → Application → Domain. Inner layers never reference outer ones.
- Domain layer is pure C# (no EF/ASP.NET dependencies). All business logic lives in domain entities, value objects, and domain events.
- Use MediatR for all use cases. Endpoints never orchestrate services directly.

## Project Structure
- Bounded contexts: `UserManagement`, `TeamCollaboration`, `ProjectManagement`, `TaskExecution` (mirrored in Domain & Application).
- Endpoints live in `src/DotNetSkills.API/Endpoints/<Context>`. Application layer holds Commands/Queries, Validators, Mappings. Infrastructure provides EF repositories, caching decorators, and UnitOfWork.

## Dependency Injection & Pipeline
- API: `DependencyInjection.cs` wires Swagger, CORS, JWT, ProblemDetails, health checks, current user service. `Program.cs` builds pipeline: ExceptionHandling → Swagger (dev) → HTTPS → CORS → Auth (conditional) → Map*Endpoints.
- Application: `DependencyInjection.cs` registers MediatR behaviors in order: Logging → Validation → Performance → DomainEventDispatch. AutoMapper & FluentValidation are auto-discovered.
- Infrastructure: `DependencyInjection.cs` sets up DbContext (SQL Server + retry), repositories, caching decorators, UnitOfWork, DomainEventDispatcher, memory cache.

## Endpoint Pattern
- Group endpoints per context: `app.MapGroup("/api/v1/<resource>").RequireAuthorization()`.
- Handler signature: `async (Command/Query dto, IMediator mediator) => await mediator.Send(dto)`.
- Do NOT inject DbContext or repositories into endpoints. Throw domain/validation exceptions; middleware + ProblemDetails handles translation.

## Domain Modeling
- Rich entities: methods enforce invariants, no public setters leaking invariants.
- Strongly-typed IDs: `record XxxId(Guid Value)`.
- Value objects for constrained primitives (e.g., `EmailAddress`).
- Domain events raised inside aggregate methods, dispatched post-handler via DomainEventDispatchBehavior.

## Repositories & Caching
- Application defines interfaces (never expose IQueryable).
- Infrastructure implements concrete repo + cached decorator (e.g., `UserRepository`, `CachedUserRepository`).
- For new aggregates: implement base repo, wrap with cached decorator (IMemoryCache, 5–10m default), register in Infrastructure DI.

## Validation & Mapping
- Create FluentValidation validators beside commands. ValidationBehavior ensures handlers receive valid input.
- Add AutoMapper Profile per bounded context. Endpoints never manually construct deep DTO graphs—delegate to Application result DTO.

## Options & Configuration
- Add new option class under `API/Configuration/Options` + validator under `Options/Validators`. Bind & validate in `AddApiServices` (ValidateOnStart).
- NEVER scatter configuration lookups in handlers—inject typed options or abstractions.

## Testing
- Unit tests per domain rule: `Domain.UnitTests`. Handler tests: `Application.UnitTests`. Use builder/test data patterns (see existing *Builder).
- Do not spin up real DbContext in unit tests—mock repositories. Integration tests use Testcontainers.

## Common Pitfalls
- Never access ApplicationDbContext outside Infrastructure.
- Never bypass MediatR.
- Never return IQueryable from repositories.
- Never put business logic in handlers instead of entities.
- Never expose primitive IDs instead of strongly-typed.
- Never catch-and-swallow exceptions in endpoints.

## Typical Workflow (macOS zsh)
```bash
dotnet restore && dotnet build
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
dotnet run --project src/DotNetSkills.API
dotnet test
```

## Adding a New Feature (Checklist)
1. Define domain entity/value object changes (`Domain/<Context>/...`).
2. Add repository interface method (`Application/<Context>/.../Abstractions` or existing interface).
3. Create Command + Validator + Handler + Mapping (Application).
4. Implement repository method + caching decorator registration (Infrastructure DI).
5. Expose endpoint calling mediator (`API/Endpoints/<Context>`). Ensure auth policy if needed.
6. Add tests (domain + handler). Run `dotnet test`.

## Auth & Current User
- JWT optional (`JwtOptions.Enabled`). When enabled, use `ICurrentUserService` (registered in API) inside handlers instead of HttpContext access.
- Add new policies in `API/Authorization`; reference by name in endpoints.

## Performance
- Prefer projection queries (Select into DTO) for read models.
- Avoid N+1: use Include cautiously; consider AsNoTracking for reads.
- Keep async all the way—no `.Result`/`.Wait()`.

## When Unsure
- Search context folder for analogous feature; replicate structure & naming. Keep diffs minimal and layered boundaries intact.

---
For extended explanations, see `docs/DotNet Coding Principles.md` & `CLAUDE.md`. Update this file when conventions change.
