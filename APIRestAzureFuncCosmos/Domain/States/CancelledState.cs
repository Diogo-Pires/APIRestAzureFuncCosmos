﻿using Domain.Consts;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Shared.Exceptions;

namespace Domain.States;

public class CancelledState : ITaskState
{
    public TaskItemStatus Status => TaskItemStatus.Pending;

    public bool CanTransitionTo(TaskItemStatus newStatus) =>
        false;

    public void Start(TaskItem task)
    {
        throw new DomainException(Constants.VALIDATION_TASK_CANNOT_START_CANCELLED);
    }

    public void Complete(TaskItem task)
    {
        throw new DomainException(Constants.VALIDATION_TASK_CANNOT_COMPLETE_CANCELLED);
    }

    public void Cancel(TaskItem task)
    {
        throw new DomainException(Constants.VALIDATION_TASK_ALREADY_CANCELLED);
    }
}
