using Domain.Entities;
using Presentation.DTOs;

namespace Application.Mappers;

public static class TaskMapper
{
    public static TaskItem ToEntity(CreateTaskDto dto) =>
        new(dto.Title, dto.Description);
}