# Project Structure Refactoring Prompt for AI Agent

## 🎯 **MANDATORY DDD & SOLID Principles Analysis**

**Before any implementation, you MUST:**

### **DDD Patterns Applied:**
- **Bounded Context Organization**: Restructure to better align with domain boundaries and feature cohesion
- **Ubiquitous Language**: Ensure folder names use consistent business terminology (Abstractions vs Interfaces, Contracts vs DTOs)
- **Application Services**: Improve CQRS handler organization for better discoverability and maintainability
- **Domain Events**: Maintain proper event dispatching through MediatR behaviors
- **Rich Domain Models**: Preserve existing domain factory methods and business rules integration

### **SOLID Principles Validation:**
- **SRP**: Each folder structure should have single responsibility and clear purpose
- **OCP**: New structure must be extensible for additional bounded contexts without modification
- **LSP**: All handlers and abstractions must maintain substitutability
- **ISP**: Segregate interfaces and contracts into focused, discoverable locations
- **DIP**: Maintain dependency on abstractions with improved organization

### **Security & Compliance:**
- **File Organization Security**: Ensure sensitive interfaces and models are properly organized
- **Discoverability**: Improve developer productivity through intuitive folder structure
- **Maintainability**: Reduce cognitive load through feature-cohesive organization

## 🎯 **Objective**

Refactor and reorganize the DotNetSkills Application layer project structure to follow enterprise-grade best practices for:
- **Discoverability**: Developers can quickly find related files
- **Semantic Namespacing**: Folder names clearly indicate their purpose and content
- **Feature Cohesion**: Related files are grouped together by business functionality
- **Scalability**: Structure can accommodate growth without becoming unwieldy

## 📋 **Current Structure Analysis**

### **Current State:**
```
src/DotNetSkills.Application/
├── Common/
│   └── IRequest.cs                    # Minimal implementation
├── UserManagement/
│   ├── Commands/                      # ✅ Good separation
│   ├── Queries/                       # ✅ Good separation
│   └── DTOs/                         # ❌ Mixed request/response DTOs
├── ProjectManagement/                 # ✅ Domain-aligned
├── TeamCollaboration/                 # ✅ Domain-aligned
└── TaskExecution/                     # ✅ Domain-aligned
```

### **Planned Structure (From CurrentPlan.md):**
```
src/DotNetSkills.Application/
├── Common/
│   ├── Interfaces/                    # ❌ Generic name
│   ├── Models/                        # ✅ Clear purpose
│   ├── Behaviors/                     # ✅ MediatR behaviors
│   └── Mappings/                      # ✅ AutoMapper profiles
├── UserManagement/
│   ├── Commands/                      # ✅ Write operations
│   ├── Queries/                       # ✅ Read operations
│   ├── Handlers/                      # ❌ All handlers mixed together
│   ├── DTOs/                         # ❌ Mixed request/response
│   ├── Validators/                    # ✅ Input validation
│   └── Mappings/                      # ✅ Entity-DTO mappings
```

## 🚀 **Required Refactoring Actions**

### **Action 1: Enhance Common Layer Organization**

**CRITICAL**: Rename and reorganize Common layer for better semantic clarity:

```bash
# Current → Enhanced
Common/Interfaces/ → Common/Abstractions/
Common/Models/ → Common/Models/ (keep)
Common/Behaviors/ → Common/Behaviors/ (keep)
Common/Mappings/ → Common/Mappings/ (keep)
```

**Specific Files to Create:**
- `src/DotNetSkills.Application/Common/Abstractions/IRepository.cs`
- `src/DotNetSkills.Application/Common/Abstractions/IUserRepository.cs`
- `src/DotNetSkills.Application/Common/Abstractions/IUnitOfWork.cs`
- `src/DotNetSkills.Application/Common/Abstractions/IDomainEventDispatcher.cs`

**Rationale**: "Abstractions" is more encompassing than "Interfaces" and better represents the architectural intent.

### **Action 2: Implement Feature-Slice Organization (PRIMARY RECOMMENDATION)**

**CRITICAL**: Reorganize UserManagement to use feature-slice pattern for better cohesion:

