## Task list (grouped) ✅

1) Implement authorization policies
- Description: Define and wire authorization policies referenced in endpoints: AdminOnly, TeamManager, ProjectManagerOrAdmin, ProjectMemberOrAdmin. Add policy constants, claims/roles mapping, and enforce via AddAuthorization in API. Include minimal tests.
- Benefits: Secures endpoints, unblocks API usage in protected routes, aligns with security guidelines.
- Effort: Medium (1–2 days).
- Prompt:
  Implement missing authorization policies in the DotNetSkills API.
  Context:
  - Solution root: /Users/marquez/Downloads/Pablo/Repos/DotNetSkills
  - Policies referenced in endpoints:
    - "AdminOnly" in UserEndpoints.cs
    - "TeamManager" in TeamMemberEndpoints.cs
    - "ProjectManagerOrAdmin" in:
      - ProjectEndpoints.cs
      - TeamEndpoints.cs
    - "ProjectMemberOrAdmin" in ProjectTaskEndpoints.cs
  - Follow dotnet.instructions.md and Clean Architecture. Do not couple to Infrastructure from API.
  Requirements:
  - Create a central static class for policy names (e.g., Api.Authorization.Policies).
  - Configure AddAuthorization in API Startup/Program to map these policies to roles/claims:
    - AdminOnly: user role == Admin
    - TeamManager: role in [Admin, TeamManager]
    - ProjectManagerOrAdmin: role in [Admin, ProjectManager]
    - ProjectMemberOrAdmin: role in [Admin, ProjectManager, Developer] OR explicit project membership claim
  - If role enums/constants exist in Domain (e.g., UserRole), reuse; else define minimal values to unblock.
  - Add extension method AddApiAuthorization(this IServiceCollection, IConfiguration).
  - Update endpoints to use constants instead of string literals.
  - Add minimal unit tests for policy registration (API.UnitTests).
  Deliverables:
  - Policy constants file
  - DI registration in Program.cs/DependencyInjection.cs
  - Endpoint updates
  - Tests in DotNetSkills.API.UnitTests
  Acceptance:
  - dotnet build succeeds
  - Endpoints compile without TODOs for auth
  - Unit tests validate policies exist

2) Replace endpoint placeholders with MediatR integration
- Description: Many endpoints return placeholders and TODO “Replace with MediatR.Send”. Wire endpoints to Application commands/queries, validate using FluentValidation if present, and return correct HTTP responses.
- Benefits: Makes endpoints functional and aligned with CQRS, removes scaffolding.
- Effort: Large (3–5 days), can split by bounded context.
- Prompt:
  Replace placeholder endpoint implementations with MediatR send calls.
  Context files with TODOs:
  - TeamCollaboration:
    - TeamEndpoints.cs (multiple)
    - TeamMemberEndpoints.cs (multiple)
  - ProjectManagement:
    - ProjectEndpoints.cs (multiple)
    - ProjectTaskEndpoints.cs (multiple)
  - TaskExecution:
    - TaskEndpoints.cs (multiple)
    - TaskAssignmentEndpoints.cs (multiple)
  Guidelines:
  - Use IMediator mediator injected by Minimal API parameter binding and await mediator.Send(command/query).
  - Map route/body to existing Application layer request types; if a matching CQRS request doesn't exist yet, create it under Application with validators.
  - Follow conventions in dotnet.instructions.md (CQRS, ValidationBehavior, ProblemDetails).
  - Replace “Get current user ID” TODOs by reading from HttpContext.User with claims (sub or custom Id).
  - Map HTTP status codes: 200/201 for success; 204 for deletes; 404 for missing; 400 for validation errors.
  - Remove placeholder responses.
  Deliverables:
  - Endpoint files updated, wiring to handlers.
  - New commands/queries and validators if missing.
  - Tests to cover at least one happy path per endpoint group using WebApplicationFactory or minimal unit tests if infra is missing.
  Acceptance:
  - Build passes
  - Endpoints compile
  - Placeholder TODOs removed
  - Basic tests passing

