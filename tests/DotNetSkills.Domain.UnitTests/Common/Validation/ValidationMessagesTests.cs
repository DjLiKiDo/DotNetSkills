using Xunit;
using FluentAssertions;
using DotNetSkills.Domain.Common.Validation;

namespace DotNetSkills.Domain.UnitTests.Common.Validation;

/// <summary>
/// Unit tests for ValidationMessages class to ensure message templates and parameter substitution work correctly.
/// This test suite validates all message categories and formatting methods.
/// </summary>
public class ValidationMessagesTests
{
    #region Common Messages Tests

    [Fact]
    public void Common_CannotBeEmpty_ShouldFormatWithFieldName()
    {
        // Arrange
        var fieldName = "User name";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Common.CannotBeEmpty, fieldName);
        
        // Assert
        result.Should().Be("User name cannot be null or whitespace");
    }

    [Fact]
    public void Common_MustBePositive_ShouldFormatWithFieldName()
    {
        // Arrange
        var fieldName = "Estimated hours";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBePositive, fieldName);
        
        // Assert
        result.Should().Be("Estimated hours must be positive");
    }

    [Fact]
    public void Common_MustBeFutureDate_ShouldFormatWithFieldName()
    {
        // Arrange
        var fieldName = "Due date";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBeFutureDate, fieldName);
        
        // Assert
        result.Should().Be("Due date must be in the future");
    }

    [Fact]
    public void Common_MustBeFutureDateFor_ShouldFormatWithFieldNameAndContext()
    {
        // Arrange
        var fieldName = "Due date";
        var context = "for active tasks";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBeFutureDateFor, fieldName, context);
        
        // Assert
        result.Should().Be("Due date must be in the future for active tasks");
    }

    [Fact]
    public void Common_ExceedsMaxLength_ShouldFormatWithFieldNameAndLength()
    {
        // Arrange
        var fieldName = "Task title";
        var maxLength = 200;
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Common.ExceedsMaxLength, fieldName, maxLength);
        
        // Assert
        result.Should().Be("Task title cannot exceed 200 characters");
    }

    [Fact]
    public void Common_MustBeInRange_ShouldFormatWithFieldNameAndBounds()
    {
        // Arrange
        var fieldName = "Priority level";
        var min = 1;
        var max = 5;
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBeInRange, fieldName, min, max);
        
        // Assert
        result.Should().Be("Priority level must be between 1 and 5");
    }

    [Fact]
    public void Common_ExceedsMaxCount_ShouldFormatWithCollectionNameAndCount()
    {
        // Arrange
        var collectionName = "Team members";
        var maxCount = 50;
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Common.ExceedsMaxCount, collectionName, maxCount);
        
        // Assert
        result.Should().Be("Team members cannot have more than 50 items");
    }

    [Fact]
    public void Common_InvalidStatusTransition_ShouldFormatWithEntityAndStatuses()
    {
        // Arrange
        var entityType = "Task";
        var currentStatus = "Done";
        var targetStatus = "InProgress";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Common.InvalidStatusTransition, entityType, currentStatus, targetStatus);
        
        // Assert
        result.Should().Be("Cannot transition Task from Done to InProgress status");
    }

    [Fact]
    public void Common_CannotModifyCompleted_ShouldFormatWithOperationAndEntity()
    {
        // Arrange
        var operation = "assign";
        var entityType = "tasks";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Common.CannotModifyCompleted, operation, entityType);
        
        // Assert
        result.Should().Be("Cannot assign completed tasks");
    }

    [Fact]
    public void Common_AlreadyExists_ShouldFormatWithEntityAndIdentifier()
    {
        // Arrange
        var entityType = "User";
        var identifier = "email 'test@example.com'";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Common.AlreadyExists, entityType, identifier);
        
        // Assert
        result.Should().Be("User with email 'test@example.com' already exists");
    }

    #endregion

    #region User Messages Tests

    [Fact]
    public void User_StaticMessages_ShouldHaveCorrectText()
    {
        // Assert - verify static messages have expected content
        ValidationMessages.User.NameRequired.Should().Be("User name cannot be empty");
        ValidationMessages.User.EmailRequired.Should().Be("Email address is required");
        ValidationMessages.User.InvalidEmailFormat.Should().Be("Invalid email address format");
        ValidationMessages.User.OnlyAdminCanCreate.Should().Be("Only admin users can create new users");
        ValidationMessages.User.OnlyAdminCanChangeRole.Should().Be("Only admin users can change user roles");
        ValidationMessages.User.CannotChangeSelfRole.Should().Be("Users cannot change their own role");
        ValidationMessages.User.AlreadyTeamMember.Should().Be("User is already a member of this team");
        ValidationMessages.User.NotTeamMember.Should().Be("User is not a member of this team");
        ValidationMessages.User.CannotBeAssignedTasks.Should().Be("User cannot be assigned tasks");
        ValidationMessages.User.InactiveUserOperation.Should().Be("Cannot perform operations on inactive users");
    }

    #endregion

    #region Task Messages Tests

    [Fact]
    public void Task_StaticMessages_ShouldHaveCorrectText()
    {
        // Assert - verify static messages have expected content
        ValidationMessages.Task.TitleRequired.Should().Be("Task title cannot be empty");
        ValidationMessages.Task.DueDateMustBeFuture.Should().Be("Due date must be in the future");
        ValidationMessages.Task.DueDateMustBeFutureForActive.Should().Be("Due date must be in the future for active tasks");
        ValidationMessages.Task.EstimatedHoursMustBePositive.Should().Be("Estimated hours must be positive");
        ValidationMessages.Task.ActualHoursMustBePositive.Should().Be("Actual hours must be positive");
        ValidationMessages.Task.CannotAssignCompleted.Should().Be("Cannot assign completed tasks");
        ValidationMessages.Task.CannotAssignCancelled.Should().Be("Cannot assign cancelled tasks");
        ValidationMessages.Task.AlreadyAssignedToUser.Should().Be("Task is already assigned to this user");
        ValidationMessages.Task.NotAssignedToAnyone.Should().Be("Task is not assigned to anyone");
        ValidationMessages.Task.SubtaskNestingLimit.Should().Be("Cannot create subtasks of subtasks (only single-level nesting allowed)");
        ValidationMessages.Task.IncompleteSubtasks.Should().Be("Cannot complete task with incomplete subtasks");
        ValidationMessages.Task.OnlyAssignedUserCanSubmit.Should().Be("Only the assigned user can submit this task for review");
        ValidationMessages.Task.CannotCancelCompleted.Should().Be("Cannot cancel completed tasks");
    }

    [Fact]
    public void Task_MustBeAssigned_ShouldFormatWithOperation()
    {
        // Arrange
        var operation = "completed";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Task.MustBeAssigned, operation);
        
        // Assert
        result.Should().Be("Task must be assigned to be completed");
    }

    [Fact]
    public void Task_InvalidStatusTransition_ShouldFormatWithStatuses()
    {
        // Arrange
        var currentStatus = "Done";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Task.InvalidStatusTransition, currentStatus);
        
        // Assert
        result.Should().Be("Cannot submit task for review from Done status");
    }

    [Fact]
    public void Task_CannotReopenFrom_ShouldFormatWithStatus()
    {
        // Arrange
        var currentStatus = "InProgress";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Task.CannotReopenFrom, currentStatus);
        
        // Assert
        result.Should().Be("Cannot reopen task from InProgress status");
    }

    #endregion

    #region Team Messages Tests

    [Fact]
    public void Team_StaticMessages_ShouldHaveCorrectText()
    {
        // Assert - verify static messages have expected content
        ValidationMessages.Team.NameRequired.Should().Be("Team name cannot be empty");
        ValidationMessages.Team.NoPermissionToCreate.Should().Be("User does not have permission to create teams");
        ValidationMessages.Team.NoPermissionToAddMembers.Should().Be("User does not have permission to add team members");
        ValidationMessages.Team.NoPermissionToRemoveMembers.Should().Be("User does not have permission to remove this team member");
        ValidationMessages.Team.CannotAddInactiveUsers.Should().Be("Cannot add inactive users to the team");
        ValidationMessages.Team.UserAlreadyMember.Should().Be("User is already a member of this team");
        ValidationMessages.Team.UserNotMember.Should().Be("User is not a member of this team");
        ValidationMessages.Team.NoPermissionToChangeRole.Should().Be("User does not have permission to change team member roles");
    }

    [Fact]
    public void Team_ExceedsMaxMembers_ShouldFormatWithMaxCount()
    {
        // Arrange
        var maxMembers = 50;
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Team.ExceedsMaxMembers, maxMembers);
        
        // Assert
        result.Should().Be("Team cannot have more than 50 members");
    }

    #endregion

    #region Project Messages Tests

    [Fact]
    public void Project_StaticMessages_ShouldHaveCorrectText()
    {
        // Assert - verify static messages have expected content
        ValidationMessages.Project.NameRequired.Should().Be("Project name cannot be empty");
        ValidationMessages.Project.NoPermissionToCreate.Should().Be("User does not have permission to create projects");
        ValidationMessages.Project.NoPermissionToModify.Should().Be("User does not have permission to modify this project");
        ValidationMessages.Project.NoPermissionToStart.Should().Be("User does not have permission to start this project");
        ValidationMessages.Project.NoPermissionToComplete.Should().Be("User does not have permission to complete this project");
        ValidationMessages.Project.NoPermissionToCancel.Should().Be("User does not have permission to cancel this project");
        ValidationMessages.Project.PlannedEndDateMustBeFuture.Should().Be("Planned end date must be in the future");
        ValidationMessages.Project.PlannedEndDateMustBeFutureForActive.Should().Be("Planned end date must be in the future for active projects");
        ValidationMessages.Project.CannotModifyCompleted.Should().Be("Cannot modify completed projects");
        ValidationMessages.Project.CanOnlyResumeFromHold.Should().Be("Can only resume projects that are on hold");
        ValidationMessages.Project.CannotCompleteWithActiveTasks.Should().Be("Cannot complete project with active tasks");
        ValidationMessages.Project.CannotCancelCompleted.Should().Be("Cannot cancel completed projects");
    }

    [Fact]
    public void Project_CannotStartFrom_ShouldFormatWithStatus()
    {
        // Arrange
        var currentStatus = "Completed";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Project.CannotStartFrom, currentStatus);
        
        // Assert
        result.Should().Be("Cannot start project from Completed status");
    }

    [Fact]
    public void Project_CannotPutOnHoldFrom_ShouldFormatWithStatus()
    {
        // Arrange
        var currentStatus = "Planning";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Project.CannotPutOnHoldFrom, currentStatus);
        
        // Assert
        result.Should().Be("Cannot put project on hold from Planning status");
    }

    [Fact]
    public void Project_CannotCompleteFrom_ShouldFormatWithStatus()
    {
        // Arrange
        var currentStatus = "OnHold";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.Project.CannotCompleteFrom, currentStatus);
        
        // Assert
        result.Should().Be("Cannot complete project from OnHold status");
    }

    #endregion

    #region Value Objects Messages Tests

    [Fact]
    public void ValueObjects_StaticMessages_ShouldHaveCorrectText()
    {
        // Assert - verify static messages have expected content
        ValidationMessages.ValueObjects.EmailCannotBeEmpty.Should().Be("Email address cannot be empty");
        ValidationMessages.ValueObjects.InvalidEmailFormat.Should().Be("Invalid email address format");
    }

    [Fact]
    public void ValueObjects_EmailTooLong_ShouldFormatWithMaxLength()
    {
        // Arrange
        var maxLength = 254;
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.ValueObjects.EmailTooLong, maxLength);
        
        // Assert
        result.Should().Be("Email address cannot exceed 254 characters");
    }

    [Fact]
    public void ValueObjects_InvalidStronglyTypedId_ShouldFormatWithTypeAndValue()
    {
        // Arrange
        var idType = "UserId";
        var invalidValue = "00000000-0000-0000-0000-000000000000";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.ValueObjects.InvalidStronglyTypedId, idType, invalidValue);
        
        // Assert
        result.Should().Be("Invalid UserId: 00000000-0000-0000-0000-000000000000");
    }

    [Fact]
    public void ValueObjects_EmptyGuid_ShouldFormatWithFieldName()
    {
        // Arrange
        var fieldName = "User ID";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.ValueObjects.EmptyGuid, fieldName);
        
        // Assert
        result.Should().Be("User ID cannot be an empty GUID");
    }

    #endregion

    #region Business Rules Messages Tests

    [Fact]
    public void BusinessRules_InsufficientPermissions_ShouldFormatWithRoleAndOperation()
    {
        // Arrange
        var userRole = "Developer";
        var operation = "create new users";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.BusinessRules.InsufficientPermissions, userRole, operation);
        
        // Assert
        result.Should().Be("User with role 'Developer' does not have permission to create new users");
    }

    [Fact]
    public void BusinessRules_InvalidStateTransition_ShouldFormatWithEntityAndStates()
    {
        // Arrange
        var entityType = "Project";
        var currentState = "Completed";
        var targetState = "Active";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.BusinessRules.InvalidStateTransition, entityType, currentState, targetState);
        
        // Assert
        result.Should().Be("Cannot transition Project from 'Completed' to 'Active' state");
    }

    [Fact]
    public void BusinessRules_BusinessRuleViolation_ShouldFormatWithRuleAndCondition()
    {
        // Arrange
        var ruleDescription = "Maximum team size exceeded";
        var violatingCondition = "Current team has 51 members";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.BusinessRules.BusinessRuleViolation, ruleDescription, violatingCondition);
        
        // Assert
        result.Should().Be("Business rule violation: Maximum team size exceeded. Current condition: Current team has 51 members");
    }

    [Fact]
    public void BusinessRules_DependencyConstraint_ShouldFormatWithParentAndDependents()
    {
        // Arrange
        var parentEntity = "Team";
        var dependentEntities = "active projects";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.BusinessRules.DependencyConstraint, parentEntity, dependentEntities);
        
        // Assert
        result.Should().Be("Cannot delete Team because it has active active projects");
    }

    [Fact]
    public void BusinessRules_ConcurrentModification_ShouldFormatWithEntityType()
    {
        // Arrange
        var entityType = "Project";
        
        // Act
        var result = ValidationMessages.Formatting.Format(ValidationMessages.BusinessRules.ConcurrentModification, entityType);
        
        // Assert
        result.Should().Be("The Project has been modified by another user. Please refresh and try again");
    }

    #endregion

    #region Formatting Tests

    [Fact]
    public void Formatting_Format_WithSingleParameter_ShouldSubstituteCorrectly()
    {
        // Arrange
        var template = "Hello {0}!";
        var parameter = "World";
        
        // Act
        var result = ValidationMessages.Formatting.Format(template, parameter);
        
        // Assert
        result.Should().Be("Hello World!");
    }

    [Fact]
    public void Formatting_Format_WithTwoParameters_ShouldSubstituteCorrectly()
    {
        // Arrange
        var template = "{0} must be between {1} and 100";
        var param1 = "Value";
        var param2 = 1;
        
        // Act
        var result = ValidationMessages.Formatting.Format(template, param1, param2);
        
        // Assert
        result.Should().Be("Value must be between 1 and 100");
    }

    [Fact]
    public void Formatting_Format_WithThreeParameters_ShouldSubstituteCorrectly()
    {
        // Arrange
        var template = "Cannot transition {0} from {1} to {2}";
        var param1 = "Task";
        var param2 = "Done";
        var param3 = "InProgress";
        
        // Act
        var result = ValidationMessages.Formatting.Format(template, param1, param2, param3);
        
        // Assert
        result.Should().Be("Cannot transition Task from Done to InProgress");
    }

    [Fact]
    public void Formatting_Format_WithMultipleParameters_ShouldSubstituteCorrectly()
    {
        // Arrange
        var template = "{0} {1} {2} {3}";
        var parameters = new object[] { "First", "Second", "Third", "Fourth" };
        
        // Act
        var result = ValidationMessages.Formatting.Format(template, parameters);
        
        // Assert
        result.Should().Be("First Second Third Fourth");
    }

    [Fact]
    public void Formatting_Format_WithNullParameter_ShouldHandleGracefully()
    {
        // Arrange
        var template = "Value is {0}";
        object? parameter = null;
        
        // Act
        var result = ValidationMessages.Formatting.Format(template, parameter!);
        
        // Assert
        result.Should().Be("Value is ");
    }

    [Fact]
    public void Formatting_Format_WithMixedParameterTypes_ShouldSubstituteCorrectly()
    {
        // Arrange
        var template = "User {0} has {1} tasks and joined on {2}";
        var userName = "John Doe";
        var taskCount = 5;
        var joinDate = new DateTime(2023, 1, 15);
        
        // Act
        var result = ValidationMessages.Formatting.Format(template, userName, taskCount, joinDate);
        
        // Assert
        result.Should().Be($"User John Doe has 5 tasks and joined on {joinDate}");
    }

    #endregion

    #region Edge Cases and Error Conditions

    [Fact]
    public void Formatting_Format_WithEmptyTemplate_ShouldReturnEmptyString()
    {
        // Arrange
        var template = "";
        var parameter = "test";
        
        // Act
        var result = ValidationMessages.Formatting.Format(template, parameter);
        
        // Assert
        result.Should().Be("");
    }

    [Fact]
    public void Formatting_Format_WithNoPlaceholders_ShouldReturnOriginalTemplate()
    {
        // Arrange
        var template = "This is a constant message";
        var parameter = "ignored";
        
        // Act
        var result = ValidationMessages.Formatting.Format(template, parameter);
        
        // Assert
        result.Should().Be("This is a constant message");
    }

    [Fact]
    public void Formatting_Format_WithMoreParametersThanPlaceholders_ShouldIgnoreExtra()
    {
        // Arrange
        var template = "Hello {0}";
        var param1 = "World";
        var param2 = "Extra";
        
        // Act
        var result = ValidationMessages.Formatting.Format(template, param1, param2);
        
        // Assert
        result.Should().Be("Hello World");
    }

    #endregion

    #region Message Consistency Tests

    [Fact]
    public void AllMessages_ShouldNotContainTrailingPeriods()
    {
        // Arrange & Act - Get all constant fields from message classes using reflection
        var messageTypes = new[]
        {
            typeof(ValidationMessages.Common),
            typeof(ValidationMessages.User),
            typeof(ValidationMessages.Task),
            typeof(ValidationMessages.Team),
            typeof(ValidationMessages.Project),
            typeof(ValidationMessages.ValueObjects),
            typeof(ValidationMessages.BusinessRules)
        };

        var messagesWithPeriods = new List<string>();

        foreach (var messageType in messageTypes)
        {
            var fields = messageType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string))
                {
                    var message = (string)field.GetValue(null)!;
                    if (message.EndsWith('.'))
                    {
                        messagesWithPeriods.Add($"{messageType.Name}.{field.Name}: {message}");
                    }
                }
            }
        }

        // Assert
        messagesWithPeriods.Should().BeEmpty("Validation messages should not end with periods for consistency");
    }

    [Fact]
    public void AllMessages_ShouldNotBeNullOrEmpty()
    {
        // Arrange & Act - Get all constant fields from message classes using reflection
        var messageTypes = new[]
        {
            typeof(ValidationMessages.Common),
            typeof(ValidationMessages.User),
            typeof(ValidationMessages.Task),
            typeof(ValidationMessages.Team),
            typeof(ValidationMessages.Project),
            typeof(ValidationMessages.ValueObjects),
            typeof(ValidationMessages.BusinessRules)
        };

        var nullOrEmptyMessages = new List<string>();

        foreach (var messageType in messageTypes)
        {
            var fields = messageType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string))
                {
                    var message = (string)field.GetValue(null)!;
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        nullOrEmptyMessages.Add($"{messageType.Name}.{field.Name}");
                    }
                }
            }
        }

        // Assert
        nullOrEmptyMessages.Should().BeEmpty("All validation messages should have meaningful content");
    }

    [Fact]
    public void ParameterizedMessages_ShouldContainPlaceholders()
    {
        // Arrange - Messages that should contain parameter placeholders
        var parametrizedMessages = new Dictionary<string, string>
        {
            [nameof(ValidationMessages.Common.CannotBeEmpty)] = ValidationMessages.Common.CannotBeEmpty,
            [nameof(ValidationMessages.Common.MustBePositive)] = ValidationMessages.Common.MustBePositive,
            [nameof(ValidationMessages.Common.MustBeFutureDate)] = ValidationMessages.Common.MustBeFutureDate,
            [nameof(ValidationMessages.Common.MustBeFutureDateFor)] = ValidationMessages.Common.MustBeFutureDateFor,
            [nameof(ValidationMessages.Common.ExceedsMaxLength)] = ValidationMessages.Common.ExceedsMaxLength,
            [nameof(ValidationMessages.Common.MustBeInRange)] = ValidationMessages.Common.MustBeInRange,
            [nameof(ValidationMessages.Common.ExceedsMaxCount)] = ValidationMessages.Common.ExceedsMaxCount,
            [nameof(ValidationMessages.Common.InvalidStatusTransition)] = ValidationMessages.Common.InvalidStatusTransition,
            [nameof(ValidationMessages.Team.ExceedsMaxMembers)] = ValidationMessages.Team.ExceedsMaxMembers,
            [nameof(ValidationMessages.ValueObjects.EmailTooLong)] = ValidationMessages.ValueObjects.EmailTooLong,
            [nameof(ValidationMessages.BusinessRules.InsufficientPermissions)] = ValidationMessages.BusinessRules.InsufficientPermissions
        };

        var messagesWithoutPlaceholders = new List<string>();

        // Act & Assert
        foreach (var kvp in parametrizedMessages)
        {
            if (!kvp.Value.Contains("{0}"))
            {
                messagesWithoutPlaceholders.Add($"{kvp.Key}: {kvp.Value}");
            }
        }

        messagesWithoutPlaceholders.Should().BeEmpty("Parameterized messages should contain at least one placeholder");
    }

    #endregion
}
