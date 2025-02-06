using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Presentation.DTOs;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        return await _taskRepository.GetAllTasksAsync();
    }

    public async Task<TaskItem> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        if (string.IsNullOrWhiteSpace(createTaskDto.Title))
        {
            throw new ArgumentException("Title is required.");
        }

        var taskEntity = TaskMapper.ToEntity(createTaskDto);
        return await _taskRepository.AddTaskAsync(taskEntity);
    }
}