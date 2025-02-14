using Application.DTOs;
using FluentResults;

namespace Application.Interfaces;
public interface ITaskService
{
    Task<List<TaskDTO>> GetAllAsync(CancellationToken cancellationToken);
    Task<TaskDTO?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Result<TaskDTO>> CreateAsync(TaskDTO createTaskDto, CancellationToken cancellationToken);
    Task<TaskDTO?> UpdateAsync(TaskDTO createTaskDto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}