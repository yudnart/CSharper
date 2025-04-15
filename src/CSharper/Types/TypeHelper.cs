using System;

namespace CSharper.Types;

/// <summary>
/// Provides helper methods for handling type-related operations, particularly for resolving unproxied types.
/// </summary>
public static class TypeHelper
{
    /// <summary>
    /// Delegate to provide the unproxied type of an object.
    /// </summary>
    private static Func<object, Type> _getUnproxiedTypeDelegate = obj => obj.GetType();

    /// <summary>
    /// Configures the delegate used to retrieve the unproxied type of an object.
    /// </summary>
    /// <param name="getUnproxiedTypeDelegate">The delegate to use for resolving the unproxied type.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="getUnproxiedTypeDelegate"/> is null.</exception>
    /// <remarks>
    /// This method allows customization of type resolution, typically for handling proxies created by ORMs
    /// like Entity Framework Core or NHibernate.
    /// </remarks>
    public static void ConfigureGetUnproxiedTypeDelegate(Func<object, Type> getUnproxiedTypeDelegate)
    {
        _getUnproxiedTypeDelegate = getUnproxiedTypeDelegate ?? throw new ArgumentNullException(nameof(getUnproxiedTypeDelegate));
    }

    /// <summary>
    /// Retrieves the unproxied type of an object using the configured delegate.
    /// </summary>
    /// <param name="obj">The object whose type is to be resolved.</param>
    /// <returns>The unproxied type of the object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
    /// <remarks>
    /// If no delegate is configured, the method returns the object's runtime type via <see cref="object.GetType"/>.
    /// </remarks>
    public static Type GetUnproxiedType(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        return _getUnproxiedTypeDelegate(obj);
    }
}