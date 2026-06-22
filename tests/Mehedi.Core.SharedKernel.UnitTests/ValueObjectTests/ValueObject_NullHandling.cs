using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.ValueObjectTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class ValueObject_NullHandling
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    [Fact]
    public void NotEqualToNull()
    {
        var vo = new TestValueObject(1);

        vo.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void NotEqualToDifferentType()
    {
        var vo = new TestValueObject(1);

        vo.Equals("not a value object").Should().BeFalse();
        vo.Equals(42).Should().BeFalse();
    }

    [Fact]
    public void EqualOperator_BothNull_ReturnsTrue()
    {
        var result = ValueObject_EqualHelper.BothNull();

        result.Should().BeTrue();
    }

    [Fact]
    public void EqualOperator_OneNull_ReturnsFalse()
    {
        var result = ValueObject_EqualHelper.OneNull(new TestValueObject(1));

        result.Should().BeFalse();
    }
}

// Helper to access protected static methods via subclass
#pragma warning disable CA1515
internal static class ValueObject_EqualHelper
#pragma warning restore CA1515
{
    private sealed class Proxy : ValueObject
    {
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield break;
        }

        public static bool CallEqual(ValueObject? left, ValueObject? right) =>
            EqualOperator(left, right);
    }

    public static bool BothNull() => Proxy.CallEqual(null, null);
    public static bool OneNull(ValueObject right) => Proxy.CallEqual(null, right);
}
