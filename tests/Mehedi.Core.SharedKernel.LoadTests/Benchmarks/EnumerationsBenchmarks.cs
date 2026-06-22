#pragma warning disable CA1515
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Mehedi.Core.SharedKernel.LoadTests.Benchmarks;

/// <summary>
/// Measures the cost of reflection-based enumeration lookup.
/// Key finding to watch: GetAll should be cached; uncached reflection at N ops/sec is unacceptable.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class EnumerationsBenchmarks
{
    [Benchmark(Baseline = true, Description = "GetAll<T> — full reflection scan")]
    public IEnumerable<BenchOrderStatus> GetAll() =>
        Enumerations.GetAll<BenchOrderStatus>().ToList();

    [Benchmark(Description = "FromValue — lookup by id")]
    public BenchOrderStatus FromValue() =>
        Enumerations.FromValue<BenchOrderStatus>(2);

    [Benchmark(Description = "FromDisplayName — lookup by name")]
    public BenchOrderStatus FromDisplayName() =>
        Enumerations.FromDisplayName<BenchOrderStatus>("Processing");

    [Benchmark(Description = "ToString — name property access")]
    public string ToStringBench() =>
        BenchOrderStatus.Placed.ToString();
}

public sealed class BenchOrderStatus : Enumerations
{
    public static readonly BenchOrderStatus Pending    = new(1, "Pending");
    public static readonly BenchOrderStatus Processing = new(2, "Processing");
    public static readonly BenchOrderStatus Shipped    = new(3, "Shipped");
    public static readonly BenchOrderStatus Delivered  = new(4, "Delivered");
    public static readonly BenchOrderStatus Cancelled  = new(5, "Cancelled");
    public static readonly BenchOrderStatus Placed     = new(6, "Placed");

    private BenchOrderStatus(int id, string name) : base(id, name) { }
}
#pragma warning restore CA1515
