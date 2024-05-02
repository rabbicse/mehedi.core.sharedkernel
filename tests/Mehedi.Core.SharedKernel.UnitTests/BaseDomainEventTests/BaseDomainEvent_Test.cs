using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.BaseDomainEventTests;

public class BaseDomainEvent_Test
{
    private class TestDomainEvent : BaseDomainEvent { }

    [Fact]
    public void SetOccurredOnToCurrentDateTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var domainEvent = new TestDomainEvent();

        // Assert
        domainEvent.OccurredOn.Should().BeOnOrAfter(beforeCreation);
        domainEvent.OccurredOn.Should().BeOnOrBefore(DateTime.UtcNow);
    }
}
