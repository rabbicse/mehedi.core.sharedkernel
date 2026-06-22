using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.EnumerationTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class Enumerations_GetAll
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    [Fact]
    public void ReturnsAllStaticInstances()
    {
        var all = Enumerations.GetAll<TestEnumeration>().ToList();

        all.Should().HaveCount(3);
        all.Should().Contain(TestEnumeration.First);
        all.Should().Contain(TestEnumeration.Second);
        all.Should().Contain(TestEnumeration.Third);
    }

    [Fact]
    public void ReturnsCorrectIds()
    {
        var ids = Enumerations.GetAll<TestEnumeration>().Select(e => e.Id).ToList();

        ids.Should().Contain(1);
        ids.Should().Contain(2);
        ids.Should().Contain(3);
    }

    [Fact]
    public void ReturnsCorrectNames()
    {
        var names = Enumerations.GetAll<TestEnumeration>().Select(e => e.Name).ToList();

        names.Should().Contain("First");
        names.Should().Contain("Second");
        names.Should().Contain("Third");
    }
}
