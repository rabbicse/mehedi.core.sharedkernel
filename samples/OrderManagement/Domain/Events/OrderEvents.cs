using Mehedi.Core.SharedKernel;

namespace OrderManagement.Domain.Events;

public record OrderPlacedEvent(Guid OrderId, string CustomerName, decimal TotalAmount)
    : BaseDomainEvent(nameof(OrderPlacedEvent), OrderId.ToString());

public record OrderItemAddedEvent(Guid OrderId, string Sku, int Quantity, decimal UnitPrice)
    : BaseDomainEvent(nameof(OrderItemAddedEvent), OrderId.ToString());

public record OrderCancelledEvent(Guid OrderId, string Reason)
    : BaseDomainEvent(nameof(OrderCancelledEvent), OrderId.ToString());

public record OrderShippedEvent(Guid OrderId, string TrackingNumber)
    : BaseDomainEvent(nameof(OrderShippedEvent), OrderId.ToString());