```bash
# Transform from technical separation to feature cohesion
UserManagement/
├── Features/
│   ├── CreateUser/
│   │   ├── CreateUserCommand.cs
│   │   ├── CreateUserHandler.cs
│   │   └── CreateUserValidator.cs
│   ├── GetUser/
│   │   ├── GetUserByIdQuery.cs
│   │   └── GetUserByIdHandler.cs
│   ├── GetUsers/
│   │   ├── GetUsersQuery.cs
│   │   ├── GetUsersHandler.cs
│   │   └── GetUsersValidator.cs
│   ├── UpdateUser/
│   │   ├── UpdateUserCommand.cs
│   │   ├── UpdateUserHandler.cs
│   │   └── UpdateUserValidator.cs
│   └── UpdateUserRole/
│       ├── UpdateUserRoleCommand.cs
│       ├── UpdateUserRoleHandler.cs
│       └── UpdateUserRoleValidator.cs
├── Contracts/
│   ├── IUserRepository.cs
│   ├── Requests/
│   │   ├── CreateUserRequest.cs
│   │   ├── UpdateUserRequest.cs
│   │   └── UpdateUserRoleRequest.cs
│   └── Responses/
│       ├── UserResponse.cs
│       ├── UserSummaryResponse.cs
│       ├── UserProfileResponse.cs
│       ├── TeamMembershipResponse.cs
│       └── PagedUserResponse.cs
└── Mappings/
    └── UserMappingProfile.cs
```

### **Action 3: Reorganize DTO Structure**

**CRITICAL**: Split DTOs into semantically clear Request/Response separation:

```bash
# Current
UserManagement/DTOs/ → UserManagement/Contracts/

# New Structure
UserManagement/Contracts/
├── IUserRepository.cs              # Domain-specific abstractions
├── Requests/                       # Input DTOs
│   ├── CreateUserRequest.cs
│   ├── UpdateUserRequest.cs
│   └── UpdateUserRoleRequest.cs
└── Responses/                      # Output DTOs
    ├── UserResponse.cs
    ├── UserSummaryResponse.cs
    ├── UserProfileResponse.cs
    ├── TeamMembershipResponse.cs
    └── PagedUserResponse.cs
```

### **Action 4: Create Alternative Structure (FALLBACK OPTION)**

If feature-slice is deemed too radical, implement this enhanced technical separation:

```bash
UserManagement/
├── Commands/                       # Keep existing
├── Queries/                        # Keep existing
├── Handlers/
│   ├── Commands/                   # Organize by type
│   │   ├── CreateUserCommandHandler.cs
│   │   ├── UpdateUserCommandHandler.cs
│   │   └── UpdateUserRoleCommandHandler.cs
│   └── Queries/
│       ├── GetUserByIdQueryHandler.cs
│       ├── GetUsersQueryHandler.cs
│       └── GetUserTeamMembershipsQueryHandler.cs
├── Contracts/                      # Renamed from DTOs
│   ├── Requests/
│   └── Responses/
├── Validation/                     # Renamed from Validators
│   ├── Commands/
│   │   ├── CreateUserCommandValidator.cs
│   │   └── UpdateUserCommandValidator.cs
│   └── Queries/
│       └── GetUsersQueryValidator.cs
└── Mappings/
    └── UserMappingProfile.cs
```

## 🛠️ **Implementation Requirements**

### **Phase 1: File Movement and Renaming**
1. **Move existing files** according to chosen structure (feature-slice preferred)
2. **Update namespace declarations** in all moved files
3. **Update using statements** and imports
4. **Verify compilation** after each batch of moves

### **Phase 2: Create Missing Files**
Execute TASK-004 from CurrentPlan.md with enhanced paths:

**Common Layer Files:**
- `Common/Abstractions/IRepository.cs` - Generic repository with strongly-typed ID constraints
- `Common/Abstractions/IUserRepository.cs` - User-specific repository extending generic
- `Common/Abstractions/IUnitOfWork.cs` - Transaction management interface
- `Common/Abstractions/IDomainEventDispatcher.cs` - Event publishing interface
- `Common/Models/PagedResponse.cs` - Pagination wrapper with metadata
- `Common/Models/Result.cs` - Result pattern implementation

**Behavior Files:**
- `Common/Behaviors/ValidationBehavior.cs` - FluentValidation pipeline behavior
- `Common/Behaviors/LoggingBehavior.cs` - Structured logging behavior
- `Common/Behaviors/PerformanceBehavior.cs` - Performance monitoring behavior
- `Common/Behaviors/DomainEventDispatchBehavior.cs` - Event dispatch behavior

