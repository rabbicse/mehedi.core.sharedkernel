using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.BaseEntityTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class BaseEntity_AddDomainEvents
#pragma warning restore CA1707
#pragma warning restore CA1515
{
#if NET9_0_OR_GREATER
    private sealed record EventA() : BaseDomainEvent(nameof(EventA), Guid.Empty.ToString());
    private sealed record EventB() : BaseDomainEvent(nameof(EventB), Guid.Empty.ToString());
    private sealed record EventC() : BaseDomainEvent(nameof(EventC), Guid.Empty.ToString());

    private sealed class MultiEventEntity : BaseEntity<Guid>
    {
        public void RaiseThree() =>
            AddDomainEvents(new EventA(), new EventB(), new EventC());

        public void RaiseOne() =>
            AddDomainEvents(new EventA());

        protected override Guid GenerateNewId() => Guid.NewGuid();
    }

    [Fact]
    public void AddDomainEvents_AddsAllEventsAtOnce()
    {
        var entity = new MultiEventEntity();
        entity.RaiseThree();

        entity.DomainEvents.Should().HaveCount(3);
    }

    [Fact]
    public void AddDomainEvents_SingleEvent_Works()
    {
        var entity = new MultiEventEntity();
        entity.RaiseOne();

        entity.DomainEvents.Should().HaveCount(1);
        entity.DomainEvents.First().Should().BeOfType<EventA>();
    }

    [Fact]
    public void AddDomainEvents_PreservesOrder()
    {
        var entity = new MultiEventEntity();
        entity.RaiseThree();

        var types = entity.DomainEvents.Select(e => e.GetType()).ToList();
        types.Should().ContainInOrder(typeof(EventA), typeof(EventB), typeof(EventC));
    }
#endif
}
