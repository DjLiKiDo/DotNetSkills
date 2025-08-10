## 1. Key Discussion Points & Decisions

- Exception taxonomy: Introduced ApplicationExceptionBase with NotFoundException (404), BusinessRuleViolationException (409), ValidationException (400).
- Centralized handling: ExceptionHandlingMiddleware maps Application + Domain + FluentValidation + generic errors to ProblemDetails.
- Validation strategy: Pipeline ValidationBehavior executes early (after Logging, before Performance). Uses FluentValidation; on failure currently returns Result failure or throws FluentValidation.ValidationException for non-Result responses.
- Repository pattern: Kept synchronous Add/Update; persistence via UnitOfWork.SaveChangesAsync only.
- Strongly-typed IDs: Use nullable checks (id is not null) instead of .HasValue; consistent messaging (NotFoundException(entity, key)).
- Mapping fixes: SubtaskResponse now includes UpdatedAt, IsAssigned etc.; handlers aligned.
- Domain vs application: DomainException remains for invariant violations; Application exceptions for workflow/resource concerns.
- Chosen status codes: Validation mapped to 400 (not 422) for consistency with other client errors.

## 2. Current Status

Completed:
- ValidationBehavior implemented and ordered: Logging → Validation → Performance → DomainEventDispatch.
- Behavior ordering corrected in Application DependencyInjection.
- ExceptionHandlingMiddleware operational with new mappings.
- Unit tests added:
  - Middleware mapping tests (NotFound 404, BusinessRule 409, Validation 400, Domain 400).
  - AssignTaskCommandHandler happy path test (verifies Update + SaveChanges).
- AssignTask handler refactored to new patterns.
- AutoMapper configuration stabilized (all profile configuration tests passing):
  - Removed duplicate IEnumerable<object> -> List<object> map from base profile; centralized in SharedValueObjectMappingProfile.
  - Added SharedValueObjectMappingProfile for strongly-typed IDs & value objects to eliminate DuplicateTypeMapConfigurationException.
  - Added ConstructUsing mappings for record DTOs (TaskAssignmentResponse, TeamMembershipResponse) to satisfy positional constructors.
  - Adjusted Task & User mappings to rely on constructor projections and ignore member mapping after construction.

In place:
- Result / Result<T> pattern (used by behaviors).
- Mapping profiles auto-registered.

Recent Additions:
- Integration test for invalid task creation (POST /api/v1/tasks) now passing (400 ValidationProblemDetails) using WebApplicationFactory + FakePolicyEvaluator.
- Adjusted TaskEndpoints: removed broad catch for CreateTask to let ValidationException bubble.
- ExceptionHandlingMiddleware now surfaces validation errors under extensions.errors to ensure consistent JSON shape.
 - Added standardized extensions.errorCode for all mapped exceptions; middleware & unit tests updated and passing.
 - AutoMapper refactor (see above) with green test verification.
 - Nullable warnings (Infrastructure ProjectRepository.cs CS8601) resolved by adding null-coalescing and defensive checks; build now free of those specific warnings.
 - CS1998 warning removed from TaskAssignmentEndpoints by converting placeholder handlers to synchronous methods (no unnecessary async).
 - Added TaskAssignmentMappingTests validating context-based population of AssignedUserName / AssignedByUser fields.

Pending (not yet implemented):
- Analyzer & nullable warning cleanup (ProjectRepository and async warnings).
- Documentation updates (exception table, validation pipeline rationale, repository sync decision).
- Additional regression / coverage (subtask flags, domain event dispatch verification).
- Correlation ID enrichment (optional future).
 - (AutoMapper follow-ups) Add contextual mapping tests for TaskAssignmentResponse with context.Items (AssignedUserName / AssignedByUserId) to lock behavior.
 - Remaining warnings to address:
  - CS1998 async method without await in TaskAssignmentEndpoints (placeholder / TODO implementation).
  - (Resolved) xUnit1031 warnings in AuthorizationExtensionsTests by converting test methods to async Task and awaiting policy/authorization calls.

## 3. Important Context for Continuation

- ValidationBehavior short-circuits before PerformanceBehavior; failing requests will not appear in performance metrics.
- ValidationBehavior currently mixes two approaches: returns failed Result for handlers returning Result/Result<T>; throws FluentValidation.ValidationException for other responses.
- ExceptionHandlingMiddleware formats ApplicationExceptionBase using ErrorCode → Title (snake_case to Title Case).
- Domain events dispatch behavior is currently a placeholder (no actual event collection logic yet).
- All repository Update/Add calls are synchronous; do not introduce async variants unless globally adopted.
- Tests rely on Moq, AutoMapper real profiles, and direct handler invocation (no full pipeline simulation yet).

