using System.Linq.Expressions;
using Mehedi.Core.SharedKernel.IntegrationTests.Fixtures;

namespace Mehedi.Core.SharedKernel.IntegrationTests.RepositoryTests;

internal sealed class InMemoryOrderRepository : ICommandRepository<Order, Guid>
{
    private readonly Dictionary<Guid, Order> _store = [];

    public Task<Order> AddAsync(Order entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _store[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<Order>> AddAsync(IEnumerable<Order> entities, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var list = entities.ToList();
        foreach (var e in list) _store[e.Id] = e;
        return Task.FromResult<IEnumerable<Order>>(list);
    }

    public Task<Order> UpdateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_store.ContainsKey(entity.Id))
            throw new KeyNotFoundException($"Entity {entity.Id} not found for update.");
        _store[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<Order>> UpdateAsync(IEnumerable<Order> entities, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var list = entities.ToList();
        foreach (var e in list)
        {
            if (!_store.ContainsKey(e.Id))
                throw new KeyNotFoundException($"Entity {e.Id} not found for update.");
            _store[e.Id] = e;
        }
        return Task.FromResult<IEnumerable<Order>>(list);
    }

    public Task DeleteAsync(Order entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _store.Remove(entity.Id);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IEnumerable<Order> entities, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var e in entities) _store.Remove(e.Id);
        return Task.CompletedTask;
    }

    public Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _store.Remove(id);
        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _store.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<Order>> GetAsync(Expression<Func<Order, bool>> predicate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var compiled = predicate.Compile();
        IReadOnlyList<Order> result = _store.Values.Where(compiled).ToList();
        return Task.FromResult(result);
    }
}
