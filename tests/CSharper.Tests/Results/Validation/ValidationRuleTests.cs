using CSharper.Results.Validation;
using FluentAssertions;

namespace CSharper.Tests.Results.Validation;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ValidationRule))]
public sealed class ValidationRuleTests
{
    [Fact]
    public void Ctor_ValidParams_Succeeds()
    {
        // Arrange
        Func<bool> predicate = () => true;
        Func<Task<bool>> asyncPredicate = () => Task.FromResult(predicate());
        string errorMessage = "Test error";

        // Act
        ValidationRule[] results = [
            new(predicate, errorMessage),
            new(asyncPredicate, errorMessage)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (ValidationRule result in results)
            {
                result.Should().NotBeNull();
                result.Predicate.Should().NotBeNull();
                result.ErrorMessage.Should().Be(errorMessage);
            }
        });
    }

    [Fact]
    public void Ctor_NullPredicate_ThrowsArgumentNullException()
    {
        // Arrange
        string errorMessage = "Test error";
        Func<bool> nullPredicate = null!;
        Func<Task<bool>> nullAsyncPredicate = null!;

        Action[] acts = [
            () => _ = new ValidationRule(nullPredicate, errorMessage),
            () => _ = new ValidationRule(nullAsyncPredicate, errorMessage)
        ];

        // Act & Assert
        Assert.Multiple(() =>
        {
            foreach (Action act in acts)
            {
                act.Should().ThrowExactly<ArgumentNullException>()
                    .And.ParamName.Should().NotBeNullOrWhiteSpace();
            }
        });
    }

    [Theory]
    [MemberData(nameof(InvalidMessages))]
    public void Ctor_InvalidMessage_ThrowsArgumentException(string errorMessage)
    {
        // Arrange
        Action act = () => new ValidationRule(() => true, errorMessage);
        Func<bool> predicate = () => true;
        Func<Task<bool>> asyncPredicate = () => Task.FromResult(predicate());

        Action[] acts = [
            () => _ = new ValidationRule(predicate, errorMessage),
            () => _ = new ValidationRule(asyncPredicate, errorMessage)
        ];

        // Act & Assert
        Assert.Multiple(() =>
        {
            foreach (Action act in acts)
            {
                act.Should().ThrowExactly<ArgumentException>()
                    .And.ParamName.Should().NotBeNullOrWhiteSpace();
            }
        });
    }

    public static TheoryData<string> InvalidMessages()
    {
        return new TheoryData<string>
        {
            { null! },
            { "" },
            { " " }
        };
    }
}
