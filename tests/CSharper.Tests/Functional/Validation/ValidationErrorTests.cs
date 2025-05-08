using CSharper.Results.Validation;
using CSharper.Tests.Errors;
using FluentAssertions;
using TestUtility = CSharper.Tests.Errors.ErrorTestUtility;

namespace CSharper.Tests.Functional.Validation;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ValidationError))]
public sealed class ValidationErrorTests
{
    [Fact]
    public void Ctor_DefaultParams_Succeeds()
    {
        ValidationError sut = new();
        TestUtility.AssertError(
            sut,
            ValidationError.DefaultErrorMessage,
            ValidationError.DefaultErrorCode);
    }

    [Theory]
    [MemberData(nameof(CtorValidTestCases))]
    public void Ctor_ValidParams_Succeeds(
        string message, string? code, ValidationErrorDetail[]? errorDetails)
    {
        // Act
        ValidationError result = new(message, code, errorDetails!);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertError(result, message, code);
            if (errorDetails?.Length > 0)
            {
                result.ErrorDetails.Should().ContainInOrder(errorDetails);
            }
            else
            {
                result.ErrorDetails.Should().BeEmpty();
            }
        });
    }

    [Fact]
    public void ErrorDetails_IsTypeOfValidationErrorDetail()
    {
        ValidationErrorDetail[] errorDetails = [.. ValidationErrorDetailData()];
        ValidationError sut = new(errorDetails: errorDetails);
        Assert.Multiple(() =>
        {
            sut.ErrorDetails.Should().AllBeOfType<ValidationErrorDetail>();
            sut.ErrorDetails.Should().ContainInOrder(errorDetails);
        });
    }

    [Theory]
    [MemberData(nameof(ToStringTestCases))]
    public void ToString_FormatsCorrectly(
        string description,
        ValidationError error,
        string expected)
    {
        // Act
        string result = error.ToString();

        // Assert
        result.Should().Be(expected, because: description);
    }

    public static TheoryData<string, string?, ValidationErrorDetail[]?> CtorValidTestCases()
    {
        return new TheoryData<string, string?, ValidationErrorDetail[]?>
        {
            { ErrorTestData.Error.Message, ErrorTestData.Error.Code, null },
            { ErrorTestData.ErrorWithCode.Message, ErrorTestData.ErrorWithCode.Code, [] },
            { ErrorTestData.ErrorNoCode.Message , ErrorTestData.ErrorNoCode.Code,[] },
            {
                ErrorTestData.ErrorWithDetails.Message,
                ErrorTestData.ErrorWithDetails.Code,
                [.. ErrorTestData.ErrorWithDetails.ErrorDetails.Select(e =>
                    new ValidationErrorDetail(e.Message, e.Code))]
            }
        };
    }

    public static TheoryData<string, ValidationError, string> ToStringTestCases()
    {
        return new TheoryData<string, ValidationError, string>
            {
                {
                    "No details",
                    new ValidationError(
                        "Validation failed", "VALIDATION_ERROR"),
                    "Validation failed, Code=VALIDATION_ERROR"
                },
                {
                    "Single detail with path",
                    new ValidationError(
                        "Validation failed", "VALIDATION_ERROR",
                        new ValidationErrorDetail(
                            "Invalid value", "ERR001", "User.Name")),
                    "Validation failed, Code=VALIDATION_ERROR\r\n"
                    + "> Invalid value, Code=ERR001, Path=User.Name"
                },
                {
                    "Single detail without path",
                    new ValidationError(
                        "Validation failed", "VALIDATION_ERROR",
                        new ValidationErrorDetail("Invalid value", "ERR001", null)),
                    "Validation failed, Code=VALIDATION_ERROR\r\n"
                    + "> Invalid value, Code=ERR001"
                },
                {
                    "Multiple details with mixed paths",
                    new ValidationError(
                        "Validation failed",
                        "VALIDATION_ERROR",
                        new ValidationErrorDetail("Invalid value", "ERR001", "User.Name"),
                        new ValidationErrorDetail("Value too long", "ERR002", null),
                        new ValidationErrorDetail("> Nested error", "ERR003", "User.Address")),
                    "Validation failed, Code=VALIDATION_ERROR\r\n"
                    + "> Invalid value, Code=ERR001, Path=User.Name\r\n"
                    + "> Value too long, Code=ERR002\r\n"
                    + ">> Nested error, Code=ERR003, Path=User.Address"
                },
                {
                    "Detail with empty path",
                    new ValidationError(
                        "Validation failed",
                        "VALIDATION_ERROR",
                        new ValidationErrorDetail("Invalid value", "ERR001", "")),
                    "Validation failed, Code=VALIDATION_ERROR\r\n"
                    + "> Invalid value, Code=ERR001"
                },
                {
                    "Detail with indented message",
                    new ValidationError(
                        "Validation failed",
                        "VALIDATION_ERROR",
                        new ValidationErrorDetail("> Nested error", "ERR001", "User.Name")),
                    "Validation failed, Code=VALIDATION_ERROR\r\n"
                    + ">> Nested error, Code=ERR001, Path=User.Name"
                }
            };
    }

    public static IEnumerable<ValidationErrorDetail> ValidationErrorDetailData()
    {
        yield return new("Value must be less than 100.", "OUT_OF_RANGE");
        yield return new("Value cannot be null/empty.", "INVALID", "Username");
        yield return new("Other error.");
    }
}
