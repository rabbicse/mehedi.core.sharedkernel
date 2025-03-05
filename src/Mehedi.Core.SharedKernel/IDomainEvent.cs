using MediatR;

namespace Mehedi.Core.SharedKernel
{
    /// <summary>
    /// Similar to INotification from MediatR
    /// Add this interface to get rid from another MediatR dependency to project 
    /// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface IDomainEvent : INotification
#pragma warning restore CA1040 // Avoid empty interfaces
    {
    }
}
