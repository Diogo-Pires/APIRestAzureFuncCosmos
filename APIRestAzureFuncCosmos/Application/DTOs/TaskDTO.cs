namespace Application.DTOs;

public record TaskDTO(
    string Id,
    string Title,
    string Description,
    bool IsCompleted, 
    DateTime? CreatedAt
);  