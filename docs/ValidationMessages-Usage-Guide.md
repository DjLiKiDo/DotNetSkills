# ValidationMessages Class Usage Guide

The `ValidationMessages` class provides centralized validation error messages with parameter substitution support for the DotNetSkills domain layer. This documentation explains how to use the class effectively.

## Overview

The `ValidationMessages` class follows Domain-Driven Design principles by centralizing all validation messages in one location, ensuring consistency across the entire domain layer. It supports parameter substitution using standard .NET string formatting.

## Message Categories

### 1. Common Messages (`ValidationMessages.Common`)

Used across multiple entities for common validation scenarios:

```csharp
// Simple parameter substitution
throw new ArgumentException(
    ValidationMessages.Formatting.Format(ValidationMessages.Common.CannotBeEmpty, "User name"), 
    nameof(name));

// Multiple parameters
throw new ArgumentException(
    ValidationMessages.Formatting.Format(ValidationMessages.Common.ExceedsMaxLength, "Task title", 200), 
    nameof(title));

// Range validation
throw new ArgumentException(
    ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBeInRange, "Priority", 1, 5), 
    nameof(priority));
```

### 2. Entity-Specific Messages

#### User Messages (`ValidationMessages.User`)

```csharp
// Static messages (no parameters)
throw new DomainException(ValidationMessages.User.OnlyAdminCanCreate);
throw new DomainException(ValidationMessages.User.AlreadyTeamMember);

// Parameterized usage with common templates
throw new ArgumentException(
    ValidationMessages.Formatting.Format(ValidationMessages.Common.CannotBeEmpty, "User name"), 
    nameof(name));
```

#### Task Messages (`ValidationMessages.Task`)

```csharp
// Static messages
throw new DomainException(ValidationMessages.Task.CannotAssignCompleted);
throw new DomainException(ValidationMessages.Task.SubtaskNestingLimit);

// Parameterized messages
throw new DomainException(
    ValidationMessages.Formatting.Format(ValidationMessages.Task.MustBeAssigned, "completed"));

throw new DomainException(
    ValidationMessages.Formatting.Format(ValidationMessages.Task.InvalidStatusTransition, currentStatus));
```

#### Team Messages (`ValidationMessages.Team`)

```csharp
// Static messages
throw new DomainException(ValidationMessages.Team.NoPermissionToCreate);
throw new DomainException(ValidationMessages.Team.UserAlreadyMember);

// Parameterized messages
throw new DomainException(
    ValidationMessages.Formatting.Format(ValidationMessages.Team.ExceedsMaxMembers, MaxMembers));
```

#### Project Messages (`ValidationMessages.Project`)

```csharp
// Static messages
throw new DomainException(ValidationMessages.Project.CannotModifyCompleted);
throw new DomainException(ValidationMessages.Project.CanOnlyResumeFromHold);

// Parameterized messages
throw new DomainException(
    ValidationMessages.Formatting.Format(ValidationMessages.Project.CannotStartFrom, Status));
```

### 3. Value Object Messages (`ValidationMessages.ValueObjects`)

```csharp
// Email validation
throw new ArgumentException(ValidationMessages.ValueObjects.EmailCannotBeEmpty, nameof(value));
throw new ArgumentException(ValidationMessages.ValueObjects.InvalidEmailFormat, nameof(value));

// Parameterized email length validation
throw new ArgumentException(
    ValidationMessages.Formatting.Format(ValidationMessages.ValueObjects.EmailTooLong, 254), 
    nameof(value));

// Strongly-typed ID validation
throw new ArgumentException(
    ValidationMessages.Formatting.Format(ValidationMessages.ValueObjects.InvalidStronglyTypedId, "UserId", value), 
    nameof(value));
```

### 4. Business Rules Messages (`ValidationMessages.BusinessRules`)

```csharp
// Authorization failures
throw new DomainException(
    ValidationMessages.Formatting.Format(
        ValidationMessages.BusinessRules.InsufficientPermissions, 
        user.Role, 
        "create new users"));

// State transitions
throw new DomainException(
    ValidationMessages.Formatting.Format(
        ValidationMessages.BusinessRules.InvalidStateTransition, 
        "Project", 
        currentStatus, 
        targetStatus));

// Business rule violations
throw new DomainException(
    ValidationMessages.Formatting.Format(
        ValidationMessages.BusinessRules.BusinessRuleViolation, 
        "Maximum team size exceeded", 
        $"Current team has {memberCount} members"));
```

## Formatting Methods

The `ValidationMessages.Formatting` class provides type-safe methods for parameter substitution:

### Single Parameter

```csharp
var message = ValidationMessages.Formatting.Format(
    ValidationMessages.Common.MustBePositive, 
    "Estimated hours");
// Result: "Estimated hours must be positive"
```

### Two Parameters

