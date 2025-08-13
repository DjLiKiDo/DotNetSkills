# Feature 02 – Functional Analyst Specification

Internal Identity Provider & Authentication Tokens (MVP)

Date: 2025-08-13  
Author: Functional Analyst  
Status: Draft v1 (aligned to Product Owner Feature v1)  
Related Document: `01 - ProductOwner - feature.md`

---
## 1. Purpose & Context
Introduce a first‑party internal identity capability to authenticate platform users via email + password and issue signed JWT access tokens enriched with domain claims. Replaces stubbed auth and enables secure controlled access as prerequisite for future enhancements (refresh tokens, MFA, external federation).

---
## 2. In-Scope (MVP)
- Password-based authentication endpoint `POST /api/v1/auth/login`
- Secure credential storage separated from `User` aggregate (`UserCredential` entity)
- PBKDF2 (SHA-256) hashing with configurable iterations (min 100,000; default 150,000) and per-user salt
- JWT issuance using existing signing / key rotation infrastructure
- Account status validation (Active only)
- Uniform failure response (401) for invalid credentials, inactive or suspended users
- Minimal validation (email format, non-empty password) via Application layer validator

### Out of Scope (Confirmed)
Registration self-service, password reset, password change, initial password provisioning endpoint, refresh tokens, MFA, account lockout, rate limiting implementation (only future placeholder), audit event persistence (placeholder), SSO federation.

---
## 3. Stakeholders
| Role | Interest | Responsibility |
|------|----------|----------------|
| Product Owner | Delivery of secure auth foundation | Defines vision & priorities |
| Functional Analyst | Requirements completeness & traceability | This specification |
| Backend Developers | Implement vertical slice | Follow DDD/Clean architecture constraints |
| Security Officer | Hashing strength / compliance | Review security controls |
| QA / Test Engineers | Validate acceptance criteria | Test design & execution |
| DevOps | Config & secret management | Parameterize iterations, keys |

---
## 4. Glossary
| Term | Definition |
|------|------------|
| UserCredential | Entity storing password hash metadata for a user (1:1 with User) |
| PBKDF2 | Password-Based Key Derivation Function 2 (SHA-256 variant) |
| Iterations | Hash work factor controlling computational cost |
| Salt | Random value per credential preventing rainbow table attacks |
| JWT | JSON Web Token access token signed by platform keys |

---
## 5. User Stories & Acceptance Criteria

### US-001 Authenticate with Email & Password
As a platform user, I can submit my email and password to receive a JWT when valid so that I can access authenticated API endpoints.

#### Acceptance Criteria (Gherkin)
AC-001 Successful authentication (active user)
```
Given an existing active user with email "user@example.com" and a stored PBKDF2 password hash for "Secret123!"
When I POST /api/v1/auth/login with body { "email": "user@example.com", "password": "Secret123!" }
Then the response status is 200
And the response JSON contains fields accessToken, expiresAt (UTC ISO-8601), tokenType = "Bearer"
And accessToken is a well-formed JWT signed by a recognized key
```
AC-002 Invalid password
```
Given an existing active user with email "user@example.com" and a stored password hash for "Secret123!"
When I POST /api/v1/auth/login with { "email": "user@example.com", "password": "WrongPass!" }
Then the response status is 401
And the response body does not reveal whether the email or password was incorrect
```
AC-003 Unknown email
```
Given no user exists with email "ghost@example.com"
When I POST /api/v1/auth/login with { "email": "ghost@example.com", "password": "AnyPass1!" }
Then the response status is 401
And the error is generic (no enumeration of reasons)
```
AC-004 Inactive user
```
Given a user with email "inactive@example.com" whose status is Inactive
When I POST /api/v1/auth/login with correct password
Then the response status is 401
```
AC-005 Suspended user
```
Given a user with email "suspended@example.com" whose status is Suspended
When I POST /api/v1/auth/login with correct password
Then the response status is 401
```
AC-006 Validation errors
```
When I POST /api/v1/auth/login with missing or malformed email or empty password
Then the response status is 400
And a validation problem details payload is returned
```
AC-007 No plaintext password exposure
```
Given any authentication attempt (success or failure)
Then no logs or persisted records contain the plaintext password
```
AC-008 Deterministic hash re-check
```
Given the same password is hashed during verification with stored salt & iterations
Then the recomputed hash segment matches the stored hash segment (constant-time comparison)
```
AC-009 Performance threshold (token issuance logic)
```
Given typical system load and a cache-warm scenario
When 50 consecutive successful logins are performed
Then median end-to-end issuance time (excluding DB claim hydration if explicitly measured separately) is < 50 ms
```

