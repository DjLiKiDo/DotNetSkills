## Current Task — Conversation & Progress Summary

Updated: 2025-08-08

### 1) Key discussion points and decisions
- Adopt an enum-first pattern across requests/commands for all enum-like values (statuses, priorities, roles) to improve type safety and consistency.
- Resolve TaskStatus name ambiguity by using an alias for the domain enum to avoid conflict with `System.Threading.Tasks.TaskStatus`.
- Keep API compatibility by relying on `JsonStringEnumConverter`, allowing clients to send/receive enum values as strings (e.g., "High", "Done", "Admin").
- Extend API to accept enum filters where appropriate (e.g., Users list filtering by role/status).

### 2) Current status of the task/project
- Enum-first refactor applied to Task Execution and Project Management flows:
	- Commands/Requests now use enums: TaskPriority, TaskStatus (aliased as DomainTaskStatus where needed).
	- Validators updated to use `IsInEnum`/`Enum.IsDefined` instead of manual string validation.
	- Queries (GetTasks, GetProjectTasks) accept nullable enum filters for status/priority.
- API endpoints updated to bind enums directly and forward them into Application layer types.
- User Management API GetUsers endpoint now supports optional role/status enum filters and forwards them to `GetUsersQuery`.
- Build: succeeded. Tests: 91/91 passed.
- Remaining warnings are unrelated placeholders (async methods without await) and a few nullable warnings in Infrastructure; tracked separately.

### 3) Important context needed for continuation
- Architecture: Clean Architecture + DDD with bounded contexts:
	- API → Application → Domain; Infrastructure depends inward.
	- Bounded contexts: UserManagement, TeamCollaboration, ProjectManagement, TaskExecution.
- Enum sources (Domain):
	- TaskExecution: `TaskPriority`, `TaskStatus`.
	- UserManagement: `UserRole`, `UserStatus`.
	- TeamCollaboration: `TeamRole`, `TeamStatus`.
	- ProjectManagement: `ProjectStatus`.
- Ambiguity handling: use `DomainTaskStatus = DotNetSkills.Domain.TaskExecution.Enums.TaskStatus` via GlobalUsings to avoid `System.Threading.Tasks.TaskStatus` clashes.
- JSON: `JsonStringEnumConverter` is configured so enums serialize/deserialize as strings in API.

### 4) Pending items / next steps
- Done: Converted response DTOs to enums where applicable for full end-to-end type safety:
	- UserManagement: `UserResponse`, `UserSummaryResponse`, `UserProfileResponse`, `TeamMembershipResponse` now use `UserRole`, `UserStatus`, and `TeamRole` as appropriate.
	- TeamCollaboration: `TeamMemberResponse` now uses `TeamRole`.
	- Mapping profile updated to map enum properties directly (no ToString()).
- Done: Migrated `*Projections.cs` to enums across bounded contexts and updated repository projection mappings accordingly:
	- TeamCollaboration: TeamSummary/Dashboard/Selection/TeamMembership projections now use TeamStatus/TeamRole.
	- TaskExecution: TaskSummary/Dashboard/Selection/Assignment projections now use TaskPriority and DomainTaskStatus alias for Status.
	- ProjectManagement: Project* projections now use ProjectStatus.
	- UserManagement: User* projections now use UserRole/UserStatus.
- Done: Updated OpenAPI to reflect enum-typed parameters and responses.
- Documentation/OpenAPI: Added EnumSchemaFilter and EnumParameterExampleOperationFilter; updated User endpoints descriptions to mention enum filters explicitly.
- Documentation/OpenAPI: Added RequestExamplesSchemaFilter to provide request-body examples showing enum string values for:
	- TaskExecution: CreateTaskRequest, UpdateTaskRequest, UpdateTaskStatusRequest
	- ProjectManagement: CreateProjectRequest, UpdateProjectRequest
	- TeamCollaboration: CreateTeamRequest, UpdateTeamRequest
	These appear in Swagger UI under each schema’s Example.
