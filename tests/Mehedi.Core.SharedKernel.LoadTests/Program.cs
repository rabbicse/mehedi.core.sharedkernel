using BenchmarkDotNet.Running;
using Mehedi.Core.SharedKernel.LoadTests.Benchmarks;

BenchmarkRunner.Run<EnumerationsBenchmarks>();
BenchmarkRunner.Run<ValueObjectBenchmarks>();
