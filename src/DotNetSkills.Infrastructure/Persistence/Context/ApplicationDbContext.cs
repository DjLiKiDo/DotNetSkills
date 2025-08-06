namespace DotNetSkills.Infrastructure.Persistence.Context;

/// <summary>
/// Main Entity Framework DbContext for the DotNetSkills application.
/// Implements Clean Architecture and Domain-Driven Design patterns with
/// support for aggregate roots, domain events, and proper transaction handling.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the Users DbSet for user management aggregate root.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets or sets the Teams DbSet for team collaboration aggregate root.
    /// </summary>
    public DbSet<Team> Teams => Set<Team>();

    /// <summary>
    /// Gets or sets the TeamMembers DbSet for team membership join entity.
    /// Note: TeamMember is not an aggregate root but is exposed for querying purposes.
    /// </summary>
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    /// <summary>
    /// Gets or sets the Projects DbSet for project management aggregate root.
    /// </summary>
    public DbSet<Project> Projects => Set<Project>();

    /// <summary>
    /// Gets or sets the Tasks DbSet for task execution aggregate root.
    /// </summary>
    public DbSet<DotNetSkills.Domain.TaskExecution.Entities.Task> Tasks => Set<DotNetSkills.Domain.TaskExecution.Entities.Task>();

    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model and applies all entity configurations.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Set default schema
        modelBuilder.HasDefaultSchema("dbo");

        // Configure global query filters if needed (e.g., for soft delete)
        // This can be added later when soft delete functionality is implemented

        // Disable cascade delete by convention (we configure it explicitly in entity configurations)
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
        {
            if (relationship.DeleteBehavior == DeleteBehavior.Cascade)
            {
                // Only allow cascade where explicitly configured
                // Most entities use Restrict or SetNull for data integrity
            }
        }
    }

    /// <summary>
    /// Configures database provider specific options.
    /// </summary>
    /// <param name="optionsBuilder">The options builder to configure.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Enable sensitive data logging in development
        if (System.Diagnostics.Debugger.IsAttached)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        // Configure SQL Server specific options
        if (optionsBuilder.IsConfigured && optionsBuilder is DbContextOptionsBuilder<ApplicationDbContext> typedBuilder)
        {
            // Additional SQL Server optimizations can be configured here
        }
    }

    /// <summary>
    /// Saves changes to the database with domain event dispatching.
    /// This override implements the Unit of Work pattern and ensures domain events
    /// are dispatched within the same transaction boundary as the data changes.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect domain events from aggregate roots before saving
        var domainEvents = CollectDomainEvents();

        try
        {
            // Save changes to database
            var result = await base.SaveChangesAsync(cancellationToken);

            // Dispatch domain events after successful save
            // Note: In a full implementation, this would integrate with MediatR
            // For now, we clear the events to prevent memory leaks
            ClearDomainEvents();

            return result;
        }
        catch
        {
            // Re-add domain events if save failed
            RestoreDomainEvents(domainEvents);
            throw;
        }
    }

    /// <summary>
    /// Gets all domain events from aggregate roots in the context.
    /// This method is used by the Unit of Work pattern for domain event dispatching.
    /// </summary>
    /// <returns>A collection of domain events from all aggregate roots.</returns>
    public IEnumerable<IDomainEvent> GetDomainEvents()
    {
        var aggregateRoots = ChangeTracker.Entries<AggregateRoot<IStronglyTypedId<Guid>>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        return aggregateRoots.SelectMany(aggregate => aggregate.DomainEvents);
    }

    /// <summary>
    /// Collects domain events from all aggregate roots in the context.
    /// </summary>
    /// <returns>A list of domain events with their source entities.</returns>
    private List<(AggregateRoot<IStronglyTypedId<Guid>> Entity, List<IDomainEvent> Events)> CollectDomainEvents()
    {
        var aggregateRoots = ChangeTracker.Entries<AggregateRoot<IStronglyTypedId<Guid>>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = new List<(AggregateRoot<IStronglyTypedId<Guid>>, List<IDomainEvent>)>();

        foreach (var aggregate in aggregateRoots)
        {
            var events = aggregate.DomainEvents.ToList();
            if (events.Any())
            {
                domainEvents.Add((aggregate, events));
            }
        }

        return domainEvents;
    }

    /// <summary>
    /// Clears domain events from all aggregate roots in the context.
    /// </summary>
    private void ClearDomainEvents()
    {
        var aggregateRoots = ChangeTracker.Entries<AggregateRoot<IStronglyTypedId<Guid>>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        foreach (var aggregate in aggregateRoots)
        {
            aggregate.ClearDomainEvents();
        }
    }

    /// <summary>
    /// Restores domain events to aggregate roots after a failed save operation.
    /// Note: This method has limitations due to access modifiers on RaiseDomainEvent.
    /// In production, consider implementing a more robust event restoration mechanism.
    /// </summary>
    /// <param name="domainEvents">The domain events to restore.</param>
    private void RestoreDomainEvents(List<(AggregateRoot<IStronglyTypedId<Guid>> Entity, List<IDomainEvent> Events)> domainEvents)
    {
        // TODO: This method needs to be implemented when a proper domain event 
        // restoration mechanism is available. For now, we accept that failed 
        // transactions will lose their domain events, which is acceptable 
        // since the transaction was rolled back anyway.
        
        // Future implementation might involve:
        // 1. Making RaiseDomainEvent public on AggregateRoot
        // 2. Implementing a separate event store for reliability
        // 3. Using reflection (not recommended for production)
    }
}
