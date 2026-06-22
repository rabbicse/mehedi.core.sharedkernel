#pragma warning disable CA1515
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Mehedi.Core.SharedKernel.LoadTests.Benchmarks;

/// <summary>
/// Measures equality and hash code performance for ValueObject implementations.
/// XOR-based GetHashCode vs HashCode.Combine can be compared if the implementation is changed.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class ValueObjectBenchmarks
{
    private readonly MoneyValue _a = new(99.99m, "USD");
    private readonly MoneyValue _b = new(99.99m, "USD");
    private readonly MoneyValue _c = new(49.99m, "EUR");

    [Benchmark(Baseline = true, Description = "GetHashCode — same values")]
    public int HashCodeSameValues() => _a.GetHashCode();

    [Benchmark(Description = "Equals — same values (true)")]
    public bool EqualsSameValues() => _a.Equals(_b);

    [Benchmark(Description = "Equals — different values (false)")]
    public bool EqualsDifferentValues() => _a.Equals(_c);

    [Benchmark(Description = "GetCopy — MemberwiseClone")]
    public ValueObject? GetCopy() => _a.GetCopy();
}

internal sealed class MoneyValue(decimal amount, string currency) : ValueObject
{
    public decimal Amount { get; } = amount;
    public string Currency { get; } = currency;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
#pragma warning restore CA1515
