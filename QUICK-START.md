# 🚀 DotNetSkills Quick Start Guide

*Get up and running with the DotNetSkills API in under 10 minutes*

## Prerequisites Checklist
- [ ] [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed
- [ ] [Docker Desktop](https://www.docker.com/products/docker-desktop) running
- [ ] Git repository cloned

## 🐳 Option 1: Docker (Recommended - 2 minutes)

**Fastest way to get the API running:**

```bash
# 1. Start the complete environment
make up
# or: docker compose up -d

# 2. Wait for health checks (30-60 seconds)
make status

# 3. Verify API is running
make health
# Should return: 200

# 4. Open Swagger UI
open http://localhost:8080/swagger
```

**That's it!** The API is running with:
- API: http://localhost:8080
- Swagger: http://localhost:8080/swagger  
- Database: SQL Server on localhost:1433
- Health Check: http://localhost:8080/health

## 💻 Option 2: Local Development (5 minutes)

**For active development with hot reload:**

```bash
# 1. Restore dependencies
dotnet restore

# 2. Build solution
dotnet build

# 3. Start only the database
docker compose up -d db

# 4. Run migrations
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# 5. Start the API
dotnet run --project src/DotNetSkills.API
```

API will be available at: https://localhost:5001

## ✅ Verify Everything Works

### Test the API
```bash
# Health check
curl http://localhost:8080/health

# Get users (should return empty array)
curl http://localhost:8080/api/v1/users

# Expected: {"users":[],"totalCount":0,"pageSize":10,"currentPage":1,"totalPages":0}
```

### Run Tests
```bash
# Quick test run
make test
# or: dotnet test
```

## 🔧 Essential Commands

```bash
# Development
make build          # Build solution
make test           # Run all tests  
dotnet run --project src/DotNetSkills.API  # Run API locally

# Docker environment
make up             # Start API + Database
make full           # Start all services (API + Database + Redis)
make logs           # View API logs
make status         # Check service health
make down           # Stop everything

# Database
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
dotnet ef migrations add MigrationName --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

## 🚨 Common Issues & Quick Fixes

| Issue | Solution |
|-------|----------|
| Port 8080 in use | `API_PORT=8081 docker compose up -d` |
| Database connection failed | `docker compose restart db` |
| Build errors | `dotnet clean && dotnet restore && dotnet build` |
| Tests failing | Check if Docker containers are running |

## 📚 Next Steps

Once you have the API running:

1. **Explore the API**: Visit [Swagger UI](http://localhost:8080/swagger) to see all endpoints
2. **Understand the architecture**: Read [Architecture Guide](docs/ARCHITECTURE.md)
3. **Start developing**: Check [Developer Guide](docs/DEVELOPER-GUIDE.md) for workflows
4. **Run tests**: See [Testing Guide](docs/TESTING-GUIDE.md) for testing practices

## 🆘 Need Help?

- **Common issues**: [Troubleshooting Guide](docs/TROUBLESHOOTING.md)
- **Docker problems**: [Docker Guide](Docker/README.md)
- **Development questions**: [Developer Guide](docs/DEVELOPER-GUIDE.md)
- **Architecture questions**: [Architecture Overview](docs/ARCHITECTURE.md)

---

**⚡ Pro Tip**: Use `make help` to see all available commands at any time.