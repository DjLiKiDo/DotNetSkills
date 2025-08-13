# DotNetSkills – AI Coding Agent Instructions

Purpose: Enable an AI agent to contribute productively within minutes. Follow these project-specific rules and patterns. Keep changes small, validated, and aligned with existing architecture.

## 1. Architecture & Boundaries
- Clean Architecture + DDD. Layer refs: `API -> Application -> Domain`; `Infrastructure -> Application -> Domain`. Domain has zero outward deps.
- Four bounded contexts mapped across layers: `UserManagement`, `TeamCollaboration`, `ProjectManagement`, `TaskExecution`.
- Put new domain concepts under `src/DotNetSkills.Domain/<Context>/` (Entities, ValueObjects, Events);
  corresponding use cases under `src/DotNetSkills.Application/<Context>/` (Commands/Queries + Handlers + Validators + DTOs);
  persistence under `src/DotNetSkills.Infrastructure/Repositories` or context subfolder; endpoints in `src/DotNetSkills.API/Endpoints/<Context>/`.

## 2. Core Patterns
- CQRS via MediatR: Commands mutate, Queries read. Names: `<Verb><Entity>Command|Query` returning explicit DTO/Response records.
- Exception-only contract (ADR-0001): Do NOT introduce `Result<T>` wrappers. Throw domain/application exceptions for invalid flows.
- Rich domain model: Business rules live inside entities. Avoid anemic services.
- Strongly typed IDs (`record XxxId(Guid Value)`) for all aggregates; map with EF value converters.
- Domain events: Raise in aggregate methods with `AddDomainEvent(new Event(...))`. They dispatch post-success via `DomainEventDispatchBehavior`.

## 3. Pipelines & Behaviors
Execution order (don’t reorder when adding): Logging → Validation → Performance → Domain Event Dispatch. Add new behavior after Validation unless clearly cross-cutting earlier.

## 4. Repositories & UoW
- Repositories expose intention-revealing methods; never return `IQueryable`.
- Use existing BaseRepository or patterns; register in `Infrastructure/DependencyInjection.cs`.
- Always call `_unitOfWork.SaveChangesAsync(ct)` in command handlers after mutations.
- Caching: If adding cache, create decorator (e.g., `CachedUserRepository`) and register using decoration, don’t embed cache logic in main repo.

## 5. API Endpoints (Minimal APIs)
- Group by context: `Endpoints/<Context>/<Entity>Endpoints.cs`.
- Pattern: `(CommandOrQuery request, IMediator mediator) => await mediator.Send(request);`
- Apply `.RequireAuthorization()` where needed; prefer policy names instead of role strings.
- Return proper status: POST -> 201 Created with location; DELETE -> 204; validation errors -> automatic ProblemDetails via validation.

## 6. Validation & Errors
- Use FluentValidation validators in Application layer; don’t revalidate in handlers.
- Domain invariants still enforced in entities (double safety is fine at different layers).
- Global exception middleware maps `DomainException` to 400 and others to 500; don’t catch and swallow inside handlers unless adding context then rethrow.

## 7. Testing Requirements
- When adding logic: add / adjust tests in matching project under `tests/` (Domain/Application/Infrastructure/API).
- Prefer builder/test utility patterns already present (e.g., `UserTestBuilder` style). Keep tests deterministic.
- Run `make test` (or `dotnet test`) before committing.

## 8. Migrations & Persistence
- If schema changes: `dotnet ef migrations add MeaningfulName --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API`.
- Don’t hand-edit existing migrations already merged; create a new one.
- Ensure value object & strongly-typed ID conversions follow existing configurations.

## 9. Naming & Structure Conventions
- Files: `CreateUserCommand.cs`, `CreateUserCommandHandler.cs`, `CreateUserCommandValidator.cs`.
- DTO/Response records end with `Response` or `Dto`.
- Keep one public type per file (except small records).
- No “Manager” / “Helper” names; choose domain terms.

## 10. Security & Config
- Never commit secrets; use environment variables (`DOTNETSKILLS_*`).
- Add new auth policies centrally (Authorization or configuration folder in API) and reference by constant.
- Input validation + domain invariants must prevent privilege escalation (e.g., no arbitrary role elevation unless explicit use case).

## 11. Performance & Observability
- Avoid N+1: Use projection (`Select`) or Includes only where required.
- Long-running operations: consider adding logging or performance thresholds (PerformanceBehavior logs slow requests).
- When adding caching or async operations, ensure thread safety (follow AsyncLocal pattern for domain event tracking if relevant).

## 12. Contribution Workflow
1. Branch: `feature/<short-kebab>` or `fix/<issue-ref>`.
2. Implement vertical slice across layers.
3. Add/Update tests + run `make test`.
4. Add migration if needed.
5. Keep PRs small; include rationale if deviating from patterns.

## 13. Common Pitfalls to Avoid
- Adding service-layer business logic instead of entity methods.
- Returning `IQueryable` or leaking EF types beyond Infrastructure.
- Introducing `Result<T>` wrappers or null-based error signaling.
- Skipping `_unitOfWork.SaveChangesAsync` after state changes.
- Mixing multiple bounded context concerns in one handler.

## 14. Quick Command Reference
- Run API: `dotnet run --project src/DotNetSkills.API`
- Full env (API+DB): `make up` (add Redis: `make full`)
- Tests: `make test`
- Add migration: see section 8
- Health check: `make health` or curl `/health`

## 15. When Unsure
Inspect analogous existing feature (e.g., User creation flow) and mirror structure. Keep changes minimal and consistent.

(End of instructions – request clarification if a new pattern seems required.)
