using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using FluentResults;
using FluentValidation;

namespace Application.Services;

public class TaskService(ITaskRepository taskRepository,
                         IValidator<TaskDTO> createValidator,
                         IHybridCacheService hybridCacheService) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly IHybridCacheService _cacheService = hybridCacheService;
    private readonly IValidator<TaskDTO> _createValidator = createValidator;
    private const string _BASE_CACHEKEY = "task:";
    private const string _BASE_CACHEKEY_ALL = "all";

    public async Task<List<TaskDTO>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string CACHEKEY = $"{_BASE_CACHEKEY}{_BASE_CACHEKEY_ALL}";
        return await _cacheService
            .GetOrSetAsync(CACHEKEY, async () => 
                (await _taskRepository.GetAllAsync(cancellationToken))
                        .Select(TaskMapper.ToDTO)
                        .ToList()
            ) ?? [];
    }

    public async Task<TaskDTO?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var cachekey = $"{_BASE_CACHEKEY}{id}";

        var task = await _cacheService
            .GetOrSetAsync(cachekey, async () =>
                await _taskRepository.GetByIdAsync(id, cancellationToken)
            );

        if (task == null)
        {
            return null;
        }

        return TaskMapper.ToDTO(task);
    }

    public async Task<Result<TaskDTO>> CreateAsync(TaskDTO createTaskDto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(createTaskDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Fail(errors);
        }

        var taskEntity = TaskMapper.ToEntity(createTaskDto);
        var createdTask = await _taskRepository.AddAsync(taskEntity, cancellationToken);
        
        await ClearAllRequestFromCache();

        return Result.Ok(TaskMapper.ToDTO(createdTask));
    }

    public async Task<TaskDTO?> UpdateAsync(TaskDTO updateTaskDto, CancellationToken cancellationToken)
    {
        TaskItem? existingTask = null;

        try
        {
            existingTask = await _taskRepository.GetByIdAsync(updateTaskDto.Id, cancellationToken);
            if (existingTask == null)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            throw new TaskServiceException($"{nameof(UpdateAsync)} - Error fetching task from database", ex);
        }
        
        existingTask.Title = updateTaskDto.Title;
        existingTask.Description = updateTaskDto.Description;
        existingTask.IsCompleted = updateTaskDto.IsCompleted;

        var updatedTask = await _taskRepository.UpdateAsync(existingTask, cancellationToken);
        if (updatedTask == null)
        {
            return null;
        }

        await _cacheService.RemoveAsync($"{_BASE_CACHEKEY}{updatedTask.Id}");
        await ClearAllRequestFromCache();

        return TaskMapper.ToDTO(updatedTask);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveAsync($"{_BASE_CACHEKEY}{id}");
        await ClearAllRequestFromCache();

        return await _taskRepository.DeleteByIdAsync(id, cancellationToken);
    }

    private async Task ClearAllRequestFromCache() =>
        await _cacheService.RemoveAsync($"{_BASE_CACHEKEY}{_BASE_CACHEKEY_ALL}");
}