namespace DotNetSkills.Domain.Common.Validation;

/// <summary>
/// Centralized validation error messages with parameter substitution support.
/// This class follows the Domain-Driven Design principle of keeping all validation messages
/// in one place for consistency and maintainability.
/// </summary>
/// <remarks>
/// Messages use string interpolation format where:
/// - {0}, {1}, etc. represent parameter positions
/// - Parameter names should be descriptive when used in validation methods
/// - All messages should be clear, consistent, and user-friendly
/// </remarks>
[ExcludeFromCodeCoverage]
public static class ValidationMessages
{
    /// <summary>
    /// Common validation messages used across multiple entities.
    /// These messages follow consistent patterns for parameter substitution.
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Message for null or whitespace string validation.
        /// Parameter: {0} = field name (e.g., "User name", "Task title")
        /// </summary>
        public const string CannotBeEmpty = "{0} cannot be null or whitespace";

        /// <summary>
        /// Message for positive number validation.
        /// Parameter: {0} = field name (e.g., "Estimated hours", "Team size")
        /// </summary>
        public const string MustBePositive = "{0} must be positive";

        /// <summary>
        /// Message for positive or zero number validation.
        /// Parameter: {0} = field name (e.g., "Actual hours", "Progress percentage")
        /// </summary>
        public const string MustBePositiveOrZero = "{0} must be positive or zero";

        /// <summary>
        /// Message for future date validation.
        /// Parameter: {0} = field name (e.g., "Due date", "Planned end date")
        /// </summary>
        public const string MustBeFutureDate = "{0} must be in the future";

        /// <summary>
        /// Message for future date validation with conditional context.
        /// Parameters: {0} = field name, {1} = context (e.g., "for active tasks", "for active projects")
        /// </summary>
        public const string MustBeFutureDateFor = "{0} must be in the future {1}";

        /// <summary>
        /// Message for past date validation.
        /// Parameter: {0} = field name (e.g., "Created date", "Completed date")
        /// </summary>
        public const string MustBePastDate = "{0} must be in the past";

        /// <summary>
        /// Message for maximum length validation.
        /// Parameters: {0} = field name, {1} = maximum length
        /// </summary>
        public const string ExceedsMaxLength = "{0} cannot exceed {1} characters";

        /// <summary>
        /// Message for minimum length validation.
        /// Parameters: {0} = field name, {1} = minimum length
        /// </summary>
        public const string BelowMinLength = "{0} must be at least {1} characters";

        /// <summary>
        /// Message for range validation.
        /// Parameters: {0} = field name, {1} = minimum value, {2} = maximum value
        /// </summary>
        public const string MustBeInRange = "{0} must be between {1} and {2}";

        /// <summary>
        /// Message for collection count validation.
        /// Parameters: {0} = collection name, {1} = maximum count
        /// </summary>
        public const string ExceedsMaxCount = "{0} cannot have more than {1} items";

        /// <summary>
        /// Message for empty collection validation.
        /// Parameter: {0} = collection name (e.g., "Team members", "Project tasks")
        /// </summary>
        public const string CollectionCannotBeEmpty = "{0} cannot be empty";

        /// <summary>
        /// Message for null parameter validation.
        /// Parameter: {0} = parameter name
        /// </summary>
        public const string CannotBeNull = "{0} cannot be null";

        /// <summary>
        /// Message for invalid status transition.
        /// Parameters: {0} = entity type, {1} = current status, {2} = target status
        /// </summary>
        public const string InvalidStatusTransition = "Cannot transition {0} from {1} to {2} status";

        /// <summary>
        /// Message for operations on completed entities.
        /// Parameters: {0} = operation, {1} = entity type
        /// </summary>
        public const string CannotModifyCompleted = "Cannot {0} completed {1}";

        /// <summary>
        /// Message for operations on cancelled entities.
        /// Parameters: {0} = operation, {1} = entity type
        /// </summary>
        public const string CannotModifyCancelled = "Cannot {0} cancelled {1}";

        /// <summary>
        /// Message for duplicate entity validation.
        /// Parameters: {0} = entity type, {1} = identifying information
        /// </summary>
        public const string AlreadyExists = "{0} with {1} already exists";