3) Implement Application handlers for Teams, Projects, Tasks
- Description: Many Application handlers are TODO stubs. Implement command/query handlers using rich domain models and repositories, returning DTOs via mappers.
- Benefits: Enables functional endpoints via CQRS; unlocks end-to-end flows.
- Effort: Large (5–8 days) across contexts; break into sub-epics.
- Prompt:
  Implement missing Application CQRS handlers and mapping for TeamCollaboration, ProjectManagement, TaskExecution.
  Files with TODO handlers:
  - TeamCollaboration: CreateTeam, UpdateTeam, DeleteTeam, GetTeams (pagination/search), GetTeamById, GetTeamMembers, AddTeamMember, RemoveTeamMember, UpdateMemberRole
  - ProjectManagement: CreateProject, UpdateProject, ArchiveProject, GetProjects (filtering), GetProjectById, GetProjectTasks, CreateTaskInProject, UpdateTaskInProject
  - TaskExecution: CreateTask, UpdateTask, DeleteTask, GetTaskById, GetTasks, CreateSubtask, UpdateSubtask, GetTaskSubtasks, AssignTask, UnassignTask, UpdateTaskStatus
  Requirements:
  - Use repositories via Application interfaces; don’t access DbContext in Application.
  - Enforce domain invariants in entities; raise domain events where relevant.
  - Implement mappers for responses; avoid anemic models.
  - Use FluentValidation for commands/queries; integrate with ValidationBehavior.
  - Ensure pagination types (PagedRequest/PagedResponse) used where noted by TODO.
  - Add unit tests for each handler’s happy path + 1 failure case using fakes/mocks.
  - Use ConfigureAwait(false) in library code.
  Acceptance:
  - Build and tests pass
  - All TODO comments in listed handlers removed
  - Endpoints can call handlers without placeholders

4) Introduce AutoMapper profiles for DTO mapping
- Description: Replace “Replace with AutoMapper when configured” TODOs in response DTOs; add profiles and mappings for User, Team, Project, Task aggregates.
- Benefits: Centralizes mapping, reduces duplication, improves maintainability.
- Effort: Small-Medium (0.5–1 day).
- Prompt:
  Add AutoMapper to Application layer and replace manual mapping TODOs.
  Context TODOs:
  - TeamResponse.cs (comments)
  - TeamMembersResponse.cs
  - ProjectResponse.cs
  - UserResponse.cs
  Requirements:
  - Add AutoMapper registration in Application DependencyInjection (AddApplicationServices).
  - Create profiles under Application for each bounded context (e.g., TeamCollaboration/Mapping/TeamProfile.cs).
  - Replace manual ToResponse methods with mapper usage where used by handlers.
  - Add simple unit tests verifying mappings for a few typical cases.
  Acceptance:
  - Build passes
  - Mapping tests pass
  - TODO comments removed

5) Current user ID retrieval from auth context
- Description: Replace TODOs “Get current user ID from authentication context” with a reusable helper service to extract UserId from claims; wire into endpoints.
- Benefits: Consistent, secure identity handling; reduces duplication.
- Effort: Small (0.5 day).
- Prompt:
  Implement CurrentUser accessor for Minimal APIs and replace TODOs.
  Context TODOs appear in:
  - ProjectManagement endpoints: several points
  - TaskExecution endpoints and TaskAssignment endpoints
  Requirements:
  - Create ICurrentUserService with UserId? GetUserId(HttpContext).
  - Implementation reads claim types: sub, nameidentifier, or custom "uid"; converts to UserId VO.
  - Register in API DI.
  - Inject into endpoints (method parameter) and use to supply CreatedBy/AssignedBy etc.
  - Add unit tests for claim parsing variants.
  Acceptance:
  - Build passes
  - Endpoints compile and no “Get current user ID” TODOs remain

6) Logging improvements in User endpoints
- Description: Replace “TODO: Log exception” occurrences with structured logging at the right levels; ensure consistent ProblemDetails.
- Benefits: Better observability, production-grade error handling.
- Effort: Small (0.5 day).
- Prompt:
  Add structured exception logging in UserManagement endpoints.
  Context TODOs in:
  - UserEndpoints.cs (multiple lines)
  Requirements:
  - Inject ILogger where needed.
  - LogError with correlation info (UserId from claims or “anonymous”), route, and request data (safe subset).
  - Ensure Global exception handling middleware is configured per project standards (if not, add).
  Acceptance:
  - Build passes
  - TODO comments removed
  - Logs include structured properties

7) Domain/User LastLoginAt consideration
- Description: TODO suggests adding LastLoginAt to User entity in Infrastructure mapping. Decide and implement field if needed with migration and usage in repository method that maps to DTO.
- Benefits: Completes repository logic and enables audit features.
- Effort: Small-Medium (0.5–1 day).
- Prompt:
  Add optional LastLoginAt to Domain User entity and persist via EF if required by responses.
  Context:
  - TODO in UserRepository.cs line ~318
  Requirements:
  - If responses or features need last login, add nullable DateTimeOffset LastLoginAt to Domain entity, update EF configuration and mapping.
  - Add migration (if EF is configured in repo; otherwise wire mapping and leave migration note).
  - Update repository projections.
  - Add unit test verifying mapping sets LastLoginAt as expected.
  Acceptance:
  - Build passes
  - Mapping compiles
  - TODO removed or resolved by explicit decision

