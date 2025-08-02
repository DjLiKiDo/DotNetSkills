namespace DotNetSkills.Domain;

/// <summary>
/// Domain layer service registration helpers.
/// Note: Domain layer should not depend on Microsoft.Extensions.DependencyInjection.
/// This class provides factory methods for creating domain services when needed.
/// </summary>
public static class DomainServiceFactory
{
    /// <summary>
    /// Creates domain validation services when needed by upper layers.
    /// This method should be called from Application or Infrastructure layers.
    /// </summary>
    /// <returns>A collection of domain service descriptors.</returns>
    public static IEnumerable<(Type ServiceType, Type ImplementationType)> GetDomainServices()
    {
        // Domain services that might be needed
        // Example: yield return (typeof(IDomainService), typeof(DomainService));
        
        // Domain event handlers (when implemented)
        // Example: yield return (typeof(IDomainEventHandler<UserCreatedDomainEvent>), typeof(UserCreatedDomainEventHandler));
        
        // Domain validators or rule engines (if needed)
        // Example: yield return (typeof(IBusinessRuleValidator), typeof(BusinessRuleValidator));
        
        yield break; // Empty for now, services will be added as needed
    }
}
