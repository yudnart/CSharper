using CSharper.Errors;
using CSharper.Extensions;
using CSharper.Results;
using CSharper.Tests.Errors;

namespace CSharper.Tests.Results;

public static class ResultTestData
{
    public static TheoryData<string, string?> FailValidParams()
    {
        return new TheoryData<string, string?>
            {
                { "ERR001", null },
                { "ERR002", "" },
                { "ERR003", "Error with code" }
            };
    }

    public static TheoryData<string> NullOrEmptyStrings()
    {
        return new TheoryData<string>
        {
            { null! },
            { "" },
            { " " }
        };
    }

    public static TheoryData<string, Result, string> ToStringTestCases()
    {
        Error error = ErrorTestData.Error;

        return new TheoryData<string, Result, string>
            {
                {
                    "Success result",
                    Result.Ok(),
                    $"{nameof(Result)}: Success"
                },
                {
                    "Error result",
                    Result.Fail(error),
                    $"{nameof(Result)}: {error}"
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
            $"{typeof(Result<string>).GetFriendlyTypeName()}: {ErrorTestData.Error}"
        ];
        yield return [
            "Null value", 
            Result.Ok<string?>(null), 
            "Result<string>: null"
        ];
    }

    //private static string MapErrorToExpectedString(Error error)
    //{
    //    StringBuilder sb = new($"Code={error.Code}");
    //    if (!string.IsNullOrWhiteSpace(error.Message))
    //    {
    //        sb.Append($", Message={error.Message}");
    //    }

    //    if (error is AggregateError detailedError)
    //    {
    //        foreach (Error detail in detailedError.Details)
    //        {
    //            sb.AppendLine();
    //            sb.Append(AggregateError.IndentMarker)
    //                .Append(' ')
    //                .Append(detail.ToString());
    //        }
    //    }

    //    return sb.ToString();
    //}
}
