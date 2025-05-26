using FluentAssertions;

namespace CSharper.Tests.Types;

[Trait("Category", "Unit")]
[Trait("TestFor", "SoftDeleteEntity<>")]
public class SoftDeleteEntityTests
{
    private readonly string _id = "TestId";
    private readonly DateTimeOffset _createdAt = new(2023, 1, 1, 12, 0, 0, TimeSpan.Zero);
    private readonly string _user1 = "User1";
    private readonly DateTimeOffset _lastModifiedAt = new(2023, 1, 2, 12, 0, 0, TimeSpan.Zero);
    private readonly string _user2 = "User2";

    [Fact]
    public void Delete_ValidValues_SetsProperties()
    {
        // Arrange
        TestSoftDeleteEntity entity = new(_id, _createdAt, _user1, _lastModifiedAt, _user2);

        // Act
        entity.Delete(_user1);

        // Assert
        Assert.Multiple(() =>
        {
            entity.IsDeleted.Should().BeTrue();
            entity.DeletedBy.Should().Be(_user1);
            entity.DeletedAt.Should().BeCloseTo(
                DateTimeOffset.UtcNow, 
                TimeSpan.FromSeconds(1)
            );
        });
    }
}