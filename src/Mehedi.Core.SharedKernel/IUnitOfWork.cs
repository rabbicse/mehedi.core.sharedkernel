namespace Mehedi.Core.SharedKernel;

/// <summary>
/// Represents a unit of work for managing database operations.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Saves the changes made in the unit of work asynchronously.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
