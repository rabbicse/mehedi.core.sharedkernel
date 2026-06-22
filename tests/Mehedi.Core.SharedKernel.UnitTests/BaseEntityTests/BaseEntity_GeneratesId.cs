using FluentAssertions;

namespace Mehedi.Core.SharedKernel.UnitTests.BaseEntityTests;

#pragma warning disable CA1515
#pragma warning disable CA1707
public class BaseEntity_GeneratesId
#pragma warning restore CA1707
#pragma warning restore CA1515
{
    private sealed class GuidEntity : BaseEntity<Guid>
    {
        protected override Guid GenerateNewId() => Guid.NewGuid();
    }

    private sealed class LongEntity : BaseEntity<long>
    {
        private static long _counter;
        protected override long GenerateNewId() => System.Threading.Interlocked.Increment(ref _counter);
    }

    private sealed class StringEntity : BaseEntity<string>
    {
        protected override string GenerateNewId() => $"entity-{Guid.NewGuid():N}";
    }

    [Fact]
    public void GuidEntityGeneratesNonEmptyId()
    {
        var entity = new GuidEntity();

        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void GuidEntityGeneratesUniqueIds()
    {
        var a = new GuidEntity();
        var b = new GuidEntity();

        a.Id.Should().NotBe(b.Id);
    }

    [Fact]
    public void LongEntityGeneratesPositiveId()
    {
        var entity = new LongEntity();

        entity.Id.Should().BePositive();
    }

    [Fact]
    public void StringEntityGeneratesNonEmptyId()
    {
        var entity = new StringEntity();

        entity.Id.Should().NotBeNullOrEmpty();
    }

    private sealed class GuidEntityWithExplicitId : BaseEntity<Guid>
    {
        public GuidEntityWithExplicitId(Guid id) : base(id) { }
        protected override Guid GenerateNewId() => Guid.NewGuid();
    }

    [Fact]
    public void ExplicitIdConstructorUsesProvidedId()
    {
        var id = Guid.NewGuid();
        var entity = new GuidEntityWithExplicitId(id);
        entity.Id.Should().Be(id);
    }
}
