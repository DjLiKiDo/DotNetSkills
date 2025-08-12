# Docker Guide - DotNetSkills

*Containerized development environment for local development and testing*

## Quick Start

```bash
# Start the full development environment
make up

# Check status and health
make status && make health

# View logs
make logs

# Stop everything
make down
```

**That's it!** The API will be available at http://localhost:8080 with Swagger at http://localhost:8080/swagger.

## Service Architecture

### Core Services
| Service | Description | Port | Health Check |
|---------|-------------|------|--------------|
| `api` | .NET 9 API application | 8080 | `/health` endpoint |
| `db` | SQL Server 2022 Developer | 1433 | SQL connection test |
| `redis` | Redis cache (optional) | 6379 | Redis ping command |

### Service Dependencies
```
┌─────────────────┐    ┌─────────────────┐
│   API Service   │───▶│  SQL Server DB  │
│   (Port 8080)   │    │   (Port 1433)   │
└─────────────────┘    └─────────────────┘
        │
        ▼ (optional)
┌─────────────────┐
│   Redis Cache   │
│   (Port 6379)   │
└─────────────────┘
```

## Essential Commands

### Basic Operations
```bash
# Start services (API + Database)
make up
# or: docker compose up -d

# Start with Redis cache
make full
# or: docker compose --profile cache --profile full up -d

# Stop all services  
make down
# or: docker compose down

# Clean restart
make restart
# or: ./Docker/scripts/restart.sh --clean
```

### Monitoring & Debugging
```bash
# Check service status
make status
# or: docker compose ps

# View API logs (follow mode)
make logs
# or: ./Docker/scripts/logs.sh api --follow

# Check health endpoint
make health
# or: curl http://localhost:8080/health

# View all service logs
docker compose logs -f

# View specific service logs
docker compose logs -f db
docker compose logs -f api
```

### Development Operations
```bash
# Rebuild API after code changes
docker compose build api
docker compose up -d api

# Force rebuild (ignore cache)
docker compose build --no-cache api

# Restart specific service
docker compose restart api
docker compose restart db

# Remove and recreate containers
docker compose up -d --force-recreate
```

## Environment Configuration

### Default Ports
- **API**: http://localhost:8080 (HTTP only in containers)
- **Database**: localhost:1433 (SQL Server)
- **Redis**: localhost:6379 (when using `make full`)

### Environment Variables
Key variables that can be customized via `.env` file or environment:

```bash
# Service Ports
API_PORT=8080
DB_PORT=1433
REDIS_PORT=6379

# Database
DB_SA_PASSWORD=DevPassword123
DB_NAME=DotNetSkills_Dev

# Application Features
RUN_MIGRATIONS=true       # Auto-run EF migrations on startup
SEED_DATA=true           # Enable data seeding (if implemented)
JWT_ENABLED=true         # Enable JWT authentication
SWAGGER_ENABLED=true     # Enable Swagger UI

# Performance Monitoring
PERF_SLOW_THRESHOLD=500  # Log slow requests (ms)
PERF_ENABLE_METRICS=true # Enable performance metrics
```

### Custom Port Configuration
```bash
# Use different ports to avoid conflicts
API_PORT=8081 DB_PORT=1434 make up

# Or set in .env file
echo "API_PORT=8081" > .env
echo "DB_PORT=1434" >> .env
make up
```

## Docker Compose Profiles

### Default Profile (API + Database)
```bash
make up
# Starts: api, db
```

### Cache Profile (+ Redis)
```bash
make full
# Starts: api, db, redis
```

### Profile Usage
```bash
# Start only redis for testing
docker compose --profile cache up -d redis

# Start everything
docker compose --profile full up -d
```

## Data Persistence

### Volumes
| Volume | Purpose | Location |
|--------|---------|----------|
| `sqlserver_data` | SQL Server database files | `/var/opt/mssql` |
| `dp_keys` | ASP.NET DataProtection keys | `/app/.aspnet/DataProtection-Keys` |
| `redis_data` | Redis data (when enabled) | `/data` |
| `./logs` | Application logs (host mount) | `/app/logs` |

