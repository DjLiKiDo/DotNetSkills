# DotNetSkills

[![Build Status](https://img.shields.io/badge/build-passing-green?style=flat-square)](https://github.com/DjLiKiDo/DotNetSkills)
[![.NET 9](https://img.shields.io/badge/.NET-9.0-5027D5?style=flat-square&logo=.net)](https://dotnet.microsoft.com)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-blue?style=flat-square)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![Domain-Driven Design](https://img.shields.io/badge/Design-DDD-orange?style=flat-square)](https://domainlanguage.com/ddd/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)](LICENSE)

> A modern, secure, and scalable project management API built with .NET 9, demonstrating Clean Architecture and Domain-Driven Design principles.

## 🚀 Quick Start

**New to the project? Get running in under 10 minutes:**

👉 **[5-Minute Quick Start Guide](QUICK-START.md)** ← Start here!

```bash
# Docker (fastest way)
make up && make health

# Local development  
dotnet run --project src/DotNetSkills.API
```

## Overview

DotNetSkills is a comprehensive project management API designed to showcase enterprise-grade development practices. Built with .NET 9 and following Clean Architecture principles, it demonstrates modern .NET development techniques including Domain-Driven Design, JWT authentication, and comprehensive testing strategies.

## Features

- **Clean Architecture**: Well-organized layers with proper dependency inversion
- **Domain-Driven Design**: Rich domain models with business logic encapsulated in entities
- **JWT Authentication**: Stateless security with role-based access control
- **Minimal APIs**: Lean, performance-focused endpoint implementations
- **Entity Framework Core**: Modern data access with SQL Server
- **Comprehensive Testing**: Unit, integration, and end-to-end test coverage
- **Domain Events**: Decoupled communication between aggregates
- **Strong Typing**: Value objects and strongly-typed identifiers throughout

## Architecture

The solution follows Clean Architecture principles with these layers:

```
┌─────────────────────────────────────────────┐
│                 API Layer                   │
│        (Controllers, Middleware)            │
└─────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────┐
│             Application Layer               │
│     (Use Cases, DTOs, Interfaces)           │
└─────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────┐
│              Domain Layer                   │
│    (Entities, Value Objects, Events)        │
└─────────────────────────────────────────────┘
                      ▲
┌─────────────────────────────────────────────┐
│            Infrastructure Layer             │
│   (Repositories, Database, External APIs)   │
└─────────────────────────────────────────────┘
```

### Project Structure

```
src/
├── DotNetSkills.API/              # Minimal APIs, authentication, validation
├── DotNetSkills.Application/      # Use cases, DTOs, service interfaces
├── DotNetSkills.Domain/           # Core business logic and domain models
└── DotNetSkills.Infrastructure/   # Database, repositories, external services

tests/
├── DotNetSkills.API.UnitTests/
├── DotNetSkills.Application.UnitTests/
├── DotNetSkills.Domain.UnitTests/
└── DotNetSkills.Infrastructure.UnitTests/
```

## Core Entities

- **User**: Core entity with role-based permissions (Admin, ProjectManager, Developer, Viewer)
- **Team**: Collection of users working together with many-to-many relationships
- **Project**: Belongs to one team, contains tasks with status management
- **Task**: Single-user assignment with one-level subtask nesting support

## Technology Stack

- **.NET 9**: Latest framework with nullable reference types enabled
- **Entity Framework Core**: Modern ORM with SQL Server provider
- **Minimal APIs**: Lightweight, performance-focused endpoints
- **JWT Authentication**: Stateless security implementation
- **AutoMapper**: Entity ↔ DTO transformations
- **FluentValidation**: Comprehensive input validation
- **xUnit + FluentAssertions**: Testing framework and assertions
- **Testcontainers**: Integration testing with real databases
- **Serilog**: Structured logging
- **Docker**: Containerization support

## Getting Started

**Prerequisites:** [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0), [Docker Desktop](https://www.docker.com/products/docker-desktop)

**Quick Setup:**
```bash
# Clone and start with Docker (recommended)
git clone https://github.com/DjLiKiDo/DotNetSkills.git
cd DotNetSkills
make up
```

**Detailed Setup Options:**
- 🚀 **[Quick Start Guide](QUICK-START.md)** - Get running in 10 minutes
- 💻 **[Developer Guide](docs/DEVELOPER-GUIDE.md)** - Local development setup
- 🐳 **[Docker Guide](Docker/README.md)** - Containerized development

## 📚 Documentation Navigation

### 👋 New Developer? Start Here
- 🚀 **[Quick Start Guide](QUICK-START.md)** - Get running in 10 minutes
- 💻 **[Developer Guide](docs/DEVELOPER-GUIDE.md)** - Daily workflows and commands
- 🐳 **[Docker Guide](Docker/README.md)** - Containerized development

### 🏗️ Understanding the System
- 📐 **[Architecture Overview](docs/ARCHITECTURE.md)** - System design and patterns
- 📋 **[Coding Standards](docs/DotNet%20Coding%20Principles.md)** - Development guidelines
- 📖 **[API Documentation](http://localhost:8080/swagger)** - Interactive endpoint docs

### 🧪 Testing & Quality
- 🔬 **[Testing Guide](docs/TESTING-GUIDE.md)** - Testing setup and practices
- 🛡️ **Security Guidelines** - Authentication and authorization

### 🚀 Operations & Deployment
- 🌐 **[Deployment Guide](docs/DEPLOYMENT-GUIDE.md)** - Production deployment
- 🔧 **[Troubleshooting](docs/TROUBLESHOOTING.md)** - Common issues and solutions
- 📊 **[Monitoring](docs/MONITORING.md)** - Performance and health monitoring

## API Overview

The API provides comprehensive project management functionality across 4 main areas:

- **User Management** - User accounts and authentication
- **Team Collaboration** - Teams and member management  
- **Project Management** - Project lifecycle and organization
- **Task Execution** - Task assignment and tracking

**📖 Complete API Documentation:** [Swagger UI](http://localhost:8080/swagger) (when running)

## Testing

```bash
# Run all tests
make test  # or: dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

**📋 Detailed Testing Information:** [Testing Guide](docs/TESTING-GUIDE.md)

## Key Features

### 🔐 Security & Authentication
- **JWT Authentication** with role-based authorization (Admin, ProjectManager, Developer, Viewer)
- **Input Validation** via FluentValidation with standardized error responses
- **SQL Injection Protection** through EF Core parameterized queries

### 🏗️ Architecture Highlights
- **Clean Architecture** with proper dependency inversion
- **Domain-Driven Design** with rich domain models and events
- **Exception-only contract** for error handling (see [ADR-0001](docs/adr/0001-result-handling-decision.md))
- **Standardized middleware pipeline** with correlation ID tracking

**🔍 Detailed Architecture Information:** [Architecture Guide](docs/ARCHITECTURE.md)

## Technology Stack

- **.NET 9** - Latest framework with enhanced performance
- **Clean Architecture** - Maintainable layered design
- **Entity Framework Core** - Modern ORM with SQL Server
- **MediatR** - CQRS pattern implementation
- **JWT Authentication** - Stateless security
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Comprehensive input validation
- **xUnit + FluentAssertions** - Testing framework
- **Docker** - Containerization support

## Project Status

**Current Version**: MVP Phase 2
- ✅ **Domain Models** - Core entities and business logic
- ✅ **Infrastructure** - Database and repository setup  
- 🚧 **Authentication** - JWT implementation in progress
- 📋 **API Endpoints** - CRUD operations planned
- 📋 **Testing** - Comprehensive test coverage planned

**📋 Detailed Roadmap**: [MVP Implementation Plan](plan/feature-dotnetskills-mvp-1.md)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For questions or support, please open an issue on GitHub.
