## Current Task: Architectural Decision – Result<T> Consistency Strategy

Date: 2025-08-11
Status: In Progress (Decision Captured – Implementation Pending)

### 1. Context

The Application layer currently exhibits a mixed error-handling approach:
- Some handlers (legacy/earlier) return plain DTO / void and signal failures via typed exceptions.
- ValidationBehavior supports two flows: (a) throwing FluentValidation.ValidationException, (b) creating failure Result/Result<T>.
- Middleware (ExceptionHandlingMiddleware) already centralizes exception → ProblemDetails translation with taxonomy (NotFound, BusinessRuleViolation, Validation, Domain, generic 500).

This dual model increases cognitive overhead and creates ambiguity for contributors (When do I return Result? When do I throw?). We need a single, explicit architectural stance before expanding features further.

### 2. Options Considered

| # | Option | Summary | Pros | Cons | Effort |
|---|--------|---------|------|------|--------|
| 1 | Exceptions Only | All handlers return DTO; failures always throw typed exceptions | Simplest pipeline; leverages existing middleware; low migration cost | Exceptions for expected failures (validation/business) | Low |
| 2 | Result Everywhere | All handlers return Result/Result<T> | Explicit success/failure; avoids exceptions for expected cases | Boilerplate in endpoints; need translation to HTTP; refactor all handlers | High |
| 3 | Hybrid (Commands=Result, Queries=DTO) | Split by intent (state change vs read) | Semantic clarity; fewer changes to queries | Two mental models remain; complexity in behaviors | Medium |
| 4 | Internal Result, External Exceptions | Handlers use Result, behavior converts to exceptions | Encapsulates pattern; consistent external contract | Indirection; added complexity without current need | High |
| 5 | DU Library (ErrorOr/OneOf) | Strongly typed union for success/failure | Rich modeling; fewer runtime surprises | Adds dependency; overkill now | High |

### 3. Evaluation (Qualitative)

| Criterion | 1 Exc | 2 Result | 3 Hybrid | 4 Internal DU | 5 External DU |
|-----------|-------|----------|----------|---------------|---------------|
| Simplicity | High | Low | Medium | Low | Low |
| Migration Cost | Minimal | High | Medium | High | High |
| Alignment w/ Existing Middleware | Strong | Weak (needs adapters) | Medium | Strong | Medium |
| Boilerplate Risk | Low | High | Medium | Medium | Medium |
| Team Cognitive Load | Low | Medium | Medium | High | High |
| Extensibility (Future CQRS/Event sourcing) | Adequate | Adequate | Adequate | Adequate | Adequate |
| Test Ergonomics | Good (Assert.Throws) | Good (pattern match) | Mixed | Mixed | Mixed |

### 4. Decision

Adopt Option 1: Exceptions Only as the standard for the Application layer.

### 5. Rationale
1. Existing robust ExceptionHandlingMiddleware + taxonomy already invested; leveraging sunk cost.
2. Lowest friction for contributors; aligns with Minimal API + ProblemDetails expectations.
3. Performance impact of using exceptions for validation/business rule failures is negligible compared to I/O (DB/network) in current architecture.
4. Simplifies pipeline behaviors: ValidationBehavior will always throw on failure (no dual path), reducing branching complexity.
5. Avoids premature abstraction; Result<T> can still be introduced narrowly later if a use case genuinely benefits (e.g., batch operations returning partial successes).

### 6. Non-Goals
- Do NOT introduce a discriminated-union library now.
- Do NOT refactor domain entities (they already throw DomainException for invariant breaches – remains valid).
- Do NOT wrap all exceptions into Result retroactively.

### 7. Impacted Components
- ValidationBehavior: remove CreateValidationResult pathway and Result branching.
- Existing handlers (if any) returning Result/Result<T>: normalize return type to direct DTO / primitive.
- Tests asserting Result failures: convert to Assert.Throws<SpecificException>().
- Documentation: update guidelines to reflect exceptions-only policy.

### 8. Risks & Mitigations
| Risk | Mitigation |
|------|------------|
| Overuse of generic Exception in new code | Enforce use of specific ApplicationExceptionBase descendants in reviews |
| Future need for composable functional error flows | Introduce localized Result adapter later without global mandate |
| Silent reintroduction of Result by newcomers | Add ADR + CONTRIBUTING note + optional Roslyn analyzer (deferred) |

### 9. Migration Plan (Phased)
1. Code Audit: Identify handlers returning Result / Result<T> (grep: "Result<" in Application layer).
2. Refactor ValidationBehavior: delete Result-return branch; always throw FluentValidation.ValidationException.
3. Update handlers: replace `return Result<T>.Success(obj)` with `return obj;` and throw for failures.
4. Adjust tests: remove assertions over result.IsSuccess; use exception assertions.
5. Docs: Add ADR file (docs/adr/000X-result-handling-decision.md) summarizing this section.
6. Update README / Coding Principles with a short rule: "Handlers throw typed exceptions; they do not return Result<T>".
7. (Optional) Add TODO for analyzer to flag new Result usage in Application.

### 10. Acceptance Criteria
- No handlers in Application return Result / Result<T>.
- ValidationBehavior contains a single failure path (throw).
- All tests green post-refactor.
- ADR committed & referenced in README.

### 11. Deferred Items
- Analyzer enforcement (deferred until need arises).
- Partial-success bulk operations pattern (no current use case).

### 12. Next Immediate Actions
- Implement Steps 1–4 of Migration Plan.

---
## Action Queue (To Execute Next)
- [x] Step 1: Scan Application for Result usages. (Completed – all occurrences identified in UserManagement handlers)
- [x] Step 2: Refactor ValidationBehavior. (Completed – now always throws ValidationException; no Result path)
- [x] Step 3: Normalize handlers. (Completed – all UserManagement handlers return direct DTO/primitive)
- [x] Step 4: Update tests. (Completed – no Result assertions remained; test suite green)
- [x] Step 5: Draft ADR file. (Completed – ADR 0001 committed and updated after cleanup)
- [x] Step 6: Update README excerpt. (Completed – exception-only contract section added & later updated after removal)
- [x] Step 7: Remove legacy Result utilities (Result/Result<T>/extensions) – fully deleted, logging behavior simplified, validators & endpoints refactored.
- [x] Step 8: Endpoint consistency audit – all API endpoints free of Result pattern (`IsSuccess`, `Value`, `Error`).

### Completion Summary (2025-08-11)
All planned migration steps executed plus cleanup extensions (Result class removal, endpoint refactor, validator updates, LoggingBehavior simplification). Build and full test suite pass (160 tests). ADR updated to reflect completed removal. README reflects final state. Pending (Deferred): optional Roslyn analyzer to enforce rule.

### Deferred / Follow-Up Tracking
- Roslyn analyzer to forbid Result-like wrappers (deferred)
- Specialized response type for future batch partial-success scenarios (not currently needed)

### Verification Log
- grep confirmed no `IRequest<Result`, `Result<`, `IsSuccess`, `result.Value`, `result.Error` usages in Application or API layers post-cleanup.
- Build succeeded post-removal. Tests: 160 passed, 0 failed.


---
Document authored automatically based on architectural evaluation.

