using Domain.Enums;

namespace Application.DTOs;

public record TaskDTO
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public TaskItemStatus? Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime? Deadline { get; init; }

    public TaskDTO(Guid id, string title, string description, TaskItemStatus? status, DateTime createdAt, DateTime? completedAt, DateTime? deadline)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        CreatedAt = createdAt;
        CompletedAt = completedAt;
        Deadline = deadline;
    }
}
