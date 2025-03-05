namespace Mehedi.Core.SharedKernel
{
    /// <summary>
    /// This is the base interface for all entities
    /// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface IEntity;
#pragma warning restore CA1040 // Avoid empty interfaces

    /// <summary>
    /// This is the base interface for all entities with a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity key.</typeparam>
    public interface IEntity<out TKey> : IEntity where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets the ID of the entity.
        /// </summary>
        TKey Id { get; }
    }
}
