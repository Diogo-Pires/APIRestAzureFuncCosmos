using Domain.Entities;

namespace Application.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllTasksAsync();
    //Task<TaskItem> GetTaskByIdAsync(string id);
    Task<TaskItem> AddTaskAsync(TaskItem task);
    //Task UpdateTaskAsync(TaskItem task);
    //Task DeleteTaskAsync(string id);
}