- Tidy warnings: address async-without-await placeholder endpoints when implementations are ready; fix nullable assignment warnings in `ProjectRepository`.

### 5) Relevant code snippets (illustrative)
- Disambiguation alias (GlobalUsings):
	- `global using DomainTaskStatus = DotNetSkills.Domain.TaskExecution.Enums.TaskStatus;`
- Example command/request shapes (after):
	- `public record UpdateTaskStatusCommand(TaskId TaskId, DomainTaskStatus Status) : IRequest<TaskResponse>;`
	- `public record CreateTaskCommand(string Title, string? Description, TaskPriority Priority, ... ) : IRequest<TaskResponse>;`
	- `public record GetTasksQuery(int Page, int PageSize, DomainTaskStatus? Status = null, TaskPriority? Priority = null) : IRequest<PagedTaskResponse>;`
	- `public record GetUsersQuery(int Page = 1, int PageSize = 20, string? SearchTerm = null, UserRole? Role = null, UserStatus? Status = null) : IRequest<Result<PagedUserResponse>>;`
- API Users list now supports enum filters:
	- `private static async Task<IResult> GetUsers(IMediator mediator, int page = 1, int pageSize = 20, string? search = null, UserRole? role = null, UserStatus? status = null)`

- Response DTOs (after enum conversion):

```csharp
// UserManagement
public record UserResponse(
	Guid Id,
	string Name,
	string Email,
	UserRole Role,
	UserStatus Status,
	DateTime CreatedAt,
	DateTime UpdatedAt,
	int TeamMembershipsCount);

public record UserSummaryResponse(
	Guid Id,
	string Name,
	string Email,
	UserRole Role,
	UserStatus Status);

public record UserProfileResponse(
	Guid Id,
	string Name,
	string Email,
	UserRole Role,
	UserStatus Status,
	DateTime CreatedAt,
	DateTime UpdatedAt,
	int TeamMembershipsCount,
	IReadOnlyList<UserProfileResponse.TeamMembershipSummary> TeamMemberships);

public record TeamMembershipResponse(
	Guid Id,
	Guid UserId,
	string UserName,
	string UserEmail,
	Guid TeamId,
	string TeamName,
	string TeamDescription,
	TeamRole Role,
	DateTime JoinedAt,
	DateTime? LeftAt,
	bool IsActive);

// TeamCollaboration
public record TeamMemberResponse(
	Guid UserId,
	TeamRole Role,
	DateTime JoinedAt);
```

### 6) Links and references
- Coding standards: `.github/instructions/dotnet.instructions.md`
- PR: Janitor improvements — https://github.com/DjLiKiDo/DotNetSkills/pull/7
- Prompt used for this summary: `.github/prompts/summarize-progress.prompt.md`
- Enums (examples):
	- `src/DotNetSkills.Domain/TaskExecution/Enums/TaskPriority.cs`
	- `src/DotNetSkills.Domain/TaskExecution/Enums/TaskStatus.cs`
	- `src/DotNetSkills.Domain/UserManagement/Enums/UserRole.cs`
	- `src/DotNetSkills.Domain/UserManagement/Enums/UserStatus.cs`
	- `src/DotNetSkills.Domain/TeamCollaboration/Enums/TeamRole.cs`
	- `src/DotNetSkills.Domain/TeamCollaboration/Enums/TeamStatus.cs`
	- `src/DotNetSkills.Domain/ProjectManagement/Enums/ProjectStatus.cs`

### 7) Build & test snapshot
- Build: success. Known warnings (to address later):
	- Async methods lacking `await` in placeholder endpoints: TeamEndpoints.cs, ProjectEndpoints.cs, ProjectTaskEndpoints.cs, TaskEndpoints.cs, TaskAssignmentEndpoints.cs.
	- Nullable assignment warnings in `ProjectRepository.cs` lines ~266/303/389.
- Tests: 91 passed, 0 failed (still green after Swagger enum documentation changes).
