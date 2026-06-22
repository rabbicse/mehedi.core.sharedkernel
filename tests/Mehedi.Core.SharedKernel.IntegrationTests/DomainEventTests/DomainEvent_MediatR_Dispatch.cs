using FluentAssertions;
using MediatR;
using Mehedi.Core.SharedKernel.IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Mehedi.Core.SharedKernel.IntegrationTests.DomainEventTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
#pragma warning disable CA2007
public class DomainEvent_MediatR_Dispatch
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    private readonly IMediator _mediator;
    private readonly OrderPlacedHandler _placedHandler;
    private readonly OrderCancelledHandler _cancelledHandler;

    public DomainEvent_MediatR_Dispatch()
    {
        _placedHandler = new OrderPlacedHandler();
        _cancelledHandler = new OrderCancelledHandler();

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderPlacedEvent).Assembly));
        services.AddSingleton<INotificationHandler<OrderPlacedEvent>>(_ => _placedHandler);
        services.AddSingleton<INotificationHandler<OrderCancelledEvent>>(_ => _cancelledHandler);

        _mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task OrderPlaced_EventDispatchedToHandler()
    {
        var order = Order.Place("Alice");
        var events = order.DomainEvents.OfType<OrderPlacedEvent>().ToList();

        foreach (var evt in events)
            await _mediator.Publish(evt);

        _placedHandler.Received.Should().HaveCount(1);
        _placedHandler.Received[0].CustomerName.Should().Be("Alice");
    }

    [Fact]
    public async Task OrderCancelled_EventDispatchedToHandler()
    {
        var order = Order.Place("Bob");
        order.ClearDomainEvents();
        order.Cancel();

        var events = order.DomainEvents.OfType<OrderCancelledEvent>().ToList();
        foreach (var evt in events)
            await _mediator.Publish(evt);

        _cancelledHandler.Received.Should().HaveCount(1);
        _cancelledHandler.Received[0].OrderId.Should().Be(order.Id);
    }

    [Fact]
    public async Task MultipleEventsDispatchedInOrder()
    {
        var order = Order.Place("Charlie");
        order.Cancel();

        var dispatched = new List<string>();
        foreach (var evt in order.DomainEvents)
        {
            await _mediator.Publish(evt);
            dispatched.Add(evt is BaseDomainEvent bde ? bde.MessageType ?? "unknown" : "unknown");
        }

        dispatched.Should().HaveCount(2);
        dispatched[0].Should().Be(nameof(OrderPlacedEvent));
        dispatched[1].Should().Be(nameof(OrderCancelledEvent));
    }
#pragma warning restore CA2007
}
