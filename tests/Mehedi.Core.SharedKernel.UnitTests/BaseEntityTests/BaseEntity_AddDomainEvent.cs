using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.BaseEntityTests;

#pragma warning disable CA1515 // Consider making public types internal
#pragma warning disable CA1707 // Identifiers should not contain underscores
public class BaseEntity_AddDomainEvent
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1515 // Consider making public types internal
{
    private record TestDomainEvent : BaseDomainEvent { }

    private class TestEntity : BaseEntity<Guid>
    {
        public void AddTestDomainEvent()
        {
            var domainEvent = new TestDomainEvent();

            // Add domain events
            AddDomainEvent(domainEvent);
        }

        protected override Guid GenerateNewId()
        {
            return Guid.NewGuid();
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
