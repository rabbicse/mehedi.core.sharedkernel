﻿namespace Mehedi.Core.SharedKernel;

/// <summary>
/// Represents the query model interface.
/// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
public interface IQueryModel;
#pragma warning restore CA1040 // Avoid empty interfaces

/// <summary>
/// Represents the interface for a query model with a generic key type.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public interface IQueryModel<out TKey> : IQueryModel where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets the ID of the query model.
    /// </summary>
    TKey Id { get; }
}
