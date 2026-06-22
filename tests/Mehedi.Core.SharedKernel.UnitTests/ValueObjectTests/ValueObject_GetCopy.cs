using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.ValueObjectTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class ValueObject_GetCopy
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    [Fact]
    public void GetCopy_ReturnsNewInstance()
    {
        var original = new TestValueObject(42);

        var copy = original.GetCopy();

        copy.Should().NotBeSameAs(original);
    }

    [Fact]
    public void GetCopy_CopiedInstanceIsEqualToOriginal()
    {
        var original = new TestValueObject(42);

        var copy = original.GetCopy();

        copy.Should().Be(original);
    }

    [Fact]
    public void GetCopy_ReturnsNonNull()
    {
        var original = new TestValueObject(1);

        var copy = original.GetCopy();

        copy.Should().NotBeNull();
    }
}
