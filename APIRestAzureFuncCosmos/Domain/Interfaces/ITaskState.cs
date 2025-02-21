using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface ITaskState
{
    TaskItemStatus Status { get; }
    bool CanTransitionTo(TaskItemStatus newStatus);
    void Start(TaskItem task);
    void Complete(TaskItem task);
    void Cancel(TaskItem task);
}
