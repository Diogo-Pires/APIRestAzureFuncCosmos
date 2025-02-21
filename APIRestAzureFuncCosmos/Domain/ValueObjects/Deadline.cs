using Domain.Consts;
using Shared.Exceptions;

namespace Domain.ValueObjects;

public class Deadline
{
    public DateTime? Value { get; }

    public Deadline(DateTime? deadlineDate, DateTime createdDate)
    {
        if (deadlineDate.HasValue && deadlineDate.Value < DateTime.UtcNow)
            throw new DomainException(Constants.VALIDATION_TASK_DEADLINE_NOT_PAST);

        if (deadlineDate.HasValue && deadlineDate.Value < createdDate)
            throw new DomainException(Constants.VALIDATION_TASK_CANNOT_BEFORE_CREATEAT);


        Value = deadlineDate;
    }
}