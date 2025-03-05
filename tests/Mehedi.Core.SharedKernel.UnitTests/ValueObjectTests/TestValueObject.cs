namespace Mehedi.Core.SharedKernel.UnitTests.ValueObjectTests;

#pragma warning disable CA1515 // Consider making public types internal
public class TestValueObject : ValueObject
#pragma warning restore CA1515 // Consider making public types internal
{
    public int Value { get; }

    public TestValueObject(int value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