```csharp
var message = ValidationMessages.Formatting.Format(
    ValidationMessages.Common.ExceedsMaxLength, 
    "Task title", 
    200);
// Result: "Task title cannot exceed 200 characters"
```

### Three Parameters

```csharp
var message = ValidationMessages.Formatting.Format(
    ValidationMessages.Common.MustBeInRange, 
    "Priority level", 
    1, 
    5);
// Result: "Priority level must be between 1 and 5"
```

### Multiple Parameters

```csharp
var message = ValidationMessages.Formatting.Format(
    "User {0} has {1} tasks and joined on {2}", 
    userName, 
    taskCount, 
    joinDate);
// Result: "User John Doe has 5 tasks and joined on 1/15/2023 12:00:00 AM"
```

## Best Practices

### 1. Use Specific Messages When Available

```csharp
// ✅ Good - Use specific message
throw new DomainException(ValidationMessages.Task.CannotAssignCompleted);

// ❌ Avoid - Generic hardcoded string
throw new DomainException("Cannot assign completed tasks");
```

### 2. Use Common Templates for Consistency

```csharp
// ✅ Good - Use common template
throw new ArgumentException(
    ValidationMessages.Formatting.Format(ValidationMessages.Common.CannotBeEmpty, "User name"), 
    nameof(name));

// ❌ Avoid - Different wording for same concept
throw new ArgumentException("User name is required", nameof(name));
```

### 3. Choose Appropriate Exception Types

```csharp
// ✅ Good - ArgumentException for input validation
if (string.IsNullOrWhiteSpace(name))
    throw new ArgumentException(ValidationMessages.User.NameRequired, nameof(name));

// ✅ Good - DomainException for business rules
if (createdByUser.Role != UserRole.Admin)
    throw new DomainException(ValidationMessages.User.OnlyAdminCanCreate);
```

### 4. Provide Parameter Names for ArgumentException

```csharp
// ✅ Good - Include parameter name
throw new ArgumentException(
    ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBePositive, "Estimated hours"), 
    nameof(estimatedHours));

// ❌ Avoid - Missing parameter name
throw new ArgumentException(
    ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBePositive, "Estimated hours"));
```

## Example: Complete Entity Validation

Here's how to use ValidationMessages in a complete entity constructor:

```csharp
public class Task : AggregateRoot<TaskId>
{
    public Task(string title, string? description, ProjectId projectId, TaskPriority priority, 
                TaskId? parentTaskId, int? estimatedHours, DateTime? dueDate, User createdBy) 
        : base(TaskId.New())
    {
        // Input validation using ArgumentException
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(ValidationMessages.Task.TitleRequired, nameof(title));

        ArgumentNullException.ThrowIfNull(projectId);
        ArgumentNullException.ThrowIfNull(createdBy);

        if (estimatedHours.HasValue && estimatedHours.Value <= 0)
            throw new ArgumentException(ValidationMessages.Task.EstimatedHoursMustBePositive, nameof(estimatedHours));

        // Business rule validation using DomainException
        if (dueDate.HasValue && dueDate.Value <= DateTime.UtcNow)
            throw new DomainException(ValidationMessages.Task.DueDateMustBeFuture);

        // Set properties...
        Title = title.Trim();
        Description = description?.Trim();
        ProjectId = projectId;
        Priority = priority;
        ParentTaskId = parentTaskId;
        EstimatedHours = estimatedHours;
        DueDate = dueDate;
        Status = TaskStatus.ToDo;

        // Raise domain event...
        RaiseDomainEvent(new TaskCreatedDomainEvent(Id, Title, ProjectId, ParentTaskId, createdBy.Id));
    }
}
```

## Message Consistency Rules

1. **No trailing periods**: All messages should end without punctuation for consistency
2. **Consistent capitalization**: First word capitalized, proper nouns capitalized
3. **Clear and descriptive**: Messages should clearly indicate what went wrong
4. **Parameter placeholders**: Use {0}, {1}, etc. for dynamic content
5. **Consistent terminology**: Use the same terms across related messages

## Testing ValidationMessages

Always test your validation messages to ensure they format correctly:

```csharp
[Fact]
public void TaskAssignmentValidation_ShouldUseCorrectMessage()
{
    // Arrange
    var task = TaskBuilder.Default().WithStatus(TaskStatus.Done).Build();
    var user = UserBuilder.Default().Build();

    // Act & Assert
    var exception = Assert.Throws<DomainException>(() => task.AssignTo(user, user));
    exception.Message.Should().Be(ValidationMessages.Task.CannotAssignCompleted);
}

[Fact]
public void EstimatedHoursValidation_ShouldFormatCorrectly()
{
    // Arrange
    var negativeHours = -5;

    // Act
    var message = ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBePositive, "Estimated hours");

    // Assert
    message.Should().Be("Estimated hours must be positive");
}
```

This approach ensures that all validation messages are consistent, maintainable, and properly tested throughout the domain layer.
