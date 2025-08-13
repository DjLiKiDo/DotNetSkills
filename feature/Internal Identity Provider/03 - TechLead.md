# Feature: Internal Identity Provider & Authentication Tokens (MVP)
**Document:** Tech Lead Architecture & Implementation Guide  
**Version:** v1 (MVP Alignment)  
**Date:** 2025-08-13  
**Owner:** Tech Lead  
**Related Docs:** 01 - ProductOwner Feature.md, 02 - FunctionalAnalyst Specification.md

---
## 1. Purpose
Provide authoritative technical direction for implementing the Internal Identity Provider MVP using existing Clean Architecture / DDD foundations. This document translates product & functional requirements into explicit architectural decisions, component responsibilities, sequencing, and guardrails. It also defines test, migration, and operational expectations.

---
## 2. Scope Alignment Summary
In-Scope (MVP):
- Password authentication (email + password) → JWT issuance (`POST /api/v1/auth/login`).
- Persistent credential storage separate from `User` aggregate (`UserCredential`).
- PBKDF2-SHA256 hashing, configurable iterations (default 150k, min 100k enforced).
- Account status enforcement (only `Active`).
- Generic 401 for: unknown user, inactive/suspended, invalid password, missing credential.
- No refresh tokens, no password reset, no registration flows (future).

Out-of-Scope Confirmed: refresh tokens, MFA, lockout, rate limiting (impl), password lifecycle endpoints, SSO federation.

---
## 3. Architecture Overview
Layer impacts:
- Domain: Introduce `UserCredential` entity (dependent, not aggregate root). Business rules minimal (structural integrity only).
- Application: Add authentication use case (`AuthenticateUserCommand` + handler) and abstractions (`IUserCredentialRepository`, `IPasswordHasher`, `IAuthTokenService`). Validation via FluentValidation.
- Infrastructure: EF Core mapping + repository + PBKDF2 hasher implementation + token service (JWT signing orchestrator using existing key provider & claims population service).
- API: Auth endpoints group `AuthEndpoints` with `POST /api/v1/auth/login`.

Dependency Direction Preserved: API → Application → Domain; Infrastructure → Application/Domain. No domain dependency on crypto libs or JWT.

---
## 4. Key Architecture Decisions (Pre-Approved)
| ID | Decision | Rationale | Status |
|----|----------|-----------|--------|
| ADR-IDP-001 | `UserCredential` is separate entity keyed by `UserId` (1:1) | Avoid polluting `User` aggregate with sensitive hash fields; facilitates rotation | Accepted |
| ADR-IDP-002 | Hash format: `pbkdf2-sha256$iterations$saltB64$hashB64` | Human readable, supports algorithm agility | Accepted |
| ADR-IDP-003 | Minimum iterations = 100,000, default config = 150,000 | Security baseline, tunable performance | Accepted |
| ADR-IDP-004 | Generic 401 for all auth failures | Enumeration resistance | Accepted |
| ADR-IDP-005 | Constant-time comparison utility inside hasher impl | Timing attack mitigation | Accepted |
| ADR-IDP-006 | Algorithm prefix mandatory in stored format | Future algorithm migration (Argon2, PBKDF2-SHA512) | Accepted |
| ADR-IDP-007 | Domain events NOT emitted for login attempts (MVP) | Keep auth path lean; future audit service will handle | Accepted |
| ADR-IDP-008 | Handler returns DTO, not raw JWT object | API contract stability | Accepted |
| ADR-IDP-009 | No result wrappers (`Result<T>`) | Project ADR-0001 (exception contract) alignment | Accepted |

(If formal ADR files needed later, replicate table rows into `/docs/adr/`.)

---
## 5. Domain Model Additions
### 5.1 Entity: UserCredential
Purpose: Store password hashing metadata and derived hash.

