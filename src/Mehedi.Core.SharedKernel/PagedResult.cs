namespace Mehedi.Core.SharedKernel;

/// <summary>
/// Wraps a page of query results together with the total count of all matching records.
/// </summary>
/// <typeparam name="T">The query model type.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>Total number of records matching the query (before pagination).</summary>
    public long Total { get; }

    /// <summary>The records in the current page.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Initializes a new <see cref="PagedResult{T}"/>.
    /// </summary>
    public PagedResult(long total, IEnumerable<T> items)
    {
        Total = total;
        Items = [.. items];
    }
}

/// <summary>
/// Non-generic factory helpers for <see cref="PagedResult{T}"/>.
/// </summary>
public static class PagedResult
{
    /// <summary>Returns an empty result with zero total and no items.</summary>
    public static PagedResult<T> Empty<T>() => new(0, []);
}
