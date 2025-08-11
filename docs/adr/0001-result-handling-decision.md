# ADR 0001: Application Error Signaling Strategy – Exceptions Only

Date: 2025-08-11
Status: Accepted
Supersedes: None
Superseded by: N/A

## 1. Context
Historically the Application layer mixed two patterns for signaling failure: (1) throwing typed exceptions (DomainException, ValidationException, etc.) and (2) returning Result / Result<T>. The ValidationBehavior also implemented dual behavior (building failed Result vs throwing). This inconsistency increased cognitive load and made contribution guidelines ambiguous.

## 2. Decision
Adopt a single strategy: Application handlers always return pure DTO / primitive (or nullable) values and signal failures exclusively via exceptions. Validation failures throw FluentValidation.ValidationException. Business rule violations throw DomainException. Unexpected conditions bubble up as ApplicationException (or derived). Middleware translates exceptions into ProblemDetails.

## 3. Rationale
1. Leverages existing ExceptionHandling middleware (taxonomy already invested).
2. Simplifies MediatR pipeline behaviors (ValidationBehavior now single path).
3. Reduces boilerplate (no Success/Failure wrapping).
4. Easier onboarding – one mental model.
5. Performance difference negligible vs I/O cost; premature optimization avoided.

## 4. Considered Options
| Option | Summary | Outcome |
|--------|---------|---------|
| Exceptions Only | Throw for all failures | Chosen |
| Result Everywhere | Return Result<T> always | Rejected – boilerplate & adapters |
| Hybrid (Commands Result) | Split semantics | Rejected – keeps duality |
| Internal Result -> Exceptions | Abstraction layer | Rejected – complexity w/o need |
| DU Library (ErrorOr/OneOf) | Discriminated union | Rejected – additional dep early |

## 5. Impacts
- ValidationBehavior simplified to always throw.
- UserManagement handlers refactored to return raw DTOs or primitives and throw exceptions.
- API endpoints updated to rely on middleware instead of inspecting Result objects.
- LoggingBehavior simplified (removed Result branches).
- Result/Result<T> types REMOVED (cleanup completed  / 2025-08-11). 
- Future handlers must follow this rule.

## 6. Migration Summary
Steps executed:
1. Searched for Result usages in Application layer.
2. Updated ValidationBehavior to throw ValidationException only.
3. Refactored all UserManagement commands/queries & handlers removing Result wrappers.
4. Confirmed tests contained no Result assertions needing changes.
5. Added this ADR.
6. Pending: README and coding principles update; optional removal of unused Result classes in a later change set.

## 7. Consequences
Positive:
- Consistency, simpler error flow, clearer tests (Assert.Throws or no exception).
Negative / Trade-offs:
- Loss of explicit success/failure object for functional chaining.
- Future bulk/partial success operations may need a custom structured response type.

## 8. Follow-ups / Open Questions
- Add Roslyn analyzer to forbid introducing Result-like wrappers (deferred).
- Introduce specialized response shapes for batch operations when required (when first batch use case appears).
- Consider enriching exception taxonomy with specific authorization exception if patterns emerge.

## 9. Rules (Enforced Going Forward)
- Handlers: `Task<TResponse>`; never `Task<Result<TResponse>>`.
- Validation errors: throw `ValidationException`.
- Domain rule violations: throw `DomainException`.
- Authorization/business policy breaches in Application layer: throw `DomainException` (or dedicated subclass later).
- Unexpected: wrap or rethrow as `ApplicationException` to preserve stack trace.

## 10. References
- CurrentTask.md architectural decision log.
- ExceptionHandlingMiddleware (API layer).

---
Decision Owner: Architecture group
Reviewers: Backend team
