using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class TaskService
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

    public async Task CreateTaskAsync(string title, string description)
    {
        var task = new TaskItem(title, description);
        await _taskRepository.AddTaskAsync(task);
    }
}