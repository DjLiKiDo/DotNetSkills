namespace DotNetSkills.Domain.Common.Rules;

/// <summary>
/// Centralized business rules and complex validation logic for domain entities.
/// This class encapsulates business rules that are more complex than simple validation
/// and provides a single source of truth for business logic decisions across the domain.
/// </summary>
/// <remarks>
/// This class follows Domain-Driven Design principles by making business rules explicit
/// and centralizing complex logic that spans multiple entities or requires intricate
/// decision trees. All methods are static and side-effect free for predictable behavior.
/// </remarks>
public static class BusinessRules
{
    /// <summary>
    /// Business rules related to project status transitions and lifecycle management.
    /// These rules enforce the valid state machine for project management.
    /// </summary>
    public static class ProjectStatus
    {
        /// <summary>
        /// Determines if a project can transition from its current status to a target status.
        /// Implements the project status state machine with business rule validation.
        /// </summary>
        /// <param name="current">The current project status.</param>
        /// <param name="target">The target status to transition to.</param>
        /// <returns>True if the transition is allowed by business rules, false otherwise.</returns>
        /// <remarks>
        /// Valid transitions:
        /// - Planning → Active (when project is ready to start)
        /// - Planning → Cancelled (when project is abandoned during planning)
        /// - Active → OnHold (when project needs to be paused)
        /// - Active → Completed (when all project tasks are finished)
        /// - Active → Cancelled (when active project is terminated)
        /// - OnHold → Active (when project is resumed from hold)
        /// - OnHold → Cancelled (when project on hold is terminated)
        /// - Completed → No transitions allowed (final state)
        /// - Cancelled → No transitions allowed (final state)
        /// </remarks>
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

