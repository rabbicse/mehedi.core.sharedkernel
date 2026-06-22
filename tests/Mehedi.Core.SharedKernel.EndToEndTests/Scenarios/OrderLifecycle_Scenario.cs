#pragma warning disable CA2007
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Mehedi.Core.SharedKernel.EndToEndTests.Scenarios;

/// <summary>
/// End-to-end scenario: full DDD lifecycle from entity creation through domain event
/// dispatch and handler side-effects, using an in-memory MediatR pipeline.
/// </summary>
#pragma warning disable CA1515
#pragma warning disable CA1707
public class OrderLifecycle_Scenario
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    // ── Domain ────────────────────────────────────────────────────────────────

    private record OrderPlaced(Guid OrderId, string Customer, decimal Total)
        : BaseDomainEvent(nameof(OrderPlaced), OrderId);

    private record OrderCancelled(Guid OrderId, string Reason)
        : BaseDomainEvent(nameof(OrderCancelled), OrderId);

    private record ItemAdded(Guid OrderId, string Sku, int Qty, decimal UnitPrice)
        : BaseDomainEvent(nameof(ItemAdded), OrderId);

    private sealed class Order : BaseEntity<Guid>, IAggregateRoot
    {
        private readonly List<(string Sku, int Qty, decimal UnitPrice)> _items = [];

        public string Customer { get; private set; } = string.Empty;
        public decimal Total => _items.Sum(i => i.Qty * i.UnitPrice);
        public bool IsCancelled { get; private set; }

        private Order() : base() { }

        public static Order Create(string customer)
        {
            var order = new Order { Customer = customer };
            order.AddDomainEvent(new OrderPlaced(order.Id, customer, 0));
            return order;
        }

        public void AddItem(string sku, int qty, decimal unitPrice)
        {
            _items.Add((sku, qty, unitPrice));
            AddDomainEvent(new ItemAdded(Id, sku, qty, unitPrice));
        }

        public void Cancel(string reason)
        {
            IsCancelled = true;
            AddDomainEvent(new OrderCancelled(Id, reason));
        }

        protected override Guid GenerateNewId() => Guid.NewGuid();
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private sealed class AuditLog
    {
        public List<string> Entries { get; } = [];
    }

    private sealed class OrderPlacedHandler(AuditLog log) : INotificationHandler<OrderPlaced>
    {
        public Task Handle(OrderPlaced notification, CancellationToken cancellationToken)
        {
            log.Entries.Add($"ORDER_PLACED:{notification.OrderId}:{notification.Customer}");
            return Task.CompletedTask;
        }
    }

    private sealed class ItemAddedHandler(AuditLog log) : INotificationHandler<ItemAdded>
    {
        public Task Handle(ItemAdded notification, CancellationToken cancellationToken)
        {
            log.Entries.Add($"ITEM_ADDED:{notification.Sku}:{notification.Qty}");
            return Task.CompletedTask;
        }
    }

    private sealed class OrderCancelledHandler(AuditLog log) : INotificationHandler<OrderCancelled>
    {
        public Task Handle(OrderCancelled notification, CancellationToken cancellationToken)
        {
            log.Entries.Add($"ORDER_CANCELLED:{notification.Reason}");
            return Task.CompletedTask;
        }
    }

    // ── Infrastructure helper ─────────────────────────────────────────────────

    private static async Task DispatchEvents(Order order, IMediator mediator)
    {
        foreach (var evt in order.DomainEvents)
            await mediator.Publish(evt);
        order.ClearDomainEvents();
    }

    private (IMediator mediator, AuditLog log) BuildPipeline()
    {
        var log = new AuditLog();
        var services = new ServiceCollection();
        services.AddSingleton(log);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(GetType().Assembly));
        var sp = services.BuildServiceProvider();
        return (sp.GetRequiredService<IMediator>(), log);
    }

    // ── Scenarios ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task HappyPath_PlaceOrderWithItems_ThenComplete()
    {
        var (mediator, log) = BuildPipeline();

        var order = Order.Create("Alice");
        await DispatchEvents(order, mediator);

        order.AddItem("SKU-001", 2, 9.99m);
        order.AddItem("SKU-002", 1, 49.00m);
        await DispatchEvents(order, mediator);

        order.Total.Should().Be(2 * 9.99m + 49.00m);
        log.Entries.Should().Contain(e => e.StartsWith("ORDER_PLACED"));
        log.Entries.Should().Contain(e => e.Contains("SKU-001"));
        log.Entries.Should().Contain(e => e.Contains("SKU-002"));
    }

    [Fact]
    public async Task CancelOrder_EventDispatchedAndStateUpdated()
    {
        var (mediator, log) = BuildPipeline();

        var order = Order.Create("Bob");
        await DispatchEvents(order, mediator);

        order.Cancel("Customer request");
        await DispatchEvents(order, mediator);

        order.IsCancelled.Should().BeTrue();
        log.Entries.Should().Contain(e => e.Contains("ORDER_CANCELLED") && e.Contains("Customer request"));
    }

    [Fact]
    public async Task EventsAreCleared_AfterDispatch()
    {
        var (mediator, _) = BuildPipeline();

        var order = Order.Create("Charlie");
        await DispatchEvents(order, mediator);

        order.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task FullLifecycle_EventsDispatchedInCreationOrder()
    {
        var (mediator, log) = BuildPipeline();

        var order = Order.Create("Dave");
        order.AddItem("WIDGET", 3, 5.00m);
        order.Cancel("Out of stock");
        await DispatchEvents(order, mediator);

        log.Entries[0].Should().StartWith("ORDER_PLACED");
        log.Entries[1].Should().StartWith("ITEM_ADDED");
        log.Entries[2].Should().StartWith("ORDER_CANCELLED");
    }
}
#pragma warning restore CA2007
