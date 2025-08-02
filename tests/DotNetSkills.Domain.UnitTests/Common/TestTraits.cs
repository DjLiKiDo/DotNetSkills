namespace DotNetSkills.Domain.UnitTests.Common;

/// <summary>
/// Provides constants for test traits and categories.
/// </summary>
public static class TestTraits
{
    /// <summary>
    /// Test category trait names.
    /// </summary>
    public static class Category
    {
        public const string Unit = "Unit";
        public const string Domain = "Domain";
        public const string Fast = "Fast";
        public const string Entity = "Entity";
        public const string ValueObject = "ValueObject";
        public const string DomainEvent = "DomainEvent";
        public const string Enum = "Enum";
        public const string BusinessRule = "BusinessRule";
    }

    /// <summary>
    /// Bounded context trait names.
    /// </summary>
    public static class BoundedContext
    {
        public const string UserManagement = "UserManagement";
        public const string TeamCollaboration = "TeamCollaboration";
        public const string ProjectManagement = "ProjectManagement";
        public const string TaskExecution = "TaskExecution";
        public const string Common = "Common";
    }

    /// <summary>
    /// Test type trait names.
    /// </summary>
    public static class TestType
    {
        public const string Creation = "Creation";
        public const string Validation = "Validation";
        public const string BusinessLogic = "BusinessLogic";
        public const string StateTransition = "StateTransition";
        public const string EventRaising = "EventRaising";
        public const string EdgeCase = "EdgeCase";
        public const string Performance = "Performance";
    }
}

// Custom trait attributes for test organization
// Note: Using standard TraitAttribute directly due to sealed class constraints