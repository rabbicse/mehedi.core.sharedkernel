using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.BaseEntityTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class BaseEntity_ClearDomainEvents
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    private sealed record TestEvent() : BaseDomainEvent(nameof(TestEvent), Guid.Empty.ToString());

    private sealed class TestEntity : BaseEntity<Guid>
    {
        public void RaiseEvent() => AddDomainEvent(new TestEvent());

        protected override Guid GenerateNewId() => Guid.NewGuid();
    }

    [Fact]
    public void ClearsDomainEventsCollection()
    {
        var entity = new TestEntity();
        entity.RaiseEvent();
        entity.RaiseEvent();
        entity.DomainEvents.Should().HaveCount(2);

        entity.ClearDomainEvents();

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ClearOnEmptyCollectionDoesNotThrow()
    {
        var entity = new TestEntity();

        var act = entity.ClearDomainEvents;

        act.Should().NotThrow();
        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void CanAddEventsAfterClearing()
    {
        var entity = new TestEntity();
        entity.RaiseEvent();
        entity.ClearDomainEvents();

        entity.RaiseEvent();

        entity.DomainEvents.Should().HaveCount(1);
    }
}
