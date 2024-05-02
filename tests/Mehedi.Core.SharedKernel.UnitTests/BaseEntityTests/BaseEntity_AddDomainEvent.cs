using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.BaseEntityTests;

public class BaseEntity_AddDomainEvent
{
    private class TestDomainEvent : BaseDomainEvent { }

    private class TestEntity : BaseEntity
    {
        public void AddTestDomainEvent()
        {
            var domainEvent = new TestDomainEvent();

            // Add domain events
            AddDomainEvent(domainEvent);
        }
    }

    [Fact]
    public void AddsDomainEventToEntity()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        entity.AddTestDomainEvent();

        // Assert
        entity.DomainEvents.Should().HaveCount(1);
        entity.DomainEvents.Should().AllBeOfType<TestDomainEvent>();
    }
}
