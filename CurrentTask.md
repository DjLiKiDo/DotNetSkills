---
goal: Standardize validation patterns across domain entities for consistency and maintainability
version: 1.0
date_created: 2025-08-01
last_updated: 2025-08-01
owner: Development Team
status: 'Planned'
tags: ['refactor', 'domain', 'validation', 'code-quality', 'technical-debt']
---

# Standardize Domain Validation Patterns

![Status: Planned](https://img.shields.io/badge/status-Planned-blue)

This implementation plan addresses the inconsistent validation patterns across domain entities in the DotNetSkills project. The goal is to establish a standardized, reusable validation framework that ensures consistency in exception handling, error messages, and validation logic across all domain entities.

## 1. Requirements & Constraints

- **REQ-001**: All validation logic must be consistent across domain entities
- **REQ-002**: Exception types must follow a clear hierarchy (ArgumentException for input validation, DomainException for business rules)
- **REQ-003**: Error messages must be consistent and descriptive
- **REQ-004**: Validation helpers must be reusable and centralized
- **REQ-005**: DateTime validation must consistently use UTC time
- **REQ-006**: String validation must handle trimming and null/whitespace consistently
- **REQ-007**: Numeric validation must handle edge cases (negative numbers, zero, etc.)
- **SEC-001**: All user inputs must be validated before processing
- **SEC-002**: Validation must prevent injection attacks and malformed data
- **CON-001**: Must maintain backward compatibility with existing domain event signatures
- **CON-002**: Must not break existing aggregate boundaries
- **GUD-001**: Follow .NET naming conventions and coding standards
- **GUD-002**: Use consistent parameter names across all validation methods
- **PAT-001**: Follow Domain-Driven Design validation patterns
- **PAT-002**: Maintain clear separation between input validation and business rule validation

## 2. Implementation Steps

### Phase 1: Create Validation Infrastructure (Day 1)

#### TASK-001: Create Domain Validation Helper Class
- Create `src/DotNetSkills.Domain/Common/Validation/Ensure.cs`
- Implement standardized validation methods for common scenarios
- Include comprehensive XML documentation
- Add unit tests for all validation methods

### Phase 2: Standardize Entity Validation (Days 2-3)

#### TASK-005: Refactor User Entity Validation
- Update `User.cs` constructor and methods to use Ensure helpers
- Standardize exception types and messages
- Maintain existing business logic integrity

#### TASK-006: Refactor Task Entity Validation
- Update `Task.cs` constructor and methods to use Ensure helpers
- Fix DateTime UTC consistency issues
- Standardize numeric validation patterns

#### TASK-007: Refactor Team Entity Validation
- Update `Team.cs` constructor and methods to use Ensure helpers
- Ensure consistent string validation patterns

#### TASK-008: Refactor Project Entity Validation
- Update `Project.cs` constructor and methods to use Ensure helpers
- Standardize date validation logic

#### TASK-009: Refactor Value Object Validation
- Update `EmailAddress.cs` and other value objects
- Use centralized constants and messages
- Maintain immutability and record semantics

### Phase 3: Comprehensive Testing (Day 4)

#### TASK-010: Create Validation Unit Tests
- Create comprehensive test suite for `Ensure` class
- Test all edge cases and error conditions
- Verify consistent exception types and messages

#### TASK-011: Update Entity Unit Tests
- Update existing entity tests to verify new validation patterns
- Add tests for validation consistency
- Ensure backward compatibility

#### TASK-012: Integration Testing
- Verify that domain events still work correctly
- Test aggregate boundaries with new validation
- Performance testing for validation overhead

### Phase 4: Documentation and Cleanup (Day 4)

#### TASK-013: Update Documentation
- Update XML documentation for all modified entities
- Create validation guidelines documentation
- Update coding standards with validation patterns

#### TASK-014: Code Review and Cleanup
- Remove duplicate validation logic
- Ensure consistent code formatting
- Verify all TODO comments are addressed

## 3. Alternatives

- **ALT-001**: Use FluentValidation library - Rejected due to domain layer dependency constraints and Clean Architecture principles
- **ALT-002**: Create attribute-based validation - Rejected as it would scatter validation logic and reduce discoverability
- **ALT-003**: Use Data Annotations - Rejected as inappropriate for rich domain models and business rule validation
- **ALT-004**: Individual validation methods per entity - Rejected due to code duplication and maintenance overhead

## 4. Dependencies

- **DEP-001**: Existing domain entities (`User`, `Task`, `Team`, `Project`)
- **DEP-002**: Domain exception classes (`DomainException`)
- **DEP-003**: Value objects (`EmailAddress`, strongly-typed IDs)
- **DEP-004**: Existing unit test infrastructure
- **DEP-005**: GlobalUsings.cs configuration

## 5. Files

- **FILE-001**: `src/DotNetSkills.Domain/Common/Validation/Ensure.cs` - Central validation helper class
- **FILE-002**: `src/DotNetSkills.Domain/Common/Validation/ValidationConstants.cs` - Primitive validation constants
- **FILE-002B**: `src/DotNetSkills.Domain/Common/Rules/BusinessRules.cs` - Complex business rule logic
- **FILE-003**: `src/DotNetSkills.Domain/Common/Validation/ValidationMessages.cs` - Centralized error messages
- **FILE-004**: `src/DotNetSkills.Domain/UserManagement/Entities/User.cs` - Updated validation patterns
- **FILE-005**: `src/DotNetSkills.Domain/TaskExecution/Entities/Task.cs` - Updated validation patterns
- **FILE-006**: `src/DotNetSkills.Domain/TeamCollaboration/Entities/Team.cs` - Updated validation patterns
- **FILE-007**: `src/DotNetSkills.Domain/ProjectManagement/Entities/Project.cs` - Updated validation patterns
- **FILE-008**: `src/DotNetSkills.Domain/UserManagement/ValueObjects/EmailAddress.cs` - Updated validation patterns
- **FILE-009**: `src/DotNetSkills.Domain/GlobalUsings.cs` - Updated global using statements
- **FILE-010**: `src/DotNetSkills.Domain/Common/Extensions/ProjectStatusExtensions.cs` - Project status business logic
- **FILE-011**: `src/DotNetSkills.Domain/Common/Extensions/TaskStatusExtensions.cs` - Task status transitions and utilities
- **FILE-012**: `src/DotNetSkills.Domain/Common/Extensions/TaskPriorityExtensions.cs` - Task priority helpers
- **FILE-013**: `src/DotNetSkills.Domain/Common/Extensions/UserRoleExtensions.cs` - User role permissions and hierarchy
- **FILE-014**: `src/DotNetSkills.Domain/Common/Extensions/UserStatusExtensions.cs` - User status transitions
- **FILE-015**: `src/DotNetSkills.Domain/Common/Extensions/TeamRoleExtensions.cs` - Team role privileges and responsibilities
- **FILE-016**: `tests/DotNetSkills.Domain.UnitTests/Common/Extensions/` - Extension method tests
- **FILE-010**: `tests/DotNetSkills.Domain.UnitTests/Common/Validation/EnsureTests.cs` - Validation helper tests
- **FILE-011**: `tests/DotNetSkills.Domain.UnitTests/UserManagement/UserValidationTests.cs` - User validation tests
- **FILE-012**: `tests/DotNetSkills.Domain.UnitTests/TaskExecution/TaskValidationTests.cs` - Task validation tests

## 6. Testing

- **TEST-001**: Unit tests for `Ensure` class covering all validation methods and edge cases
- **TEST-002**: Unit tests for each entity's validation logic using the new patterns
- **TEST-003**: Integration tests to verify domain events still fire correctly after validation changes
- **TEST-004**: Performance tests to ensure validation doesn't introduce significant overhead
- **TEST-005**: Negative testing to verify proper exception handling and error messages
- **TEST-006**: Regression tests to ensure existing business logic remains intact
- **TEST-007**: Edge case testing for boundary conditions (empty strings, null values, extreme dates)

## 7. Risks & Assumptions

- **RISK-001**: Breaking changes to existing validation could affect dependent code - Mitigation: Comprehensive testing and gradual rollout
- **RISK-002**: Performance impact from centralized validation - Mitigation: Performance testing and optimization
- **RISK-003**: Developer adoption of new patterns - Mitigation: Clear documentation and code review enforcement
- **ASSUMPTION-001**: Current domain events and business logic are correct and should be preserved
- **ASSUMPTION-002**: UTC time handling is the desired standard for all date operations
- **ASSUMPTION-003**: Existing entity constructors and method signatures should remain unchanged for compatibility

## 8. Related Specifications / Further Reading

- [DotNet Coding Principles - Validation Guidelines](/Users/marquez/Downloads/Pablo/Repos/DotNetSkills/.github/instructions/dotnet.instructions.md)
- [Domain Technical Debt Analysis - Validation Patterns](/Users/marquez/Downloads/Pablo/Repos/DotNetSkills/DomainTechnicalDebt.md)
- [Clean Architecture Validation Patterns](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles#separation-of-concerns)
- [Domain-Driven Design Validation Patterns](https://martinfowler.com/articles/domain-validation.html)

---

## Detailed Implementation Specifications

### Ensure Class Design

```csharp
public static class Ensure
{
    // String validation
    public static void NotNullOrWhiteSpace(string value, string paramName, string? customMessage = null)
    public static void HasMaxLength(string value, int maxLength, string paramName, string? customMessage = null)
    public static void HasMinLength(string value, int minLength, string paramName, string? customMessage = null)
    
    // Numeric validation
    public static void Positive(int value, string paramName, string? customMessage = null)
    public static void PositiveOrZero(int value, string paramName, string? customMessage = null)
    public static void InRange(int value, int min, int max, string paramName, string? customMessage = null)
    
    // DateTime validation
    public static void FutureDate(DateTime value, string paramName, string? customMessage = null)
    public static void FutureDateOrNull(DateTime? value, string paramName, string? customMessage = null)
    public static void PastDate(DateTime value, string paramName, string? customMessage = null)
    
    // Business rule validation
    public static void BusinessRule(bool condition, string message)
    public static void BusinessRule(Func<bool> condition, string message)
    
    // Collection validation
    public static void NotEmpty<T>(IEnumerable<T> collection, string paramName, string? customMessage = null)
    public static void MaxCount<T>(IEnumerable<T> collection, int maxCount, string paramName, string? customMessage = null)
}
```

### Validation Constants Structure

```csharp
public static class ValidationConstants
{
    public static class StringLengths
    {
        public const int UserNameMaxLength = 100;
        public const int TaskTitleMaxLength = 200;
        public const int TeamNameMaxLength = 100;
        public const int ProjectNameMaxLength = 150;
        public const int DescriptionMaxLength = 1000;
        public const int EmailMaxLength = 254; // RFC 5321
    }
    
    public static class Numeric
    {
        public const int TeamMaxMembers = 50;
        public const int TaskMaxEstimatedHours = 1000;
        public const int TaskMinEstimatedHours = 1;
    }
    
    public static class DateTime
    {
        public static readonly System.DateTime MinAllowedDate = new(2000, 1, 1);
        public static readonly System.DateTime MaxAllowedDate = new(2100, 12, 31);
    }
}
```

### Business Rules Structure

```csharp
public static class BusinessRules
{
    public static class ProjectStatus
    {
        public static bool CanTransitionTo(Domain.ProjectManagement.Enums.ProjectStatus current, 
                                         Domain.ProjectManagement.Enums.ProjectStatus target)
        {
            return current switch
            {
                Domain.ProjectManagement.Enums.ProjectStatus.Planning => 
                    target is Domain.ProjectManagement.Enums.ProjectStatus.Active or 
                             Domain.ProjectManagement.Enums.ProjectStatus.Cancelled,
                Domain.ProjectManagement.Enums.ProjectStatus.Active => 
                    target is Domain.ProjectManagement.Enums.ProjectStatus.OnHold or 
                             Domain.ProjectManagement.Enums.ProjectStatus.Completed or 
                             Domain.ProjectManagement.Enums.ProjectStatus.Cancelled,
                Domain.ProjectManagement.Enums.ProjectStatus.OnHold => 
                    target is Domain.ProjectManagement.Enums.ProjectStatus.Active or 
                             Domain.ProjectManagement.Enums.ProjectStatus.Cancelled,
                _ => false
            };
        }
        
        public static bool IsFinalized(Domain.ProjectManagement.Enums.ProjectStatus status) =>
            status is Domain.ProjectManagement.Enums.ProjectStatus.Completed or 
                     Domain.ProjectManagement.Enums.ProjectStatus.Cancelled;
    }
    
    public static class TaskStatus
    {
        public static bool CanTransitionTo(Domain.TaskExecution.Enums.TaskStatus current, 
                                         Domain.TaskExecution.Enums.TaskStatus target)
        {
            return current switch
            {
                Domain.TaskExecution.Enums.TaskStatus.ToDo => 
                    target is Domain.TaskExecution.Enums.TaskStatus.InProgress,
                Domain.TaskExecution.Enums.TaskStatus.InProgress => 
                    target is Domain.TaskExecution.Enums.TaskStatus.InReview or 
                             Domain.TaskExecution.Enums.TaskStatus.Done or
                             Domain.TaskExecution.Enums.TaskStatus.ToDo,
                Domain.TaskExecution.Enums.TaskStatus.InReview => 
                    target is Domain.TaskExecution.Enums.TaskStatus.Done or 
                             Domain.TaskExecution.Enums.TaskStatus.InProgress,
                Domain.TaskExecution.Enums.TaskStatus.Done => false, // Final state
                _ => false
            };
        }
    }
    
    public static class Authorization
    {
        public static bool CanCreateUser(UserRole role) => role == UserRole.Admin;
        
        public static bool CanManageTeam(UserRole role) => 
            role is UserRole.Admin or UserRole.ProjectManager;
            
        public static bool CanManageProject(UserRole role) => 
            role is UserRole.Admin or UserRole.ProjectManager;
    }
}
```

### Error Message Centralization

```csharp
public static class ValidationMessages
{
    public static class Common
    {
        public const string CannotBeEmpty = "{0} cannot be null or whitespace";
        public const string MustBePositive = "{0} must be positive";
        public const string MustBeFutureDate = "{0} must be in the future";
        public const string ExceedsMaxLength = "{0} cannot exceed {1} characters";
    }
    
    public static class User
    {
        public const string NameRequired = "User name cannot be empty";
        public const string EmailRequired = "Email address is required";
        public const string OnlyAdminCanCreate = "Only admin users can create new users";
    }
    
    public static class Task
    {
        public const string TitleRequired = "Task title cannot be empty";
        public const string DueDateMustBeFuture = "Due date must be in the future";
        public const string EstimatedHoursMustBePositive = "Estimated hours must be positive";
    }
}
```

This plan provides a comprehensive approach to standardizing validation patterns while maintaining the integrity of the existing domain model and ensuring thorough testing coverage.
