using Mehedi.Core.SharedKernel;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain;

public sealed class OrderLine : BaseEntity<Guid>
{
    public string Sku { get; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; }
    public Money LineTotal => UnitPrice.Multiply(Quantity);

    private OrderLine() : base() { Sku = string.Empty; UnitPrice = new Money(0, "USD"); }

    internal OrderLine(string sku, int quantity, Money unitPrice) : base(Guid.NewGuid())
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(newQuantity));
        Quantity = newQuantity;
    }

    protected override Guid GenerateNewId() => Guid.NewGuid();
}
