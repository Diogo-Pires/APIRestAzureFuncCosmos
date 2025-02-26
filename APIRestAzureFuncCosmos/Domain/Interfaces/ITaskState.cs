using Domain.Entities;
using Domain.Enums;
using Shared.Interfaces;

namespace Domain.Interfaces;

public interface ITaskState
{
    TaskItemStatus Status { get; }
    bool CanTransitionTo(TaskItemStatus newStatus);
    void Start(TaskItem task);
    void Complete(TaskItem task, IDateTimeProvider? dateTimeProvider = null);
    void Cancel(TaskItem task);
}
