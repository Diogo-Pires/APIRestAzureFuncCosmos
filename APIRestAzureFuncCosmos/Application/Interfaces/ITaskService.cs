using Application.DTOs;
using FluentResults;

namespace Application.Interfaces;
public interface ITaskService
{
    Task<List<TaskDTO>> GetAllAsync(CancellationToken cancellationToken);
    Task<TaskDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<TaskDTO>> CreateAsync(TaskDTO taskDto, CancellationToken cancellationToken);
    Task<Result<TaskDTO?>> UpdateAsync(TaskDTO taskDto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> AssignTaskToUserAsync(Guid taskId, string email, CancellationToken cancellationToken);
}