---
## 6. Functional Requirements (FR)
| ID | Requirement | Rationale |
|----|-------------|-----------|
| FR-001 | System SHALL expose POST /api/v1/auth/login accepting JSON { email, password }. | Provide entry point |
| FR-002 | System SHALL validate email format and non-empty password before processing. | Input integrity |
| FR-003 | System SHALL locate user by normalized (lowercase) email. | Consistency |
| FR-004 | System SHALL reject authentication if user not found with 401 (generic). | Security (enumeration prevention) |
| FR-005 | System SHALL retrieve associated UserCredential (1:1). | Needed for verification |
| FR-006 | System SHALL use stored algorithm id and iterations to recompute PBKDF2 hash. | Support algorithm agility |
| FR-007 | System SHALL perform constant-time comparison between stored hash and candidate hash. | Mitigate timing attacks |
| FR-008 | System SHALL reject users with status != Active (401). | Policy enforcement |
| FR-009 | System SHALL issue JWT including standard claims (sub, exp, iat) and domain claims (roles, team memberships) via existing claim population service. | Authorization continuity |
| FR-010 | System SHALL return JSON response { accessToken, expiresAt, tokenType="Bearer" } on success. | Contract clarity |
| FR-011 | System SHALL never log plaintext passwords or derived partial secrets. | Security & compliance |
| FR-012 | System SHALL support configurable PBKDF2 iterations via configuration with enforced minimum threshold. | Security tunability |
| FR-013 | System SHALL store password hash in format pbkdf2-sha256$iterations$saltBase64$hashBase64. | Interoperable, parseable |
| FR-014 | System SHALL create UserCredential at user provisioning time (out of scope activation path stub). | Future readiness |
| FR-015 | System SHALL handle absent credential record as authentication failure (401 generic). | Security parity |
| FR-016 | System SHALL normalize email input (trim, lowercase). | Lookup reliability |
| FR-017 | System SHOULD produce structured audit intents (LoginSuccess, LoginFailed) as stubs (no persistence yet). | Future audit extensibility |
| FR-018 | System SHALL encapsulate password hashing behind `IPasswordHasher` abstraction. | Clean architecture |
| FR-019 | System SHALL reject requests with Content-Type other than application/json with 415 or validation error (per existing API pattern). | Consistency |
| FR-020 | System SHALL allow algorithm evolution (future algorithms) without modifying existing stored hashes. | Future flexibility |

---
## 7. Non-Functional Requirements (NFR)
| ID | Category | Requirement |
|----|----------|------------|
| NFR-001 | Security | Minimum PBKDF2 iterations ≥ 100,000; default 150,000. |
| NFR-002 | Security | Unique 16-byte (≥128-bit) random salt per credential. |
| NFR-003 | Security | Constant-time hash comparison. |
| NFR-004 | Performance | Median token issuance latency (excluding claim hydration) < 50 ms under nominal load. |
| NFR-005 | Observability | Log only userId or email (normalized) and outcome (success/failure); no granular failure reason. |
| NFR-006 | Testability | ≥ 90% unit test coverage for hashing & auth service logic. |
| NFR-007 | Maintainability | Hash format remains backward compatible for future algorithms. |
| NFR-008 | Reliability | Hashing failures produce generic 500 responses without leaking details. |
| NFR-009 | Compliance | No storage/logging of plaintext or reversible password artifacts. |
| NFR-010 | Configurability | Iteration count adjustable via environment variable / configuration with guard enforcing minimum. |
| NFR-011 | Scalability | Design supports horizontal scaling (stateless login except DB read). |
| NFR-012 | Extensibility | Interface abstraction enables adding Argon2 or bcrypt in future without API change. |
| NFR-013 | Monitoring | Expose counters: login.success, login.failure (for future rate-limit decisions). |
| NFR-014 | Security | Uniform 401 for invalid credentials, inactive, suspended to prevent user enumeration. |

---
## 8. Use Cases

