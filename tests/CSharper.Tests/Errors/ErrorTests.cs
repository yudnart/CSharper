using CSharper.Errors;
using System.Text;

namespace CSharper.Tests.Errors;

public sealed class ErrorTests
{
    [Theory]
    [MemberData(nameof(ValidCtorParams))]
    public void Ctor_ValidParams_Succeeds(
        string message, string? code, ErrorDetail[]? errorDetails)
    {
        Error result = new(message, code, errorDetails!);

        Assert.Multiple(() =>
        {
            Assert.NotNull(result);
            Assert.Equal(message, result.Message);
            Assert.Equal(code, result.Code);
        });
    }

    [Theory]
    [MemberData(
        nameof(ErrorTestData.NullOrEmptyStringData),
        MemberType = typeof(ErrorTestData)
    )]
    public void Ctor_NullOrEmptyMessage_ThrowsArgumentException(string message)
    {
        Assert.Throws<ArgumentException>(() => new Error(message));
    }

    [Theory]
    [MemberData(nameof(ToStringTestData))]
    public void ToString_FormatsCorrectly(ErrorBase sut, string expected)
    {
        string result = sut.ToString();

        Assert.Multiple(() =>
        {
            Assert.Equal(expected, result);
        });
    }

    private static string MapString(Error error)
    {
        StringBuilder sb = new(error.Message);
        if (!string.IsNullOrWhiteSpace(error.Code))
        {
            sb.Append($", Code={error.Code}");
        }
        foreach (ErrorDetail detail in error.ErrorDetails)
        {
            sb.AppendLine();
            string detailMessage = detail.Message;
            if (detailMessage.StartsWith(Error.IndentMarker))
            {
                sb.Append($">{detailMessage}");
            }
            else
            {
                sb.Append($"> {detailMessage}");
            }
        }
        return sb.ToString();
    }

    public static TheoryData<Error, string> ToStringTestData()
    {
        return new TheoryData<Error, string>
        {
            { ErrorTestData.Error, MapString(ErrorTestData.Error) },
            { ErrorTestData.ErrorWithCode, MapString(ErrorTestData.ErrorWithCode) },
            { ErrorTestData.ErrorWithDetails, MapString(ErrorTestData.ErrorWithDetails) }
        };
    }

    public static TheoryData<string, string?, ErrorDetail[]?> ValidCtorParams()
    {
        return new TheoryData<string, string?, ErrorDetail[]?>
        {
            { "Test error.", null, null },
            { "Test error.", null, [] },
            { "Test error.", "", null },
            { "Test error.", "", [] },
            { "Test error.", "ERR001", null },
            { "Test error.", "ERR001", [] },
        };
    }
}
