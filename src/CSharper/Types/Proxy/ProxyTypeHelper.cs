using CSharper.Extensions;
using System;

namespace CSharper.Types.Proxy;

/// <summary>
/// Provides helper methods for handling type-related operations, particularly for resolving unproxied types.
/// </summary>
public static class ProxyTypeHelper
{
    private static readonly Func<object, Type> _defaultProxyTypeDelegate = obj => obj.GetType();

    /// <summary>
    /// Delegate to provide the unproxied type of an object.
    /// </summary>
    private static Func<object, Type> _getUnproxiedTypeDelegate = _defaultProxyTypeDelegate;

    /// <summary>
    /// Configures the delegate used to retrieve the unproxied type of an object.
    /// </summary>
    /// <param name="getUnproxiedTypeDelegate">The delegate to use for resolving the unproxied type.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="getUnproxiedTypeDelegate"/> is null.</exception>
    /// <remarks>
    /// This method customizes type resolution for handling proxies created by ORMs like Entity Framework Core or NHibernate.
    /// To reset to the default behavior, use <see cref="ResetGetUnproxiedTypeDelegate"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(obj => obj.GetType().BaseType ?? obj.GetType());
    /// Type unproxiedType = ProxyTypeHelper.GetUnproxiedType(new object());
    /// </code>
    /// </example>
    public static void ConfigureGetUnproxiedTypeDelegate(Func<object, Type> getUnproxiedTypeDelegate)
    {
        getUnproxiedTypeDelegate.ThrowIfNull(nameof(getUnproxiedTypeDelegate));
        _getUnproxiedTypeDelegate = getUnproxiedTypeDelegate;
    }

    /// <summary>
    /// Resets the delegate used to retrieve the unproxied type to its default behavior.
    /// </summary>
    /// <remarks>
    /// The default delegate returns the object's runtime type via <see cref="object.GetType"/>.
    /// This method is useful for reverting custom proxy handling configurations.
    /// </remarks>
    /// <example>
    /// <code>
    /// ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(obj => obj.GetType().BaseType ?? obj.GetType());
    /// ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
    /// Type unproxiedType = ProxyTypeHelper.GetUnproxiedType(new object()); // Returns runtime type
    /// </code>
    /// </example>
    public static void ResetGetUnproxiedTypeDelegate()
    {
        _getUnproxiedTypeDelegate = _defaultProxyTypeDelegate;
    }

    /// <summary>
    /// Retrieves the unproxied type of an object using the configured delegate.
    /// </summary>
    /// <param name="obj">The object whose type is to be resolved.</param>
    /// <returns>The unproxied type of the object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
    /// <remarks>
    /// If no delegate is configured or after a reset, the method returns the object's runtime type via <see cref="object.GetType"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var obj = new object();
    /// Type unproxiedType = ProxyTypeHelper.GetUnproxiedType(obj); // Returns typeof(object)
    /// </code>
    /// </example>
    public static Type GetUnproxiedType(object obj)
    {
        obj.ThrowIfNull(nameof(obj));
        return _getUnproxiedTypeDelegate(obj);
    }
}