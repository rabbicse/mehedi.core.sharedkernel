namespace Mehedi.Core.SharedKernel.IntegrationTests.Fixtures;

internal sealed record OrderPlacedEvent(Guid OrderId, string CustomerName)
    : BaseDomainEvent(nameof(OrderPlacedEvent), OrderId.ToString());

internal sealed record OrderCancelledEvent(Guid OrderId)
    : BaseDomainEvent(nameof(OrderCancelledEvent), OrderId.ToString());
