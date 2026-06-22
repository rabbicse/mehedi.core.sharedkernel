using MediatR;
using OrderManagement.Domain.Events;

namespace OrderManagement.Handlers;

public sealed class OrderPlacedHandler : INotificationHandler<OrderPlacedEvent>
{
    public Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[EVENT] Order placed — Id: {notification.OrderId}, Customer: {notification.CustomerName}");
        return Task.CompletedTask;
    }
}

public sealed class OrderItemAddedHandler : INotificationHandler<OrderItemAddedEvent>
{
    public Task Handle(OrderItemAddedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[EVENT] Item added — Order: {notification.OrderId}, SKU: {notification.Sku}, Qty: {notification.Quantity}");
        return Task.CompletedTask;
    }
}

public sealed class OrderCancelledHandler : INotificationHandler<OrderCancelledEvent>
{
    public Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[EVENT] Order cancelled — Id: {notification.OrderId}, Reason: {notification.Reason}");
        return Task.CompletedTask;
    }
}

public sealed class OrderShippedHandler : INotificationHandler<OrderShippedEvent>
{
    public Task Handle(OrderShippedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[EVENT] Order shipped — Id: {notification.OrderId}, Tracking: {notification.TrackingNumber}");
        return Task.CompletedTask;
    }
}