### Why DataProtection Keys?
Persisting DataProtection keys prevents:
- JWT tokens becoming invalid after container restarts
- Session cookies being invalidated
- Authentication issues in multi-replica scenarios

### Database Migrations
```bash
# Automatic migrations (default in development)
RUN_MIGRATIONS=true

# Manual migrations (recommended for production)
RUN_MIGRATIONS=false
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

## Health Monitoring

### Health Check Endpoints
```bash
# API health check
curl http://localhost:8080/health
# Returns: {"status": "Healthy", "checks": {...}}

# Check all service health
make status
# Shows: container status, health check results

# Detailed container inspection
docker inspect dotnetskills-api | jq '.[0].State.Health'
```

### Health Check Configuration
- **API**: HTTP GET `/health` every 30s (after 60s startup)
- **Database**: SQL connection test every 30s (after 30s startup)  
- **Redis**: Ping command every 30s

## Networking

### Container Communication
- Services communicate via Docker network: `dotnetskills-network`
- API connects to database via hostname: `db:1433`
- Redis accessible via hostname: `redis:6379`

### External Access
- API exposed on host port 8080
- Database exposed on host port 1433 (for development tools)
- Redis exposed on host port 6379 (when enabled)

### HTTPS Configuration
**Development**: HTTPS redirection disabled in containers (HTTP only)
**Production**: Terminate TLS at load balancer/ingress controller

## Troubleshooting

### Common Issues

#### Port Conflicts
```bash
# Check what's using the port
lsof -i :8080

# Use different ports
API_PORT=8081 make up
```

#### Database Connection Issues
```bash
# Test database connectivity
docker compose exec db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P DevPassword123 -C -Q "SELECT 1"

# Check database health
make status

# Restart database
docker compose restart db
```

#### Container Build Issues
```bash
# Clear Docker cache
docker builder prune -f

# Rebuild from scratch
make down
docker compose build --no-cache
make up
```

#### Out of Space Issues
```bash
# Clean up Docker resources
docker system prune -a
docker volume prune

# Check space usage
docker system df
```

### Log Analysis
```bash
# Follow all logs
docker compose logs -f

# Filter for errors
docker compose logs api | grep -i error

# View logs with timestamps
docker compose logs -t api

# Get last 100 lines
docker compose logs --tail 100 api
```

### Database Troubleshooting
```bash
# Connect to SQL Server container
docker compose exec db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P DevPassword123 -C

# Check database status
SELECT name, database_id, state_desc FROM sys.databases;

# Reset database (WARNING: destroys all data)
make down
docker volume rm dotnetskills-sqlserver-data
make up
```

## Advanced Usage

### Custom Docker Compose Override
Create `docker-compose.override.yml` for local customizations:
```yaml
version: '3.8'
services:
  api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - DOTNETSKILLS_Jwt__Enabled=false
    volumes:
      - ./custom-logs:/app/custom-logs
```

### Integration with External Tools
```bash
# Database management tools
# Connect to: localhost:1433, sa/DevPassword123

# Redis management (when enabled)
# Connect to: localhost:6379, password: DevRedisPassword123!

# API testing with curl
curl -X GET http://localhost:8080/api/v1/users \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Multi-Environment Setup
```bash
# Development (default)
make up

# Staging-like environment
ASPNETCORE_ENVIRONMENT=Staging \
SWAGGER_ENABLED=false \
RUN_MIGRATIONS=false \
make up

# Production-like environment
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Performance Optimization

### Container Resources
```bash
# Monitor resource usage
docker stats

# Set resource limits in docker-compose.yml
services:
  api:
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: '1.0'
        reservations:
          memory: 512M
          cpus: '0.5'
```

### Database Performance
```bash
# Enable query logging for performance analysis
docker compose exec api env | grep DOTNETSKILLS_Database
# Add: DOTNETSKILLS_Database__EnableQueryLogging=true
```

---

**🐳 Docker Best Practices Summary**:
- Use `make` commands for consistent operations
- Monitor health checks for service status
- Persist important data with named volumes
- Clean up resources regularly to avoid space issues
- Use profiles to control which services run
- Check logs first when troubleshooting issues