Fields:
- `UserId` (strongly typed id, PK & FK)  
- `PasswordHash` (string, <=512)  
- `PasswordAlgorithm` (string, <=32)  
- `Iterations` (int)  
- `CreatedAtUtc` (DateTime)  
- `UpdatedAtUtc` (DateTime?)  

Behavior: None beyond static factory + update method for future rotation. No domain invariants beyond non-null and iteration guard.

Access Pattern: Read-heavy (authentication). Writes only during provisioning / password change (future).

### 5.2 Strongly Typed IDs
If not existing, add `UserCredentialId`? Not required—primary key is `UserId`. Avoid extra synthetic ID.

---
## 6. Application Layer Contracts
Interfaces (new):
- `IUserCredentialRepository`
  - `Task<UserCredential?> GetByUserIdAsync(UserId id);`
  - `Task<UserCredential?> GetByEmailAsync(EmailAddress email);` (optional shortcut to join User & Credential; consider projection) OR keep separation: reuse existing `IUserRepository.GetByEmailAsync` then call credential repo. PICK: Keep minimal → only `GetByUserIdAsync` for now.
  - `Task AddAsync(UserCredential credential);` (for future provisioning)
  - `Task UpdateAsync(UserCredential credential);` (future rotation)

- `IPasswordHasher`
  - `string Hash(string password);`
  - `bool Verify(string password, string storedHash);`
  - `PasswordHashMetadata Parse(string storedHash);` (internal use; may be hidden if verify encapsulates)

- `IAuthTokenService`
  - `Task<AuthenticationResponse> CreateTokenAsync(User user, CancellationToken ct);`

Record DTOs:
- `AuthenticateUserCommand` (email, password) → returns `AuthenticationResponse`.
- `AuthenticationResponse` (accessToken, expiresAtUtc, tokenType="Bearer").

Validator:
- `AuthenticateUserCommandValidator`: email format, password not empty, length >=8.

Handler Flow:
1. Normalize email (lowercase/trim).
2. Fetch user by email (fail → generic 401). 
3. Check status Active (else generic 401). 
4. Load credential `GetByUserIdAsync` (null → generic 401). 
5. Verify password (false → generic 401). 
6. Create JWT via `IAuthTokenService`. 
7. Return response.

Exceptions: Throw custom `AuthenticationFailedException` (maps to 401 generically via middleware) OR reuse a generic `UnauthorizedAccessException`. Preferred: New `AuthenticationFailedException` deriving from `Exception` to differentiate in logs but still mapped to 401 with generic message.

---
## 7. Infrastructure Components
### 7.1 EF Core Mapping
Table: `UserCredentials` (schema default). Fluent config:
- Key: `HasKey(x => x.UserId)`.
- Relationship: `HasOne<User>().WithOne().HasForeignKey<UserCredential>(x => x.UserId).OnDelete(DeleteBehavior.Cascade)` (confirm cascade acceptable; OK for MVP).
- Property constraints: lengths; index on `UserId` implicit via PK.

### 7.2 Repository Implementation
`EfUserCredentialRepository` using `ApplicationDbContext`.
Avoid exposing IQueryable. Optimize fetch with simple query including only needed columns.

### 7.3 Password Hasher (PBKDF2)
- Use `Rfc2898DeriveBytes` (SHA256) with `saltLength = 16 bytes`.
- Guard iteration count: read from `Identity:PasswordHashing:Iterations`; if < min (100k) → throw on startup (DI validation) or first use; choose startup validation (add options validation service).
- Hash size: 32 bytes (256-bit) derived key.
- Format builder & parser with strict segment count (4).
- Constant-time compare: XOR aggregate without early exit.

### 7.4 Token Service
`JwtAuthTokenService` (wrap existing signing infrastructure). Steps:
1. Resolve signing credentials via existing key provider.
2. Build claims: `sub`, `iat`, `exp`, plus roles/teams via existing ClaimPopulationService (inject). 
3. Expiry: reuse configured `Jwt:AccessTokenMinutes` (existing) else default (e.g., 60). Return expiry in UTC.
4. Return assembled `AuthenticationResponse`.

