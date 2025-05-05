using CSharper.Errors;

namespace CSharper.Tests.Errors;

public sealed class ErrorBaseTests
{
    [Theory]
    [MemberData(
        nameof(ErrorTestData.ErrorBaseValidCtorParams),
        MemberType = typeof(ErrorTestData)
    )]
    public void Ctor_ValidParams_Succeeds(string message, string? code)
    {
        TestError result = new(message, code);

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
        Assert.Throws<ArgumentException>(() => new TestError(message));
    }

    [Theory]
    [MemberData(
        nameof(ErrorTestData.ErrorBaseToStringTestData),
        MemberType = typeof(ErrorTestData)
    )]
    public void ToString_FormatsCorrectly(ErrorBase sut, string expected)
    {
        string result = sut.ToString();

        Assert.Multiple(() =>
        {
            Assert.Equal(expected, result);
        });
    }

    private class TestError : ErrorBase
    {
        public TestError(string message, string? code = null) 
            : base(message, code)
        {
            // Intentionally blank
        }
    }
}
