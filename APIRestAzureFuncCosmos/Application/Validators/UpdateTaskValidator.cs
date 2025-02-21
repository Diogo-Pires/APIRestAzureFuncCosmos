using Domain.Entities;
using FluentValidation;
using Shared.Exceptions;

namespace Application.Validators;

public class UpdateTaskValidator : AbstractValidator<TaskItem>
{
    public UpdateTaskValidator()
    {
        RuleFor(t => t)
            .Custom((task, context) =>
            {
                try
                {
                    task.ValidateUpdate();
                }
                catch (DomainException ex)
                {
                    context.AddFailure(ex.Message);
                }
            });
    }
}