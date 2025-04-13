using System;

namespace CSharpStarter.Types;

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
    /// This method sets up <see cref="TypeHelper"/> to identify NHibernate proxy types by checking if the type's
    /// string representation ends with <see cref="TypePrefix"/>. If a proxy is detected, it returns the base type;
    /// otherwise, it returns the original type.
    /// </remarks>
    public static void Configure()
    {
        TypeHelper.ConfigureGetUnproxiedTypeDelegate(obj =>
        {
            Type type = obj.GetType();
            string typeString = type.ToString();

            if (typeString.EndsWith(TypePrefix))
            {
                return type.BaseType ?? type;
            }

            return type;
        });
    }
}