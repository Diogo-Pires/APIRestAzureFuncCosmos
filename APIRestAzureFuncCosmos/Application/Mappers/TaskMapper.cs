using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers;

public static class TaskMapper
{
    public static TaskItem ToEntity(TaskDTO dto) =>
        new(dto.Title, dto.Description);

    public static TaskDTO ToDTO(TaskItem entity) =>
        new(entity.Id, entity.Title, entity.Description, entity.IsCompleted, entity.CreatedAt);
}