        /// <summary>
        /// Message for entity not found validation.
        /// Parameters: {0} = entity type, {1} = identifying information
        /// </summary>
        public const string NotFound = "{0} with {1} not found";
    }

    /// <summary>
    /// User-specific validation messages.
    /// These messages are related to user management operations and business rules.
    /// </summary>
    public static class User
    {
        /// <summary>
        /// Message for empty user name validation.
        /// </summary>
        public const string NameRequired = "User name cannot be empty";

        /// <summary>
        /// Message for empty email validation.
        /// </summary>
        public const string EmailRequired = "Email address is required";

        /// <summary>
        /// Message for invalid email format validation.
        /// </summary>
        public const string InvalidEmailFormat = "Invalid email address format";

        /// <summary>
        /// Message for admin-only user creation business rule.
        /// </summary>
        public const string OnlyAdminCanCreate = "Only admin users can create new users";

        /// <summary>
        /// Message for admin-only role change business rule.
        /// </summary>
        public const string OnlyAdminCanChangeRole = "Only admin users can change user roles";

        /// <summary>
        /// Message for self role change prevention.
        /// </summary>
        public const string CannotChangeSelfRole = "Users cannot change their own role";

        /// <summary>
        /// Message for duplicate team membership.
        /// </summary>
        public const string AlreadyTeamMember = "User is already a member of this team";

        /// <summary>
        /// Message for team membership not found.
        /// </summary>
        public const string NotTeamMember = "User is not a member of this team";

        /// <summary>
        /// Message for task assignment capability check.
        /// </summary>
        public const string CannotBeAssignedTasks = "User cannot be assigned tasks";

        /// <summary>
        /// Message for inactive user operations.
        /// </summary>
        public const string InactiveUserOperation = "Cannot perform operations on inactive users";
    }

    /// <summary>
    /// Task-specific validation messages.
    /// These messages are related to task management operations and business rules.
    /// </summary>
    public static class Task
    {
        /// <summary>
        /// Message for empty task title validation.
        /// </summary>
        public const string TitleRequired = "Task title cannot be empty";

        /// <summary>
        /// Message for future due date validation.
        /// </summary>
        public const string DueDateMustBeFuture = "Due date must be in the future";

        /// <summary>
        /// Message for future due date validation with context.
        /// </summary>
        public const string DueDateMustBeFutureForActive = "Due date must be in the future for active tasks";

        /// <summary>
        /// Message for positive estimated hours validation.
        /// </summary>
        public const string EstimatedHoursMustBePositive = "Estimated hours must be positive";

        /// <summary>
        /// Message for positive actual hours validation.
        /// </summary>
        public const string ActualHoursMustBePositive = "Actual hours must be positive";

        /// <summary>
        /// Message for assignment to completed tasks.
        /// </summary>
        public const string CannotAssignCompleted = "Cannot assign completed tasks";

        /// <summary>
        /// Message for assignment to cancelled tasks.
        /// </summary>
        public const string CannotAssignCancelled = "Cannot assign cancelled tasks";

        /// <summary>
        /// Message for user cannot be assigned tasks.
        /// </summary>
        public const string CannotBeAssignedTasks = "User cannot be assigned tasks";

        /// <summary>
        /// Message for duplicate task assignment.
        /// </summary>
        public const string AlreadyAssignedToUser = "Task is already assigned to this user";

        /// <summary>
        /// Message for unassignment of unassigned tasks.
        /// </summary>
        public const string NotAssignedToAnyone = "Task is not assigned to anyone";

        /// <summary>
        /// Message for unassigning completed tasks.
        /// </summary>
        public const string CannotUnassignCompleted = "Cannot unassign completed tasks";

        /// <summary>
        /// Message for operations requiring assignment.
        /// </summary>
        public const string MustBeAssigned = "Task must be assigned to be {0}";

        /// <summary>
        /// Message for subtask nesting limit.
        /// </summary>
        public const string SubtaskNestingLimit = "Cannot create subtasks of subtasks (only single-level nesting allowed)";

        /// <summary>
        /// Message for incomplete subtasks validation.
        /// </summary>
        public const string IncompleteSubtasks = "Cannot complete task with incomplete subtasks";

        /// <summary>
        /// Message for status transition validation.
        /// Parameters: {0} = current status
        /// </summary>
        public const string InvalidStatusTransition = "Cannot submit task for review from {0} status";

        /// <summary>
        /// Message for unauthorized task operations.
        /// </summary>
        public const string OnlyAssignedUserCanSubmit = "Only the assigned user can submit this task for review";

        /// <summary>
        /// Message for only assigned user can start task.
        /// </summary>
        public const string OnlyAssignedUserCanStart = "Only the assigned user can start this task";

        /// <summary>
        /// Message for cannot start from status.
        /// Parameter: {0} = current status
        /// </summary>
        public const string CannotStartFrom = "Cannot start task from {0} status";

        /// <summary>
        /// Message for cancelling completed tasks.
        /// </summary>
        public const string CannotCancelCompleted = "Cannot cancel completed tasks";

        /// <summary>
        /// Message for only reopening completed or cancelled tasks.
        /// </summary>
        public const string CanOnlyReopenCompletedOrCancelled = "Can only reopen completed or cancelled tasks";

        /// <summary>
        /// Message for reopening validation.
        /// Parameters: {0} = current status
        /// </summary>
        public const string CannotReopenFrom = "Cannot reopen task from {0} status";

        /// <summary>
        /// Message for invalid subtask parent ID.
        /// </summary>
        public const string SubtaskParentIdMismatch = "Subtask parent ID does not match this task";

        /// <summary>
        /// Message for subtask already added.
        /// </summary>
        public const string SubtaskAlreadyAdded = "Subtask is already added to this task";
    }

    /// <summary>
    /// Team-specific validation messages.
    /// These messages are related to team management operations and business rules.
    /// </summary>
    public static class Team
    {
        /// <summary>
        /// Message for empty team name validation.
        /// </summary>
        public const string NameRequired = "Team name cannot be empty";

        /// <summary>
        /// Message for team creation permission.
        /// </summary>
        public const string NoPermissionToCreate = "User does not have permission to create teams";

        /// <summary>
        /// Message for team member addition permission.
        /// </summary>
        public const string NoPermissionToAddMembers = "User does not have permission to add team members";

        /// <summary>
        /// Message for team member removal permission.
        /// </summary>
        public const string NoPermissionToRemoveMembers = "User does not have permission to remove this team member";

        /// <summary>
        /// Message for adding inactive users.
        /// </summary>
        public const string CannotAddInactiveUsers = "Cannot add inactive users to the team";

        /// <summary>
        /// Message for duplicate team membership.
        /// </summary>
        public const string UserAlreadyMember = "User is already a member of this team";

        /// <summary>
        /// Message for team membership not found.
        /// </summary>
        public const string UserNotMember = "User is not a member of this team";

        /// <summary>
        /// Message for maximum team members validation.
        /// Parameter: {0} = maximum member count
        /// </summary>
        public const string ExceedsMaxMembers = "Team cannot have more than {0} members";

        /// <summary>
        /// Message for role change permission.
        /// </summary>
        public const string NoPermissionToChangeRole = "User does not have permission to change team member roles";
    }

    /// <summary>
    /// Project-specific validation messages.
    /// These messages are related to project management operations and business rules.
    /// </summary>
    public static class Project
    {
        /// <summary>
        /// Message for empty project name validation.
        /// </summary>
        public const string NameRequired = "Project name cannot be empty";

        /// <summary>
        /// Message for project creation permission.
        /// </summary>
        public const string NoPermissionToCreate = "User does not have permission to create projects";

        /// <summary>
        /// Message for project modification permission.
        /// </summary>
        public const string NoPermissionToModify = "User does not have permission to modify this project";

        /// <summary>
        /// Message for project start permission.
        /// </summary>
        public const string NoPermissionToStart = "User does not have permission to start this project";

        /// <summary>
        /// Message for project completion permission.
        /// </summary>
        public const string NoPermissionToComplete = "User does not have permission to complete this project";

        /// <summary>
        /// Message for project cancellation permission.
        /// </summary>
        public const string NoPermissionToCancel = "User does not have permission to cancel this project";

        /// <summary>
        /// Message for future planned end date validation.
        /// </summary>
        public const string PlannedEndDateMustBeFuture = "Planned end date must be in the future";

        /// <summary>
        /// Message for future planned end date validation with context.
        /// </summary>
        public const string PlannedEndDateMustBeFutureForActive = "Planned end date must be in the future for active projects";

        /// <summary>
        /// Message for modifying completed projects.
        /// </summary>
        public const string CannotModifyCompleted = "Cannot modify completed projects";

        /// <summary>
        /// Message for invalid status transitions.
        /// Parameters: {0} = current status
        /// </summary>
        public const string CannotStartFrom = "Cannot start project from {0} status";

        /// <summary>
        /// Message for invalid hold transitions.
        /// Parameters: {0} = current status
        /// </summary>
        public const string CannotPutOnHoldFrom = "Cannot put project on hold from {0} status";

        /// <summary>
        /// Message for resuming non-hold projects.
        /// </summary>
        public const string CanOnlyResumeFromHold = "Can only resume projects that are on hold";

        /// <summary>
        /// Message for invalid completion transitions.
        /// Parameters: {0} = current status
        /// </summary>
        public const string CannotCompleteFrom = "Cannot complete project from {0} status";

        /// <summary>
        /// Message for completing projects with active tasks.
        /// </summary>
        public const string CannotCompleteWithActiveTasks = "Cannot complete project with active tasks";

        /// <summary>
        /// Message for cancelling completed projects.
        /// </summary>
        public const string CannotCancelCompleted = "Cannot cancel completed projects";
    }

    /// <summary>
    /// Value object specific validation messages.
    /// These messages are related to value object validation and constraints.
    /// </summary>
    public static class ValueObjects
    {
        /// <summary>
        /// Message for empty email address validation.
        /// </summary>
        public const string EmailCannotBeEmpty = "Email address cannot be empty";

        /// <summary>
        /// Message for invalid email format validation.
        /// </summary>
        public const string InvalidEmailFormat = "Invalid email address format";

        /// <summary>
        /// Message for email too long validation.
        /// Parameter: {0} = maximum length (254 characters per RFC 5321)
        /// </summary>
        public const string EmailTooLong = "Email address cannot exceed {0} characters";

        /// <summary>
        /// Message for invalid strongly-typed ID validation.
        /// Parameters: {0} = ID type name, {1} = invalid value
        /// </summary>
        public const string InvalidStronglyTypedId = "Invalid {0}: {1}";

        /// <summary>
        /// Message for empty GUID validation.
        /// Parameter: {0} = ID field name
        /// </summary>
        public const string EmptyGuid = "{0} cannot be an empty GUID";
    }

    /// <summary>
    /// Business rule specific validation messages.
    /// These messages are related to complex business logic validation.
    /// </summary>
    public static class BusinessRules
    {
        /// <summary>
        /// Message for general authorization failures.
        /// Parameters: {0} = user role, {1} = required operation
        /// </summary>
        public const string InsufficientPermissions = "User with role '{0}' does not have permission to {1}";

        /// <summary>
        /// Message for invalid state transitions.
        /// Parameters: {0} = entity type, {1} = current state, {2} = target state
        /// </summary>
        public const string InvalidStateTransition = "Cannot transition {0} from '{1}' to '{2}' state";

        /// <summary>
        /// Message for business rule violations with context.
        /// Parameters: {0} = rule description, {1} = violating condition
        /// </summary>
        public const string BusinessRuleViolation = "Business rule violation: {0}. Current condition: {1}";

        /// <summary>
        /// Message for dependency constraints.
        /// Parameters: {0} = parent entity, {1} = dependent entities
        /// </summary>
        public const string DependencyConstraint = "Cannot delete {0} because it has active {1}";

        /// <summary>
        /// Message for concurrent modification detection.
        /// Parameter: {0} = entity type
        /// </summary>
        public const string ConcurrentModification = "The {0} has been modified by another user. Please refresh and try again";
    }

    /// <summary>
    /// Helper methods for formatting validation messages with parameters.
    /// These methods provide type-safe message formatting and parameter substitution.
    /// </summary>
    public static class Formatting
    {
        /// <summary>
        /// Formats a validation message with a single parameter.
        /// </summary>
        /// <param name="template">The message template with {0} placeholder.</param>
        /// <param name="param1">The parameter to substitute.</param>
        /// <returns>The formatted message string.</returns>
        public static string Format(string template, object param1)
            => string.Format(template, param1);

        /// <summary>
        /// Formats a validation message with two parameters.
        /// </summary>
        /// <param name="template">The message template with {0} and {1} placeholders.</param>
        /// <param name="param1">The first parameter to substitute.</param>
        /// <param name="param2">The second parameter to substitute.</param>
        /// <returns>The formatted message string.</returns>
        public static string Format(string template, object param1, object param2)
            => string.Format(template, param1, param2);

        /// <summary>
        /// Formats a validation message with three parameters.
        /// </summary>
        /// <param name="template">The message template with {0}, {1}, and {2} placeholders.</param>
        /// <param name="param1">The first parameter to substitute.</param>
        /// <param name="param2">The second parameter to substitute.</param>
        /// <param name="param3">The third parameter to substitute.</param>
        /// <returns>The formatted message string.</returns>
        public static string Format(string template, object param1, object param2, object param3)
            => string.Format(template, param1, param2, param3);

        /// <summary>
        /// Formats a validation message with multiple parameters.
        /// </summary>
        /// <param name="template">The message template with parameter placeholders.</param>
        /// <param name="parameters">The parameters to substitute.</param>
        /// <returns>The formatted message string.</returns>
        public static string Format(string template, params object[] parameters)
            => string.Format(template, parameters);
    }
}
