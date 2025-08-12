# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DotNetSkills is a modern project management API built with .NET 9, demonstrating Clean Architecture and Domain-Driven Design principles. The solution showcases enterprise-grade development practices with JWT authentication, Minimal APIs, and comprehensive testing strategies.

## Quick Reference

**📖 For detailed information, see our specialized guides:**
- 🚀 **[Quick Start Guide](QUICK-START.md)** - Get running in 10 minutes
- 💻 **[Developer Guide](docs/DEVELOPER-GUIDE.md)** - Daily workflows and patterns
- 📐 **[Architecture Guide](docs/ARCHITECTURE.md)** - System design and patterns
- 🧪 **[Testing Guide](docs/TESTING-GUIDE.md)** - Testing practices and setup
- 🐳 **[Docker Guide](Docker/README.md)** - Container development

## Architecture Summary

**Clean Architecture**: API → Application → Domain ← Infrastructure
**Bounded Contexts**: UserManagement, TeamCollaboration, ProjectManagement, TaskExecution

## Essential Commands

```bash
# Build and test
make build && make test

# Local development
dotnet run --project src/DotNetSkills.API

# Docker environment  
make up     # Start API + Database
make health # Check health
make down   # Stop everything

# Database migrations
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
dotnet ef migrations add MigrationName --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

## Project Structure

```
src/
├── DotNetSkills.API/              # Minimal APIs, authentication
├── DotNetSkills.Application/      # CQRS, DTOs, validation  
├── DotNetSkills.Domain/           # Entities, events, business logic
└── DotNetSkills.Infrastructure/   # EF Core, repositories
```

## Key Patterns

**MediatR Pipeline**: Logging → Validation → Performance → DomainEvents
**Endpoint Pattern**: `async (Command/Query dto, IMediator mediator) => await mediator.Send(dto)`
**Exception Handling**: Exception-only contract (no Result<T> wrappers) per [ADR-0001](docs/adr/0001-result-handling-decision.md)
**Repository Pattern**: Interfaces in Application, implementations in Infrastructure with caching decorators

## Development Workflow

**Adding a Feature**:
1. **Domain**: Entities, value objects, events in bounded context
2. **Application**: Repository contract, Command/Query, Handler, Validator, Mapping
3. **Infrastructure**: Repository implementation + cached decorator + DI registration
4. **API**: Endpoint calling `mediator.Send()`

**Environment Variables**: Prefixed with `DOTNETSKILLS_*`
- `RUN_MIGRATIONS=true` - Auto-run EF migrations
- `JWT_ENABLED=true` - Enable authentication
- `SWAGGER_ENABLED=true` - Enable API docs

## Key Files

- `src/DotNetSkills.API/Program.cs` - Application startup
- `src/DotNetSkills.Application/DependencyInjection.cs` - MediatR behaviors
- `src/DotNetSkills.Infrastructure/DependencyInjection.cs` - EF Core setup
- `Makefile` - Development commands
- `docs/DotNet Coding Principles.md` - Coding standards

## Essential Conventions

**✅ DO**:
- Keep business rules in Domain entities
- Use MediatR for all API operations
- Throw exceptions, let middleware handle responses
- Follow existing patterns and naming

**❌ DON'T**:
- Bypass MediatR in endpoints
- Return `IQueryable` from repositories
- Use Result<T> wrappers (deprecated)
- Inject `DbContext` directly into endpoints