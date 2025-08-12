# Troubleshooting Guide

*Common issues and solutions for DotNetSkills development and deployment*

## Quick Diagnostics

### ⚡ First Steps for Any Issue
```bash
# 1. Check overall system health
make status

# 2. Check API logs
make logs

# 3. Test health endpoint
make health  # Should return: 200

# 4. Check Docker container status
docker compose ps
```

## Development Environment Issues

### 🐳 Docker Issues

#### Port Already in Use
**Symptoms**: `Error: bind: address already in use`

**Solutions**:
```bash
# Check what's using the port
lsof -i :8080  # or :5001 for local dev

# Kill the process
pkill -f "dotnet"

# Use different ports
API_PORT=8081 make up
# or
dotnet run --project src/DotNetSkills.API --urls="https://localhost:5002"
```

#### Docker Daemon Not Running
**Symptoms**: `Cannot connect to the Docker daemon`

**Solutions**:
```bash
# macOS - Start Docker Desktop
open -a Docker

# Linux - Start Docker service
sudo systemctl start docker

# Verify Docker is running
docker version
```

#### Out of Disk Space
**Symptoms**: `No space left on device`

**Solutions**:
```bash
# Clean up Docker resources
docker system prune -a
docker volume prune

# Remove unused images
docker image prune -a

# Check disk usage
df -h
docker system df
```

#### Container Won't Start
**Symptoms**: Container exits immediately or restarts repeatedly

**Diagnostic Steps**:
```bash
# Check container logs
docker compose logs api

# Check container status
docker compose ps

# Inspect container details
docker inspect dotnetskills-api

# Run container interactively for debugging
docker run -it --rm dotnetskills:latest /bin/bash
```

### 🔌 Database Connection Issues

#### Cannot Connect to SQL Server
**Symptoms**: `A network-related or instance-specific error occurred`

**Solutions**:
```bash
# Check if database container is running
docker compose ps db

# Test database connectivity
docker compose exec db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P DevPassword123 -C -Q "SELECT 1"

# Restart database service
docker compose restart db

# Check database logs
docker compose logs db

# Reset database completely (WARNING: destroys data)
make down
docker volume rm dotnetskills-sqlserver-data
make up
```

#### Database Connection String Issues
**Symptoms**: Connection timeouts or authentication failures

**Check Configuration**:
```bash
# Verify environment variables
docker compose exec api printenv | grep -i database

# Check connection string format
# Should be: "Server=db,1433;Database=DotNetSkills_Dev;User Id=sa;Password=...;TrustServerCertificate=True;"
```

#### Migration Failures
**Symptoms**: Application fails to start with migration errors

**Solutions**:
```bash
# Check migration status
dotnet ef migrations list --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Reset database and re-run migrations
make down
docker volume rm dotnetskills-sqlserver-data
make up

# Manual migration (if automatic fails)
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

### ⚙️ .NET Development Issues

#### Build Failures
**Symptoms**: Compilation errors, missing dependencies

**Solutions**:
```bash
# Clean and restore
dotnet clean
dotnet restore
dotnet build

# Clear NuGet cache
dotnet nuget locals all --clear

# Check .NET version
dotnet --version  # Should be 9.0+

# Verify project references
dotnet list reference
```

#### Package Restore Issues
**Symptoms**: `Package X could not be found`

**Solutions**:
```bash
# Clear package cache
dotnet nuget locals all --clear

# Restore with verbose output
dotnet restore --verbosity diagnostic

# Check NuGet configuration
dotnet nuget list source

# Add official NuGet source if missing
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
```

#### Hot Reload Not Working
**Symptoms**: Changes not reflected when using `dotnet watch`

**Solutions**:
```bash
# Stop and restart watch
Ctrl+C
dotnet watch run --project src/DotNetSkills.API

# Clear temp files
rm -rf src/DotNetSkills.API/bin
rm -rf src/DotNetSkills.API/obj

# Check file watching limits (Linux)
echo fs.inotify.max_user_watches=524288 | sudo tee -a /etc/sysctl.conf
sudo sysctl -p
```

## Testing Issues

### 🧪 Test Execution Problems

#### Tests Failing Randomly
**Symptoms**: Tests pass individually but fail in suite

**Solutions**:
```bash
# Stop Docker containers that might interfere
make down

# Run tests in isolation
dotnet test tests/DotNetSkills.Domain.UnitTests

# Run specific failing test with verbose output
dotnet test --filter "YourFailingTestName" --logger "console;verbosity=detailed"

# Check for shared state between tests
# Look for static variables, singletons, or database conflicts
```

#### Entity Framework In-Memory Issues
**Symptoms**: `InvalidOperationException` with EF operations

**Solutions**:
```csharp
// Use separate database names for each test
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;

// Dispose context properly in tests
public void Dispose()
{
    _context.Dispose();
}
```

#### Mock Setup Issues
**Symptoms**: Mocks not working as expected

**Common Fixes**:
```csharp
// Ensure methods are virtual for mocking
public virtual async Task<User> GetByIdAsync(UserId id) 

