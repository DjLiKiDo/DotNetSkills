## Feature: Internal Identity Provider & Authentication Tokens

Role: Product Owner
Date: 2025-08-13
Status: Draft (v1)

### 1. Problem / Opportunity
Current platform lacks a first‑party identity provider to issue and manage JWT access tokens. We rely on stubbed auth enabling roles but have no secure password-based login, no credential lifecycle controls, and no standardized token issuance pipeline. This blocks:
- External user onboarding without manual DB seeding
- Secure session revocation / rotation policies
- Future SSO / federation roadmap (needs internal issuer first)
- Consistent enforcement of password & account policies

### 2. Vision
Deliver a secure, extensible internal identity service that can:
1. Store and verify user credentials (initially email + password)
2. Issue signed JWT access tokens enriched with domain claims (roles, team memberships, permissions)
3. Support future enhancements: refresh tokens, MFA, external IdP federation, audit trails, account lockout

### 3. Goals (MVP Scope)
Must (MVP):
- Password-based login endpoint: POST /api/v1/auth/login
- PBKDF2 password hashing (configurable iterations & salt length)
- Credential storage decoupled from User aggregate (new UserCredential entity keyed by UserId)
- JWT issuance using existing multi-key signing provider & claims population service
- Basic security policies: minimum password length, inactive/suspended user rejection
Should:
- Rate-limit failed logins (future middleware stub)
- Structured audit log events (LoginSuccess, LoginFailed)
Won't (this iteration):
- Refresh tokens / silent reauth
- Password reset workflow
- MFA / TOTP
- Account lockout after repeated failures (placeholder metrics only)

### 4. Success Metrics
- P0: 100% of interactive API access uses issued JWT (no anonymous modifications)
- < 50 ms median token issuance (excluding claim hydration DB calls)
- ≥ 90% test coverage on hashing & auth service logic
- Zero plaintext passwords in logs / DB

### 5. High-Level Architecture Additions
- Domain: UserCredential (aggregate? -> simple entity referencing UserId as PK; no business logic except state) 
- Application: Repositories + Auth DTOs (LoginRequest, AuthenticationResponse) + IPasswordHasher abstraction
- Infrastructure: EF config + repository + PBKDF2PasswordHasher implementation + migration (UserCredentials table)
- API: TokenService orchestrator + AuthEndpoints (login) + integration with existing JwtOptions & ClaimsPopulationService

### 6. User Stories
1. As a user, I can submit email & password to receive a JWT if valid and active.
	 Acceptance:
	 - Returns 200 with token + expiry when credentials correct & status Active
	 - Returns 401 for bad credentials or inactive/suspended
	 - Response includes: accessToken, expiresAt (UTC), tokenType="Bearer"
2. As an admin, I can create users and then set an initial password (future story: password assignment / invite flow) – OUT OF SCOPE NOW.
3. As the system, I can hash and store a password without ever persisting plaintext.

### 7. Data Model Changes
Table: UserCredentials
- UserId (PK, FK -> Users)
- PasswordHash (nvarchar 512)
- PasswordAlgorithm (nvarchar 32) e.g., PBKDF2-SHA256
- Iterations (int)
- Salt (varbinary / base64 stored inside hash payload) -> decide to embed in composite hash string
- CreatedAtUtc (datetime2)
- UpdatedAtUtc (datetime2 nullable)

Hash Format (proposed):
`pbkdf2-sha256$<iterations>$<saltBase64>$<hashBase64>`

### 8. API Contract (Draft)
Request: POST /api/v1/auth/login
{
	"email": "user@example.com",
	"password": "Secret123!"
}

Response 200:
{
	"accessToken": "<jwt>",
	"expiresAt": "2025-08-13T12:34:56Z",
	"tokenType": "Bearer"
}

Errors:
- 400: validation (missing email/password, malformed email)
- 401: invalid credentials or inactive/suspended

### 9. Security & Compliance
- PBKDF2 with min 100k iterations (configurable; default 150k) – performance trade-off measured
- Per-user unique salt
- Constant-time comparison
- No differential error messages (generic 401)
- Logging: only userId/email on success, no password fragments

### 10. Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|-----------|
| Weak hashing params degrade security | High | Central config + unit test guard |
| Timing attacks on credential check | Medium | Constant-time comparison utility |
| Brute force credential stuffing | High | Future rate limiting + IP / user attempt counters |
| Key rotation mismatch | Medium | Use existing multi-key resolver via provider |

### 11. Open Questions / Future Epics
- Refresh & revoke tokens (blacklist vs short-lived + rotate)
- Password reset & invite flow (email service integration)
- MFA (TOTP / WebAuthn)
- External IdP federation (OIDC) while retaining internal accounts
- Account lockout policies & adaptive risk scoring

### 12. Delivery Plan (Incremental)
Sprint 1 (This PR scope): Domain entity, repository, password hasher, login endpoint, migration, tests.
Sprint 2: Rate limiting, audit events, minimal password policy enforcement service.
Sprint 3: Refresh tokens + revoke list.

### 13. Definition of Done
- All new code follows DDD layer boundaries & project instructions
- Migration created & applied locally
- Tests: unit (hasher), integration (login success/fail), coverage threshold met
- No secrets committed; config sourced via env/user-secrets
- Swagger updated with /auth/login endpoint & security scheme unchanged

### 14. Acceptance Checklist (MVP)
- [ ] UserCredential table exists
- [ ] PBKDF2 hashing service returns deterministic hash (same input) & differing salt each new hash
- [ ] Login returns JWT with expected standard + custom claims
- [ ] Inactive user login blocked
- [ ] Suspended user login blocked
- [ ] Incorrect password -> 401 (generic)
- [ ] No plaintext password logged

### 15. Out of Scope (Explicit)
- UI flows, password reset, registration self-service
- Admin-set initial password endpoint (placeholder for next iteration)

---
Owner: Product Owner (Identity Initiative)
Next Update: After implementation PR merged
