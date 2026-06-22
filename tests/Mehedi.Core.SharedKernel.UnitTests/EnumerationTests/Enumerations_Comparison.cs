using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.EnumerationTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class Enumerations_Comparison
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    [Fact]
    public void LowerIdIsLessThan()
    {
        (TestEnumeration.First < TestEnumeration.Second).Should().BeTrue();
        (TestEnumeration.Second < TestEnumeration.First).Should().BeFalse();
    }

    [Fact]
    public void GreaterIdIsGreaterThan()
    {
        (TestEnumeration.Third > TestEnumeration.First).Should().BeTrue();
        (TestEnumeration.First > TestEnumeration.Third).Should().BeFalse();
    }

    [Fact]
    public void SameIdIsLessThanOrEqual()
    {
        var a = TestEnumeration.Second;
        var b = TestEnumeration.Second;
        (a <= b).Should().BeTrue();
    }

    [Fact]
    public void SameIdIsGreaterThanOrEqual()
    {
        var a = TestEnumeration.Second;
        var b = TestEnumeration.Second;
        (a >= b).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_NullThrows()
    {
        var act = () => TestEnumeration.First.CompareTo(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CompareTo_WrongTypeThrows()
    {
        var act = () => TestEnumeration.First.CompareTo("not an enumeration");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CompareTo_SortableInList()
    {
        var unsorted = new List<TestEnumeration> { TestEnumeration.Third, TestEnumeration.First, TestEnumeration.Second };
        var sorted = unsorted.Order().ToList();

        sorted[0].Should().Be(TestEnumeration.First);
        sorted[1].Should().Be(TestEnumeration.Second);
        sorted[2].Should().Be(TestEnumeration.Third);
    }
}
