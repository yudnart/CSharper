using System;
using System.Linq;

namespace CSharper.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="Type"/> objects.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets a human-readable string representation of the specified type, including generic type parameters.
    /// </summary>
    /// <param name="type">The type to get the friendly name for.</param>
    /// <returns>
    /// A string representing the type in a human-readable format, such as <c>List&lt;string&gt;</c> or <c>int?</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    /// <remarks>
    /// For generic types, the format is <c>TypeName&lt;GenericArg1, GenericArg2&gt;</c>.
    /// For nullable types, the format is <c>UnderlyingType?</c>.
    /// Common C# types (e.g., <see cref="string"/>, <see cref="int"/>) are mapped to their keyword equivalents.
    /// </remarks>
    public static string GetFriendlyTypeName(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        // Handle nullable types (e.g., int? -> int?)
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return $"{type.GetGenericArguments()[0].GetFriendlyTypeName()}?";
        }

        // Handle non-generic types and generic parameters
        if (!type.IsGenericType)
        {
            return GetSimpleTypeName(type);
        }

        // Handle generic types
        string typeName = type.Name.Split('`')[0]; // Remove generic arity (e.g., List`1 -> List)
        Type[] genericArgs = type.GetGenericArguments();
        string[] argNames = genericArgs.Select(t => t.GetFriendlyTypeName()).ToArray();
        return $"{typeName}<{string.Join(", ", argNames)}>";
    }

    /// <summary>
    /// Maps a type name to its C# keyword equivalent for common types, or returns the type name as-is.
    /// </summary>
    /// <param name="type">The type whose name to map.</param>
    /// <returns>
    /// The C# keyword equivalent (e.g., <c>string</c> for <see cref="string"/>, <c>int</c> for <see cref="int"/>)
    /// or the type's name if no mapping exists.
    /// </returns>
    private static string GetSimpleTypeName(Type type)
    {
        // Map common C# type names to their keyword equivalents
        return type.Name switch
        {
            "String" => "string",
            "Int32" => "int",
            "Int64" => "long",
            "Double" => "double",
            "Boolean" => "bool",
            "Object" => "object",
            "Void" => "void",
            _ => type.Name
        };
    }
}