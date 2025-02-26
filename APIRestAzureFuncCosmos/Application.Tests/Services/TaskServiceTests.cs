using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Moq;
using Shared.Interfaces;

namespace Application.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IValidator<TaskDTO>> _createValidatorMock;
    private readonly Mock<IValidator<TaskItem>> _updateValidatorMock;
    private readonly Mock<IHybridCacheService> _cacheServiceMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _createValidatorMock = new Mock<IValidator<TaskDTO>>();
        _updateValidatorMock = new Mock<IValidator<TaskItem>>();
        _cacheServiceMock = new Mock<IHybridCacheService>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();

        _taskService = new TaskService(
            _taskRepositoryMock.Object,
            _userRepositoryMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _cacheServiceMock.Object,
            _dateTimeProviderMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenTaskIsValid()
    {
        var dateTime = DateTime.UtcNow;
        var taskDto = new TaskDTO(Guid.NewGuid(), "Test Task", "Test Description", TaskItemStatus.Pending, dateTime, dateTime, dateTime, null);
        var taskEntity = TaskMapper.ToEntity(taskDto, _dateTimeProviderMock.Object);

        _createValidatorMock.Setup(v => v.ValidateAsync(taskDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _taskRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskEntity);

        var result = await _taskService.CreateAsync(taskDto, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenValidationFails()
    {
        var dateTime = DateTime.UtcNow;
        var taskDto = new TaskDTO(Guid.NewGuid(), null, null, TaskItemStatus.Pending, dateTime, dateTime, dateTime, null);
        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
        {
            new("Title", "Title cannot be empty"),
            new("Description", "Description cannot be empty")
        });

        _createValidatorMock.Setup(v => v.ValidateAsync(taskDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var result = await _taskService.CreateAsync(taskDto, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var datetime = new DateTime(2025, 1, 1);

        _dateTimeProviderMock
            .Setup(m => m.GetUTCNow())
            .Returns(datetime);

        var dateTime = datetime.AddDays(1);
        var taskId = Guid.NewGuid();
        var taskDto = new TaskItem("Test Task", "Description", dateTime, TaskItemStatus.Pending, null, _dateTimeProviderMock.Object);
        
        _cacheServiceMock.Setup(cache => cache.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<TaskItem>>>()))
            .ReturnsAsync(taskDto);

        _taskRepositoryMock
            .Setup(repo => repo.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskDto);

        var result = await _taskService.GetByIdAsync(taskId, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        var taskId = Guid.NewGuid();
        _taskRepositoryMock
            .Setup(repo => repo.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        var result = await _taskService.GetByIdAsync(taskId, CancellationToken.None);

        Assert.Null(result);
    }
}