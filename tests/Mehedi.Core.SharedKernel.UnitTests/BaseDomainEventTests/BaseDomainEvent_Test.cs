using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.BaseDomainEventTests;

#pragma warning disable CA1515 // Consider making public types internal
#pragma warning disable CA1707 // Identifiers should not contain underscores
public class BaseDomainEvent_Test
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1515 // Consider making public types internal
{
    private sealed record TestDomainEvent() : BaseDomainEvent(nameof(TestDomainEvent), Guid.Empty.ToString());

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
