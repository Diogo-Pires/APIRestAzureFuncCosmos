using Domain.Entities;
using Presentation.DTOs;

namespace Application.Services;
public interface ITaskService
{
    Task<TaskItem> CreateTaskAsync(CreateTaskDto createTaskDto);
    Task<List<TaskItem>> GetAllTasksAsync();
}