8) Implement Project retrieval and filtering
- Description: Several TODOs indicate “Implement project retrieval with filtering” and “Get project tasks” projections.
- Benefits: Enables key read operations for ProjectManagement.
- Effort: Medium (1–2 days).
- Prompt:
  Implement Project queries with filtering and task projection.
  Context TODOs:
  - GetProjectsQueryHandler.cs (filtering)
  - GetProjectByIdQueryHandler.cs
  - GetProjectTasksQueryHandler.cs (and TODO to use repository for ProjectName)
  - ProjectTaskEndpoints.cs placeholder returns
  Requirements:
  - Define repository methods with pagination, search by name/status/teamId, and include task counts.
  - Handlers return paged DTOs using mapping.
  - Replace placeholder in endpoints with mediator calls.
  - Add unit tests for filtering scenarios.
  Acceptance:
  - Build passes
  - TODOs removed
  - Tests passing

9) Implement Team operations end-to-end
- Description: CRUD and membership operations in Application and API have TODOs.
- Benefits: Core team collaboration flows functional.
- Effort: Medium-Large (2–4 days).
- Prompt:
  Complete TeamCollaboration CQRS and endpoints.
  Context TODOs:
  - Handlers: Create/Update/Delete/Get/GetTeams/AddMember/RemoveMember/UpdateMemberRole/GetMembers
  - Endpoints: TeamEndpoints.cs and TeamMemberEndpoints.cs have placeholders and auth TODOs.
  Requirements:
  - Implement domain methods on Team aggregate enforcing invariants, raise events.
  - Implement repository and handlers, wire endpoints with mediator.
  - Add validators and mapping.
  - Tests for at least one operation per feature.
  Acceptance:
  - Build passes
  - TODOs removed
  - Basic tests pass

10) Implement TaskExecution operations
- Description: Multiple TODOs across Create/Update/Delete/Assign/Unassign/Status/Subtasks and query handlers, plus endpoints placeholders.
- Benefits: Enables core task management.
- Effort: Large (3–5 days).
- Prompt:
  Implement TaskExecution CQRS handlers and endpoints.
  Context TODOs:
  - Handlers in TaskExecution features (many listed in grep)
  - Endpoints in TaskEndpoints.cs + TaskAssignmentEndpoints.cs
  Requirements:
  - Domain Task aggregate must enforce rules (cannot assign completed tasks, etc.)
  - Implement repository methods; use projection for Queries; AsSplitQuery when needed for collections.
  - Implement validators; map DTOs; ensure status transitions are validated.
  - Replace “Reopen()” TODO path in UpdateTaskStatus with full state machine: ToDo -> InProgress -> InReview -> Done; allow Reopen from Done/Cancelled if business rules permit.
  - Tests for happy path + invalid transitions.
  Acceptance:
  - Build passes
  - TODOs removed
  - Tests passing

11) Wire Minimal API validation using FluentValidation
- Description: For endpoints receiving commands, ensure validators execute and return Results.ValidationProblem. Some handlers reference validation TODOs implicitly.
- Benefits: Input safety, consistent API behavior.
- Effort: Small (0.5 day).
- Prompt:
  Ensure FluentValidation is registered and integrated in the API request pipeline for Minimal APIs.
  Requirements:
  - Register validators and ValidationBehavior in Application DI.
  - Add Minimal API middleware to surface validation errors as 400 ProblemDetails.
  - Update endpoints to call validators if you’re not using behavior-based validation.
  - Add one unit test per context to ensure validation errors are returned as 400.
  Acceptance:
  - Build passes
  - Validation behavior proven via tests

12) Replace hard-coded sample data in GetProjectTasks
- Description: Replace hard-coded ProjectName and items with repository-backed data.
- Benefits: Accurate data; removes stubs.
- Effort: Tiny (0.25 day).
- Prompt:
  Remove sample data from GetProjectTasks and use repository projection for ProjectName and task list.
  Context:
  - GetProjectTasksQueryHandler.cs lines with TODOs for repository and ProjectName.
  Requirements:
  - Implement repository call to fetch tasks with project context.
  - Map to response DTO.
  Acceptance:
  - Build passes
  - No sample literals remain

13) Add Global exception handling middleware (if missing)
- Description: Ensure a central middleware exists and endpoints delegate to it; remove inline try/catch with TODO “Log exception” when global handler is present.
- Benefits: Consistent error responses, less duplication.
- Effort: Small (0.5 day).
- Prompt:
  Add/verify global exception middleware and standard ProblemDetails mapping; integrate logging.
  Requirements:
  - Middleware per dotnet.instructions.md pattern.
  - Register in Program.cs.
  - Remove redundant try/catch in endpoints; rely on middleware.
  Acceptance:
  - Build passes
  - Consistent error responses
  - TODOs resolved

## Notes
- I kept prompts self-contained so an agent can run autonomously using repo paths and standards in dotnet.instructions.md.
- If you’d like, I can split large items (#2, #3, #10) into smaller PR-sized chunks by bounded context.