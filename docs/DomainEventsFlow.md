# Domain Events Flow Documentation

## Overview

The DotNetSkills project implements a complete Domain Event Collection and Dispatch system following Clean Architecture principles. This system enables decoupled communication between aggregates while maintaining domain purity.

## Key Components

### 1. Domain Layer
- **IDomainEvent**: Base interface for all domain events
- **IAggregateRoot**: Non-generic interface for polymorphic aggregate handling
- **AggregateRoot<TId>**: Base class for aggregate roots with domain event capabilities
- **TaskAssignedDomainEvent**: Example domain event

### 2. Application Layer
- **IDomainEventCollectionService**: Interface for tracking modified aggregates
- **DomainEventCollectionService**: Thread-safe implementation using AsyncLocal
- **DomainEventDispatchBehavior**: MediatR pipeline behavior for event dispatching
- **DomainEventNotification<T>**: Wrapper bridging domain events to MediatR notifications
- **TaskAssignedDomainEventHandler**: Example event handler

### 3. Infrastructure Layer
- **BaseRepository**: Automatically registers modified aggregates during Add/Update operations
- **UnitOfWork**: Simplified to focus on data persistence, delegates event handling to behavior
- **DomainEventDispatcher**: Uses reflection to create generic notification wrappers

## Event Flow

```
1. Domain Entity Method Called
   ↓
2. Business Logic Executed + Domain Event Raised
   ↓
3. Repository Add/Update Called
   ↓
4. BaseRepository Registers Modified Aggregate
   ↓
5. Command Handler Completes Successfully
   ↓
6. DomainEventDispatchBehavior Executes
   ↓
7. Events Retrieved from Modified Aggregates
   ↓
8. Events Wrapped in DomainEventNotification<T>
   ↓
9. Events Dispatched via MediatR
   ↓
10. Domain Event Handlers Execute
   ↓
11. Aggregates Cleared of Events
   ↓
12. Tracking Cleared
```

## Thread Safety

The system uses `AsyncLocal<HashSet<IAggregateRoot>>` to ensure proper isolation of aggregate tracking per request context. This prevents cross-request pollution while maintaining thread safety.

## Clean Architecture Compliance

- **Domain Layer**: Contains pure domain events and interfaces
- **Application Layer**: Handles event collection, dispatch logic, and handlers
- **Infrastructure Layer**: Provides concrete implementations and MediatR integration
- **No Dependencies**: Domain never references Application or Infrastructure layers

## Example Usage

### 1. Domain Entity Raising Event

```csharp
public class Task : AggregateRoot<TaskId>
{
    public void AssignTo(UserId assignedUserId, UserId assignedByUserId)
    {
        // Business logic validation
        if (Status == TaskStatus.Completed)
            throw new DomainException("Cannot assign completed tasks");
            
        // Update state
        AssignedUserId = assignedUserId;
        AssignedByUserId = assignedByUserId;
        AssignedAt = DateTime.UtcNow;
        Status = TaskStatus.InProgress;
        
        // Raise domain event
        RaiseDomainEvent(new TaskAssignedDomainEvent(Id, assignedUserId, assignedByUserId));
    }
}
```

### 2. Repository Integration

```csharp
public class EfTaskRepository : BaseRepository<Task, TaskId>, ITaskRepository
{
    public async Task<Task> AddAsync(Task task)
    {
        _context.Tasks.Add(task);
        
        // BaseRepository automatically registers modified aggregate
        RegisterModifiedAggregate(task);
        
        return task;
    }
}
```

### 3. Event Handler

```csharp
public class TaskAssignedDomainEventHandler : INotificationHandler<DomainEventNotification<TaskAssignedDomainEvent>>
{
    public async Task Handle(DomainEventNotification<TaskAssignedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        // Handle the event (send notifications, update read models, etc.)
        _logger.LogInformation("Task {TaskId} was assigned to user {UserId} by {AssignedByUserId}",
            domainEvent.TaskId, domainEvent.AssignedUserId, domainEvent.AssignedByUserId);
            
        // Additional business logic...
    }
}
```

## Testing

The system includes comprehensive tests covering:

- **Unit Tests**: Domain event collection, dispatch behavior, event handlers
- **Integration Tests**: End-to-end event flow verification
- **Edge Cases**: Error handling, cancellation, thread safety

All 160 tests pass, ensuring robust functionality.

## Benefits

1. **Decoupling**: Aggregates don't need to know about each other
2. **Consistency**: Events are only dispatched after successful command execution
3. **Reliability**: Thread-safe implementation prevents data corruption
4. **Testability**: All components are easily unit testable
5. **Extensibility**: New event handlers can be added without modifying existing code
6. **Performance**: Efficient tracking using AsyncLocal and reflection only when needed

## Best Practices

1. **Keep Events Simple**: Domain events should be immutable and contain only essential data
2. **Handle Failures Gracefully**: Event handlers should not fail the main operation
3. **Use Correlation IDs**: For tracing event flows across handlers
4. **Test Event Flow**: Verify events are raised and handled correctly
5. **Monitor Performance**: Watch for performance impact of event dispatching

This implementation provides a solid foundation for event-driven architecture while maintaining Clean Architecture principles and ensuring high code quality.
