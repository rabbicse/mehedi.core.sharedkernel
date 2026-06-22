using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.EnumerationTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class Enumerations_FromValue
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    [Theory]
    [InlineData(1, "First")]
    [InlineData(2, "Second")]
    [InlineData(3, "Third")]
    public void ReturnsCorrectEnumerationForValidId(int id, string expectedName)
    {
        var result = Enumerations.FromValue<TestEnumeration>(id);

        result.Name.Should().Be(expectedName);
        result.Id.Should().Be(id);
    }

    [Fact]
    public void ThrowsForInvalidValue()
    {
        var act = () => Enumerations.FromValue<TestEnumeration>(99);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*99*");
    }
}
