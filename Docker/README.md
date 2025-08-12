# Docker Runbook - DotNetSkills

This runbook documents how to build and run the local stack, what each service does, and key notes for persistence and security.

## Services
- db: SQL Server 2022 (Developer) with healthcheck.
- api: ASP.NET Core 9 API (Clean Architecture) with health at /health.
- redis (optional): For caching/DP keys (future).

## Environment
- Compose file: docker-compose.yml at repo root.
- Env vars: .env (optional). Sample values are embedded via defaults in compose.
- Ports: API 8080 (HTTP), DB 1433.

Tip: Use the Makefile shortcuts from repo root:
- make up / make down / make restart
- make logs / make status / make health
- make full (start redis profiles) / make cache

## Volumes
- sqlserver_data: persists SQL Server data.
- dp_keys: persists ASP.NET Core DataProtection keys at /app/.aspnet/DataProtection-Keys.
- logs: host folder mapped to /app/logs in API.

Why dp_keys? Persisting keys avoids invalidating cookies/tokens across container restarts or multiple replicas.

## HTTPS in Docker
HTTPS redirection is disabled inside containers for now. Expose HTTP 8080 locally; terminate TLS at an ingress/proxy in cloud, or configure ASP.NET Core HTTPS when needed.

## Health
- API: GET http://localhost:8080/health → 200 OK when healthy.
- DB: Compose healthcheck executes SELECT 1.

## Migrations and Seeding
- RUN_MIGRATIONS=true triggers automatic EF migrations at startup.
- SEED_DATA=true enables optional seeding (if implemented).

## Troubleshooting
- If API build fails on analyzers: the Dockerfile performs a restore after copying full source to ensure transitive analyzers are restored.
- EF model warnings (shadow FKs): fixed by centralizing TeamMember relationships.
- Slow DB health: first calls may be slower on cold start; adjust thresholds if needed.

## Next (optional)
- Add Redis-backed DataProtection for multi-replica scenarios.
- Add make/scripts for up/down/logs.
- Add staging/production compose overlays.