// Use proper mock setup
mock.Setup(x => x.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(user);

// Verify mock calls
mock.Verify(x => x.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
```

## Runtime Issues

### 🔥 Application Startup Problems

#### Application Won't Start
**Symptoms**: Application exits during startup

**Diagnostic Steps**:
```bash
# Check application logs
docker compose logs api

# Look for these common issues:
# - Configuration validation errors
# - Database connection failures
# - Missing environment variables
# - Port binding issues

# Run with detailed logging
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/DotNetSkills.API
```

#### Configuration Validation Errors
**Symptoms**: `OptionsValidationException` during startup

**Solutions**:
```bash
# Check all required environment variables are set
docker compose exec api printenv | grep DOTNETSKILLS

# Validate configuration format
# JWT signing key must be at least 256 bits (32 characters)
# Connection strings must be properly formatted
# CORS origins must be valid URLs
```

#### Middleware Pipeline Issues
**Symptoms**: Requests fail with unexpected errors

**Check Middleware Order**:
```csharp
// Correct order in Program.cs:
app.UseCorrelationId();      // First - for request tracing
app.UsePerformanceLogging(); // Early - for timing
app.UseExceptionHandling();  // Before other middleware
app.UseHttpsRedirection();   // HTTPS enforcement
app.UseCors();              // CORS policy
app.UseAuthentication();     // Before authorization
app.UseAuthorization();      // Before endpoints
```

### 🐌 Performance Issues

#### Slow API Response Times
**Symptoms**: Requests taking longer than expected

**Diagnostic Steps**:
```bash
# Check performance logs
make logs | grep -i "slow\|performance"

# Monitor database performance
docker compose exec db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P DevPassword123 -C -Q "
SELECT 
    qs.execution_count,
    qs.total_elapsed_time / qs.execution_count as avg_time_ms,
    qt.text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
ORDER BY avg_time_ms DESC"

# Check container resource usage
docker stats
```

**Common Solutions**:
- **Database Indexing**: Add indexes for frequently queried columns
- **Connection Pool**: Tune EF Core connection pool settings
- **Caching**: Enable Redis caching for read-heavy operations
- **Async Operations**: Ensure all I/O operations are async

#### Memory Issues
**Symptoms**: High memory usage, OutOfMemoryException

**Diagnostic Steps**:
```bash
# Monitor memory usage
docker stats dotnetskills-api

# Check for memory leaks in logs
make logs | grep -i "memory\|gc\|heap"

# Increase container memory limit
docker update --memory=2g dotnetskills-api
```

### 🔐 Authentication & Authorization Issues

#### JWT Token Issues
**Symptoms**: 401 Unauthorized responses

**Diagnostic Steps**:
```bash
# Check JWT configuration
docker compose exec api printenv | grep JWT

# Verify token is being sent
curl -H "Authorization: Bearer YOUR_TOKEN" http://localhost:8080/api/v1/users

# Check token expiration
# Decode JWT at https://jwt.io to check expiration
```

**Common Issues**:
- **Invalid Signing Key**: Must be at least 256 bits for HS256
- **Wrong Issuer/Audience**: Must match configuration
- **Expired Tokens**: Check token lifetime settings
- **Missing Authorization Header**: Client must send `Bearer <token>`

#### CORS Issues
**Symptoms**: Browser console shows CORS errors

**Solutions**:
```bash
# Check CORS configuration
docker compose exec api printenv | grep CORS

# Test CORS with curl
curl -H "Origin: http://localhost:3000" \
     -H "Access-Control-Request-Method: POST" \
     -H "Access-Control-Request-Headers: Content-Type" \
     -X OPTIONS \
     http://localhost:8080/api/v1/users

# Update CORS configuration to allow your frontend origin
DOTNETSKILLS_Cors__AllowedOrigins__0=http://localhost:3000
```

## Production Issues

### 🚨 Deployment Problems

#### Health Check Failures
**Symptoms**: Load balancer marks service as unhealthy

**Diagnostic Steps**:
```bash
# Test health endpoint directly
curl -v http://your-app/health

# Check health check configuration
# Endpoint: /health
# Expected: 200 OK with JSON response

# Common issues:
# - Database connection failure
# - Application startup not complete
# - Health check timeout too short
```

#### Container Restart Loops
**Symptoms**: Container repeatedly restarts

**Solutions**:
```bash
# Check container logs for crash reasons
docker logs container_id

# Common causes:
# - Configuration errors
# - Database connection failures
# - Out of memory
# - Unhandled exceptions during startup

# Increase health check timeouts
healthcheck:
  interval: 30s
  timeout: 10s
  retries: 5
  start_period: 60s  # Increase for slow startup
```

#### Database Migration Issues in Production
**Symptoms**: Application fails to start with migration errors

**Solutions**:
```bash
# Never run automatic migrations in production
RUN_MIGRATIONS=false

# Generate migration script manually
dotnet ef migrations script --output migration.sql

# Apply migration manually during maintenance window
# Review script before applying
```

### 📊 Monitoring & Logging

#### Missing Logs
**Symptoms**: No logs appearing in monitoring system

**Check Log Configuration**:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      {
        "Name": "Console"  // Ensure console output for containers
      }
    ]
  }
}
```

#### Correlation ID Missing
**Symptoms**: Unable to trace requests across services

**Verification**:
```bash
# Check if correlation ID middleware is enabled
curl -v http://localhost:8080/api/v1/users

# Response should include:
# X-Correlation-Id: generated-uuid

# If missing, check middleware order in Program.cs
app.UseCorrelationId(); // Must be first
```

## Environment-Specific Issues

### 🏠 Local Development

#### SSL Certificate Issues
**Symptoms**: HTTPS certificate errors in browser

**Solutions**:
```bash
# Trust development certificate
dotnet dev-certs https --trust

# Clean and regenerate certificates
dotnet dev-certs https --clean
dotnet dev-certs https --trust

# For Docker, HTTPS is disabled automatically
```

#### File Watching Issues (macOS/Linux)
**Symptoms**: Hot reload not detecting file changes

**Solutions**:
```bash
# macOS - Check file watching limits
launchctl limit maxfiles

# Linux - Increase inotify limits
echo fs.inotify.max_user_watches=524288 | sudo tee -a /etc/sysctl.conf
sudo sysctl -p

# Use polling instead of file watching
DOTNET_USE_POLLING_FILE_WATCHER=true dotnet watch run
```

### ☁️ Cloud Deployment

#### Azure Container Instance Issues
**Symptoms**: Container fails to start in ACI

**Common Issues**:
- **Resource Limits**: Insufficient CPU/memory allocation
- **Network Configuration**: Incorrect port mapping
- **Environment Variables**: Missing or incorrect configuration
- **Image Access**: Registry authentication issues

**Solutions**:
```bash
# Check ACI logs
az container logs --resource-group rg --name container-name

# Increase resources
az container create --cpu 2 --memory 4

# Verify environment variables
az container show --resource-group rg --name container-name --query containers[0].environmentVariables
```

#### Kubernetes Issues
**Symptoms**: Pods failing or not ready

**Diagnostic Steps**:
```bash
# Check pod status
kubectl get pods
kubectl describe pod pod-name

# Check logs
kubectl logs pod-name

# Check events
kubectl get events --sort-by=.metadata.creationTimestamp

# Common issues:
# - Image pull failures
# - Resource constraints
# - Health check failures
# - Configuration errors
```

## Emergency Recovery Procedures

### 🆘 Quick Recovery Steps

#### Service Completely Down
```bash
# 1. Check if it's a container issue
docker compose ps

# 2. Quick restart
make down && make up

# 3. Check logs for errors
make logs

# 4. If database corruption suspected
make down
docker volume rm dotnetskills-sqlserver-data
make up
```

#### Database Corruption
```bash
# 1. Stop application
make down

# 2. Backup current state (if possible)
docker run --rm -v dotnetskills-sqlserver-data:/source alpine tar czf - -C /source . > backup.tar.gz

# 3. Restore from known good backup
# (Restore procedures depend on your backup strategy)

# 4. Restart services
make up
```

#### Configuration Issues
```bash
# 1. Reset to default configuration
cp appsettings.json.backup appsettings.json

# 2. Restart with minimal configuration
docker compose down
docker compose up -d db  # Only database
dotnet run --project src/DotNetSkills.API  # Local with defaults

# 3. Gradually re-add configuration
```

## Getting Help

### 📋 Information to Gather Before Asking for Help

1. **Environment Details**:
   - Operating System (macOS, Windows, Linux)
   - .NET version (`dotnet --version`)
   - Docker version (`docker --version`)

2. **Error Information**:
   - Complete error messages
   - Stack traces
   - Correlation IDs from logs

3. **Steps to Reproduce**:
   - What you were trying to do
   - Commands you ran
   - Expected vs actual behavior

4. **Log Files**:
   ```bash
   # Collect relevant logs
   make logs > api-logs.txt
   docker compose logs db > db-logs.txt
   ```

### 🔍 Self-Help Resources

- **Application Logs**: Check `make logs` first
- **Health Endpoint**: Test `/health` for service status
- **Architecture Guide**: [docs/ARCHITECTURE.md](ARCHITECTURE.md)
- **Developer Guide**: [docs/DEVELOPER-GUIDE.md](DEVELOPER-GUIDE.md)
- **GitHub Issues**: Check existing issues and solutions

---

**🛠️ Troubleshooting Mindset**:
- **Start with logs** - They usually contain the answer
- **Isolate the problem** - Test components individually
- **Check the basics** - Connectivity, permissions, configuration
- **Reproduce consistently** - Intermittent issues are harder to fix
- **Document the solution** - Help future you and others