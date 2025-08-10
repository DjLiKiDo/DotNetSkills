namespace DotNetSkills.Infrastructure.Repositories.Common;

/// <summary>
/// Extension methods and utilities for repository implementations.
/// Provides common query patterns and optimization strategies.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Applies search filter to a queryable based on the specified search term and properties.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable to filter.</param>
    /// <param name="searchTerm">The search term to filter by.</param>
    /// <param name="searchProperties">The properties to search in.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> ApplySearch<T>(
        this IQueryable<T> query,
        string? searchTerm,
        params Expression<Func<T, string>>[] searchProperties)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || !searchProperties.Any())
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? searchExpression = null;

        foreach (var propertyExpression in searchProperties)
        {
            // Replace parameter in the property expression
            var propertyBody = ReplaceParmeterVisitor.ReplaceParameter(
                propertyExpression.Body, 
                propertyExpression.Parameters[0], 
                parameter);

            // Create Contains expression for the property
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var containsExpression = Expression.Call(
                propertyBody,
                containsMethod,
                Expression.Constant(searchTerm, typeof(string)));

            // Combine with OR
            searchExpression = searchExpression == null 
                ? containsExpression 
                : Expression.OrElse(searchExpression, containsExpression);
        }

        if (searchExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    /// <summary>
    /// Applies optional enum filter to a queryable.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="query">The queryable to filter.</param>
    /// <param name="filterValue">The optional enum value to filter by.</param>
    /// <param name="propertyExpression">The property expression to filter on.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> ApplyEnumFilter<T, TEnum>(
        this IQueryable<T> query,
        TEnum? filterValue,
        Expression<Func<T, TEnum>> propertyExpression)
        where TEnum : struct, Enum
    {
        if (filterValue.HasValue)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyBody = ReplaceParmeterVisitor.ReplaceParameter(
                propertyExpression.Body,
                propertyExpression.Parameters[0],
                parameter);

            var equalsExpression = Expression.Equal(
                propertyBody,
                Expression.Constant(filterValue.Value, typeof(TEnum)));

            var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    /// <summary>
    /// Applies date range filter to a queryable.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable to filter.</param>
    /// <param name="dateFrom">The start date (inclusive).</param>
    /// <param name="dateTo">The end date (inclusive).</param>
    /// <param name="propertyExpression">The date property expression to filter on.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> ApplyDateRangeFilter<T>(
        this IQueryable<T> query,
        DateTime? dateFrom,
        DateTime? dateTo,
        Expression<Func<T, DateTime?>> propertyExpression)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyBody = ReplaceParmeterVisitor.ReplaceParameter(
            propertyExpression.Body,
            propertyExpression.Parameters[0],
            parameter);

        Expression? filterExpression = null;

        if (dateFrom.HasValue)
        {
            var fromExpression = Expression.GreaterThanOrEqual(
                propertyBody,
                Expression.Constant(dateFrom.Value, typeof(DateTime?)));
            filterExpression = fromExpression;
        }

        if (dateTo.HasValue)
        {
            var toExpression = Expression.LessThanOrEqual(
                propertyBody,
                Expression.Constant(dateTo.Value.Date.AddDays(1).AddTicks(-1), typeof(DateTime?)));
            
            filterExpression = filterExpression == null 
                ? toExpression 
                : Expression.AndAlso(filterExpression, toExpression);
        }

        if (filterExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    /// <summary>
    /// Applies ordering to a queryable with multiple sort criteria.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable to order.</param>
    /// <param name="orderBy">The primary order by expression.</param>
    /// <param name="orderByDescending">Whether to order descending.</param>
    /// <param name="thenByExpressions">Additional then by expressions.</param>
    /// <returns>The ordered queryable.</returns>
    public static IOrderedQueryable<T> ApplyOrdering<T>(
        this IQueryable<T> query,
        Expression<Func<T, object>> orderBy,
        bool orderByDescending = false,
        params (Expression<Func<T, object>> expression, bool descending)[] thenByExpressions)
    {
        var orderedQuery = orderByDescending 
            ? query.OrderByDescending(orderBy) 
            : query.OrderBy(orderBy);

        foreach (var (expression, descending) in thenByExpressions)
        {
            orderedQuery = descending 
                ? orderedQuery.ThenByDescending(expression) 
                : orderedQuery.ThenBy(expression);
        }

        return orderedQuery;
    }

    /// <summary>
    /// Gets a paginated result with total count optimization.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The base query to paginate.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the items and total count.</returns>
    public static async Task<(IReadOnlyList<T> Items, int TotalCount)> ToPaginatedListAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0)
            throw new ArgumentException("Page number must be greater than zero.", nameof(pageNumber));
        
        if (pageSize <= 0)
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

        // Execute count and data queries in parallel for better performance
        var totalCountTask = query.CountAsync(cancellationToken);
        var itemsTask = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        await System.Threading.Tasks.Task.WhenAll(totalCountTask, itemsTask);

        var totalCount = await totalCountTask;
        var items = await itemsTask;

        return (items.AsReadOnly(), totalCount);
    }

    /// <summary>
    /// Checks if any entity matches the specified condition efficiently.
    /// Uses EXISTS pattern for optimal performance.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable to check.</param>
    /// <param name="predicate">The condition to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any entity matches the condition, otherwise false.</returns>
    public static async Task<bool> ExistsAsync<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return await query
            .AsNoTracking()
            .Where(predicate)
            .AnyAsync(cancellationToken);
    }
}

/// <summary>
/// Expression visitor for replacing parameter references in lambda expressions.
/// Used for combining expressions in repository query methods.
/// </summary>
internal class ReplaceParmeterVisitor : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly ParameterExpression _newParameter;

    private ReplaceParmeterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    public static Expression ReplaceParameter(
        Expression expression, 
        ParameterExpression oldParameter, 
        ParameterExpression newParameter)
    {
        return new ReplaceParmeterVisitor(oldParameter, newParameter).Visit(expression);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _oldParameter ? _newParameter : base.VisitParameter(node);
    }
}
