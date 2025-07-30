---
goal: Complete MVP Implementation of DotNetSkills Project Management API with Clean Architecture and Domain-Driven Design
version: 1.0
date_created: 2025-07-30
last_updated: 2025-07-30
owner: Development Team
status: 'Planned'
tags: ['feature', 'mvp', 'architecture', 'api', 'authentication', 'clean-architecture', 'ddd']
---

# Complete MVP Implementation Plan - DotNetSkills Project Management API

![Status: Planned](https://img.shields.io/badge/status-Planned-blue)

This comprehensive implementation plan covers the complete development of the DotNetSkills Project Management API MVP, implementing Clean Architecture and Domain-Driven Design principles with .NET 9, JWT authentication, and Azure deployment capabilities.

## 1. Requirements & Constraints

- **REQ-001**: Implement Clean Architecture with clear layer separation (Domain, Application, Infrastructure, API)
- **REQ-002**: Apply Domain-Driven Design principles with rich domain models and domain events
- **REQ-003**: Use .NET 9 with Minimal APIs for presentation layer
- **REQ-004**: Implement JWT-based stateless authentication with role-based authorization
- **REQ-005**: Support four user roles: Admin, ProjectManager, Developer, Viewer with specific permissions
- **REQ-006**: Provide full CRUD operations for Users, Teams, Projects, and Tasks
- **REQ-007**: Implement single-level subtask nesting only
- **REQ-008**: Ensure single task assignment model (one user per task)
- **REQ-009**: Use Entity Framework Core with SQL Server for data persistence
- **REQ-010**: Achieve minimum 80% test coverage across Domain and Application layers
- **REQ-011**: Deploy to Azure with proper CI/CD pipeline using GitHub Actions
- **REQ-012**: Generate comprehensive OpenAPI/Swagger documentation

- **SEC-001**: Implement secure password hashing using BCrypt
- **SEC-002**: Protect against SQL injection using EF Core parameterized queries
- **SEC-003**: Implement input validation using FluentValidation
- **SEC-004**: Configure secure CORS policies
- **SEC-005**: Implement rate limiting for authentication endpoints
- **SEC-006**: Use Azure Key Vault for production secrets management

- **CON-001**: No self-service user registration in MVP
- **CON-002**: No password recovery functionality in MVP
- **CON-003**: No refresh token implementation in MVP
- **CON-004**: Single-level subtask nesting only (no multi-level hierarchy)
- **CON-005**: Admin-only user creation and management
- **CON-006**: Response times must be under 200ms for simple CRUD operations

- **GUD-001**: Follow established coding principles from General Coding Principles.md
- **GUD-002**: Use AutoMapper for entity-DTO mappings
- **GUD-003**: Implement comprehensive error handling with ProblemDetails RFC 7807
- **GUD-004**: Use Serilog for structured logging
- **GUD-005**: Container-first deployment strategy with Docker

- **PAT-001**: Repository pattern with interfaces in Application layer
- **PAT-002**: Domain events for cross-aggregate communication
- **PAT-003**: Rich domain models with business logic encapsulation
- **PAT-004**: Command Query Responsibility Segregation (CQRS) for complex operations
- **PAT-005**: Global exception handling middleware

## 2. Implementation Steps

### Phase 1: Infrastructure and Foundation Setup (Week 1-2)

#### TASK-001: Repository and Project Structure Setup
- Initialize .NET 9 solution with Clean Architecture project structure
- Configure project references and dependencies
- Setup GitHub repository with proper .gitignore and development documentation
- Create initial README.md with setup instructions

#### TASK-002: Domain Layer Implementation
- Create core entities: User, Team, Project, Task, TeamMember
- Implement value objects and enumerations (UserRole, ProjectStatus, TaskStatus, etc.)
- Define domain events: TaskAssignedDomainEvent, TaskStatusUpdatedDomainEvent, TaskCompletedDomainEvent
- Implement domain methods with business logic in entities
- Create domain interfaces and specifications

#### TASK-003: Database Layer Configuration
- Install and configure Entity Framework Core packages
- Create ApplicationDbContext with entity configurations
- Configure entity relationships, indexes, and constraints
- Implement database seeding for initial roles and admin user
- Create initial migration and test database schema

#### TASK-004: Containerization and Local Development
- Create multi-stage Dockerfile for API application
- Setup docker-compose.yml with API and SQL Server services
- Configure development environment variables and connections
- Create database initialization scripts
- Test complete local development stack

#### TASK-005: Logging and Observability Foundation
- Install and configure Serilog with multiple sinks (Console, File)
- Setup structured logging with JSON formatting
- Configure logging levels for different environments
- Implement request/response logging middleware
- Add health check endpoints

### Phase 2: Security and Authentication (Week 3-4)

#### TASK-006: Password Security Implementation
- Implement secure password hashing service using BCrypt
- Create password validation rules and strength requirements
- Setup password security unit tests
- Implement password verification methods

#### TASK-007: JWT Authentication System
- Install JWT authentication packages and configure middleware
- Implement JWT token generation and validation services
- Setup JWT secret management (User Secrets for development)
- Configure JWT policies with role-based claims
- Create token refresh preparation (interfaces for future implementation)

#### TASK-008: Authentication Endpoints
- Implement POST /api/v1/auth/register (admin-only user creation)
- Implement POST /api/v1/auth/login with JWT token response
- Create authentication DTOs and validation rules
- Add comprehensive authentication error handling
- Implement logout functionality (token invalidation)

#### TASK-009: Authorization and Security Middleware
- Configure role-based authorization policies for each endpoint
- Implement rate limiting middleware for authentication endpoints
- Setup CORS policies for secure cross-origin requests
- Add security headers middleware (HSTS, XSS protection, etc.)
- Create global exception handling middleware with ProblemDetails

#### TASK-010: Role Management and Permissions
- Create database seeders for roles and initial admin user
- Implement role assignment and validation logic
- Setup authorization policies: Admin, ProjectManager, Developer, Viewer
- Create role-based filtering for data access
- Implement user activation/deactivation functionality

### Phase 3: Core Domain Implementation and APIs (Week 5-8)

#### TASK-011: Application Layer Services
- Create repository interfaces in Application layer
- Implement UserService with role-based operations
- Implement TeamService with member management logic
- Implement ProjectService with team association and status management
- Implement TaskService with assignment, subtask, and status management

#### TASK-012: Data Transfer Objects and Mapping
- Create comprehensive request/response DTOs for all entities
- Configure AutoMapper profiles with custom mappings
- Implement DTO validation using FluentValidation
- Create pagination and filtering DTOs
- Setup mapping between domain events and DTOs

#### TASK-013: Repository Implementation
- Implement EF Core repositories in Infrastructure layer
- Create base repository with common CRUD operations
- Implement specialized repositories: UserRepository, TeamRepository, ProjectRepository, TaskRepository
- Add repository unit tests with in-memory database
- Register repositories in dependency injection container

#### TASK-014: User Management API Endpoints
- GET /api/v1/users (Admin/ProjectManager access with role-based filtering)
- GET /api/v1/users/{id} (Self or Admin access)
- PUT /api/v1/users/{id} (Admin only for role changes, self for profile)
- DELETE /api/v1/users/{id} (Admin only with cascading rules)
- Implement comprehensive input validation and error handling

#### TASK-015: Team Management API Endpoints
- GET /api/v1/teams (with member filtering based on user role)
- POST /api/v1/teams (Admin/ProjectManager only)
- GET /api/v1/teams/{id} (team members and authorized users)
- PUT /api/v1/teams/{id} (Admin/ProjectManager only)
- DELETE /api/v1/teams/{id} (Admin only, validate no active projects)
- POST /api/v1/teams/{id}/members (Add team member)
- DELETE /api/v1/teams/{id}/members/{userId} (Remove team member)
- GET /api/v1/teams/{id}/members (List team members)

#### TASK-016: Project Management API Endpoints
- GET /api/v1/projects (with team-based filtering)
- POST /api/v1/projects (Admin/ProjectManager only)
- GET /api/v1/projects/{id} (team members and authorized users)
- PUT /api/v1/projects/{id} (Admin/ProjectManager only)
- DELETE /api/v1/projects/{id} (Admin only, validate no active tasks)
- GET /api/v1/projects/{id}/tasks (project tasks with role-based filtering)

#### TASK-017: Task Management API Endpoints
- GET /api/v1/projects/{projectId}/tasks (with filtering by status, assignee, priority)
- POST /api/v1/projects/{projectId}/tasks (Admin/ProjectManager only)
- GET /api/v1/tasks/{id} (assigned user, team members, Admin/ProjectManager)
- PUT /api/v1/tasks/{id} (update task details - Admin/ProjectManager)
- DELETE /api/v1/tasks/{id} (Admin/ProjectManager only, handle subtask cascading)
- PUT /api/v1/tasks/{id}/assign (task assignment - Admin/ProjectManager)
- PUT /api/v1/tasks/{id}/status (status updates - assigned user, Admin/ProjectManager)
- POST /api/v1/tasks/{taskId}/subtasks (create subtask - Admin/ProjectManager)

#### TASK-018: Advanced Query Features
- Implement pagination with cursor-based or offset-based strategies
- Add filtering capabilities (project, status, assigned user, priority, date ranges)
- Implement search functionality across task titles and descriptions
- Add sorting options for all list endpoints
- Create efficient database queries to prevent N+1 problems

#### TASK-019: Domain Events and Business Rules
- Implement domain event dispatcher and handlers
- Create TaskAssignedDomainEvent with notification logic
- Implement TaskStatusUpdatedDomainEvent for audit trail
- Add TaskCompletedDomainEvent for completion workflows
- Implement business rule validation (status transitions, assignment constraints)

### Phase 4: Testing and Quality Assurance (Week 9-10)

#### TASK-020: Unit Testing Infrastructure
- Setup unit test projects for each layer with proper naming conventions
- Configure Moq for dependency mocking and FluentAssertions for readable tests
- Create test data builders and factories for consistent test data
- Implement test base classes with common setup and teardown
- Configure code coverage reporting with minimum 80% target

#### TASK-021: Domain Layer Unit Tests
- Test all domain entity methods and business logic
- Validate domain event firing and handling
- Test business rule enforcement and validation
- Verify entity state changes and invariants
- Test value object behavior and immutability

#### TASK-022: Application Layer Unit Tests
- Test all service methods with mock dependencies
- Validate use case orchestration and error handling
- Test repository interface contracts
- Verify DTO mapping and validation logic
- Test authorization and permission logic

#### TASK-023: Infrastructure Layer Testing
- Create integration tests with Testcontainers for SQL Server
- Test repository implementations with real database
- Validate EF Core entity configurations and migrations
- Test database constraints and relationship enforcement
- Verify query performance and optimization

#### TASK-024: API Integration Testing
- Create end-to-end tests for all API endpoints
- Test complete authentication and authorization flows
- Validate request/response serialization and deserialization
- Test error scenarios and edge cases
- Verify API contract compliance and status codes

### Phase 5: Documentation and DevOps (Week 11-12)

#### TASK-025: API Documentation
- Configure Swashbuckle for comprehensive OpenAPI generation
- Add detailed XML documentation comments for all endpoints
- Create example requests and responses for all operations
- Setup API versioning with proper documentation
- Generate interactive Swagger UI with authentication support

#### TASK-026: Development Documentation
- Create comprehensive README.md with setup instructions
- Document architecture decisions and patterns used
- Create API usage examples and integration guides
- Document database schema and relationships
- Create troubleshooting and FAQ sections

#### TASK-027: CI Pipeline Implementation
- Create .github/workflows/ci.yml for continuous integration
- Configure build, test, and code coverage reporting
- Add security scanning with GitHub CodeQL
- Implement Docker image building and testing
- Setup branch protection rules and required checks

#### TASK-028: CD Pipeline and Azure Deployment
- Create .github/workflows/cd-staging.yml for deployment
- Setup Azure resource provisioning (App Service, SQL Database, Key Vault)
- Configure Azure Key Vault for production secrets
- Implement database migration automation
- Create staging environment smoke tests

#### TASK-029: Monitoring and Observability
- Configure Application Insights for Azure monitoring
- Setup structured logging with correlation IDs
- Implement custom metrics and performance counters
- Create alerting rules for errors and performance issues
- Setup log aggregation and analysis

#### TASK-030: Final Testing and Performance Validation
- Conduct load testing to validate performance requirements
- Perform security testing and vulnerability assessment
- Validate all acceptance criteria from PRD
- Conduct user acceptance testing scenarios
- Prepare production deployment checklist

## 3. Alternatives

- **ALT-001**: Use Entity Framework migrations vs Database-first approach - Chosen migrations for better version control and team collaboration
- **ALT-002**: Implement CQRS with separate read/write models vs Simple CRUD - Chosen simple CRUD for MVP to reduce complexity
- **ALT-003**: Use Redis for caching vs In-memory caching - Chosen in-memory for MVP, Redis planned for future scaling
- **ALT-004**: Implement refresh tokens vs Stateless JWT only - Chosen stateless JWT for MVP simplicity
- **ALT-005**: Use Azure Container Apps vs App Service - Chosen App Service for easier management in MVP
- **ALT-006**: Implement event sourcing vs Traditional CRUD - Chosen traditional CRUD for MVP simplicity

## 4. Dependencies

- **DEP-001**: .NET 9 SDK and runtime availability
- **DEP-002**: SQL Server (local development) and Azure SQL Database (staging/production)
- **DEP-003**: Azure subscription with appropriate service tiers
- **DEP-004**: GitHub repository with Actions enabled
- **DEP-005**: Docker Desktop for local development and testing
- **DEP-006**: Azure CLI and PowerShell for deployment scripts
- **DEP-007**: Visual Studio or VS Code with .NET extensions
- **DEP-008**: Postman or similar tool for API testing

## 5. Files

- **FILE-001**: `/src/DotNetSkills.Domain/` - Core domain entities, value objects, enums, events, and interfaces
- **FILE-002**: `/src/DotNetSkills.Application/` - Use cases, DTOs, repository interfaces, services, and validation
- **FILE-003**: `/src/DotNetSkills.Infrastructure/` - EF Core implementation, repositories, external services
- **FILE-004**: `/src/DotNetSkills.API/` - Minimal API endpoints, middleware, configuration, and DTOs
- **FILE-005**: `/tests/` - All test projects organized by layer with unit and integration tests
- **FILE-006**: `/.github/workflows/` - CI/CD pipeline definitions for build, test, and deployment
- **FILE-007**: `/docker/` - Dockerfile, docker-compose files, and container configurations
- **FILE-008**: `/docs/` - API documentation, architecture diagrams, and developer guides
- **FILE-009**: `/scripts/` - Database migration, deployment, and utility scripts
- **FILE-010**: `appsettings.json` and environment-specific configuration files

## 6. Testing

- **TEST-001**: Domain entity unit tests validating business logic and rules
- **TEST-002**: Application service unit tests with mocked dependencies
- **TEST-003**: Repository integration tests with Testcontainers SQL Server
- **TEST-004**: API endpoint integration tests covering all CRUD operations
- **TEST-005**: Authentication and authorization flow tests
- **TEST-006**: Input validation tests for all DTOs and endpoints
- **TEST-007**: Error handling and edge case scenario tests
- **TEST-008**: Performance tests validating response time requirements
- **TEST-009**: Security tests for authentication, authorization, and input validation
- **TEST-010**: Database migration and schema tests

## 7. Risks & Assumptions

- **RISK-001**: EF Core migration complexity in team environment - Mitigated by clear migration procedures and testing
- **RISK-002**: JWT security implementation vulnerabilities - Mitigated by using established libraries and security review
- **RISK-003**: Azure deployment configuration issues - Mitigated by Infrastructure as Code and thorough testing
- **RISK-004**: Performance bottlenecks with complex queries - Mitigated by query optimization and monitoring
- **RISK-005**: Third-party package dependency issues - Mitigated by version pinning and regular updates
- **RISK-006**: Team knowledge gaps with Clean Architecture - Mitigated by documentation and code reviews

- **ASSUMPTION-001**: Development team has experience with .NET Core and Entity Framework
- **ASSUMPTION-002**: Azure environment will be available and properly configured
- **ASSUMPTION-003**: No major framework updates during development period
- **ASSUMPTION-004**: SQL Server licensing available for development and staging
- **ASSUMPTION-005**: GitHub Actions runner capacity sufficient for CI/CD needs
- **ASSUMPTION-006**: Team follows established Git workflow and code review processes

## 8. Related Specifications / Further Reading

- [Product Requirements Document (PRD)](../doc/PRD.md) - Complete MVP requirements and acceptance criteria
- [General Coding Principles](../doc/General%20Coding%20Principles.md) - Development standards and best practices
- [Clean Architecture Documentation](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design Reference](https://www.domainlanguage.com/ddd/)
- [.NET 9 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
