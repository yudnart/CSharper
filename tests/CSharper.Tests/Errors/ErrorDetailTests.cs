using CSharper.Errors;

namespace CSharper.Tests.Errors;

public sealed class ErrorDetailTests
{
    [Theory]
    [MemberData(
        nameof(ErrorTestData.ErrorBaseValidCtorParams),
        MemberType = typeof(ErrorTestData)
    )]
    public void Ctor_ValidParams_Succeeds(string message, string? code)
    {
        ErrorDetail result = new(message, code);

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
        Assert.Throws<ArgumentException>(() => new ErrorDetail(message));
    }

    [Theory]
    [MemberData(
        nameof(ErrorTestData.ErrorBaseToStringTestData),
        MemberType = typeof(ErrorTestData)
    )]
    public void ToString_FormatsCorrectly(ErrorBase initial, string expected)
    {
        ErrorDetail sut = new(initial.Message, initial.Code);

        string result = sut.ToString();

        Assert.Multiple(() =>
        {
            Assert.Equal(expected, result);
        });
    }
}