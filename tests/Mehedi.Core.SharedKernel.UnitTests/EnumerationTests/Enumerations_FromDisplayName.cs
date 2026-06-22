using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.EnumerationTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class Enumerations_FromDisplayName
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    [Theory]
    [InlineData("First", 1)]
    [InlineData("Second", 2)]
    [InlineData("Third", 3)]
    public void ReturnsCorrectEnumerationForValidName(string name, int expectedId)
    {
        var result = Enumerations.FromDisplayName<TestEnumeration>(name);

        result.Id.Should().Be(expectedId);
        result.Name.Should().Be(name);
    }

    [Fact]
    public void ThrowsForUnknownName()
    {
        var act = () => Enumerations.FromDisplayName<TestEnumeration>("DoesNotExist");

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*DoesNotExist*");
    }

    [Fact]
    public void IsCaseSensitive()
    {
        var act = () => Enumerations.FromDisplayName<TestEnumeration>("first");

        act.Should().Throw<InvalidOperationException>();
    }
}
