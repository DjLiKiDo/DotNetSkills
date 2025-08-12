# Docker Scripts for DotNetSkills Local Environment

This directory contains utility scripts to manage the local Docker development environment for DotNetSkills. These scripts provide enterprise-grade DevOps functionality for local development, testing, and troubleshooting.

## Available Scripts

### 🔄 restart.sh
**Purpose**: Restart the local Docker environment with various configurations

```bash
./restart.sh                 # Standard restart with API and DB
./restart.sh --clean         # Clean restart (safe, keeps data)
./restart.sh --full-clean    # Complete fresh start (⚠️ data loss)
./restart.sh --db-only       # Database only for local API development
./restart.sh --full          # All services including Redis cache
./restart.sh --no-cache      # Rebuild Docker images without cache
```

**Features**:
- Automatic prerequisite checking
- Environment file management
- Service health monitoring
- Comprehensive status reporting

---

### 🧹 cleanup.sh
**Purpose**: Clean up Docker resources with granular control

```bash
./cleanup.sh                    # Interactive cleanup mode
./cleanup.sh --containers-only  # Safe cleanup - stops containers but keeps data
./cleanup.sh --full            # Complete cleanup - removes everything
./cleanup.sh --volumes-only     # Remove data volumes only (⚠️ DATA LOSS)
./cleanup.sh --images-only      # Remove project-specific Docker images
```

**Features**:
- Interactive and automated modes
- Safety confirmations for destructive operations
- Selective cleanup options
- Comprehensive cleanup reporting

---

### 📊 status.sh
**Purpose**: Comprehensive environment status monitoring

```bash
./status.sh                  # Full status report
./status.sh --services       # Services status only
./status.sh --health         # Health checks only
./status.sh --resources      # Resource usage only
./status.sh --connectivity   # Connectivity tests only
```

**Provides**:
- Docker service status
- Container health checks
- Resource usage (CPU, Memory)
- Network connectivity tests
- Service endpoint availability
- Database connection validation
- Environment configuration overview

---

### 📝 logs.sh
**Purpose**: Advanced Docker container log viewing

```bash
./logs.sh                    # Show all logs (last 100 lines)
./logs.sh api --follow       # Follow API logs in real-time
./logs.sh db --tail 50       # Show last 50 database log lines
./logs.sh all --since 1h     # Show all logs from last hour
```

**Features**:
- Service-specific log filtering
- Real-time log following
- Time-based filtering
- Customizable tail options
- Integration with grep for advanced filtering

---

## Quick Start Guide

### 1. Initial Setup
```bash
# Make scripts executable (if not already done)
chmod +x Docker/scripts/*.sh

# Start the environment
./Docker/scripts/restart.sh
```

### 2. Daily Development Workflow
```bash
# Check environment status
./Docker/scripts/status.sh

# View API logs during development
./Docker/scripts/logs.sh api --follow

# Clean restart after changes
./Docker/scripts/restart.sh --clean
```

### 3. Troubleshooting
```bash
# Full environment status
./Docker/scripts/status.sh

# Check specific service health
./Docker/scripts/status.sh --health

# View recent error logs
./Docker/scripts/logs.sh api --since 10m | grep -i error
```

### 4. Environment Cleanup
```bash
# Safe cleanup (keeps data)
./Docker/scripts/cleanup.sh --containers-only

# Complete cleanup (removes all data)
./Docker/scripts/cleanup.sh --full
```

## Script Architecture

All scripts follow enterprise DevOps best practices:

- **Error Handling**: Comprehensive error checking with `set -euo pipefail`
- **Colored Output**: Consistent color coding for status, success, warnings, and errors
- **Help Documentation**: `--help` option for all scripts
- **Safety Checks**: Confirmation prompts for destructive operations
- **Logging**: Structured output for monitoring and debugging
- **Portability**: Works across different Unix-like systems

## Environment Configuration

Scripts automatically detect and use:
- `.env` files for environment configuration
- `docker-compose.yml` for service definitions
- Project-specific Docker networks and volumes

## Security Considerations

- Scripts validate Docker daemon accessibility
- No sensitive data is logged or displayed
- Confirmation required for data deletion operations
- Safe defaults for all operations

## Integration with CI/CD

These scripts are designed to integrate with:
- GitHub Actions workflows
- Azure DevOps pipelines
- Local development IDEs
- Monitoring and alerting systems

## Troubleshooting

### Common Issues

1. **Docker not running**
   ```bash
   # Start Docker Desktop or Docker daemon
   ./Docker/scripts/status.sh  # Will show Docker status
   ```

2. **Port conflicts**
   ```bash
   # Check .env file for port configuration
   # Default ports: API=8080, DB=1433, Redis=6379
   ```

3. **Permission issues**
   ```bash
   chmod +x Docker/scripts/*.sh
   ```

4. **Database connection issues**
   ```bash
   ./Docker/scripts/logs.sh db  # Check database logs
   ./Docker/scripts/status.sh --health  # Verify health checks
   ```

### Getting Help

- Use `--help` with any script for detailed usage information
- Check service logs: `./Docker/scripts/logs.sh [service]`
- View comprehensive status: `./Docker/scripts/status.sh`
- Review main documentation: `docs/SECURITY-DEPLOYMENT.md`

## Contributing

When modifying scripts:
1. Follow existing error handling patterns
2. Add help documentation for new options
3. Include safety checks for destructive operations
4. Test on different environments
5. Update this README for new features

---

**Note**: These scripts are part of the DotNetSkills DevOps infrastructure and follow Clean Architecture principles for maintainability and extensibility.