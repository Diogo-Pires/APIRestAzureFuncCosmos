using Application.DTOs;
using FluentResults;

namespace Application.Interfaces;
public interface ITaskService
{
    Task<List<TaskDTO>> GetAllAsync();
    Task<TaskDTO?> GetByIdAsync(string id);
    Task<Result<TaskDTO>> CreateAsync(TaskDTO createTaskDto);
    Task<TaskDTO?> UpdateAsync(TaskDTO createTaskDto);
    Task<bool> DeleteAsync(string id);
}