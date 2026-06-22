namespace Mehedi.Core.SharedKernel;

/// <summary>
/// Represents a read-only repository interface.
/// </summary>
/// <typeparam name="TQueryModel">The type of the query model.</typeparam>
/// <typeparam name="TKey">The type of the key for the query model.</typeparam>
public interface IQueryRepository<TQueryModel, in TKey>
    where TQueryModel : IQueryModel<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>Returns all records and the total count.</summary>
    Task<PagedResult<TQueryModel>> GetAllCollectionAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a single page of records and the total count.</summary>
    Task<PagedResult<TQueryModel>> GetCollectionAsync(int pageNumber = 1, int pageSize = 100, CancellationToken cancellationToken = default);

    /// <summary>Returns the query model with the given key, or <c>null</c> if not found.</summary>
    Task<TQueryModel?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
}
