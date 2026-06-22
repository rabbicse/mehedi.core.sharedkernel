using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.BaseDomainEventTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class BaseDomainEvent_Properties
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    private sealed record OrderPlacedEvent(Guid OrderId) : BaseDomainEvent(nameof(OrderPlacedEvent), OrderId.ToString());

    [Fact]
    public void StoresMessageType()
    {
        var orderId = Guid.NewGuid();
        var evt = new OrderPlacedEvent(orderId);

        evt.MessageType.Should().Be(nameof(OrderPlacedEvent));
    }

    [Fact]
    public void StoresAggregateId()
    {
        var orderId = Guid.NewGuid();
        var evt = new OrderPlacedEvent(orderId);

        evt.AggregateId.Should().Be(orderId.ToString());
    }

    [Fact]
    public void SameParametersHaveSameMessageTypeAndAggregateId()
    {
        var orderId = Guid.NewGuid();
        var evt1 = new OrderPlacedEvent(orderId);
        var evt2 = new OrderPlacedEvent(orderId);

        // OccurredOn is set to TimeProvider.System.GetUtcNow() at construction time so two
        // separate instances differ by timestamp — test individual properties instead.
        evt1.MessageType.Should().Be(evt2.MessageType);
        evt1.AggregateId.Should().Be(evt2.AggregateId);
        evt1.OrderId.Should().Be(evt2.OrderId);
    }

    [Fact]
    public void DifferentAggregateIdsProduceDifferentAggregateId()
    {
        var evt1 = new OrderPlacedEvent(Guid.NewGuid());
        var evt2 = new OrderPlacedEvent(Guid.NewGuid());

        evt1.AggregateId.Should().NotBe(evt2.AggregateId);
    }

    [Fact]
    public void NullMessageTypeIsAllowed()
    {
        var evt = new BaseDomainEvent_NullMessageType(Guid.Empty);

        evt.MessageType.Should().BeNull();
    }

    private sealed record BaseDomainEvent_NullMessageType(Guid Id) : BaseDomainEvent(null, Id.ToString());
}
