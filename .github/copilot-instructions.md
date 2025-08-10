## Copilot Instructions (Essential Playbook)
Concise, project-specific rules so an AI agent can contribute productively. Read this first; consult full docs in: docs/, README.md, CLAUDE.md.

### 1. Architecture Guardrails
Dependency flow: API → Application → Domain; Infrastructure → Application → Domain. Inner layers never reference outer ones. Keep domain pure (no EF/ASP.NET). Use MediatR for all use cases (no direct service orchestration from endpoints).

### 2. Project Layout (Bounded Contexts)
Contexts: UserManagement, TeamCollaboration, ProjectManagement, TaskExecution present across Domain & Application. Mirror folder names when adding features. Place endpoints in API/Endpoints/<Context>. Application layer holds Commands/Queries, Validators, Mappings. Infrastructure supplies EF repositories + caching decorators + UoW.

### 3. DI & Pipeline (Key Files)
API: src/DotNetSkills.API/DependencyInjection.cs (AddApiServices) wires options (Swagger/Cors/Jwt), JSON config, ProblemDetails, auth (conditional JwtOptions.Enabled), health checks, current user service. Program.cs builds pipeline: UseExceptionHandling → Swagger (dev) → HTTPS → CORS → Auth (conditional) → Map*Endpoints.
Application: DependencyInjection registers MediatR behaviors order: Logging → Validation → Performance → DomainEventDispatch (respect this sequence). AutoMapper & FluentValidation auto-discovered.
Infrastructure: DependencyInjection sets DbContext (SQL Server + retry), registers repositories and wraps each with Cached* decorator, adds UnitOfWork + DomainEventDispatcher, memory cache.

### 4. Endpoint Pattern
Group endpoints per context: var g = app.MapGroup("/api/v1/<resource>").RequireAuthorization(); Add .WithSummary()/.Produces<>(). Handler signature: async (Command/Query dto, IMediator mediator) => await mediator.Send(dto). Do NOT inject DbContext or repositories into endpoints. Throw domain/validation exceptions; middleware + ProblemDetails handles translation.

### 5. Domain Modeling Rules
Use rich entities (methods enforce invariants) + strongly-typed IDs (record XxxId(Guid Value)). Value objects for constrained primitives (EmailAddress). Raise domain events inside aggregate methods; they are dispatched post handler via DomainEventDispatchBehavior. No public setters leaking invariants.

### 6. Repositories & Caching
Application defines interfaces (no IQueryable exposure). Infrastructure implements concrete repo + cached decorator (e.g., UserRepository + CachedUserRepository). When adding a new aggregate: implement base repo, then wrap with Cached<Aggregate>Repository using IMemoryCache (5–10m sensible default) and register it in Infrastructure DI similarly to existing ones.

### 7. Validation & Mapping
Create FluentValidation validators beside commands. Rely on ValidationBehavior—handlers assume valid input. For mapping, add AutoMapper Profile per bounded context; endpoints never manually construct deep DTO graphs (delegate to Application result DTO).

### 8. Options & Configuration
Add new option class under API/Configuration/Options + validator under Options/Validators; bind & validate in AddApiServices (ValidateOnStart). NEVER scatter configuration lookups in handlers—inject typed options or abstractions.

### 9. Testing Focus
Add unit tests per domain rule (Domain.UnitTests). Handler tests go in Application.UnitTests. Use builder/test data patterns (follow existing *Builder when added). Don’t spin up real DbContext in unit tests—mock repositories. Integration tests (future) will use Testcontainers; keep constructors friendly to DI.

### 10. Common Pitfalls to Avoid
Do not: access ApplicationDbContext outside Infrastructure; bypass MediatR; return IQueryable; put business logic in handlers instead of entities; expose primitive IDs instead of strongly-typed; catch-and-swallow exceptions in endpoints.

### 11. Typical Workflow (macOS zsh)
```bash
dotnet restore && dotnet build
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
dotnet run --project src/DotNetSkills.API
dotnet test
```

### 12. Adding a New Feature (Checklist)
1. Define domain entity/value object changes (Domain/<Context>/...).
2. Add repository interface method (Application/<Context>/.../Abstractions or existing interface).
3. Create Command + Validator + Handler + Mapping (Application).
4. Implement repository method + caching decorator registration (Infrastructure DI).
5. Expose endpoint calling mediator (API/Endpoints/<Context>). Ensure auth policy if needed.
6. Add tests (domain + handler). Run dotnet test.

### 13. Auth & Current User
JWT optional (JwtOptions.Enabled). When enabled, use ICurrentUserService (registered in API) inside handlers instead of HttpContext access. Add new policies in API Authorization folder; reference by name in endpoints.

### 14. Performance Practices
Prefer projection queries (Select into DTO) for read models. Avoid N+1: use Include cautiously; consider AsNoTracking for reads. Keep async all the way—no .Result/.Wait().

### 15. When Unsure
Search existing context folder for analogous feature; replicate structure & naming. Keep diff minimal and layered boundaries intact.

Refer to docs/DotNet Coding Principles.md & CLAUDE.md for extended explanations. Keep this file concise—update when conventions change.
