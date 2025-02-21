using Domain.Consts;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Shared.Exceptions;

namespace Domain.States;

public class InProgressState : ITaskState
{
    public TaskItemStatus Status => TaskItemStatus.InProgress;

    public bool CanTransitionTo(TaskItemStatus newStatus) =>
        newStatus == TaskItemStatus.Completed || newStatus == TaskItemStatus.Cancelled;

    public void Start(TaskItem task)
    {
        throw new DomainException(Constants.VALIDATION_TASK_ALREADY_PROGRESS);
    }

    public void Complete(TaskItem task)
    {
        task.ChangeStatus(TaskItemStatus.Completed);
        task.SetCompletedAt(DateTime.UtcNow);
    }

    public void Cancel(TaskItem task)
    {
        task.ChangeStatus(TaskItemStatus.Cancelled);
    }
}
