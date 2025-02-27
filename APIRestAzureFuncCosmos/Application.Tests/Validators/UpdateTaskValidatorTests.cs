using Application.Validators;
using Domain.Entities;
using Domain.Enums;
using Moq;
using Shared.Interfaces;

namespace Application.Tests.Validators;

public class UpdateTaskValidatorTests
{
    private readonly UpdateTaskValidator _validator;

    public UpdateTaskValidatorTests()
    {
        _validator = new UpdateTaskValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WhenTaskIsValid()
    {
        //Arrange
        var dateTime = DateTime.UtcNow;
        var _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock
           .Setup(m => m.GetUTCNow())
           .Returns(dateTime);

        var entity = new TaskItem("Test Task", "Description", dateTime.AddDays(1), TaskItemStatus.Pending, null, _dateTimeProviderMock.Object);

        //Act
        var result = _validator.Validate(entity);

        //Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenTitleIsEmpty()
    {
        //Arrange
        var _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var dateTime = DateTime.UtcNow;
        var entity = new TaskItem("", "Description", dateTime, TaskItemStatus.Pending, null, _dateTimeProviderMock.Object);

        //Act
        var result = _validator.Validate(entity);

        //Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("title"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenDeadlineIsInPast()
    {
        //Arrange
        var _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var dateTime = DateTime.UtcNow;
        var entity = new TaskItem("Test Task", "Description", dateTime.AddDays(-1), TaskItemStatus.Pending, null, _dateTimeProviderMock.Object);

        //Act
        var result = _validator.Validate(entity);

        //Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("deadline"));
    }
}

