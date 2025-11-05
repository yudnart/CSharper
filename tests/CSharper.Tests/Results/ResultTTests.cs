using CSharper.Errors;
using CSharper.Results;
using CSharper.Tests.Errors;
using FluentAssertions;
using TestData = CSharper.Tests.Results.ResultTestData;
using TestUtility = CSharper.Tests.Results.ResultTestUtility;

namespace CSharper.Tests.Results;

[Trait("Category", "Unit")]
[Trait("TestFor", "ResultT")]
public sealed class ResultTTests
{
    [Theory]
    [MemberData(nameof(TValues))]
    public void OkT_ReturnsSuccessResult<T>(T value)
    {
        // Act
        Result<T> result = Result.Ok(value);

        // Assert
        TestUtility.AssertSuccess(result, value);
    }

    [Fact]
    public void FailT_WithError_ReturnsFailureResult()
    {
        // Act
        Error error = ErrorTestData.ErrorNoMessage;
        Result<int> result = Result.Fail<int>(error);

        // Assert
        TestUtility.AssertFailure(result, error);
    }

    [Fact]
    public void FailT_NullError_ThrowsArgumentNullException()
    {
        // Arrange
        Error nullError = null!;

        // Act
        Action act = () => _ = Result.Fail<long>(nullError);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.FailValidParams),
        MemberType = typeof(TestData)
    )]
    public void FailT_ValidParams_ReturnsFailureResult(
        string code, string? message = null)
    {
        // Act
        Result<string> result = Result.Fail<string>(code, message);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);
            result.Error!.Code.Should().Be(code);
            result.Error!.Message.Should().Be(message);
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.NullOrEmptyStrings),
        MemberType = typeof(TestData)
    )]
    public void FailT_InvalidMessage_ThrowArgumentNullException(string code)
    {
        // Arrange
        Action act = () => _ = Result.Fail<string>(code: code);

        // Act & Assert
        act.Should().ThrowExactly<ArgumentException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(FailTWithDataTestCases))]
    public void FailT_WithCodeAndData_ReturnsFailureResult<T>(string code, object data)
    {
        // Act
        Result<T> result = Result.Fail<T>(code, data);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);

            Error error = result.Error!;
            error.Code.Should().Be(code);
            error.Data.Should().NotBeNull().And.NotBeEmpty();
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.NullOrEmptyStrings),
        MemberType = typeof(TestData)
    )]
    public void FailT_WithCodeAndData_InvalidCode_ThrowsArgumentException(string? code)
    {
        // Arrange
        var data = new { Value = 42 };
        Action act = () => Result.Fail<int>(code!, data);

        // Act & Assert
        act.Should().ThrowExactly<ArgumentException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Fact]
    public void FailT_WithCodeAndData_NullData_ThrowsArgumentNullException()
    {
        // Arrange
        Action act = () => Result.Fail<string>("ERR001", data: null!);

        // Act & Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(FailTWithCodeMessageDataTestCases))]
    public void FailT_WithCodeMessageAndData_ReturnsFailureResult<T>(
        string code, string? message, object? data)
    {
        // Act
        Result<T> result = Result.Fail<T>(code, message, data);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);

            Error error = result.Error!;
            error.Code.Should().Be(code);
            error.Message.Should().Be(message);
            if (data != null)
            {
                error.Data.Should().NotBeNull().And.NotBeEmpty();
            }
            else
            {
                error.Data.Should().BeEmpty();
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.NullOrEmptyStrings),
        MemberType = typeof(TestData)
    )]
    public void FailT_WithCodeMessageAndData_InvalidCode_ThrowsArgumentException(string? code)
    {
        // Arrange
        Action act = () => Result.Fail<int>(code!, "message", new { Value = 42 });

        // Act & Assert
        act.Should().ThrowExactly<ArgumentException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTToStringTestCases),
        MemberType = typeof(TestData)
    )]
    public void ToString_FormatsCorrectly<T>(
        string description, Result<T> sut, string expected)
    {
        // Act
        string result = sut.ToString();

        // Assert
        result.Should().Be(expected, description);
    }

    public static IEnumerable<object[]> TValues()
    {
        yield return [42];
        yield return [42L];
        yield return [42.0];
        yield return ["Forty-two"];
        yield return [true];
        yield return [false];
    }

    public static IEnumerable<object[]> FailTWithDataTestCases()
    {
        yield return ["ERR001", new { Value = 42 }];
        yield return ["ERR002", new { Name = "Test", Id = 1 }];
        yield return ["ERR003", new { Key = "value", Count = 5 }];
        yield return ["ERR004", new { Numbers = new[] { 1, 2, 3 }, Total = 6 }];
    }

    public static IEnumerable<object?[]> FailTWithCodeMessageDataTestCases()
    {
        yield return new object?[] { "ERR001", "Error message", new { Value = 42 } };
        yield return new object?[] { "ERR002", "Another error", null };
        yield return new object?[] { "ERR003", null, new { Id = 1 } };
        yield return new object?[] { "ERR004", "", new { Count = 5, Status = "Failed" } };
    }
}
