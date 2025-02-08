using Application.DTOs;

namespace Application.Interfaces;
public interface ITaskService
{
    Task<List<TaskDTO>> GetAllAsync();
    Task<TaskDTO> GetByIdAsync(string id);
    Task<TaskDTO> CreateAsync(TaskDTO createTaskDto);
    Task<TaskDTO?> UpdateAsync(TaskDTO createTaskDto);
    Task<bool> DeleteAsync(string id);
}