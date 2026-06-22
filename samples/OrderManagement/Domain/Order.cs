using Mehedi.Core.SharedKernel;
using OrderManagement.Domain.Enumerations;
using OrderManagement.Domain.Events;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain;

/// <summary>
/// Order aggregate root — demonstrates BaseEntity, IAggregateRoot, domain events,
/// ValueObject, and Enumerations all working together.
/// </summary>
public sealed class Order : BaseEntity<Guid>, IAggregateRoot
{
    private readonly List<OrderLine> _lines = [];

    public string CustomerName { get; private set; }
    public Address ShippingAddress { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public Money Total => _lines.Aggregate(
        new Money(0, "USD"),
        (acc, line) => acc.Add(line.LineTotal));

    private Order() : base()
    {
        CustomerName = string.Empty;
        ShippingAddress = new Address(string.Empty, string.Empty, string.Empty, string.Empty);
        Status = OrderStatus.Pending;
    }

    public static Order Place(string customerName, Address shippingAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerName);

        var order = new Order
        {
            CustomerName = customerName,
            ShippingAddress = shippingAddress,
            Status = OrderStatus.Pending
        };

        order.AddDomainEvent(new OrderPlacedEvent(order.Id, customerName, 0));
        return order;
    }

    public void AddItem(string sku, int quantity, Money unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);

        var existing = _lines.Find(l => l.Sku == sku);
        if (existing is not null)
        {
            existing.UpdateQuantity(existing.Quantity + quantity);
        }
        else
        {
            _lines.Add(new OrderLine(sku, quantity, unitPrice));
        }

        AddDomainEvent(new OrderItemAddedEvent(Id, sku, quantity, unitPrice.Amount));
    }

    public void Ship(string trackingNumber)
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be shipped.");

        Status = OrderStatus.Shipped;
        AddDomainEvent(new OrderShippedEvent(Id, trackingNumber));
    }

    public void Cancel(string reason)
    {
        if (Status >= OrderStatus.Shipped)
            throw new InvalidOperationException("Cannot cancel a shipped or delivered order.");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Id, reason));
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed.");
        if (_lines.Count == 0)
            throw new InvalidOperationException("Cannot confirm an order with no items.");

        Status = OrderStatus.Confirmed;
    }

    protected override Guid GenerateNewId() => Guid.NewGuid();
}
