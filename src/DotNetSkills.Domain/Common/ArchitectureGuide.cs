// using System.Runtime.CompilerServices;

// namespace DotNetSkills.Domain.Common;

// /// <summary>
// /// Contains discovery helpers for IDE support and developer experience.
// /// </summary>
// /// <remarks>
// /// This class provides compile-time hints and documentation for better IntelliSense support.
// /// It helps developers discover related components and understand the architecture.
// /// </remarks>
// public static class ArchitectureGuide
// {
//     /// <summary>
//     /// Entry points for each bounded context.
//     /// Use these as starting points when exploring the codebase.
//     /// </summary>
//     public static class BoundedContexts
//     {
//         /// <summary>
//         /// User Management - Core user operations and authentication
//         /// Entry Point: <see cref="UserManagement.Entities.User"/>
//         /// Key Operations: Create, Activate, Deactivate users
//         /// </summary>
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         public static string UserManagement => "Domain.UserManagement.Entities.User";
        
//         /// <summary>
//         /// Team Collaboration - Team and membership management
//         /// Entry Point: <see cref="TeamCollaboration.Entities.Team"/>
//         /// Key Operations: Create teams, manage membership, assign roles
//         /// </summary>
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         public static string TeamCollaboration => "Domain.TeamCollaboration.Entities.Team";
        
//         /// <summary>
//         /// Project Management - Project lifecycle and status
//         /// Entry Point: <see cref="ProjectManagement.Entities.Project"/>
//         /// Key Operations: Create, start, complete projects
//         /// </summary>
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         public static string ProjectManagement => "Domain.ProjectManagement.Entities.Project";
        
//         /// <summary>
//         /// Task Execution - Task management and assignment
//         /// Entry Point: <see cref="TaskExecution.Entities.Task"/>
//         /// Key Operations: Create, assign, track task progress
//         /// </summary>
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         public static string TaskExecution => "Domain.TaskExecution.Entities.Task";
//     }
    
//     /// <summary>
//     /// Common patterns used throughout the solution.
//     /// Reference these when implementing new features.
//     /// </summary>
//     public static class Patterns
//     {
//         /// <summary>
//         /// All aggregate roots follow this pattern.
//         /// See: <see cref="Common.Entities.AggregateRoot{TId}"/>
//         /// </summary>
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         public static string AggregateRoot => "Inherit from AggregateRoot<TId>, raise domain events";
        
//         /// <summary>
//         /// Strong-typed identifiers pattern.
//         /// Example: <see cref="UserManagement.ValueObjects.UserId"/>
//         /// </summary>
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         public static string StronglyTypedIds => "record EntityId(Guid Value) : IStronglyTypedId<Guid>";
        
//         /// <summary>
//         /// Domain events pattern for cross-aggregate communication.
//         /// Example: <see cref="UserManagement.Events.UserCreatedDomainEvent"/>
//         /// </summary>
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         public static string DomainEvents => "Use domain events for loose coupling between aggregates";
        
//         /// <summary>
//         /// Business rule validation pattern.
//         /// See: <see cref="Common.Validation.Ensure"/>
//         /// </summary>
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         public static string BusinessRules => "Use Ensure.BusinessRule() for domain validation";
//     }
    
//     /// <summary>
//     /// Navigation helpers for finding related components.
//     /// </summary>
//     public static class Navigation
//     {
//         /// <summary>
//         /// Given an entity, find its related components:
//         /// - Repository: Application/[Context]/Interfaces/I[Entity]Repository
//         /// - Commands: Application/[Context]/Commands/[Action][Entity]Command
//         /// - Queries: Application/[Context]/Queries/[Query][Entity]Query
//         /// - DTOs: Application/[Context]/DTOs/[Entity]Response
//         /// - Endpoints: API/Endpoints/[Context]/[Entity]Endpoints
//         /// </summary>
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         public static string EntityToComponents => "Follow naming conventions to find related components";
        
//         /// <summary>
//         /// Given a use case, find its implementation:
//         /// - Handler: Application/[Context]/Handlers/[Command/Query]Handler
//         /// - Validator: Application/[Context]/Validators/[Command/Query]Validator
//         /// - Mapping: Application/[Context]/Mappings/[Entity]MappingProfile
//         /// </summary>
//         [MethodImpl(MethodImplOptions.NoInlining)]
//         public static string UseCaseToImplementation => "Handlers, validators, and mappings follow naming conventions";
//     }
// }
