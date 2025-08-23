using CSharper.Errors;

namespace CSharper.Tests.Errors;

public abstract class ErrorTestData
{
    public static readonly Error Error = new("ERR001");
    public static readonly Error ErrorWithMessage = new("ERR002", "Error with code");
    public static readonly Error ErrorNoMessage = new("ERR002");
    public static readonly AggregateError DetailedError = new(
        "ERR001", "Error with details",
        new Error("Error detail 1", "ERR001-1"),
        new Error("Error detail 2", "ERR001-2"));

    public static TheoryData<string, string?> ErrorCtorValidTestCases()
    {
        return new TheoryData<string, string?>
        {
            { Error.Code, Error.Message },
            { ErrorWithMessage.Code, ErrorWithMessage.Message },
            { ErrorNoMessage.Code, ErrorNoMessage.Message },
            {
                DetailedError.Code,
                DetailedError.Message
            }
        };
    }

    public static TheoryData<string, string?, Error[]?> DetailedErrorCtorValidTestCases()
    {
        return new TheoryData<string, string?, Error[]?>
        {
            { Error.Code, Error.Message, null },
            { ErrorWithMessage.Code, ErrorWithMessage.Message, [] },
            { ErrorNoMessage.Code, ErrorNoMessage.Message, [] },
            {
                DetailedError.Code,
                DetailedError.Message,
                [.. DetailedError.Details]
            }
        };
    }

    public static TheoryData<string> CtorNullOrEmptyCode()
    {
        return new TheoryData<string>
        {
            { null! },
            { "" },
            { " " }
        };
    }

    public static TheoryData<string, Error, string> DetailedErrorToStringTestCases()
    {
        return new TheoryData<string, Error, string>
        {
            { "Error", Error, $"Code={Error.Code}" },
            { 
                "Error with message", 
                ErrorWithMessage, 
                $"Code={ErrorWithMessage.Code}; Message={ErrorWithMessage.Message}" 
            },
            { "Error no message", ErrorNoMessage, $"Code={ErrorNoMessage.Code}" }
        };
    }
}
