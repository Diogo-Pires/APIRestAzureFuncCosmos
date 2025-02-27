using Application.DTOs;
using Domain.Entities;
using Shared.Interfaces;

namespace Application.Mappers;

public static class TaskMapper
{
    public static TaskItem ToEntity(TaskDTO dto, IDateTimeProvider dateTimeProvider) =>
        new(dto.Title.Trim(),
            dto.Description.Trim(),
            dto.Deadline,
            dto.Status,
            dto.User != null ? UserMapper.ToEntity(dto.User) : null,
            dateTimeProvider);

    public static TaskDTO ToDTO(TaskItem entity) =>
        new(entity.Id,
            entity.Title,
            entity.Description,
            entity.Status,
            entity.CreatedAt,
            entity.CompletedAt,
            entity.Deadline,
            entity.AssignedUser != null ? UserMapper.ToDTO(entity.AssignedUser) : null);
}