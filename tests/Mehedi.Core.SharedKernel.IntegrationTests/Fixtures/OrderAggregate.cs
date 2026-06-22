namespace Mehedi.Core.SharedKernel.IntegrationTests.Fixtures;

// A minimal Order aggregate used across integration tests.
internal sealed class Order : BaseEntity<Guid>, IAggregateRoot
{
    public string CustomerName { get; private set; }
    public OrderState State { get; private set; } = OrderState.Pending;

    private Order() : base() { CustomerName = string.Empty; }

    public static Order Place(string customerName)
    {
        var order = new Order { CustomerName = customerName };
        order.AddDomainEvent(new OrderPlacedEvent(order.Id, customerName));
        return order;
    }

    public void Cancel()
    {
        State = OrderState.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Id));
    }

    protected override Guid GenerateNewId() => Guid.NewGuid();
}

internal enum OrderState { Pending, Cancelled }
