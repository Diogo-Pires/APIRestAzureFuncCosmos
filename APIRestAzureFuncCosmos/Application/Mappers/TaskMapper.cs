using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers;

public static class TaskMapper
{
    public static TaskItem ToEntity(TaskDTO dto) =>
        new(dto.Title, dto.Description, dto.Deadline, dto.Status);

    public static TaskDTO ToDTO(TaskItem entity) =>
        new(entity.Id, entity.Title, entity.Description, entity.Status, entity.CreatedAt, entity.CompletedAt, entity.Deadline);
}