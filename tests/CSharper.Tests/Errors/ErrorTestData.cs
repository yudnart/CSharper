using CSharper.Errors;

namespace CSharper.Tests.Errors;

public static class ErrorTestData
{
    public static Error Error = new("Test error");
    public static Error ErrorWithCode = new("Error with code", "ERR001");
    public static Error ErrorWithDetails = new("Error with details", "ERR001",
        new ErrorDetail("Error detail 1", "ERR001-1"),
        new ErrorDetail("Error detail 2", "ERR001-2"));

    public static TheoryData<ErrorBase, string> ErrorBaseToStringTestData()
    {
        return new TheoryData<ErrorBase, string>
        {
            { Error, Error.Message },
            { ErrorWithCode, $"{ErrorWithCode.Message}, Code={ErrorWithCode.Code}" },
            { ErrorWithDetails, $"{ErrorWithDetails.Message}, Code={ErrorWithDetails.Code}" }
        };
    }

    public static TheoryData<string> NullOrEmptyStringData()
    {
        return new TheoryData<string>
        {
            { null! },
            { "" },
            { " " }
        };
    }

    public static TheoryData<string, string?> ErrorBaseValidCtorParams()
    {
        return new TheoryData<string, string?>
        {
            { "Test error.", null },
            { "Test error.", "" },
            { "Test error.", "ERR001" }
        };
    }
}
