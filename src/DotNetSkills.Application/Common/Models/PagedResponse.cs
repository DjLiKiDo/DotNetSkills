namespace DotNetSkills.Application.Common.Models;

/// <summary>
/// Represents a paginated response containing data and pagination metadata.
/// Provides a standardized way to return paginated results from queries.
/// </summary>
/// <typeparam name="T">The type of items in the paginated response.</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Initializes a new instance of the PagedResponse class.
    /// </summary>
    /// <param name="data">The collection of items for the current page.</param>
    /// <param name="pageNumber">The current page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        PageNumber = pageNumber > 0 ? pageNumber : throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than 0.");
        PageSize = pageSize > 0 ? pageSize : throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than 0.");
        TotalCount = totalCount >= 0 ? totalCount : throw new ArgumentOutOfRangeException(nameof(totalCount), "Total count cannot be negative.");
    }

    /// <summary>
    /// Gets the collection of items for the current page.
    /// </summary>
    public IEnumerable<T> Data { get; }

    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets a value indicating whether this is the first page.
    /// </summary>
    public bool IsFirstPage => PageNumber == 1;

    /// <summary>
    /// Gets a value indicating whether this is the last page.
    /// </summary>
    public bool IsLastPage => PageNumber == TotalPages || TotalCount == 0;

    /// <summary>
    /// Gets the number of items on the current page.
    /// </summary>
    public int ItemCount => Data.Count();

    /// <summary>
    /// Gets the starting item number for the current page (1-based).
    /// </summary>
    public int StartItem => TotalCount == 0 ? 0 : ((PageNumber - 1) * PageSize) + 1;

    /// <summary>
    /// Gets the ending item number for the current page (1-based).
    /// </summary>
    public int EndItem => TotalCount == 0 ? 0 : StartItem + ItemCount - 1;

    /// <summary>
    /// Creates an empty paged response.
    /// </summary>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>An empty paged response.</returns>
    public static PagedResponse<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PagedResponse<T>(Enumerable.Empty<T>(), pageNumber, pageSize, 0);
    }

    /// <summary>
    /// Creates a paged response from a full collection by applying pagination.
    /// Note: This method loads all data into memory and should only be used for small datasets.
    /// For large datasets, use database-level pagination instead.
    /// </summary>
    /// <param name="source">The source collection.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A paged response containing the requested page of data.</returns>
    public static PagedResponse<T> FromCollection(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(source);
        
        var totalCount = source.Count();
        var data = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return new PagedResponse<T>(data, pageNumber, pageSize, totalCount);
    }
}