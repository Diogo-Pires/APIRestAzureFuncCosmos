using Domain.Consts;
using Domain.Enums;
using Domain.Interfaces;
using Domain.States;
using Domain.ValueObjects;
using Newtonsoft.Json;
using Shared.Exceptions;

namespace Domain.Entities;

public class TaskItem
{
    [JsonProperty("id")]
    public Guid Id { get; private set; }

    [JsonProperty("title")]
    public string Title { get; private set; }

    [JsonProperty("description")]
    public string Description { get; private set; }

    [JsonProperty("status")]
    public TaskItemStatus Status { get; private set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [JsonProperty("completedAt")]
    public DateTime? CompletedAt { get; private set; }

    [JsonProperty("deadline")]
    public DateTime? Deadline { get; private set; }

    [JsonProperty("assignedUserEmail")]
    public string? AssignedUserEmail { get; private set; }

    [JsonIgnore]
    public ITaskState State => TaskStateManager.GetState(Status);

    [JsonIgnore]
    public User? AssignedUser { get; private set; }

    public TaskItem(string title,
                    string description,
                    DateTime? deadline,
                    TaskItemStatus? taskItemStatus,
                    User? assignedUser)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        Deadline = new Deadline(deadline, CreatedAt)?.Value;
        Status = taskItemStatus ?? TaskItemStatus.Pending;
        AssignedUser = assignedUser;
    }

    public void SetCompletedAt(DateTime dateTime) => CompletedAt = dateTime;

    public static void ValidateCreation(string title, string description, DateTime? deadline)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException(Constants.VALIDATION_TASK_TITLE_NOT_EMPTY);

        if (title.Length > 100)
            throw new DomainException(Constants.VALIDATION_TASK_TITLE_LENGTH);

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException(Constants.VALIDATION_TASK_DESCRIPTION_NOT_EMPTY);

        if (deadline.HasValue && deadline.Value < DateTime.UtcNow)
            throw new DomainException(Constants.VALIDATION_TASK_DEADLINE_NOT_PAST);
    }

    public void ValidateUpdate() =>
        ValidateCreation(Title, Description, Deadline);

    public void UpdateTask(string title, string description, DateTime? deadline, TaskItemStatus? newTaskItemStatus)
    {
        if (title != null && title != Title)
            Title = title.Trim();

        if (description != null && description != Description)
            Description = description.Trim();

        if (deadline.HasValue && deadline != Deadline)
            Deadline = deadline;

        UpdateTaskState(newTaskItemStatus);
    }

    public void ChangeStatus(TaskItemStatus newStatus)
    {
        TaskStateManager.ValidateStatusTransition(newStatus, State, Status);
        Status = newStatus;
    }

    private void UpdateTaskState(TaskItemStatus? newTaskItemStatus)
    {
        if (newTaskItemStatus != null && newTaskItemStatus != Status)
        {
            TaskStateManager.ApplyStateTransition(this, (TaskItemStatus)newTaskItemStatus, State, Status);
        }
    }

    public void AssignToUser(User user)
    {
        AssignedUserEmail = user.Id;
        AssignedUser = user;
    }
}