### **Phase 3: Update GlobalUsings.cs**
Update namespace imports to reflect new structure:

```csharp
// Application layer globals - updated paths
global using DotNetSkills.Application.Common.Abstractions;
global using DotNetSkills.Application.Common.Models;
global using DotNetSkills.Application.Common.Behaviors;

// MediatR support
global using MediatR;
global using AutoMapper;
global using FluentValidation.Results;
```

### **Phase 4: Update DependencyInjection.cs**
Ensure all services are registered with correct namespace references.

## 📋 **Acceptance Criteria**

### **Structure Quality:**
- [ ] All files are in semantically appropriate folders
- [ ] Related files are co-located (feature-slice preferred)
- [ ] Folder names clearly indicate their contents and purpose
- [ ] No more than 8-10 files per folder (cognitive load management)
- [ ] Consistent naming patterns across all bounded contexts

### **Code Quality:**
- [ ] All namespace declarations updated correctly
- [ ] All using statements and imports updated
- [ ] No compilation errors or warnings
- [ ] GlobalUsings.cs updated with new namespace paths
- [ ] DependencyInjection.cs references correct namespaces

### **Discoverability:**
- [ ] Developers can find command handlers within 2 clicks from command
- [ ] Request/Response DTOs are clearly separated
- [ ] Validation logic is co-located with related commands/queries
- [ ] Repository interfaces are easily discoverable

### **Scalability:**
- [ ] Structure can accommodate new bounded contexts
- [ ] Pattern is consistent across UserManagement, ProjectManagement, etc.
- [ ] Easy to add new features without structural changes
- [ ] Clear separation between shared (Common) and domain-specific code

## 🚨 **Critical Requirements**

### **Mandatory Patterns:**
- **MUST** use strongly-typed IDs throughout (UserId, TeamId, etc.)
- **MUST** maintain async/await patterns with CancellationToken support
- **MUST** preserve existing domain factory method usage
- **MUST** maintain Result pattern for error handling
- **MUST** keep MediatR request/response patterns intact

### **Namespace Standards:**
```csharp
// Feature-slice structure
namespace DotNetSkills.Application.UserManagement.Features.CreateUser;
namespace DotNetSkills.Application.UserManagement.Features.GetUser;

// OR Enhanced technical structure
namespace DotNetSkills.Application.UserManagement.Commands;
namespace DotNetSkills.Application.UserManagement.Handlers.Commands;
namespace DotNetSkills.Application.UserManagement.Contracts.Requests;
```

### **File Naming Standards:**
- Commands: `{Entity}{Action}Command.cs` (e.g., `CreateUserCommand.cs`)
- Handlers: `{Entity}{Action}CommandHandler.cs` or `{Entity}{Action}QueryHandler.cs`
- Validators: `{Entity}{Action}CommandValidator.cs`
- DTOs: Clear Request/Response suffix

## 🧪 **Validation Steps**

After completing the refactoring:

1. **Compilation Check**: `dotnet build` must succeed without errors
2. **Namespace Verification**: All files have correct namespace declarations
3. **Import Verification**: All using statements reference correct paths
4. **Pattern Consistency**: Same structure applied across all bounded contexts
5. **Discoverability Test**: Can locate related files quickly

## 📁 **Recommended Tools**

Use these approaches for efficient refactoring:

1. **Batch file operations** using `mv` commands or IDE refactoring tools
2. **Find and replace** for namespace updates across multiple files
3. **Incremental compilation** to catch issues early
4. **Git staging** to track changes incrementally

## 🎯 **Success Metrics**

The refactoring is successful when:
- **Developer Experience**: New team members can navigate structure intuitively
- **Feature Cohesion**: Related files are discoverable within same folder/context
- **Semantic Clarity**: Folder names immediately indicate their purpose
- **Scalability**: Structure can grow without becoming unwieldy
- **Consistency**: Same patterns applied across all bounded contexts

## 🔄 **Implementation Preference**

**PRIMARY**: Implement Feature-Slice organization (Action 2) for maximum cohesion
**FALLBACK**: Use Enhanced Technical organization (Action 4) if feature-slice is too disruptive

The feature-slice approach is strongly recommended as it provides superior discoverability and maintainability for growing applications with multiple bounded contexts.

---

**Note**: This refactoring aligns with the project's Clean Architecture and DDD principles while significantly improving developer productivity through better organization and discoverability.
