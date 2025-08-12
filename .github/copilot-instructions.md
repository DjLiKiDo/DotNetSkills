
## DotNetSkills • Copilot instructions (short)

- Clean Architecture with strict dependencies: API → Application → Domain; Infrastructure → Application → Domain. See `src/DotNetSkills.API/Program.cs` and each layer’s `DependencyInjection.cs`.
- Bounded contexts across the solution: `UserManagement`, `TeamCollaboration`, `ProjectManagement`, `TaskExecution` (mirrored in Domain & Application).

Architecture in practice
- Endpoints are Minimal APIs grouped per context and call MediatR directly; no DbContext/repositories in endpoints.
	Example: `Program.cs` maps `MapUserManagementEndpoints/MapTeamCollaborationEndpoints/...` and handlers use `IMediator`.
- Application registers MediatR behaviors in order: Logging → Validation → Performance → DomainEventDispatch (see `Application/DependencyInjection.cs`).
- Domain uses rich entities, strongly-typed IDs (`record XxxId(Guid)`), and raises domain events inside aggregate methods.
- Infrastructure wires EF Core SQL Server (retry, timeouts), UnitOfWork, repositories + cached decorators, health checks, and optional Azure Key Vault (see `Infrastructure/DependencyInjection.cs`).

Conventions that matter
- Endpoint handler pattern: `async (Command/Query dto, IMediator mediator) => await mediator.Send(dto)`; groups: `app.MapGroup("/api/v1/<resource>").RequireAuthorization()`.
- Options are bound/validated in API (`SwaggerOptions`, `CorsOptions`, `JwtOptions`) and config validated on startup (`ValidateConfiguration`).
- Migrations can run on startup via `await app.RunDatabaseMigrationsAsync();` toggled by `RUN_MIGRATIONS` env var.
- JWT is optional: when enabled, pipeline adds AuthN/Z; handlers use `ICurrentUserService` instead of `HttpContext`.
- DataProtection keys persist to filesystem; HTTPS is disabled when `DOTNET_RUNNING_IN_CONTAINER=true`.

Developer workflow (macOS zsh)
- Build/test: `dotnet restore && dotnet build` → `dotnet test`.
- Local run: `dotnet run --project src/DotNetSkills.API` (Swagger gated by `SwaggerOptions.Enabled` and env).
- EF migrations (local): `dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API`.
- Docker compose: use Makefile shortcuts — `make up`, `make logs`, `make status`, `make down`; health at `http://localhost:8080/health`.

Do / Don’t (project-specific)
- Do keep business rules in Domain entities; map to DTOs in Application (AutoMapper profiles per context).
- Don’t bypass MediatR or return `IQueryable` from repositories; use interfaces in Application and caching decorators in Infrastructure.
- Don’t inject DbContext into endpoints; throw domain/validation exceptions and let ProblemDetails middleware shape responses.

Adding a feature (happy path)
1) Model rule in Domain (entity/value object/event). 2) Add repo contract in Application + Command/Query, Validator, Handler, Mapping. 3) Implement repo (+cached decorator) + register in Infrastructure DI. 4) Expose endpoint under `API/Endpoints/<Context>` calling `mediator.Send`.

Pointers
- Key files: `API/Program.cs`, `API/DependencyInjection.cs`, `Application/DependencyInjection.cs`, `Infrastructure/DependencyInjection.cs`, `Makefile`, `README.md`.
- When unsure, find an analogous context and mirror its structure/naming. Keep diffs minimal and boundaries intact.
