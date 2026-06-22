using MediatR;

namespace Mehedi.Core.SharedKernel.IntegrationTests.Fixtures;

internal sealed class OrderPlacedHandler : INotificationHandler<OrderPlacedEvent>
{
    public List<OrderPlacedEvent> Received { get; } = [];

    public Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        Received.Add(notification);
        return Task.CompletedTask;
    }
}

internal sealed class OrderCancelledHandler : INotificationHandler<OrderCancelledEvent>
{
    public List<OrderCancelledEvent> Received { get; } = [];

    public Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
    {
        Received.Add(notification);
        return Task.CompletedTask;
    }
}
