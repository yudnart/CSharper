using CSharper.Errors;
using CSharper.Results;
using CSharper.Tests.Errors;
using System.Text;

namespace CSharper.Tests.Results;

public static class ResultTestData
{
    public static TheoryData<string?> FailInvalidMessageTestCases()
    {
        return new TheoryData<string?>
        {
            { null },
            { "" },
            { " " }
        };
    }

    public static TheoryData<string, string?> FailValidTestCases()
    {
        return new TheoryData<string, string?>
            {
                { "Test error", null },
                { "Error with code", "ERR001" },
                { "Minimal", "" }
            };
    }

    public static TheoryData<string, Result, string> ToStringTestCases()
    {
        Error error = ErrorTestData.ErrorWithDetails;

        return new TheoryData<string, Result, string>
            {
                {
                    "Success result",
                    Result.Ok(),
                    "Success" },
                {
                    "Error result",
                    Result.Fail("Test error."),
                    "Error: Test error."
                },
                {
                    "Error result with details",
                    Result.Fail(error),
                    MapErrorToExpectedString(error)
                }
            };
    }

    public static IEnumerable<object[]> ResultTToStringTestCases()
    {
        yield return [
            "Int value", Result.Ok(42), 42.ToString()];
        yield return [
            "Boolean value", Result.Ok(true), true.ToString()];
        yield return [
            "Failed string result",
            Result.Fail<string>(ErrorTestData.Error),
            MapErrorToExpectedString(ErrorTestData.Error)
        ];
    }

    private static string MapErrorToExpectedString(Error error)
    {
        StringBuilder sb = new($"Error: {error.Message}");
        if (!string.IsNullOrWhiteSpace(error.Code))
        {
            sb.Append($", Code={error.Code}");
        }

        foreach (ErrorDetail detail in error.ErrorDetails)
        {
            sb.AppendLine();
            sb.Append($"> {detail}");
        }

        return sb.ToString();
    }
}
