using CSharper.Errors;

namespace CSharper.Tests.Errors;

public abstract class ErrorTestData
{
    public static Error Error = new("Test error");
    public static Error ErrorWithCode = new("Error with code", "ERR001");
    public static Error ErrorNoCode = new("Minimal", "");
    public static Error ErrorWithDetails = new(
        "Error with details", "ERR001",
        new ErrorDetail("Error detail 1", "ERR001-1"),
        new ErrorDetail("Error detail 2", "ERR001-2"));

    public static TheoryData<string, string?> ErrorBaseCtorValidTestCases()
    {
        return new TheoryData<string, string?>
        {
            { Error.Message, Error.Code },
            { ErrorWithCode.Message, ErrorWithCode.Code },
            { ErrorNoCode.Message, ErrorNoCode.Code },
            {
                ErrorWithDetails.Message,
                ErrorWithDetails.Code
            }
        };
    }

    public static TheoryData<string, string?, ErrorDetail[]?> ErrorCtorValidTestCases()
    {
        return new TheoryData<string, string?, ErrorDetail[]?>
        {
            { Error.Message, Error.Code, null },
            { ErrorWithCode.Message, ErrorWithCode.Code, [] },
            { ErrorNoCode.Message, ErrorNoCode.Code, [] },
            {
                ErrorWithDetails.Message,
                ErrorWithDetails.Code,
                [.. ErrorWithDetails.ErrorDetails]
            }
        };
    }

    public static TheoryData<string> CtorInvalidMessageTestCases()
    {
        return new TheoryData<string>
        {
            { null! },
            { "" },
            { " " }
        };
    }

    public static TheoryData<string, ErrorBase, string> ErrorBaseToStringTestCases()
    {
        return new TheoryData<string, ErrorBase, string>
        {
            { "Error", Error, "Test error" },
            { "Error with code", ErrorWithCode, "Error with code, Code=ERR001" },
            { "Error no code", ErrorNoCode, "Minimal" }
        };
    }

    public static TheoryData<string, Error, string> ErrorToStringTestCases()
    {
        return new TheoryData<string, Error, string>
        {
            { "Error", Error, "Test error" },
            {
                "Error with code",
                ErrorWithCode,
                "Error with code, Code=ERR001"
            },
            { "Error node code", ErrorNoCode, "Minimal" },
            {
                "Error with details",
                ErrorWithDetails,
                "Error with details, Code=ERR001"
                + "\r\n> Error detail 1, Code=ERR001-1"
                + "\r\n> Error detail 2, Code=ERR001-2"
            }
        };
    }
}