### UC-001 Authenticate User
| Item | Description |
|------|-------------|
| Primary Actor | User |
| Goal | Obtain JWT for authenticated session |
| Preconditions | User exists, credential stored, system operational |
| Trigger | User submits login request |
| Main Success Scenario | 1. User submits credentials 2. System normalizes email 3. System fetches User + Credential 4. System validates status Active 5. System recomputes hash 6. Constant-time compare passes 7. Claims populated 8. JWT generated 9. Response 200 with token payload |
| Extensions | 4a. User status not Active -> 401 | 
|  | 5a. Credential missing -> 401 |
|  | 6a. Comparison fails -> 401 |
|  | 3a. User not found -> 401 |
| Postconditions | Successful auth: JWT issued; no secret exposures |
| Non-Functional Notes | Bound by performance & security NFRs |

### UC-002 Hash Password (System)
System process executed during credential creation or password change (future).
- Inputs: plaintext password, configured iteration count, random salt generator
- Outputs: structured hash string
- Rules: Enforce minimum length & complexity (basic: length ≥ 8 for now) – complexity extension deferred.

### UC-003 Validate Account Status
- Determine if user status is Active. Otherwise deny authentication uniformly.

---
## 9. Data Model & Structures

### Entity: UserCredential
| Field | Type | Constraints | Notes |
|-------|------|------------|-------|
| UserId (PK, FK) | GUID | Not null | 1:1 with User |
| PasswordHash | NVARCHAR(512) | Not null | Composite format string |
| PasswordAlgorithm | NVARCHAR(32) | Not null | e.g., pbkdf2-sha256 |
| Iterations | INT | Not null | ≥ 100,000 |
| CreatedAtUtc | DATETIME2 | Not null | Timestamp creation |
| UpdatedAtUtc | DATETIME2 | Nullable | On rotation/change |

### Hash Format Rationale
`pbkdf2-sha256$<iterations>$<saltBase64>$<hashBase64>`
- Human-auditable & parseable.
- Allows per-user iteration adjustment if future tuning needed.
- Backward compatibility: Additional variants could prefix algorithm (e.g., `pbkdf2-sha512$...`).

### Relationships
- User (Aggregate Root) -> UserCredential (dependent, not aggregate root) – credential does not enforce complex invariants; business rules minimal.

### Lifecycle
1. Provision user (future story) – create credential
2. Authenticate – read only
3. Future: Rotate / update on password change – update hash / iterations
4. Future: Deactivate – user status blocks auth, credential persists

---
## 10. Business Rules (BR)
| ID | Rule | Source |
|----|------|--------|
| BR-001 | Only Active users may authenticate. | PO Spec |
| BR-002 | Suspended or Inactive users receive generic 401. | PO Spec |
| BR-003 | Password minimum length 8 characters (initial baseline). | Security baseline |
| BR-004 | Hash iterations must meet or exceed configured minimum (≥100k). | Security requirement |
| BR-005 | Each credential must have unique salt. | Standard practice |
| BR-006 | Do not reveal account existence via error differentiation. | Security requirement |
| BR-007 | Plaintext password never persisted or logged. | Compliance |
| BR-008 | Hash comparison must be constant-time. | Security requirement |
| BR-009 | Email normalization (trim + lowercase) before lookup. | Consistency |
| BR-010 | Missing credential treated as authentication failure (generic). | Security consistency |

---
## 11. Error Handling & Edge Cases
| Scenario | HTTP | Response Style | Notes |
|----------|------|---------------|-------|
| Malformed email / empty password | 400 | Validation ProblemDetails | FluentValidation |
| User not found | 401 | Generic auth failure | Prevent enumeration |
| Credential absent | 401 | Generic auth failure | Prevent inference |
| User inactive/suspended | 401 | Generic auth failure | Unified response |
| Incorrect password | 401 | Generic auth failure | Timing-safe compare |
| Unsupported algorithm id | 500 | Internal error | Logged (no detail to client) |
| Hash parse failure (corruption) | 500 | Internal error | Logged / flagged |
| Configuration iterations below min | Startup failure | Config validation | Guardrail test |
| DB connectivity failure | 500 | Standard error | Outside scope of differentiation |

### Edge Cases
- Leading/trailing spaces in email -> trimmed, lowercased.
- Extremely long password: Still processed (bounded by reasonable max, e.g., 512 chars – implicit, can add guard later).
- Simultaneous login attempts: Stateless; each processed independently.
- Algorithm migration later: Parser chooses by prefix; current MVP only pbkdf2-sha256.

---
## 12. Logging & Observability
- Log template: `Authentication attempt {EmailNormalized} outcome={Outcome}` (Outcome: Success/Failure)
- No stack traces for expected 401 failures.
- Counters: login.success, login.failure (increment) – placeholder instrumentation.
- Performance metric: authentication.duration (ms) around full use case.

