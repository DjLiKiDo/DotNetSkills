namespace DotNetSkills.Infrastructure.Repositories.Common;

/// <summary>
/// Unit of Work implementation that coordinates changes across multiple repositories
/// and manages database transactions.
/// Domain events are handled by the DomainEventDispatchBehavior in the application layer.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IDomainEventCollectionService _eventCollectionService;
    private readonly ILogger<UnitOfWork> _logger;

    // Repository instances (lazy initialization)
    private IUserRepository? _users;
    private ITeamRepository? _teams;
    private IProjectRepository? _projects;
    private ITaskRepository? _tasks;

    // Transaction management
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="eventCollectionService">The domain event collection service.</param>
    /// <param name="logger">The logger instance.</param>
    public UnitOfWork(
        ApplicationDbContext context, 
        IDomainEventCollectionService eventCollectionService,
        ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _eventCollectionService = eventCollectionService ?? throw new ArgumentNullException(nameof(eventCollectionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the User repository with lazy initialization.
    /// </summary>
    public IUserRepository Users
    {
        get
        {
            _users ??= new UserRepository(_context, _eventCollectionService);
            return _users;
        }
    }

    /// <summary>
    /// Gets the Team repository with lazy initialization.
    /// </summary>
    public ITeamRepository Teams
    {
        get
        {
            _teams ??= new TeamRepository(_context, _eventCollectionService);
            return _teams;
        }
    }

    /// <summary>
    /// Gets the Project repository with lazy initialization.
    /// </summary>
    public IProjectRepository Projects
    {
        get
        {
            _projects ??= new ProjectRepository(_context, _eventCollectionService);
            return _projects;
        }
    }

    /// <summary>
    /// Gets the Task repository with lazy initialization.
    /// </summary>
    public ITaskRepository Tasks
    {
        get
        {
            _tasks ??= new TaskRepository(_context, _eventCollectionService);
            return _tasks;
        }
    }

    /// <summary>
    /// Saves all changes made in this unit of work to the underlying data store asynchronously.
    /// This method commits the current transaction and persists all pending changes.
    /// Domain events are handled by the DomainEventDispatchBehavior in the application layer.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of entities that were affected by the save operation.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting SaveChanges operation");
        
        try
        {
            // Save changes to the database
            var affectedRows = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            
            _logger.LogDebug("SaveChanges completed successfully, affected {RowCount} rows", affectedRows);

            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during SaveChanges operation");
            throw;
        }
    }

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// Use this when you need explicit transaction control beyond the default SaveChanges behavior.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async System.Threading.Tasks.Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already active. Complete the current transaction before starting a new one.");
        }

        _logger.LogDebug("Beginning new database transaction");
        
        try
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while beginning database transaction");
            throw;
        }
    }

    /// <summary>
    /// Commits the current database transaction asynchronously.
    /// Should be called after BeginTransactionAsync and successful operations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async System.Threading.Tasks.Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit. Call BeginTransactionAsync first.");
        }

        _logger.LogDebug("Committing database transaction");
        
        try
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Database transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while committing database transaction");
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current database transaction asynchronously.
    /// Should be called when an error occurs during transaction processing.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async System.Threading.Tasks.Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _logger.LogWarning("Attempted to rollback transaction, but no active transaction exists");
            return;
        }

        _logger.LogDebug("Rolling back database transaction");
        
        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            _logger.LogDebug("Database transaction rolled back successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while rolling back database transaction");
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously disposes the core resources of the UnitOfWork.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_currentTransaction != null)
        {
            _logger.LogWarning("Disposing UnitOfWork with active transaction - rolling back");
            await _currentTransaction.RollbackAsync().ConfigureAwait(false);
            await _currentTransaction.DisposeAsync().ConfigureAwait(false);
            _currentTransaction = null;
        }

        // Note: We don't dispose the DbContext here as it's managed by DI container
        _logger.LogDebug("UnitOfWork async disposed successfully");
    }

    /// <summary>
    /// Disposes the UnitOfWork and its resources.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources, false otherwise.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            try
            {
                // Rollback any active transaction
                if (_currentTransaction != null)
                {
                    _logger.LogWarning("Disposing UnitOfWork with active transaction - rolling back");
                    _currentTransaction.Rollback();
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }

                // Note: We don't dispose the DbContext here as it's managed by DI container
                _logger.LogDebug("UnitOfWork disposed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while disposing UnitOfWork");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Finalizer for UnitOfWork.
    /// </summary>
    ~UnitOfWork()
    {
        Dispose(false);
    }
}
