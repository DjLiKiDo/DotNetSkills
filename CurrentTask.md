## Task list (grouped) ✅


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