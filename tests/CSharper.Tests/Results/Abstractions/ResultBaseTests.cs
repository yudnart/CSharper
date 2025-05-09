using CSharper.Errors;
using CSharper.Results.Abstractions;
using FluentAssertions;
using TestData = CSharper.Tests.Results.ResultTestData;
using TestUtility = CSharper.Tests.Results.ResultTestUtility;

namespace CSharper.Tests.Results.Abstractions;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ResultBase))]
public sealed class ResultBaseTests
{
    [Fact]
    public void Ctor_Default_IsSuccessResult()
    {
        // Act
        TestResultBase result = new();

        // Assert
        TestUtility.AssertSuccessResult(result);
    }

    [Fact]
    public void Ctor_WithError_IsFailureResult()
    {
        // Arrange
        Error error = new("Test error.");

        // Act
        TestResultBase result = new(error);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailureResult(result);
            result.Error.Should().BeSameAs(error);
        });
    }

    [Fact]
    public void Ctor_NullError_ThrowsArgumentNullException()
    {
        // Arrange
        Error error = null!;

        // Act
        Action act = () => _ = new TestResultBase(error);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.ToStringTestCases),
        MemberType = typeof(TestData)
    )]
    public void ToString_FormatsCorrectly(
        string description, ResultBase sut, string expected)
    {
        // Act
        string result = sut.ToString();

        // Assert
        result.Should().Be(expected, description);
    }

    private sealed class TestResultBase : ResultBase
    {
        public TestResultBase() : base()
        {
            // Intentionally blank
        }

        public TestResultBase(Error error) : base(error)
        {
            // Intentionally blank
        }
    }
}
