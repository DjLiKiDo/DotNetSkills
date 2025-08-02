---
goal: Remove Placeholder Code and Implement Real Domain APIs
version: 1.0
date_created: 2025-08-02
last_updated: 2025-08-03
owner: AI Agent Implementation
status: 'In Progress'
tags: ['feature', 'api', 'endpoints', 'refactor', 'placeholder-removal']
---

# Remove Placeholder Code and Implement Real Domain APIs

![Status: In Progress](https://img.shields.io/badge/status-In%20Progress-yellow)

This implementation plan removes the weather template placeholder code from the API layer and implements proper domain-specific endpoints for User, Team, Project, and Task management. The plan follows Clean Architecture principles with Minimal API patterns and proper separation of concerns.

**Progress**: 
- TASK-001 ✅ Completed
- TASK-002 ✅ Completed
- TASK-003 ✅ Completed
- TASK-004 ✅ Completed (4/14 tasks)

## 1. Requirements & Constraints

- **REQ-001**: Remove all weather forecast placeholder code from Program.cs
- **REQ-002**: Implement domain-specific endpoints for all 4 bounded contexts (User, Team, Project, Task)
- **REQ-003**: Use Minimal API with extension methods for endpoint organization
- **REQ-004**: Follow established Clean Architecture dependency rules (API → Application → Domain)
- **REQ-005**: Implement proper HTTP status codes and response patterns
- **REQ-006**: Use MediatR pattern for command/query handling
- **REQ-007**: Implement proper input validation and error handling
- **REQ-008**: Support JSON serialization with proper naming conventions
- **SEC-001**: Prepare for JWT authentication integration (commented placeholders)
- **SEC-002**: Use authorization policies for protected endpoints
- **CON-001**: Application layer must be implemented before API endpoints can function
- **CON-002**: Infrastructure layer repositories must be available for data access
- **CON-003**: Must maintain existing DI registration patterns
- **GUD-001**: Follow naming conventions in .github/instructions/dotnet-arquitecture.instructions.md
- **GUD-002**: Use strongly-typed IDs throughout API layer
- **PAT-001**: Implement RESTful endpoint patterns with proper HTTP verbs
- **PAT-002**: Use endpoint grouping with consistent route prefixes (/api/v1/{resource})

## 2. Implementation Steps

### Phase 1: Remove Placeholder Code and Prepare Infrastructure

**TASK-001**: ✅ **COMPLETED** - Remove weather forecast placeholder code from Program.cs
- **File**: `src/DotNetSkills.API/Program.cs`
- **Action**: Remove lines 27-54 (weather forecast endpoint and WeatherForecast record)
- **Details**: Remove the entire weather forecast implementation including the summaries array, MapGet endpoint, and WeatherForecast record definition
- **Validation**: ✅ Verified no weather-related code remains in Program.cs
- **Completion Date**: 2025-08-03
- **Result**: Successfully removed all weather forecast placeholder code. Application builds and runs correctly with only essential API infrastructure remaining.

**TASK-002**: ✅ **COMPLETED** - Create Endpoints directory structure with bounded context alignment
- **Files**: 
  - `src/DotNetSkills.API/Endpoints/UserManagement/` (new directory)
  - `src/DotNetSkills.API/Endpoints/TeamCollaboration/` (new directory)
  - `src/DotNetSkills.API/Endpoints/ProjectManagement/` (new directory)
  - `src/DotNetSkills.API/Endpoints/TaskExecution/` (new directory)
- **Action**: Create directory structure that mirrors domain bounded contexts
- **Rationale**: Maintains semantic consistency across all layers (API ↔ Domain)
- **Validation**: ✅ All bounded context directories exist and follow Domain layer organization
- **Completion Date**: 2025-08-03
- **Result**: Successfully created all four bounded context directories under API/Endpoints. Directory structure perfectly mirrors Domain layer organization: UserManagement, TeamCollaboration, ProjectManagement, and TaskExecution.

**TASK-003**: ✅ **COMPLETED** - Update GlobalUsings.cs and prepare Application layer structure
- **File**: `src/DotNetSkills.API/GlobalUsings.cs`
- **Action**: Add endpoint-specific using statements for bounded contexts
- **Details**: Add usings for MediatR, validation, domain types, and bounded context namespaces
- **Additional Action**: Prepare Application layer bounded context structure
- **Files to Prepare**:
  - `src/DotNetSkills.Application/UserManagement/Commands/`
  - `src/DotNetSkills.Application/UserManagement/Queries/`
  - `src/DotNetSkills.Application/UserManagement/DTOs/`
  - `src/DotNetSkills.Application/TeamCollaboration/Commands/`
  - `src/DotNetSkills.Application/TeamCollaboration/Queries/`
  - `src/DotNetSkills.Application/TeamCollaboration/DTOs/`
  - `src/DotNetSkills.Application/ProjectManagement/Commands/`
  - `src/DotNetSkills.Application/ProjectManagement/Queries/`
  - `src/DotNetSkills.Application/ProjectManagement/DTOs/`
  - `src/DotNetSkills.Application/TaskExecution/Commands/`
  - `src/DotNetSkills.Application/TaskExecution/Queries/`
  - `src/DotNetSkills.Application/TaskExecution/DTOs/`
- **Validation**: ✅ All necessary usings are available globally and Application structure mirrors Domain organization
- **Completion Date**: 2025-08-03
- **Result**: Successfully updated API GlobalUsings.cs with bounded context-specific using statements for MediatR, FluentValidation, and all Application layer namespaces. Created complete Application layer structure with Commands/Queries/DTOs directories for all 4 bounded contexts plus Common directory. Structure perfectly mirrors Domain layer organization. Project builds successfully.

### Phase 2: Implement User Management Endpoints

**TASK-004**: ✅ **COMPLETED** - Create UserEndpoints.cs with complete CRUD operations and proper namespacing
- **File**: `src/DotNetSkills.API/Endpoints/UserManagement/UserEndpoints.cs`
- **Namespace**: `DotNetSkills.API.Endpoints.UserManagement`
- **Class**: `public static class UserEndpoints`
- **Action**: Implement user management endpoints with bounded context organization
- **Details**: 
  - GET /api/v1/users (list with pagination)
  - GET /api/v1/users/{id} (get by ID)
  - POST /api/v1/users (create - admin only)
  - PUT /api/v1/users/{id} (update)
  - DELETE /api/v1/users/{id} (soft delete - admin only)
- **Authentication**: RequireAuthorization for all endpoints
- **Validation**: Input validation using FluentValidation patterns
- **Error Handling**: Proper HTTP status codes (200, 201, 400, 401, 404, 409)
- **Completion Date**: 2025-08-02
- **Result**: Successfully implemented complete UserEndpoints.cs with proper Minimal API patterns, bounded context organization, comprehensive error handling, and placeholder Application layer. All endpoints are properly documented with OpenAPI/Swagger integration. Also created supporting DTOs, Commands, and Queries in Application layer. API builds successfully and runs with Swagger UI displaying all endpoints.

**TASK-005**: Create UserAccountEndpoints.cs for account-specific operations
- **File**: `src/DotNetSkills.API/Endpoints/UserManagement/UserAccountEndpoints.cs`
- **Namespace**: `DotNetSkills.API.Endpoints.UserManagement`
- **Class**: `public static class UserAccountEndpoints`
- **Action**: Implement user account management endpoints
- **Details**:
  - POST /api/v1/users/{id}/activate (activate user)
  - POST /api/v1/users/{id}/deactivate (deactivate user)
  - PUT /api/v1/users/{id}/role (update user role - admin only)
  - GET /api/v1/users/{id}/teams (get user team memberships)
- **Business Rules**: Follow User.Activate(), User.Deactivate() domain methods
- **Authorization**: Account operations require Admin or self-modification rights

### Phase 3: Implement Team Collaboration Endpoints

**TASK-006**: Create TeamEndpoints.cs with team management operations
- **File**: `src/DotNetSkills.API/Endpoints/TeamCollaboration/TeamEndpoints.cs`
- **Namespace**: `DotNetSkills.API.Endpoints.TeamCollaboration`
- **Class**: `public static class TeamEndpoints`
- **Action**: Implement team collaboration endpoints
- **Details**:
  - GET /api/v1/teams (list teams)
  - GET /api/v1/teams/{id} (get team with members)
  - POST /api/v1/teams (create team)
  - PUT /api/v1/teams/{id} (update team)
  - DELETE /api/v1/teams/{id} (delete team)
- **Business Rules**: Enforce Team domain rules and aggregate boundaries
- **Authorization**: Team management requires ProjectManager or Admin role

**TASK-007**: Create TeamMemberEndpoints.cs for member management operations
- **File**: `src/DotNetSkills.API/Endpoints/TeamCollaboration/TeamMemberEndpoints.cs`
- **Namespace**: `DotNetSkills.API.Endpoints.TeamCollaboration`
- **Class**: `public static class TeamMemberEndpoints`
- **Action**: Implement team member management endpoints (part of Team aggregate)
- **Details**:
  - POST /api/v1/teams/{id}/members (add member)
  - DELETE /api/v1/teams/{teamId}/members/{userId} (remove member)
  - PUT /api/v1/teams/{teamId}/members/{userId}/role (update member role)
  - GET /api/v1/teams/{id}/members (list team members)
- **Business Rules**: Enforce Team.MaxMembers = 50 constraint, prevent duplicate memberships
- **Validation**: Handle TeamMember entity operations as part of Team aggregate

### Phase 4: Implement Project Management Endpoints

**TASK-008**: Create ProjectEndpoints.cs with project operations
- **File**: `src/DotNetSkills.API/Endpoints/ProjectManagement/ProjectEndpoints.cs`
- **Namespace**: `DotNetSkills.API.Endpoints.ProjectManagement`
- **Class**: `public static class ProjectEndpoints`
- **Action**: Implement project management endpoints
- **Details**:
  - GET /api/v1/projects (list projects)
  - GET /api/v1/projects/{id} (get project details)
  - POST /api/v1/projects (create project)
  - PUT /api/v1/projects/{id} (update project)
  - DELETE /api/v1/projects/{id} (archive project)
- **Business Rules**: Projects belong to exactly one team
- **Filtering**: Support filtering by team, status, date ranges

**TASK-009**: Create ProjectTaskEndpoints.cs for project-task relationship management
- **File**: `src/DotNetSkills.API/Endpoints/ProjectManagement/ProjectTaskEndpoints.cs`
- **Namespace**: `DotNetSkills.API.Endpoints.ProjectManagement`
- **Class**: `public static class ProjectTaskEndpoints`
- **Action**: Implement project-task relationship endpoints
- **Details**:
  - GET /api/v1/projects/{id}/tasks (get project tasks)
  - POST /api/v1/projects/{id}/tasks (create task in project)
  - PUT /api/v1/projects/{projectId}/tasks/{taskId} (update task in project context)
- **Business Rules**: Tasks belong to project's team members
- **Validation**: Ensure tasks belong to project's team members and respect project boundaries

### Phase 5: Implement Task Execution Endpoints

**TASK-010**: Create TaskEndpoints.cs with task management operations
- **File**: `src/DotNetSkills.API/Endpoints/TaskExecution/TaskEndpoints.cs`
- **Namespace**: `DotNetSkills.API.Endpoints.TaskExecution`
- **Class**: `public static class TaskEndpoints`
- **Action**: Implement task execution endpoints
- **Details**:
  - GET /api/v1/tasks (list tasks with filtering)
  - GET /api/v1/tasks/{id} (get task details)
  - POST /api/v1/tasks (create task)
  - PUT /api/v1/tasks/{id} (update task)
  - DELETE /api/v1/tasks/{id} (delete task)
  - PUT /api/v1/tasks/{id}/status (update task status)
- **Business Rules**: Single assignment, one-level subtask nesting
- **Status Transitions**: Implement valid status change validation

**TASK-011**: Create TaskAssignmentEndpoints.cs for assignment and subtask operations
- **File**: `src/DotNetSkills.API/Endpoints/TaskExecution/TaskAssignmentEndpoints.cs`
- **Namespace**: `DotNetSkills.API.Endpoints.TaskExecution`
- **Class**: `public static class TaskAssignmentEndpoints`
- **Action**: Implement task assignment and subtask management endpoints
- **Details**:
  - POST /api/v1/tasks/{id}/assign (assign to user)
  - POST /api/v1/tasks/{id}/unassign (unassign from user)
  - POST /api/v1/tasks/{id}/subtasks (create subtask)
  - GET /api/v1/tasks/{id}/subtasks (list subtasks)
  - PUT /api/v1/tasks/{taskId}/subtasks/{subtaskId} (update subtask)
- **Business Rules**: Follow Task.AssignTo(), Task.CreateSubtask() domain methods
- **Validation**: Prevent invalid assignments and enforce one-level nesting limit

### Phase 6: Update Program.cs and Integration

**TASK-012**: Update Program.cs to register all endpoint groups with bounded context organization
- **File**: `src/DotNetSkills.API/Program.cs`
- **Action**: Replace weather endpoint with domain endpoint mappings organized by bounded context
- **Details**:
  ```csharp
  // Map all domain endpoints by bounded context
  app.MapUserManagementEndpoints();
  app.MapTeamCollaborationEndpoints();
  app.MapProjectManagementEndpoints();
  app.MapTaskExecutionEndpoints();
  ```
- **Extension Methods**: Create extension methods that group endpoints by bounded context
- **Location**: After health checks and before app.RunAsync()
- **Validation**: All endpoints are registered and routable with semantic organization

**TASK-013**: Add endpoint-specific service registrations
- **File**: `src/DotNetSkills.API/DependencyInjection.cs`
- **Action**: Add any API-specific services needed by endpoints
- **Details**: Configure JSON options, model binding, validation services
- **Integration**: Ensure proper integration with Application and Infrastructure layers

**TASK-014**: Update Swagger configuration for domain endpoints
- **File**: `src/DotNetSkills.API/DependencyInjection.cs`
- **Action**: Enhance Swagger documentation for domain APIs
- **Details**: Add proper API documentation, response types, and authentication requirements
- **Validation**: Swagger UI displays all endpoints with proper documentation

## 3. Alternatives

- **ALT-001**: Keep weather endpoint alongside domain endpoints - Rejected because it creates confusion and doesn't represent the actual domain
- **ALT-002**: Implement all endpoints in a single file - Rejected because it violates separation of concerns and makes maintenance difficult
- **ALT-003**: Use Controller classes instead of Minimal API - Rejected to maintain consistency with existing Minimal API approach
- **ALT-004**: Implement endpoints without MediatR - Rejected because it would violate Clean Architecture and CQRS patterns

## 4. Dependencies

- **DEP-001**: Application layer commands and queries organized by bounded context (CreateUserCommand, GetUserQuery, etc.)
- **DEP-002**: Infrastructure layer repositories organized by bounded context (IUserRepository, ITeamRepository, etc.)
- **DEP-003**: MediatR library for command/query dispatch across bounded contexts
- **DEP-004**: FluentValidation for input validation per bounded context
- **DEP-005**: AutoMapper for entity-to-DTO mapping with bounded context awareness
- **DEP-006**: JSON serialization configuration for strongly-typed IDs across all contexts
- **DEP-007**: Authentication middleware (JWT) - prepared but not implemented
- **DEP-008**: Authorization policies and role-based access control per bounded context
- **DEP-009**: Cross-bounded context integration patterns (Domain Events, Application Services)
- **DEP-010**: Consistent namespace structure across API, Application, and Domain layers

## 5. Files

- **FILE-001**: `src/DotNetSkills.API/Program.cs` - Remove weather code, add bounded context endpoint mappings
- **FILE-002**: `src/DotNetSkills.API/Endpoints/UserManagement/UserEndpoints.cs` - User CRUD operations
- **FILE-003**: `src/DotNetSkills.API/Endpoints/UserManagement/UserAccountEndpoints.cs` - User account management
- **FILE-004**: `src/DotNetSkills.API/Endpoints/TeamCollaboration/TeamEndpoints.cs` - Team management operations
- **FILE-005**: `src/DotNetSkills.API/Endpoints/TeamCollaboration/TeamMemberEndpoints.cs` - Team member management
- **FILE-006**: `src/DotNetSkills.API/Endpoints/ProjectManagement/ProjectEndpoints.cs` - Project management operations
- **FILE-007**: `src/DotNetSkills.API/Endpoints/ProjectManagement/ProjectTaskEndpoints.cs` - Project-task relationships
- **FILE-008**: `src/DotNetSkills.API/Endpoints/TaskExecution/TaskEndpoints.cs` - Task management operations
- **FILE-009**: `src/DotNetSkills.API/Endpoints/TaskExecution/TaskAssignmentEndpoints.cs` - Task assignment and subtasks
- **FILE-010**: `src/DotNetSkills.API/DependencyInjection.cs` - Updated service registrations
- **FILE-011**: `src/DotNetSkills.API/GlobalUsings.cs` - Bounded context using statements
- **FILE-012**: `src/DotNetSkills.Application/{BoundedContext}/Commands/` - Application layer structure preparation
- **FILE-013**: `src/DotNetSkills.Application/{BoundedContext}/Queries/` - Application layer structure preparation
- **FILE-014**: `src/DotNetSkills.Application/{BoundedContext}/DTOs/` - Application layer structure preparation

## 6. Testing

- **TEST-001**: Unit tests for endpoint route registration in Program.cs with bounded context organization
- **TEST-002**: Integration tests for UserManagement endpoints (UserEndpoints + UserAccountEndpoints)
- **TEST-003**: Integration tests for TeamCollaboration endpoints (TeamEndpoints + TeamMemberEndpoints)
- **TEST-004**: Integration tests for ProjectManagement endpoints (ProjectEndpoints + ProjectTaskEndpoints)
- **TEST-005**: Integration tests for TaskExecution endpoints (TaskEndpoints + TaskAssignmentEndpoints)
- **TEST-006**: Cross-bounded context integration tests (e.g., User joining Team, Team creating Project)
- **TEST-007**: Authentication tests for protected endpoints (when auth is implemented)
- **TEST-008**: Authorization tests for role-based access control per bounded context
- **TEST-009**: Validation tests for all input DTOs and business rule enforcement
- **TEST-010**: Error handling tests for 400, 401, 404, 409 scenarios across all contexts
- **TEST-011**: Performance tests for pagination and filtering operations
- **TEST-012**: Namespace and routing consistency tests across bounded contexts

## 7. Risks & Assumptions

- **RISK-001**: Application layer is not yet implemented - endpoints will return 500 errors until Application layer is complete with proper bounded context structure
- **RISK-002**: Infrastructure layer repositories are placeholder classes - data access will fail until proper implementation with bounded context organization
- **RISK-003**: Authentication is not implemented - all endpoints will be accessible without proper security
- **RISK-004**: Domain events are not yet dispatched - side effects may not occur across bounded contexts
- **RISK-005**: Cross-bounded context operations may require additional coordination patterns
- **ASSUMPTION-001**: MediatR is already configured in Application layer DI registration for all bounded contexts
- **ASSUMPTION-002**: FluentValidation is available and configured for input validation per bounded context
- **ASSUMPTION-003**: JSON serialization can handle strongly-typed ID conversions across all contexts
- **ASSUMPTION-004**: Authorization policies will be implemented before production deployment
- **ASSUMPTION-005**: Domain layer bounded context organization will remain stable during API implementation
- **ASSUMPTION-006**: Application layer will follow the same bounded context structure as Domain and API layers

## 8. Related Specifications / Further Reading

- [DotNetSkills MVP Plan](plan/feature-dotnetskills-mvp-1.md)
- [DDD and .NET Architecture Guidelines](.github/instructions/dotnet-arquitecture.instructions.md)
- [.NET Coding Principles](.github/instructions/dotnet.instructions.md)
- [Clean Architecture Documentation](docs/DependencyInjection-Architecture.md)
- [Domain Model Analysis](CurrentTask.md)
- [ASP.NET Core Minimal APIs Documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [MediatR Pattern Documentation](https://github.com/jbogard/MediatR)
