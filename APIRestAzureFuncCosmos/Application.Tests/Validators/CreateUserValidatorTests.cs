using Application.DTOs;
using Application.Validators;

namespace Application.Tests.Validators;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator;

    public CreateUserValidatorTests()
    {
        _validator = new CreateUserValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        //Arrange
        var user = new UserDTO("", "user@example.com");

        //Act
        var result = _validator.Validate(user);

        //Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("name"));
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        //Arrange
        var user = new UserDTO("Valid Name", "");
        
        //Act
        var result = _validator.Validate(user);

        //Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("email"));
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        //Arrange
        var user = new UserDTO("", "userexample.com");

        //Act
        var result = _validator.Validate(user);

        //Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("email"));
    }

    [Fact]
    public void Should_Not_Have_Error_When_User_Is_Valid()
    {
        //Arrange
        var user = new UserDTO("Valid Name", "user@example.com");

        //Act
        var result = _validator.Validate(user);

        //Assert
        Assert.True(result.IsValid);
    }
}