### 7.5 Configuration & Options
Add section (example):
```
"Identity": {
  "PasswordHashing": {
    "Algorithm": "pbkdf2-sha256",
    "Iterations": 150000,
    "MinIterations": 100000,
    "SaltBytes": 16,
    "KeyBytes": 32
  }
}
```
Provide PO-approved defaults in `appsettings.Development.json`; production can override via env vars `DOTNETSKILLS_IDENTITY__PASSWORDHASHING__ITERATIONS`.

### 7.6 Startup Registration
In `Infrastructure.DependencyInjection` (or Application if abstraction placement demands):
- Register options + validation.
- Register `IPasswordHasher` (singleton).
- Register `IUserCredentialRepository` (scoped).
- Register `IAuthTokenService` (scoped).

---
## 8. API Layer
### 8.1 Endpoint Group
File: `src/DotNetSkills.API/Endpoints/UserManagement/AuthEndpoints.cs` (Context: UserManagement).
```
var group = app.MapGroup("/api/v1/auth").WithTags("Auth");
group.MapPost("/login", Authenticate).AllowAnonymous()
  .WithSummary("Authenticate using email and password")
  .Produces<AuthenticationResponse>(StatusCodes.Status200OK)
  .Produces(StatusCodes.Status400BadRequest)
  .Produces(StatusCodes.Status401Unauthorized);
```
Delegate pattern: `(AuthenticateUserCommand command, IMediator mediator) => mediator.Send(command)`.

### 8.2 Error Handling
`AuthenticationFailedException` mapped by global exception middleware to 401 with generic message: "Invalid credentials". No distinction across failure types.

---
## 9. Migration Plan
1. Add entity + config.
2. Create migration: `AddUserCredentials`.
3. Columns as defined; ensure collation/case-insensitivity handled at lookup layer (email normalized). No unique index required beyond PK.
4. Deployment: apply migration before enabling endpoint in production.
5. Backfill (manual for MVP): seed credential rows for existing test users.

Rollback: dropping table safe (stateless tokens). Coordinate with auth feature flag if needed.

---
## 10. Testing Strategy (Expanded)
Unit Tests:
- `PBKDF2PasswordHasherTests`: hash format, parse error cases, iteration guard, verify success/failure, salt uniqueness (statistical), constant-time check (time difference not asserted—structural path).
- `AuthenticateUserCommandHandlerTests`: success, user not found, inactive, suspended, missing credential, invalid password.

Integration Tests:
- Endpoint tests exercising 200 / 400 / 401 paths (see acceptance criteria).
- JWT structure test: decode header/payload (no signature validation duplication) ensure expected claims presence.

Performance (Optional Harness):
- Measure median issuance time for repeated handler execution with pre-seeded user + credential (can mark `[Collection("Performance")]`).

Logging Safety Test:
- Intercept logger to assert password value absence (no substring of provided plaintext).

Coverage Target Enforcement:
- Add coverage gate in CI (future). For now include coverage report generation.

---
## 11. Observability & Metrics
Introduce counters via abstraction (placeholder):
- `login.success`, `login.failure` (increment inside handler after verification). For MVP can log structured events; metrics wiring deferred.
Latency Logging: Surround handler with stopwatch (or rely on existing PerformanceBehavior—ensure it captures this command).

---
## 12. Security & Compliance Controls
| Control | Implementation |
|---------|----------------|
| Password Strength | Min length 8 (validator) |
| Hashing Strength | Iterations validated >= MinIterations at startup |
| Salt Uniqueness | RNGCryptoServiceProvider / RandomNumberGenerator.Create() |
| Timing Attack Mitigation | Constant-time compare (XOR accumulation) |
| Enumeration Resistance | Single exception → 401 generic |
| Secrets Hygiene | No password in logs; do not store partial hash components separately |
| Future Algorithm Agility | Algorithm prefix + parser switch logic |

