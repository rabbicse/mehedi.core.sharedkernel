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
    /// <summary>
    /// Gets the collection for the specified query model.
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <returns></returns>
    Task<(long, IEnumerable<TQueryModel>)> GetAllCollectionAsync();
    /// <summary>
    /// Gets the collection for the specified query model with page.
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<(long, IEnumerable<TQueryModel>)> GetCollectionAsync(int pageNumber = 1, int pageSize = 100);
    /// <summary>
    /// Gets the Query Model by id.
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TQueryModel?> GetByIdAsync(TKey id);
}
