using Mehedi.Core.SharedKernel;
using BaseEnum = Mehedi.Core.SharedKernel.Enumerations;

namespace OrderManagement.Domain.Enumerations;

// Rich enum — add new statuses here as static fields, never change existing ids.
public sealed class OrderStatus : BaseEnum
{
    public static readonly OrderStatus Pending    = new(1, "Pending");
    public static readonly OrderStatus Confirmed  = new(2, "Confirmed");
    public static readonly OrderStatus Shipped    = new(3, "Shipped");
    public static readonly OrderStatus Delivered  = new(4, "Delivered");
    public static readonly OrderStatus Cancelled  = new(5, "Cancelled");

    private OrderStatus(int id, string name) : base(id, name) { }
}