Threat Considerations: Resist offline cracking (high iterations), enumeration, timing leaks; deferred: brute force mitigation (rate limiting), credential stuffing detection.

---
## 13. Risks & Mitigations (Tech Focus)
| Risk | Impact | Mitigation |
|------|--------|-----------|
| Performance regression at high iteration count | Elevated login latency | Allow tuning via config + monitor p95; benchmark 150k locally |
| Misconfiguration lower iterations in prod | Security weakening | Startup validation + log critical + refuse to run |
| Incorrect generic error leaks stack trace | Enumeration | Middleware ensures sanitized ProblemDetails |
| Future algorithm migration complexity | Tech debt | Central parser + strategy interface from start |
| Silent logging of passwords | Compliance breach | Code review checklist + unit log interception test |

---
## 14. Execution Plan (Implementation Order)
1. Domain: Add `UserCredential` entity + strongly typed adjustments.
2. Infrastructure: EF configuration + migration generation.
3. Application: Command, DTOs, validator, interfaces.
4. Infrastructure: Repositories, hasher, token service, DI registrations + options validation.
5. API: Endpoint mapping.
6. Tests: unit (hasher, handler) then integration (endpoint).
7. Seed local dev credential for a test user manually (script or README note).
8. Run full test suite & verify coverage target.

Feature Flag (Optional): Could wrap endpoint registration behind config `Features:Auth:Enabled` for safe release.

---
## 15. Open Questions (For Later Iterations)
- Standardized JWT expiries across contexts—should we centralize into `SecurityOptions` aggregate configuration? (Deferred)
- Introduce refresh tokens vs short-lived access tokens only? (Planned Sprint 3)
- Password complexity policy beyond length (entropy / character classes)? (Security review)
- Audit persistence layer design (domain events vs outbox) for login events.

---
## 16. Developer Checklist (DoD Alignment)
- [ ] Entity + migration compiled
- [ ] Iteration guard in place
- [ ] Hasher tests ≥ 90% coverage
- [ ] Handler tests cover all 401 variants
- [ ] Endpoint documented in Swagger/OpenAPI
- [ ] No plaintext password logging (verified)
- [ ] Generic 401 path confirmed
- [ ] JWT contains required domain claims
- [ ] Performance smoke (<50 ms median locally, excluding first-run JIT)

---
## 17. Appendices
### 17.1 Pseudocode – Password Verify
```
Verify(password, stored):
  parts = stored.split('$')
  if parts.length != 4: throw FormatException
  algo, iterStr, saltB64, hashB64 = parts
  assert algo == 'pbkdf2-sha256'
  iterations = int(iterStr)
  derived = PBKDF2(password, base64(saltB64), iterations, keyLen=32, HMAC-SHA256)
  return ConstantTimeEquals(base64(derived), hashB64)
```

### 17.2 Constant-Time Compare
```
bool ConstantTimeEquals(byte[] a, byte[] b) {
  if (a.Length != b.Length) return false;
  int diff = 0;
  for (int i=0; i<a.Length; i++) diff |= a[i] ^ b[i];
  return diff == 0;
}
```

### 17.3 Example Stored Hash
`pbkdf2-sha256$150000$3qH3TBROLq4oJ2t0Yv2eVg==$R8Sx6y0fZg8k27+5W8fM4kvh2L8Q5X3wNRK5u6p6T4E=` (sample only)

---
## 18. Future Extension Hooks
- Add `IPasswordPolicyEvaluator` for complexity rules.
- Introduce `IRefreshTokenStore` with secure rotation.
- Add Argon2 implementation behind same `IPasswordHasher`.
- Plug metrics into OpenTelemetry counters.

---
## 19. Approval & Change Control
This document governs MVP implementation. Deviations require tech lead review & update to section 4 table.

---
**End of Document**
