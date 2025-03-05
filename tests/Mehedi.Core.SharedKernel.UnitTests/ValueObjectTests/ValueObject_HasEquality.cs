using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.ValueObjectTests;

#pragma warning disable CA1515 // Consider making public types internal
#pragma warning disable CA1707 // Identifiers should not contain underscores
public class ValueObject_HasEquality
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1515 // Consider making public types internal
{
    [Fact]
    public void WithSameValuesHaveSameHashCode()
    {
        // Arrange
        var valueObject1 = new TestValueObject(1);
        var valueObject2 = new TestValueObject(1);

        // Act & Assert
        valueObject1.GetHashCode().Should().Be(valueObject2.GetHashCode());
    }

    [Fact]
    public void WithDifferentValuesAreNotEqual()
    {
        // Arrange
        var valueObject1 = new TestValueObject(1);
        var valueObject2 = new TestValueObject(2);

        // Act & Assert
        valueObject1.Should().NotBe(valueObject2);
    }
}
