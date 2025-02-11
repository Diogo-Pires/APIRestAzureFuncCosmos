using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using FluentResults;
using FluentValidation;

namespace Application.Services;

public class TaskService(ITaskRepository taskRepository, IValidator<TaskDTO> createValidator) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly IValidator<TaskDTO> _createValidator = createValidator;

    public async Task<List<TaskDTO>> GetAllAsync() =>
        (await _taskRepository.GetAllAsync())
                .Select(TaskMapper.ToDTO)
                .ToList();

    public async Task<TaskDTO?> GetByIdAsync(string id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            return null;
        }

        return TaskMapper.ToDTO(task);
    }

    public async Task<Result<TaskDTO>> CreateAsync(TaskDTO createTaskDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createTaskDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Fail(errors);
        }

        var taskEntity = TaskMapper.ToEntity(createTaskDto);
        var createdTask = await _taskRepository.AddAsync(taskEntity);

        return Result.Ok(TaskMapper.ToDTO(createdTask));
    }

    public async Task<TaskDTO?> UpdateAsync(TaskDTO updateTaskDto)
    {
        TaskItem? existingTask = null;

        try
        {
            existingTask = await _taskRepository.GetByIdAsync(updateTaskDto.Id);
            if (existingTask == null)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            throw new TaskServiceException($"{nameof(UpdateAsync)} - Error fetching task from database", ex);
        }

        try
        {
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
        catch (Exception ex)
        {
            throw new TaskServiceException($"{nameof(UpdateAsync)} - Error updating task from database", ex);
        }
    }

    public async Task<bool> DeleteAsync(string id) =>
        await _taskRepository.DeleteByIdAsync(id);
}