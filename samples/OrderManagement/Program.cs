using MediatR;
using Mehedi.Core.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Domain;
using OrderManagement.Domain.Enumerations;
using OrderManagement.Domain.ValueObjects;
using OrderManagement.Handlers;
using OrderManagement.Repositories;
using SharedKernelEnums = Mehedi.Core.SharedKernel.Enumerations;

// ── Bootstrap ─────────────────────────────────────────────────────────────────

var services = new ServiceCollection();
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderPlacedHandler).Assembly));
services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

var sp = services.BuildServiceProvider();
var mediator = sp.GetRequiredService<IMediator>();
var repo     = sp.GetRequiredService<IOrderRepository>();

// Helper: dispatch all pending domain events then clear them
async Task Dispatch(Order order)
{
    foreach (var evt in order.DomainEvents)
        await mediator.Publish(evt);
    order.ClearDomainEvents();
}

// ── Demo: Full Order Lifecycle ────────────────────────────────────────────────

Console.WriteLine("=== Mehedi.Core.SharedKernel — Order Management Sample ===\n");

// 1. Place an order
var shippingAddress = new Address("123 Main St", "Dhaka", "BD", "1000");
var order = Order.Place("Alice", shippingAddress);
await repo.AddAsync(order);
await Dispatch(order);

// 2. Add items
order.AddItem("LAPTOP-001", 1, new Money(999.99m, "USD"));
order.AddItem("MOUSE-002", 2, new Money(29.99m, "USD"));
await Dispatch(order);

Console.WriteLine($"\nOrder total: {order.Total}");
Console.WriteLine($"Line count:  {order.Lines.Count}");
Console.WriteLine($"Status:      {order.Status}");

// 3. Confirm and ship
order.Confirm();
order.Ship("TRACK-9999");
await Dispatch(order);

Console.WriteLine($"\nStatus after shipping: {order.Status}");

// 4. Demonstrate Enumerations
Console.WriteLine("\n=== OrderStatus Enumerations ===");
foreach (var status in SharedKernelEnums.GetAll<OrderStatus>())
    Console.WriteLine($"  [{status.Id}] {status.Name}");

var byId   = SharedKernelEnums.FromValue<OrderStatus>(3);
var byName = SharedKernelEnums.FromDisplayName<OrderStatus>("Delivered");
Console.WriteLine($"\nFromValue(3)            → {byId}");
Console.WriteLine($"FromDisplayName(Delivered) → {byName}");
Console.WriteLine($"Shipped < Delivered?    → {OrderStatus.Shipped < OrderStatus.Delivered}");

// 5. Demonstrate ValueObject equality
Console.WriteLine("\n=== Money ValueObject Equality ===");
var price1 = new Money(99.99m, "USD");
var price2 = new Money(99.99m, "USD");
var price3 = new Money(49.99m, "USD");
Console.WriteLine($"price1 == price2 (same): {price1.Equals(price2)}");
Console.WriteLine($"price1 == price3 (diff): {price1.Equals(price3)}");
Console.WriteLine($"price1 + price3        : {price1.Add(price3)}");

Console.WriteLine("\nDone.");
