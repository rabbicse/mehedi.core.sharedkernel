using FluentAssertions;
using Mehedi.Core.SharedKernel.IntegrationTests.Fixtures;

namespace Mehedi.Core.SharedKernel.IntegrationTests.RepositoryTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
#pragma warning disable CA2007
public class CommandRepository_Contract
#pragma warning restore CA1707
#pragma warning restore CA1515
#pragma warning restore CA2007
{
    private readonly InMemoryOrderRepository _repo = new();

    [Fact]
    public async Task AddAsync_StoresEntity()
    {
        var order = Order.Place("Alice");

        await _repo.AddAsync(order);
        var found = await _repo.GetByIdAsync(order.Id);

        found.Should().NotBeNull().And.Be(order);
    }

    [Fact]
    public async Task AddAsync_Batch_StoresAllEntities()
    {
        var orders = new[] { Order.Place("A"), Order.Place("B"), Order.Place("C") };

        await _repo.AddAsync(orders);

        foreach (var o in orders)
        {
            var found = await _repo.GetByIdAsync(o.Id);
            found.Should().NotBeNull().And.Be(o);
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        var order = Order.Place("Bob");
        await _repo.AddAsync(order);

        await _repo.DeleteAsync(order);

        var found = await _repo.GetByIdAsync(order.Id);
        found.Should().BeNull();
    }

    [Fact]
    public async Task DeleteByIdAsync_RemovesEntity()
    {
        var order = Order.Place("Charlie");
        await _repo.AddAsync(order);

        await _repo.DeleteByIdAsync(order.Id);

        var found = await _repo.GetByIdAsync(order.Id);
        found.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullForMissingEntity()
    {
        var found = await _repo.GetByIdAsync(Guid.NewGuid());
        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_FiltersByPredicate()
    {
        var alice = Order.Place("Alice");
        var bob = Order.Place("Bob");
        await _repo.AddAsync(new[] { alice, bob });

        var result = await _repo.GetAsync(o => o.CustomerName == "Alice");

        result.Should().HaveCount(1);
        result[0].CustomerName.Should().Be("Alice");
    }

    [Fact]
    public async Task GetAsync_EmptyResultWhenNoMatch()
    {
        var order = Order.Place("Dave");
        await _repo.AddAsync(order);

        var result = await _repo.GetAsync(o => o.CustomerName == "Nobody");

        result.Should().BeEmpty();
    }
}
