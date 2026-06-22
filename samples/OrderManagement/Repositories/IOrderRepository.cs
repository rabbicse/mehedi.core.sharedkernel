using Mehedi.Core.SharedKernel;
using OrderManagement.Domain;

namespace OrderManagement.Repositories;

public interface IOrderRepository : ICommandRepository<Order, Guid>
{
    Task<IReadOnlyList<Order>> GetByCustomerAsync(string customerName);
}
