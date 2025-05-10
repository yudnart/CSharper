using System;

namespace CSharper.Types.Utilities
{
    /// <summary>
    /// Provides helper methods for handling Entity Framework Core proxy types.
    /// </summary>
    public static class EFCoreProxyTypeHelper
    {
        /// <summary>
        /// The prefix used in the string representation of EF Core proxy types.
        /// </summary>
        public const string TypePrefix = "Castle.Proxies";

        /// <summary>
        /// Configures the delegate used to retrieve the unproxied type of an object.
        /// </summary>
        /// <remarks>
        /// This method sets up <see cref="ProxyTypeHelper"/> to identify EF Core proxy types by checking for the
        /// <see cref="TypePrefix"/> in the type's string representation. If a proxy is detected, it returns
        /// the base type; otherwise, it returns the original type.
        /// </remarks>
        public static void Configure()
        {
            ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(obj =>
            {
                Type type = obj.GetType();
                string typeString = type.ToString();

                if (typeString.Contains(TypePrefix))
                {
                    return type.BaseType ?? type;
                }

                return type;
            });
        }
    }
}