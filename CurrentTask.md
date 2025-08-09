## Task list (grouped) ✅


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