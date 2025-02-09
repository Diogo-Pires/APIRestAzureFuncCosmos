using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;

namespace Application.Services;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;

    public async Task<List<TaskDTO>> GetAllAsync() =>
        (await _taskRepository.GetAllAsync())
                .Select(TaskMapper.ToDTO)
                .ToList();

    public async Task<TaskDTO> GetByIdAsync(string id) =>
        TaskMapper.ToDTO(await _taskRepository.GetByIdAsync(id));

    public async Task<TaskDTO> CreateAsync(TaskDTO createTaskDto)
    {
        if (string.IsNullOrWhiteSpace(createTaskDto.Title))
        {
            throw new ArgumentException("Title is required.");
        }

        var taskEntity = TaskMapper.ToEntity(createTaskDto);
        var createdTask = await _taskRepository.AddAsync(taskEntity);

        return TaskMapper.ToDTO(createdTask);
    }

    public async Task<TaskDTO?> UpdateAsync(TaskDTO updateTaskDto)
    {
        if (string.IsNullOrWhiteSpace(updateTaskDto.Title))
        {
            throw new ArgumentException("Title is required.");
        }

        var existingTask = await _taskRepository.GetByIdAsync(updateTaskDto.Id);
        if (existingTask == null)
        {
            return null;        
        }

        existingTask.Title = updateTaskDto.Title;
        existingTask.Description = updateTaskDto.Description;
        existingTask.IsCompleted = updateTaskDto.IsCompleted;

        var updatedTask = await _taskRepository.UpdateAsync(existingTask);
        if (updatedTask == null)
        { 
            return null;
        }

        return TaskMapper.ToDTO(updatedTask);
    }

    public async Task<bool> DeleteAsync(string id) =>
        await _taskRepository.DeleteByIdAsync(id);
}