                Domain.ProjectManagement.Enums.ProjectStatus.Completed => false, // Final state
                Domain.ProjectManagement.Enums.ProjectStatus.Cancelled => false, // Final state
                _ => false
            };
        }

        /// <summary>
        /// Determines if a project status represents a finalized state that cannot be changed.
        /// </summary>
        /// <param name="status">The project status to check.</param>
        /// <returns>True if the status is finalized, false otherwise.</returns>
        public static bool IsFinalized(Domain.ProjectManagement.Enums.ProjectStatus status) =>
            status is Domain.ProjectManagement.Enums.ProjectStatus.Completed or
                     Domain.ProjectManagement.Enums.ProjectStatus.Cancelled;

        /// <summary>
        /// Determines if a project in the given status can accept new tasks.
        /// </summary>
        /// <param name="status">The project status to check.</param>
        /// <returns>True if tasks can be added to the project, false otherwise.</returns>
        public static bool CanAcceptNewTasks(Domain.ProjectManagement.Enums.ProjectStatus status) =>
            status is Domain.ProjectManagement.Enums.ProjectStatus.Planning or
                     Domain.ProjectManagement.Enums.ProjectStatus.Active;

        /// <summary>
        /// Determines if a project can be deleted based on its current status.
        /// </summary>
        /// <param name="status">The project status to check.</param>
        /// <param name="hasActiveTasks">Whether the project has any active tasks.</param>
        /// <returns>True if the project can be deleted, false otherwise.</returns>
        public static bool CanBeDeleted(Domain.ProjectManagement.Enums.ProjectStatus status, bool hasActiveTasks) =>
            status == Domain.ProjectManagement.Enums.ProjectStatus.Planning && !hasActiveTasks;
    }

    /// <summary>
    /// Business rules related to task status transitions and task lifecycle management.
    /// These rules enforce the valid state machine for task execution workflow.
    /// </summary>
    public static class TaskStatus
    {
        /// <summary>
        /// Determines if a task can transition from its current status to a target status.
        /// Implements the task status state machine with business rule validation.
        /// </summary>
        /// <param name="current">The current task status.</param>
        /// <param name="target">The target status to transition to.</param>
        /// <returns>True if the transition is allowed by business rules, false otherwise.</returns>
        /// <remarks>
        /// Valid transitions:
        /// - ToDo → InProgress (when work begins)
        /// - ToDo → Cancelled (when task is no longer needed)
        /// - InProgress → ToDo (when work is paused/reset)
        /// - InProgress → InReview (when work is submitted for review)
        /// - InProgress → Done (when work is completed without review)
        /// - InProgress → Cancelled (when active work is terminated)
        /// - InReview → InProgress (when review feedback requires more work)
        /// - InReview → Done (when review is approved)
        /// - InReview → Cancelled (when task is cancelled during review)
        /// - Done → No transitions allowed (final state)
        /// - Cancelled → No transitions allowed (final state)
        /// </remarks>
        public static bool CanTransitionTo(Domain.TaskExecution.Enums.TaskStatus current,
                                         Domain.TaskExecution.Enums.TaskStatus target)
        {
            return current switch
            {
                Domain.TaskExecution.Enums.TaskStatus.ToDo =>
                    target is Domain.TaskExecution.Enums.TaskStatus.InProgress or
                             Domain.TaskExecution.Enums.TaskStatus.Cancelled,

                Domain.TaskExecution.Enums.TaskStatus.InProgress =>
                    target is Domain.TaskExecution.Enums.TaskStatus.ToDo or
                             Domain.TaskExecution.Enums.TaskStatus.InReview or
                             Domain.TaskExecution.Enums.TaskStatus.Done or
                             Domain.TaskExecution.Enums.TaskStatus.Cancelled,

                Domain.TaskExecution.Enums.TaskStatus.InReview =>
                    target is Domain.TaskExecution.Enums.TaskStatus.InProgress or
                             Domain.TaskExecution.Enums.TaskStatus.Done or
                             Domain.TaskExecution.Enums.TaskStatus.Cancelled,

                Domain.TaskExecution.Enums.TaskStatus.Done => false, // Final state
                Domain.TaskExecution.Enums.TaskStatus.Cancelled => false, // Final state
                _ => false
            };
        }

        /// <summary>
        /// Determines if a task status represents a finalized state that cannot be changed.
        /// </summary>
        /// <param name="status">The task status to check.</param>
        /// <returns>True if the status is finalized, false otherwise.</returns>
        public static bool IsFinalized(Domain.TaskExecution.Enums.TaskStatus status) =>
            status is Domain.TaskExecution.Enums.TaskStatus.Done or
                     Domain.TaskExecution.Enums.TaskStatus.Cancelled;

        /// <summary>
        /// Determines if a task can be assigned to a user in the given status.
        /// </summary>
        /// <param name="status">The task status to check.</param>
        /// <returns>True if the task can be assigned, false otherwise.</returns>
        public static bool CanBeAssigned(Domain.TaskExecution.Enums.TaskStatus status) =>
            status is Domain.TaskExecution.Enums.TaskStatus.ToDo or
                     Domain.TaskExecution.Enums.TaskStatus.InProgress;

        /// <summary>
        /// Determines if a task can have subtasks created in the given status.
        /// </summary>
        /// <param name="status">The task status to check.</param>
        /// <returns>True if subtasks can be created, false otherwise.</returns>
        public static bool CanHaveSubtasks(Domain.TaskExecution.Enums.TaskStatus status) =>
            status is Domain.TaskExecution.Enums.TaskStatus.ToDo or
                     Domain.TaskExecution.Enums.TaskStatus.InProgress;

        /// <summary>
        /// Determines if a task can be completed when it has subtasks.
        /// </summary>
        /// <param name="subtaskStatuses">The statuses of all subtasks.</param>
        /// <returns>True if the parent task can be completed, false otherwise.</returns>
        public static bool CanCompleteWithSubtasks(IEnumerable<Domain.TaskExecution.Enums.TaskStatus> subtaskStatuses) =>
            subtaskStatuses.All(status => status == Domain.TaskExecution.Enums.TaskStatus.Done);
    }

    /// <summary>
    /// Business rules related to user authorization and permission management.
    /// These rules enforce role-based access control and hierarchical permissions.
    /// </summary>
    public static class Authorization
    {
        /// <summary>
        /// Determines if a user role has permission to create new users.
        /// Only administrators can create users to maintain security and control.
        /// </summary>
        /// <param name="role">The user role to check (null means no creator, which allows creation).</param>
        /// <returns>True if the role can create users, false otherwise.</returns>
        public static bool CanCreateUser(UserRole? role) =>
            role == UserRole.Admin || role == null;

        /// <summary>
        /// Determines if a user role has permission to manage team membership.
        /// Project managers and administrators can manage teams for organizational flexibility.
        /// </summary>
        /// <param name="role">The user role to check.</param>
        /// <returns>True if the role can manage teams, false otherwise.</returns>
        public static bool CanManageTeam(UserRole role) =>
            role is UserRole.Admin or UserRole.ProjectManager;

        /// <summary>
        /// Determines if a user role has permission to manage projects.
        /// Project managers and administrators can manage projects within their domain.
        /// </summary>
        /// <param name="role">The user role to check.</param>
        /// <returns>True if the role can manage projects, false otherwise.</returns>
        public static bool CanManageProject(UserRole role) =>
            role is UserRole.Admin or UserRole.ProjectManager;

        /// <summary>
        /// Determines if a user role can be assigned tasks.
        /// Developers, project managers, and administrators can be assigned work.
        /// Viewers are excluded as they have read-only access.
        /// </summary>
        /// <param name="role">The user role to check.</param>
        /// <returns>True if the role can be assigned tasks, false otherwise.</returns>
        public static bool CanBeAssignedTasks(UserRole role) =>
            role is UserRole.Developer or UserRole.ProjectManager or UserRole.Admin;

        /// <summary>
        /// Determines if a user role can assign tasks to other users.
        /// Project managers and administrators can assign tasks for workflow management.
        /// </summary>
        /// <param name="role">The user role to check.</param>
        /// <returns>True if the role can assign tasks, false otherwise.</returns>
        public static bool CanAssignTasks(UserRole role) =>
            role is UserRole.ProjectManager or UserRole.Admin;

        /// <summary>
        /// Determines if a user role can modify another user's role.
        /// Only administrators can change roles to maintain security hierarchy.
        /// </summary>
        /// <param name="modifierRole">The role of the user making the change.</param>
        /// <param name="targetRole">The current role of the user being modified.</param>
        /// <returns>True if the role change is authorized, false otherwise.</returns>
        public static bool CanModifyUserRole(UserRole modifierRole, UserRole targetRole) =>
            modifierRole == UserRole.Admin && targetRole != UserRole.Admin;

        /// <summary>
        /// Determines if a user has sufficient privileges to perform an action on another user.
        /// Uses role hierarchy to enforce privilege-based access control.
        /// </summary>
        /// <param name="actorRole">The role of the user performing the action.</param>
        /// <param name="targetRole">The role of the user being acted upon.</param>
        /// <returns>True if the actor has sufficient privileges, false otherwise.</returns>
        public static bool HasSufficientPrivileges(UserRole actorRole, UserRole targetRole)
        {
            var actorLevel = GetRoleHierarchyLevel(actorRole);
            var targetLevel = GetRoleHierarchyLevel(targetRole);
            return actorLevel > targetLevel;
        }

        /// <summary>
        /// Gets the numeric hierarchy level for a user role.
        /// Higher numbers indicate greater privileges in the system.
        /// </summary>
        /// <param name="role">The user role.</param>
        /// <returns>A numeric value representing the hierarchy level.</returns>
        private static int GetRoleHierarchyLevel(UserRole role) => role switch
        {
            UserRole.Viewer => 1,
            UserRole.Developer => 2,
            UserRole.ProjectManager => 3,
            UserRole.Admin => 4,
            _ => 0
        };
    }

    /// <summary>
    /// Business rules related to team management and collaboration constraints.
    /// These rules enforce team composition, membership, and operational guidelines.
    /// </summary>
    public static class TeamManagement
    {
        /// <summary>
        /// Determines if a user can be added to a team based on various business constraints.
        /// </summary>
        /// <param name="userStatus">The status of the user being added.</param>
        /// <param name="userRole">The role of the user being added.</param>
        /// <param name="currentMemberCount">The current number of team members.</param>
        /// <param name="requestorRole">The role of the user making the request.</param>
        /// <returns>True if the user can be added to the team, false otherwise.</returns>
        public static bool CanAddMemberToTeam(UserStatus userStatus, UserRole userRole,
                                            int currentMemberCount, UserRole requestorRole)
        {
            return (requestorRole, userStatus, currentMemberCount) switch
            {
                var (role, status, count) when !Authorization.CanManageTeam(role) => false,
                var (_, status, _) when status != UserStatus.Active => false,
                var (_, _, count) when count >= ValidationConstants.Numeric.TeamMaxMembers => false,
                _ => true
            };
        }

        /// <summary>
        /// Determines if a user can be removed from a team.
        /// </summary>
        /// <param name="requestorRole">The role of the user making the request.</param>
        /// <param name="targetRole">The role of the user being removed.</param>
        /// <param name="teamHasActiveProjects">Whether the team has active projects.</param>
        /// <returns>True if the user can be removed, false otherwise.</returns>
        public static bool CanRemoveMemberFromTeam(UserRole requestorRole, UserRole targetRole,
                                                 bool teamHasActiveProjects)
        {
            // Check if requestor has permission to manage teams
            if (!Authorization.CanManageTeam(requestorRole))
                return false;

            // Prevent removing team members if team has active projects
            // unless the requestor is an admin
            if (teamHasActiveProjects && requestorRole != UserRole.Admin)
                return false;

            // Check privilege hierarchy
            return Authorization.HasSufficientPrivileges(requestorRole, targetRole);
        }

        /// <summary>
        /// Determines if a team can be deleted based on its current state.
        /// </summary>
        /// <param name="hasActiveProjects">Whether the team has any active projects.</param>
        /// <param name="memberCount">The current number of team members.</param>
        /// <param name="requestorRole">The role of the user making the request.</param>
        /// <returns>True if the team can be deleted, false otherwise.</returns>
        public static bool CanDeleteTeam(bool hasActiveProjects, int memberCount, UserRole requestorRole)
        {
            // Only admins and project managers can delete teams
            if (!Authorization.CanManageTeam(requestorRole))
                return false;

            // Cannot delete teams with active projects
            if (hasActiveProjects)
                return false;

            // For safety, require admin privileges to delete teams with members
            if (memberCount > 0 && requestorRole != UserRole.Admin)
                return false;

            return true;
        }
    }

    /// <summary>
    /// Business rules related to task assignment and work allocation.
    /// These rules ensure proper task distribution and assignment constraints.
    /// </summary>
    public static class TaskAssignment
    {
        /// <summary>
        /// Determines if a task can be assigned to a specific user.
        /// </summary>
        /// <param name="taskStatus">The current status of the task.</param>
        /// <param name="assigneeRole">The role of the user being assigned.</param>
        /// <param name="assigneeStatus">The status of the user being assigned.</param>
        /// <param name="assignerRole">The role of the user making the assignment.</param>
        /// <param name="isAlreadyAssigned">Whether the task is already assigned to someone.</param>
        /// <returns>True if the task can be assigned, false otherwise.</returns>
        public static bool CanAssignTask(Domain.TaskExecution.Enums.TaskStatus taskStatus,
                                       UserRole assigneeRole, UserStatus assigneeStatus,
                                       UserRole assignerRole, bool isAlreadyAssigned)
        {
            return (assignerRole, taskStatus, assigneeRole, assigneeStatus) switch
            {
                var (role, _, _, _) when !Authorization.CanAssignTasks(role) => false,
                var (_, status, _, _) when !TaskStatus.CanBeAssigned(status) => false,
                var (_, _, role, _) when !Authorization.CanBeAssignedTasks(role) => false,
                var (_, _, _, status) when status != UserStatus.Active => false,
                _ => true
            };
        }

        /// <summary>
        /// Determines if a task can be unassigned from its current assignee.
        /// </summary>
        /// <param name="taskStatus">The current status of the task.</param>
        /// <param name="unassignerRole">The role of the user removing the assignment.</param>
        /// <param name="assigneeRole">The role of the currently assigned user.</param>
        /// <returns>True if the task can be unassigned, false otherwise.</returns>
        public static bool CanUnassignTask(Domain.TaskExecution.Enums.TaskStatus taskStatus,
                                         UserRole unassignerRole, UserRole assigneeRole)
        {
            // Check if unassigner has permission to assign/unassign tasks
            if (!Authorization.CanAssignTasks(unassignerRole))
                return false;

            // Cannot unassign completed or cancelled tasks
            if (TaskStatus.IsFinalized(taskStatus))
                return false;

            // Check privilege hierarchy for removing assignments
            return Authorization.HasSufficientPrivileges(unassignerRole, assigneeRole);
        }

        /// <summary>
        /// Determines the maximum number of tasks that can be assigned to a user concurrently.
        /// </summary>
        /// <param name="role">The user role.</param>
        /// <returns>The maximum number of concurrent task assignments.</returns>
        public static int GetMaxConcurrentAssignments(UserRole role) => role switch
        {
            UserRole.Developer => 5,
            UserRole.ProjectManager => 10,
            UserRole.Admin => 15,
            _ => 0
        };
    }

    /// <summary>
    /// Business rules related to data validation and business constraint enforcement.
    /// These rules provide complex validation logic that spans multiple entities.
    /// </summary>
    public static class DataValidation
    {
        /// <summary>
        /// Validates if a due date is acceptable based on business rules.
        /// </summary>
        /// <param name="dueDate">The proposed due date.</param>
        /// <param name="entityType">The type of entity (task, project, etc.).</param>
        /// <returns>True if the due date is valid, false otherwise.</returns>
        public static bool IsValidDueDate(DateTime? dueDate, string entityType)
        {
            var now = DateTime.UtcNow.AddMinutes(ValidationConstants.DateTimes.FutureDateBufferMinutes);
            
            return (dueDate, entityType?.ToLowerInvariant()) switch
            {
                (null, _) => true, // Null due dates are allowed
                (var date, _) when date <= now => false, // Due date must be in the future
                (var date, "task") => IsValidTaskDueDate(date.Value, now),
                (var date, "project") => IsValidProjectDueDate(date.Value, now),
                (var date, _) => IsValidGenericDueDate(date.Value, now)
            };
        }

        /// <summary>
        /// Validates task-specific due date constraints.
        /// </summary>
        private static bool IsValidTaskDueDate(DateTime dueDate, DateTime now)
        {
            var maxFutureDate = now.AddDays(ValidationConstants.DateTimes.TaskMaxFutureDays);
            return dueDate <= maxFutureDate;
        }

        /// <summary>
        /// Validates project-specific due date constraints.
        /// </summary>
        private static bool IsValidProjectDueDate(DateTime dueDate, DateTime now)
        {
            var maxFutureDate = now.AddDays(ValidationConstants.DateTimes.ProjectMaxFutureDays);
            return dueDate <= maxFutureDate;
        }

        /// <summary>
        /// Validates generic due date constraints.
        /// </summary>
        private static bool IsValidGenericDueDate(DateTime dueDate, DateTime _)
        {
            return dueDate >= ValidationConstants.DateTimes.MinAllowedDate &&
                   dueDate <= ValidationConstants.DateTimes.MaxAllowedDate;
        }

        /// <summary>
        /// Validates if an estimated hours value is reasonable for the given context.
        /// </summary>
        /// <param name="estimatedHours">The estimated hours value.</param>
        /// <param name="parentEstimatedHours">The estimated hours of the parent task (if any).</param>
        /// <returns>True if the estimation is valid, false otherwise.</returns>
        public static bool IsValidEstimatedHours(int? estimatedHours, int? parentEstimatedHours)
        {
            return (estimatedHours, parentEstimatedHours) switch
            {
                (null, _) => true, // Null estimates are allowed
                (var hours, _) when hours < ValidationConstants.Numeric.TaskMinEstimatedHours ||
                                    hours > ValidationConstants.Numeric.TaskMaxEstimatedHours => false,
                (var hours, var parent) when parent.HasValue && hours > parent.Value => false,
                _ => true
            };
        }
    }
}
