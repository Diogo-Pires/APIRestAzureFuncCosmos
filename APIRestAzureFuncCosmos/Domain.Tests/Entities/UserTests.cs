using Domain.Entities;

namespace Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";

        // Act
        var user = new User(name, email);

        // Assert
        Assert.Equal(email, user.Id);
        Assert.Equal(name, user.Name);
    }

    [Fact]
    public void Constructor_ShouldAssignEmailAsId()
    {
        // Arrange
        var name = "Alice";
        var email = "alice@example.com";

        // Act
        var user = new User(name, email);

        // Assert
        Assert.Equal(email, user.Id);
    }
}