---
## 13. Security Controls Summary
| Control | Mechanism |
|---------|-----------|
| Credential confidentiality | PBKDF2 with per-user salt, high iterations |
| Replay prevention | JWT short-lived (existing config) – not altered here |
| Enumeration resistance | Uniform 401 responses |
| Timing attack mitigation | Constant-time compare function |
| Parameter hardening | Minimum iteration guard + validation tests |
| Secrets hygiene | No plaintext in storage/logging |

---
## 14. Traceability Matrix (Excerpt)
| Artifact | References |
|----------|------------|
| Goal: Password login | FR-001..FR-010, UC-001, US-001 |
| Goal: Secure hashing | FR-006, FR-012, FR-013, BR-004, BR-005, NFR-001..003 |
| Goal: Status enforcement | FR-008, BR-001, BR-002, AC-004, AC-005 |
| Goal: Generic failures | FR-004, FR-015, BR-006, NFR-014, AC-002..005 |
| Goal: Performance | NFR-004, AC-009 |
| Goal: No plaintext | FR-011, BR-007, AC-007, NFR-009 |
| Goal: Extensibility | FR-018, FR-020, NFR-007, NFR-012 |

### Acceptance Criteria Mapping
| AC ID | FR | BR | NFR |
|-------|----|----|-----|
| AC-001 | FR-001..010 | BR-001,003,005,008,009 | NFR-001..005 |
| AC-002 | FR-001,004,006,007 | BR-006,008 | NFR-014 |
| AC-003 | FR-001,004 | BR-006 | NFR-014 |
| AC-004 | FR-001,008 | BR-001,006 | NFR-014 |
| AC-005 | FR-001,008 | BR-002,006 | NFR-014 |
| AC-006 | FR-001,002 | BR-003 |  |
| AC-007 | FR-011 | BR-007 | NFR-009 |
| AC-008 | FR-006,007 | BR-004,008 | NFR-001..003 |
| AC-009 | FR-009 |  | NFR-004 |

---
## 15. Test Coverage Strategy
| Test Type | Focus |
|-----------|-------|
| Unit – Hasher | Iteration minimum, format parse, deterministic recompute, random salt uniqueness |
| Unit – Auth Service | Success path, user not found, invalid password, inactive user, suspended user, missing credential |
| Integration – Endpoint | 200 success, 401 variants, 400 validation |
| Performance (micro) | Median issuance timing harness (optional gating) |
| Security (static) | Ensure no logs contain password via log interceptor test |

---
## 16. Risks & Mitigations (Reconciled)
(See PO spec Section 10 – aligned.) Added note: Algorithm migration risk mitigated by prefix format separation.

---
## 17. Assumptions
- User provisioning path will ensure credential creation before first login attempt (manual setup in MVP).
- Existing JWT infrastructure already supports multiple signing keys and key rotation without change.
- Claim population service performant enough not to breach <50 ms target (else excluded from metric per definition).

---
## 18. Open Questions (Deferred)
| ID | Question | Status |
|----|----------|--------|
| OQ-001 | Exact JWT expiry (duration) standardization for all clients? | Use existing global config for now |
| OQ-002 | Password complexity policy (beyond length) introduction timeline? | Future security iteration |
| OQ-003 | Rate limiting strategy (per IP vs per user) | To define Sprint 2 |

---
## 19. Implementation Guidance (Non-Design-Enforcing)
- Application layer command/query: Introduce `LoginCommand` + `AuthenticationResponse`.
- Validator for `LoginCommand` (email format, not empty password).
- `IPasswordHasher` interface + `PBKDF2PasswordHasher` in Infrastructure.
- Repository: `IUserCredentialRepository` with `GetByUserIdAsync`.
- Endpoint file: `AuthEndpoints.cs` under appropriate context folder.
- Domain: `UserCredential` entity within `UserManagement` context.
- Migration: `AddUserCredential` creates table with constraints.

---
## 20. Definition of Done Alignment
All Acceptance Checklist items map to FR / AC sets above ensuring verifiable closure. No additional scope added.

---
## 21. Approval
| Role | Name | Date | Status |
|------|------|------|--------|
| Product Owner | (TBD) |  | Pending |
| Security Review | (TBD) |  | Pending |
| Architecture | (TBD) |  | Pending |

---
## 22. Change Log
| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-08-13 | Functional Analyst | Initial draft aligned to PO feature v1 |

---
End of Document.
