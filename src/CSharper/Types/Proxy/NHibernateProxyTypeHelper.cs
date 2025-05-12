using CSharper.Extensions;
using System;

namespace CSharper.Types.Proxy;

/// <summary>
/// Provides helper methods for handling NHibernate proxy types.
/// </summary>
public static class NHibernateProxyTypeHelper
{
    /// <summary>
    /// The suffix used in the string representation of NHibernate proxy types.
    /// </summary>
    public const string TypePrefix = "Proxy";

    /// <summary>
    /// Configures the delegate used to retrieve the unproxied type of an object.
    /// </summary>
    /// <remarks>
    /// This method sets up <see cref="ProxyTypeHelper"/> to identify NHibernate proxy types by checking if the type's
    /// string representation ends with <see cref="TypePrefix"/>. If a proxy is detected and its base type is neither
    /// null nor <see cref="System.Object"/>, it returns the base type; otherwise, it returns the original type.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if the input object is null.</exception>
    /// <example>
    /// <code>
    /// NHibernateProxyTypeHelper.Configure();
    /// var proxy = new MyNamespace.SomeEntityProxy(); // Simulated NHibernate proxy
    /// Type unproxiedType = ProxyTypeHelper.GetUnproxiedType(proxy); // Returns SomeEntity
    /// </code>
    /// </example>
    public static void Configure()
    {
        ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(obj =>
        {
            obj.ThrowIfNull(nameof(obj));

            Type type = obj.GetType();
            string typeString = type.ToString();

            if (typeString.EndsWith(TypePrefix))
            {
                return type.BaseType != null && type.BaseType != typeof(object) ? type.BaseType : type;
            }

            return type;
        });
    }
}