using Domain.Entities;

namespace Application.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<TaskItem?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken);
    Task<TaskItem?> UpdateAsync(TaskItem task, CancellationToken cancellationToken);
    Task<bool> DeleteByIdAsync(string id, CancellationToken cancellationToken);
}