## 4. Pending Items / Next Steps

Immediate (next work cycle):
 - (Done) Add errorCode to ProblemDetails.Extensions["errorCode"] for all mapped exceptions.
 - (Done) Resolve AutoMapper duplicate type map & record constructor failures (now green).
- Warning Resolution:
  - Fix nullable warnings in repositories (add null checks or null-forgiving operator where safe).
  - Convert any blocking .Result / .Wait() test usages to await.
  - Remove async keyword from methods with no awaits or add awaited operations.
- Documentation:
  - Add exception taxonomy table (Exception → HTTP Status → ErrorCode).
  - Describe pipeline ordering rationale.
  - Note 400 for validation (decision + client impact).
  - State repository sync design reasons (simplicity, UoW flush boundary).
- Additional Unit Tests:
  - Negative path for AssignTask (task not found → NotFoundException).
  - Validation failure surface test via mediator (optional).
  - Mapping context test for TaskAssignmentResponse (ensures context items populate names & assigner IDs when provided).
- Optional Quick Wins:
  - Add correlation ID middleware (include X-Correlation-Id header in responses & ProblemDetails).
  - Add error code to ProblemDetails.Extensions["errorCode"] for consistency.

Medium-term:
- Implement real domain event collection & dispatch in DomainEventDispatchBehavior.
- Broaden handler tests (UnassignTask, UpdateTaskStatus).
- Add projections / caching coverage tests.
- Introduce Result<T> consistency (decide whether to migrate all handlers or stick to exceptions).

Future enhancements:
- Correlation + tracing (Activity / W3C TraceContext).
- Structured ProblemDetails error codes reference doc.
- PerformanceBehavior metrics export (e.g., OpenTelemetry).

## 5. Relevant Code Snippets

ValidationBehavior ordering (DependencyInjection):
```csharp
cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(DomainEventDispatchBehavior<,>));
```

ValidationBehavior core decision:
```csharp
if (failures.Count != 0)
    return CreateValidationResult<TResponse>(failures);
```

ExceptionHandlingMiddleware mapping excerpt:
```csharp
exception switch
{
  ApplicationExceptionBase appEx => new ProblemDetails { Status = appEx.StatusCode, Title = FormatTitle(appEx.ErrorCode), Detail = appEx.Message },
  DomainException domainEx => new ProblemDetails { Status = 400, Title = "Domain Rule Violation", Detail = domainEx.Message },
  FluentValidation.ValidationException valEx => CreateValidationProblemDetails(valEx, context.Request.Path),
  _ => new ProblemDetails { Status = 500, Title = "Internal Server Error" }
};
```

AssignTask handler (core flow):
```csharp
var task = await _taskRepository.GetByIdAsync(request.TaskId, ct) ?? throw new NotFoundException("Task", request.TaskId);
var assignee = await _userRepository.GetByIdAsync(request.AssignedUserId, ct) ?? throw new NotFoundException("User", request.AssignedUserId);
var assigner = await _userRepository.GetByIdAsync(request.AssignedByUserId, ct) ?? throw new NotFoundException("User", request.AssignedByUserId);
task.AssignTo(assignee, assigner);
_taskRepository.Update(task);
await _unitOfWork.SaveChangesAsync(ct);
```

Middleware tests added (file):
ExceptionHandlingMiddlewareTests.cs

Handler test added (file):
AssignTaskCommandHandlerTests.cs

## 6. Documentation / Resource Links

Internal:
- ValidationBehavior.cs
- ExceptionHandlingMiddleware.cs
- `src/DotNetSkills.Application/Common/Exceptions/*`
- `docs/DotNet Coding Principles.md`
- README.md (needs update for new exception & validation pipeline)
- DependencyInjection.cs

External References:
- FluentValidation: https://docs.fluentvalidation.net
- RFC 7807 (ProblemDetails): https://datatracker.ietf.org/doc/html/rfc7807
- Clean Architecture: https://blog.cleancoder.com/
- DDD Reference: https://domainlanguage.com/ddd/

---

This plan contains the required context and actionable next steps for seamless continuation.