# Current Task – IOptions Pattern Implementation (as of 2025-08-08)

## Key discussion points and decisions

- Implement the Microsoft Options pattern across the solution following the project’s configuration strategy.
- Database configuration moved to strongly-typed options with startup validation; EF Core DbContext configured via IOptions.
- Azure Key Vault integrated for production-only secrets using SecretClient + Azure configuration provider; custom prefix mapping for secrets.
- Extended Options to API concerns: Swagger, CORS, JWT.
	- Added validators: JwtOptionsValidator and CorsOptionsValidator; used ValidateOnStart.
	- Middleware is enabled conditionally based on options (Swagger in dev and Enabled; JWT when Enabled).
- Swagger title/version driven by configuration via options-aware SwaggerExtensions overload.
- Configuration pipeline standardized in Program.cs: appsettings → appsettings.{ENV}.json → env vars (DOTNETSKILLS_) → Key Vault (prod) → User Secrets (dev).
- Unit tests added for options validation (Infrastructure: Database; API: JWT and CORS). All tests passing.

## Current status

- Build: Succeeds (warnings remain in some API endpoints unrelated to options work).
- Tests: 71 total, 0 failed, 0 skipped.
- Options implemented and validated for:
	- Infrastructure: DatabaseOptions (+ DatabaseOptionsValidator).
	- API: SwaggerOptions, CorsOptions (+ CorsOptionsValidator), JwtOptions (+ JwtOptionsValidator).
- Program/API DI updated to bind, validate, and conditionally wire middleware.
- Appsettings updated for base/dev/staging/prod with new sections.

## Important context for continuation

- Configuration precedence and sources:
	1) appsettings.json
	2) appsettings.{Environment}.json
	3) Environment variables with prefix DOTNETSKILLS_
	4) Azure Key Vault (production only)
	5) User secrets (development)

- Key Vault secret naming (prefix mapping):
	- Secrets starting with "DotNetSkills-" are considered; "--" maps to ":" to form sectioned keys.
	- Example: DotNetSkills-Jwt--SigningKey → "Jwt:SigningKey".

- CORS behavior: when AllowCredentials is true, AllowedOrigins must be explicit (no "*"). Default policy name is "AllowAll".
- JWT behavior: when Enabled, Issuer, Audience, and SigningKey are required; SigningKey typically supplied by Key Vault in production.

## Pending items / next steps

- Optional: Add SwaggerOptionsValidator to enforce Title/Version when Enabled.
- Clean up compiler warnings in API endpoints (unused variables, async methods without await).
- Optional integration tests: verify Key Vault secret prefix mapping and configuration override behavior.
- Documentation: brief README/Docs note on how to configure options per environment.

## Relevant code snippets

Small excerpts for reference (full files in `src/`):

```csharp
// src/DotNetSkills.API/Configuration/Options/JwtOptions.cs
public class JwtOptions
{
		public bool Enabled { get; set; } = false;
		public string Issuer { get; set; } = string.Empty;
		public string Audience { get; set; } = string.Empty;
		public string SigningKey { get; set; } = string.Empty; // Key Vault in prod
		public int TokenLifetimeMinutes { get; set; } = 60;
}
```

```csharp
// src/DotNetSkills.API/Configuration/Options/CorsOptions.cs
public class CorsOptions
{
		public string PolicyName { get; set; } = "AllowAll";
		public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
		public string[] AllowedMethods { get; set; } = new[] { "GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS" };
		public string[] AllowedHeaders { get; set; } = new[] { "*" };
		public bool AllowCredentials { get; set; } = false;
}
```

```json
// src/DotNetSkills.API/appsettings.Development.json (excerpt)
{
	"Swagger": { "Enabled": true, "Title": "DotNetSkills API", "Version": "v1" },
	"Cors": {
		"PolicyName": "Default",
		"AllowedOrigins": [ "https://localhost:5173" ],
		"AllowCredentials": true
	},
	"Jwt": {
		"Enabled": false,
		"Issuer": "dev-issuer",
		"Audience": "dev-audience",
		"SigningKey": "dev-signing-key-override-in-prod",
		"TokenLifetimeMinutes": 60
	}
}
```

## Links to documentation and resources

- Configuration strategy: docs/IOptions-Configuration-Strategy-v1.1.md
- DI architecture: docs/DependencyInjection-Architecture.md
- Project coding standards: .github/instructions/dotnet.instructions.md
- DDD/.NET architecture guidance: .github/instructions/dotnet-arquitecture.instructions.md

## Quick stats

- Branch: janitor-plan-c3 (default: main)
- Last verified: dotnet test → All tests passing (71/71)

