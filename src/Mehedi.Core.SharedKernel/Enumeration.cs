using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Mehedi.Core.SharedKernel;

/// <summary>
/// Enumeration
/// </summary>
public abstract class Enumerations : IComparable
{
    [Required]
    public string Name { get; private set; }

    public int Id { get; private set; }

    protected Enumerations(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    /// <summary>
    /// GetAll
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumerations =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                    .Select(f => f.GetValue(null))
                    .Cast<T>();

    /// <summary>
    /// Check equality of objects
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) // Allow nullable object
    {
        if (obj is null) return false; // Explicit null check

        if (obj is not Enumerations otherValue)
        {
            return false;
        }

        return GetType() == obj.GetType() && Id == otherValue.Id;
    }


    /// <summary>
    /// GetHashCode
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// AbsoluteDifference
    /// </summary>
    /// <param name="firstValue"></param>
    /// <param name="secondValue"></param>
    /// <returns></returns>
    public static int AbsoluteDifference(Enumerations? firstValue, Enumerations? secondValue) =>
        Math.Abs((firstValue?.Id ?? throw new ArgumentNullException(nameof(firstValue))) -
                 (secondValue?.Id ?? throw new ArgumentNullException(nameof(secondValue))));



    /// <summary>
    /// FromValue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T FromValue<T>(int value) where T : Enumerations
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    /// <summary>
    /// FromDisplayName
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="displayName"></param>
    /// <returns></returns>
    public static T FromDisplayName<T>(string displayName) where T : Enumerations
    {
        var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
        return matchingItem;
    }

    /// <summary>
    /// Parse
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <param name="value"></param>
    /// <param name="description"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumerations
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        return matchingItem ?? throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
    }


    /// <summary>
    /// CompareTo
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj), "Cannot compare to null.");
        }

        if (obj is not Enumerations enumeration)
        {
            throw new ArgumentException($"Object must be of type {nameof(Enumerations)}", nameof(obj));
        }

        return Id.CompareTo(enumeration.Id);
    }


    public static bool operator ==(Enumerations left, Enumerations right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator !=(Enumerations left, Enumerations right)
    {
        return !(left == right);
    }

    public static bool operator <(Enumerations left, Enumerations right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(Enumerations left, Enumerations right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator >(Enumerations left, Enumerations right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(Enumerations left, Enumerations right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }
}
