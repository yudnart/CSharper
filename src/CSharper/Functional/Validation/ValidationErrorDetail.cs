using CSharper.Errors;
using System.Text;

namespace CSharper.Results.Validation;

/// <summary>
/// Represents an error detail with message and optional code/path.
/// </summary>
public sealed class ValidationErrorDetail : ErrorDetail
{
    public string? Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationErrorDetail"/>
    /// class with message and optional code/path.
    /// </summary>
    /// <param name="path"></param>
    /// <inheritdoc cref="ErrorDetail(string, string?)" />
    public ValidationErrorDetail(
        string message,
        string? code = null,
        string? path = null) : base(message, code)
    {
        Path = path;
    }

    public override string ToString()
    {
        StringBuilder sb = new(base.ToString());

        if (!string.IsNullOrWhiteSpace(Path))
        {
            sb.Append($", Path={Path}");
        }

        return sb.ToString();
    }
}
