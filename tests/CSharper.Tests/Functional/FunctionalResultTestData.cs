using CSharper.Errors;
using CSharper.Results;
using CSharper.Results.Abstractions;

namespace CSharper.Tests.Functional;

public static class FunctionalResultTestData
{
    public static IEnumerable<object[]> ResultData()
        => GetResults();

    public static IEnumerable<object[]> ResultBindTestData()
        => GetBindTestData();

    public static IEnumerable<object[]> ResultEnsureInvalidTestCases()
    {
        foreach (object result in ResultData().SelectMany(r => r))
        {
            yield return [
                result, null!, null!
            ];
            yield return [
                result, "", "ERR001"
            ];
            yield return [
                result, " ", ""
            ];
        }
    }

    /// <remarks>
    /// Cannot use <see cref="TheoryData{T}"/> because
    /// it requires strongly typed T.
    /// </remarks>
    /// <returns></returns>
    public static IEnumerable<object[]> BindTTestCases()
        => GetBindTestData(false, true);

    public static IEnumerable<object[]> ResultMatchTestCases()
        => GetMatchTestData();

    public static IEnumerable<object[]> ResultTData()
        => GetResults(true);

    public static IEnumerable<object[]> ResultTBindTestCases()
        => GetBindTestData(true);

    public static IEnumerable<object[]> ResultTBindTTestCases()
        => GetBindTestData(true, true);

    public static IEnumerable<object[]> ResultTEnsureInvalidTestCases()
    {
        foreach (object result in ResultTData().SelectMany(r => r))
        {
            yield return [
                result, null!, null!
            ];
            yield return [
                result, "", "ERR001"
            ];
            yield return [
                result, " ", ""
            ];
        }
    }

    public static IEnumerable<object[]> ResultTMapTestCases()
    {
        ResultBase[] suts = GetResultTData();
        foreach (ResultBase sut in suts)
        {
            yield return [sut, 42];
            yield return [sut, 42L];
            yield return [sut, 42.0];
            yield return [sut, "Hello world!"];
            yield return [sut, true];
        }
    }

    public static IEnumerable<object[]> ResultTMatchTestCases()
        => GetMatchTestData(true);

    public static IEnumerable<object[]> ResultTRecoverTestCases()
    {
        yield return [Result.Ok(42), 24];
        yield return [GetFailureResult<int>(), 24];

        yield return [Result.Ok(42L), 24L];
        yield return [GetFailureResult<long>(), 24L];

        yield return [Result.Ok(42.0), 24.0];
        yield return [GetFailureResult<decimal>(), 24.0];

        yield return [Result.Ok("42"), "24"];
        yield return [GetFailureResult<string>(), "24"];

        yield return [Result.Ok(true), false];
        yield return [GetFailureResult<bool>(), false];
    }

    #region Internal

    private static IEnumerable<object[]> GetBindTestData(
        bool isGenericSut = false,
        bool isGenericNextResult = false)
    {
        ResultBase[] suts = isGenericSut
            ? GetResultTData() : GetResultData();
        ResultBase[] nextValues = isGenericNextResult
            ? GetResultTData() : GetResultData();
        foreach (ResultBase sut in suts)
        {
            foreach (ResultBase nextValue in nextValues)
            {
                yield return [sut, nextValue];
            }
        }
    }

    public static Error GetError(int detailSize = 1)
    {
        return new Error("Someting went wrong.",
            errorDetails: [.. GetErrorDetails(detailSize)]);
    }

    private static IEnumerable<ErrorDetail> GetErrorDetails(int size = 1)
    {
        for (int i = 0; i < size; i++)
        {
            yield return new ErrorDetail($"{i} - Something went wrong.");
        }
    }

    public static Result GetFailureResult(params ErrorDetail[] errorDetails)
    {
        if (errorDetails == null || errorDetails.Length == 0)
        {
            errorDetails = [.. GetErrorDetails(1)];
        }
        return Result.Fail(new("Something wrong.", errorDetails: errorDetails));
    }

    public static Result<T> GetFailureResult<T>(params ErrorDetail[] errorDetails)
    {
        if (errorDetails == null || errorDetails.Length == 0)
        {
            errorDetails = [.. GetErrorDetails(1)];
        }
        return Result.Fail<T>(new("Something wrong.", errorDetails: errorDetails));
    }

    private static IEnumerable<object[]> GetMatchTestData(
        bool isGenericSut = false)
    {
        IEnumerable<object[]> testCases =
        [
            // successValue, errorValue
            [1, -1],
            [true, false],
            ["Success", "Failure"]
        ];
        ResultBase[] suts = isGenericSut ? GetResultTData() : GetResultData();
        foreach (ResultBase sut in suts)
        {
            foreach (object[] testCase in testCases)
            {
                yield return [sut, .. testCase];
            }
        }
    }

    private static IEnumerable<object[]> GetResults(
        bool isGenericSut = false)
    {
        ResultBase[] suts = isGenericSut
            ? GetResultTData() : GetResultData();
        foreach (ResultBase sut in suts)
        {
            yield return [sut];
        }
    }

    private static Result[] GetResultData()
    {
        return [
            Result.Ok(),
            GetFailureResult()
        ];
    }

    private static ResultBase[] GetResultTData()
    {
        return [
            Result.Ok(42),
            GetFailureResult<int>(),
            Result.Ok(42L),
            GetFailureResult<long>(),
            Result.Ok(42.0),
            GetFailureResult<decimal>(),
            Result.Ok("Hello world!"),
            GetFailureResult<string>(),
            Result.Ok(true),
            GetFailureResult<bool>()
        ];
    }

    #endregion
}
