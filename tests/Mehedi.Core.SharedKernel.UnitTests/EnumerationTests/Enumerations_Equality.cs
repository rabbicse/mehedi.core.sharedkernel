using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.EnumerationTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class Enumerations_Equality
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    [Fact]
    public void SameInstanceAreEqual()
    {
        var a = TestEnumeration.First;
        var b = TestEnumeration.First;
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void SameIdAreEqual()
    {
        var a = Enumerations.FromValue<TestEnumeration>(1);
        var b = Enumerations.FromValue<TestEnumeration>(1);

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void DifferentIdAreNotEqual()
    {
        (TestEnumeration.First == TestEnumeration.Second).Should().BeFalse();
        (TestEnumeration.First != TestEnumeration.Second).Should().BeTrue();
    }

    [Fact]
    public void ToStringReturnsName()
    {
        TestEnumeration.First.ToString().Should().Be("First");
    }

    [Fact]
    public void GetHashCodeBasedOnId()
    {
        var a = Enumerations.FromValue<TestEnumeration>(1);
        var b = Enumerations.FromValue<TestEnumeration>(1);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void AbsoluteDifference_ReturnsCorrectValue()
    {
        var diff = Enumerations.AbsoluteDifference(TestEnumeration.First, TestEnumeration.Third);

        diff.Should().Be(2);
    }

    [Fact]
    public void AbsoluteDifference_ThrowsOnNull()
    {
        var act = () => Enumerations.AbsoluteDifference(null!, TestEnumeration.Second);

        act.Should().Throw<ArgumentNullException>();
    }
}
