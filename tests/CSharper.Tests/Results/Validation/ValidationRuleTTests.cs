using CSharper.Results.Validation;
using FluentAssertions;

namespace CSharper.Tests.Results.Validation;

[Trait("Category", "Unit")]
[Trait("TestOf", "ValidationRuleT")]
public sealed class ValidationRuleTTests
{
    [Fact]
    public void Ctor_ValidParams_Succeeds()
    {
        // Arrange
        bool predicate(int _) => true;
        Task<bool> asyncPredicate(int _) => Task.FromResult(predicate(_));
        string errorMessage = "Test error";

        // Act
        ValidationRule<int>[] results = [
            new(predicate, errorMessage),
            new(asyncPredicate, errorMessage)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (ValidationRule<int> result in results)
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
        Func<int, bool> nullPredicate = null!;
        Func<int, Task<bool>> nullAsyncPredicate = null!;

        Action[] acts = [
            () => _ = new ValidationRule<int>(nullPredicate, errorMessage),
            () => _ = new ValidationRule<int>(nullAsyncPredicate, errorMessage)
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
    [MemberData(
        nameof(ValidationRuleTests.InvalidMessages),
        MemberType = typeof(ValidationRuleTests)
    )]
    public void Ctor_InvalidMessage_ThrowsArgumentException(string errorMessage)
    {
        // Arrange
        bool predicate(int _) => true;
        Task<bool> asyncPredicate(int _) => Task.FromResult(predicate(_));

        Action[] acts = [
            () => _ = new ValidationRule<int>(predicate, errorMessage),
            () => _ = new ValidationRule<int>(asyncPredicate, errorMessage)
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
}
